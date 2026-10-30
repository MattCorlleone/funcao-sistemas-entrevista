using System;

namespace FI.AtividadeEntrevista.BLL
{
    /// <summary>
    /// Violação de regra de negócio (mensagem pode ser exibida ao usuário)
    /// </summary>
    public class RegraDeNegocioException : Exception
    {
        public RegraDeNegocioException(string mensagem) : base(mensagem) { }
    }
}