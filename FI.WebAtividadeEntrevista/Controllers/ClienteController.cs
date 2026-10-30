using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        // Recebe o JSON da tela. Erro de DataAnnotations ou de regra de negócio volta como HTTP 400
        // com o texto, que o JS mostra no modal. As regras reais ficam no BoCliente.
        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                try
                {
                    model.Id = bo.Incluir(Mapear(model));

                    return Json("Cadastro efetuado com sucesso");
                }
                catch (RegraDeNegocioException ex)
                {
                    Response.StatusCode = 400;
                    return Json(ex.Message);
                }
            }
        }

        // Igual ao Incluir. O Id vem no corpo (Model.Id) e define quem será alterado e
        // qual cliente é "ele mesmo" na checagem de CPF duplicado.
        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                try
                {
                    bo.Alterar(Mapear(model));

                    return Json("Cadastro alterado com sucesso");

                }
                catch (RegraDeNegocioException ex)
                {
                    Response.StatusCode = 400;
                    return Json(ex.Message);
                }
            }
        }

        // Abre a tela de edição já com cliente e beneficiários carregados
        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = Mapear(cliente);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        // Model (tela) -> entidade. A lista é tratada como vazia quando vem nula,
        // porque o binder do MVC não cria lista a partir de "[]".
        private static Cliente Mapear(ClienteModel model)
        {
            return new Cliente()
            {
                Id = model.Id,
                CEP = model.CEP,
                Cidade = model.Cidade,
                CPF = model.CPF,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone,
                Beneficiarios = (model.Beneficiarios ?? new List<BeneficiarioModel>()).Select(b => new Beneficiario()
                {
                    Id = b.Id,
                    CPF = b.CPF,
                    Nome = b.Nome
                }).ToList()
            };
        }

        // Entidade -> model (tela)
        private static ClienteModel Mapear(Cliente cliente)
        {
            return new ClienteModel()
            {
                Id = cliente.Id,
                CEP = cliente.CEP,
                Cidade = cliente.Cidade,
                CPF = cliente.CPF,
                Email = cliente.Email,
                Estado = cliente.Estado,
                Logradouro = cliente.Logradouro,
                Nacionalidade = cliente.Nacionalidade,
                Nome = cliente.Nome,
                Sobrenome = cliente.Sobrenome,
                Telefone = cliente.Telefone,
                Beneficiarios = (cliente.Beneficiarios ?? new List<Beneficiario>())
                    .Select(b => new BeneficiarioModel() { Id = b.Id, CPF = b.CPF, Nome = b.Nome }).ToList()
            };
        }


    }
}