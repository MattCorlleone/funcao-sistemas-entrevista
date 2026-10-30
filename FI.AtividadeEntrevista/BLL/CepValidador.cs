using System.Linq;
using System.Text.RegularExpressions;

namespace FI.AtividadeEntrevista.BLL
{
    /// <summary>
    /// Validação e normalização de CEP
    /// </summary>
    public static class CepValidador
    {
        // Aceita 8 dígitos ou o padrão 99999-999 (nada além disso)
        private static readonly Regex Formato = new Regex(@"^([0-9]{8}|[0-9]{5}-[0-9]{3})$");

        /// <summary>
        /// Remove tudo que não é dígito (use só depois de EhValido)
        /// </summary>
        public static string Normalizar(string cep)
        {
            if (string.IsNullOrEmpty(cep))
                return string.Empty;

            return new string(cep.Where(c => c >= '0' && c <= '9').ToArray());
        }

        /// <summary>
        /// Só confere o formato (8 dígitos, com ou sem hífen). Não consulta se o CEP existe.
        /// </summary>
        public static bool EhValido(string cep)
        {
            return !string.IsNullOrWhiteSpace(cep) && Formato.IsMatch(cep);
        }
    }
}
