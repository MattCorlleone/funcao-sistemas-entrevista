using System.Linq;
using System.Text.RegularExpressions;

namespace FI.AtividadeEntrevista.BLL
{
    /// <summary>
    /// Validação e normalização de telefone brasileiro (DDD + 8 dígitos fixo ou DDD + 9 dígitos móvel)
    /// </summary>
    public static class TelefoneValidador
    {
        // Aceita só dígitos ou (99) 9999-9999 / (99) 99999-9999 (nada além disso)
        private static readonly Regex Formato = new Regex(@"^([0-9]{10,11}|\([0-9]{2}\) [0-9]{4,5}-[0-9]{4})$");

        // DDD 11..99 (sem zero); fixo: 8 dígitos iniciando em 2-5; móvel: 9 dígitos iniciando em 9
        private static readonly Regex Digitos = new Regex(@"^[1-9][1-9]([2-5][0-9]{7}|9[0-9]{8})$");

        /// <summary>
        /// Remove tudo que não é dígito (use só depois de EhValido)
        /// </summary>
        public static string Normalizar(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
                return string.Empty;

            return new string(telefone.Where(c => c >= '0' && c <= '9').ToArray());
        }

        /// <summary>
        /// Dois testes: primeiro o formato do texto, depois as regras de DDD e do primeiro dígito.
        /// Ordem importa: validar o formato antes de normalizar impede que lixo como "ab1199999999" passe.
        /// </summary>
        public static bool EhValido(string telefone)
        {
            return !string.IsNullOrWhiteSpace(telefone)
                && Formato.IsMatch(telefone)
                && Digitos.IsMatch(Normalizar(telefone));
        }
    }
}
