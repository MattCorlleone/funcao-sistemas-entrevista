namespace FI.AtividadeEntrevista.DML
{
    /// <summary>
    /// Classe de beneficiário que representa o registro na tabela BENEFICIARIOS
    /// </summary>
    public class Beneficiario
    {
        public long Id { get; set; }

        /// <summary>
        /// CPF (somente dígitos)
        /// </summary>
        public string CPF { get; set; }

        public string Nome { get; set; }

        public long IdCliente { get; set; }
    }
}