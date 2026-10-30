using System.Linq;
using System.Text.RegularExpressions;

namespace FI.AtividadeEntrevista.BLL
{
    /// <summary>
    /// Validação e normalização de CPF
    /// </summary>
    public static class CpfValidador
    {
        // Aceita 11 dígitos ou o padrão 999.999.999-99 (nada além disso)
        private static readonly Regex Formato = new Regex(@"^([0-9]{11}|[0-9]{3}\.[0-9]{3}\.[0-9]{3}-[0-9]{2})$");

        /// <summary>
        /// Remove tudo que não é dígito (use só depois de EhValido)
        /// </summary>
        public static string Normalizar(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return string.Empty;

            return new string(cpf.Where(c => c >= '0' && c <= '9').ToArray());
        }

        /// <summary>
        /// Valida o formato, recusa sequências repetidas e confere os dois dígitos verificadores.
        /// Ordem importa: o formato vem antes de normalizar, senão "abc52998224725" passaria.
        /// </summary>
        public static bool EhValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || !Formato.IsMatch(cpf))
                return false;

            string d = Normalizar(cpf);

            if (d.Distinct().Count() == 1)
                return false;

            return d[9] - '0' == CalcularDigito(d, 9) && d[10] - '0' == CalcularDigito(d, 10);
        }

        // quantidade = 9 -> primeiro dígito (pesos 10..2); 10 -> segundo (pesos 11..2)
        private static int CalcularDigito(string d, int quantidade)
        {
            int soma = 0;
            for (int i = 0; i < quantidade; i++)
                soma += (d[i] - '0') * (quantidade + 1 - i);

            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}