using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Beneficiário
    /// </summary>
    internal class DaoBeneficiario : AcessoDados
    {
        // Insere o beneficiário e devolve o Id gerado (0 se a proc não retornar linha)
        internal long Incluir(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("Nome", beneficiario.Nome));
            parametros.Add(new SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new SqlParameter("IdCliente", beneficiario.IdCliente));

            DataSet ds = base.Consultar("FI_SP_IncBenef", parametros);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        // Atualiza só CPF e nome. O vínculo com o cliente nunca muda
        internal void Alterar(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("Nome", beneficiario.Nome));
            parametros.Add(new SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new SqlParameter("ID", beneficiario.Id));

            base.Executar("FI_SP_AltBenef", parametros);
        }

        // Exclui um beneficiário pelo Id
        internal void Excluir(long id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("ID", id));

            base.Executar("FI_SP_DelBenef", parametros);
        }

        // Lista os beneficiários de um cliente. Devolve lista vazia, nunca null
        internal List<Beneficiario> Consultar(long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter("IdCliente", idCliente));

            DataSet ds = base.Consultar("FI_SP_ConsBenef", parametros);

            List<Beneficiario> lista = new List<Beneficiario>();
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    lista.Add(new Beneficiario()
                    {
                        Id = row.Field<long>("Id"),
                        CPF = row.Field<string>("CPF"),
                        Nome = row.Field<string>("Nome"),
                        IdCliente = row.Field<long>("IdCliente")
                    });
                }
            }

            return lista;
        }
    }
}

