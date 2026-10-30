using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        /// <summary>
        /// Inclui um novo cliente. Cliente e beneficiários entram na mesma transação:
        /// se algo falhar no meio, nada fica gravado.
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public long Incluir(DML.Cliente cliente)
        {
            Preparar(cliente);
            Validar(cliente);

            try
            {
                using (TransactionScope transacao = new TransactionScope())
                {
                    cliente.Id = new DAL.DaoCliente().Incluir(cliente);
                    SalvarBeneficiarios(cliente);

                    transacao.Complete(); // sem isso, o Dispose desfaz tudo
                    return cliente.Id;
                }
            }
            catch (SqlException ex) when (EhViolacaoDeUnicidade(ex))
            {
                throw TraduzirUnicidade(ex);
            }
        }


        /// <summary>
        /// Altera um cliente e sincroniza a lista de beneficiários (mesma transação do Incluir).
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public void Alterar(DML.Cliente cliente)
        {
            Preparar(cliente);
            Validar(cliente);

            try
            {
                using (TransactionScope transacao = new TransactionScope())
                {
                    new DAL.DaoCliente().Alterar(cliente);
                    SalvarBeneficiarios(cliente);

                    transacao.Complete();
                }
            }
            catch (SqlException ex) when (EhViolacaoDeUnicidade(ex))
            {
                throw TraduzirUnicidade(ex);
            }
        }



        /// <summary>
        /// Consulta o cliente pelo id, já com os beneficiários. Devolve null se não existir.
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public DML.Cliente Consultar(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            DML.Cliente cliente = cli.Consultar(id);

            if (cliente != null)
                cliente.Beneficiarios = new DAL.DaoBeneficiario().Consultar(id);

            return cliente;
        }

        /// <summary>
        /// Excluir o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Excluir(id);
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Listar()
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Listar();
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Pesquisa(iniciarEm,  quantidade, campoOrdenacao, crescente, out qtd);
        }

        /// <summary>
        /// Existe outro cliente (id diferente de idIgnorado) com este CPF?
        /// </summary>
        public bool VerificarExistencia(string CPF, long idIgnorado = 0)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.VerificarExistencia(CPF, idIgnorado);
        }

        // Arruma os dados antes de validar: CPF só com dígitos, nome sem espaços nas pontas,
        // lista de beneficiários nunca nula. Crítico: só normaliza se o formato já for válido.
        // Se limpasse antes, "abc52998224725" viraria um CPF aceito.
        private static void Preparar(DML.Cliente cliente)
        {
            if (CpfValidador.EhValido(cliente.CPF))
                cliente.CPF = CpfValidador.Normalizar(cliente.CPF);

            if (cliente.Beneficiarios == null)
                cliente.Beneficiarios = new List<DML.Beneficiario>();

            foreach (DML.Beneficiario b in cliente.Beneficiarios)
            {
                b.Nome = (b.Nome ?? string.Empty).Trim();
                if (CpfValidador.EhValido(b.CPF))
                    b.CPF = CpfValidador.Normalizar(b.CPF);
            }
        }


        // Regras de negócio do cliente e dos beneficiários. Junta todos os erros e lança uma
        // única exceção, para o usuário corrigir tudo de uma vez.
        // Crítico: as mensagens não podem conter texto digitado pelo usuário, porque o JS as exibe como HTML.
        private void Validar(DML.Cliente cliente)
        {
            List<string> erros = new List<string>();

            if (!CpfValidador.EhValido(cliente.CPF))
                erros.Add("CPF inválido.");
            else if (VerificarExistencia(cliente.CPF, cliente.Id))
                erros.Add("Já existe um cliente com este CPF.");

            HashSet<string> vistos = new HashSet<string>();
            for (int i = 0; i < cliente.Beneficiarios.Count; i++)
            {
                DML.Beneficiario b = cliente.Beneficiarios[i];
                string quem = "Beneficiário " + (i + 1) + ": ";

                if (b.Nome.Length == 0 || b.Nome.Length > 50)
                    erros.Add(quem + "informe o nome (até 50 caracteres).");

                if (!CpfValidador.EhValido(b.CPF))
                    erros.Add(quem + "CPF inválido.");
                else
                {
                    if (b.CPF == cliente.CPF)
                        erros.Add(quem + "o CPF não pode ser o mesmo do cliente.");

                    if (!vistos.Add(b.CPF))
                        erros.Add(quem + "CPF repetido na lista.");
                }
            }

            if (erros.Count > 0)
                throw new RegraDeNegocioException(string.Join("<br>", erros));
        }

        // Sincroniza os beneficiários: exclui os que sumiram da tela, insere os novos (Id = 0)
        // e atualiza os que já existiam. Crítico: um Id que não pertence a este cliente é recusado,
        // senão alguém poderia alterar o beneficiário de outro cliente mandando o Id dele.
        private static void SalvarBeneficiarios(DML.Cliente cliente)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            List<DML.Beneficiario> atuais = dao.Consultar(cliente.Id);

            // 1) removidos: estavam no banco e não vieram mais
            foreach (DML.Beneficiario atual in atuais)
                if (!cliente.Beneficiarios.Any(b => b.Id == atual.Id))
                    dao.Excluir(atual.Id);

            // 2) novos (Id = 0) e alterados (Id existente DESTE cliente)
            foreach (DML.Beneficiario b in cliente.Beneficiarios)
            {
                b.IdCliente = cliente.Id;

                if (b.Id == 0)
                    b.Id = dao.Incluir(b);
                else if (atuais.Any(a => a.Id == b.Id))
                    dao.Alterar(b);
                else
                    throw new RegraDeNegocioException("Beneficiário inválido para este cliente.");
            }
        }

        // Transforma o erro de índice único do banco numa mensagem que o usuário entende.
        // É a rede de segurança para duas gravações simultâneas com o mesmo CPF.
        private static RegraDeNegocioException TraduzirUnicidade(SqlException ex)
        {
            if (ex.Message.Contains("UX_BENEFICIARIOS_CLIENTE_CPF"))
                return new RegraDeNegocioException("CPF de beneficiário repetido para este cliente.");

            return new RegraDeNegocioException("Já existe um cliente com este CPF.");
        }

        // 2601 = índice único; 2627 = constraint única
        private static bool EhViolacaoDeUnicidade(SqlException ex)
        {
            return ex.Number == 2601 || ex.Number == 2627;
        }
    }
}
