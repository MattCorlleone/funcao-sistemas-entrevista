using System.ComponentModel.DataAnnotations;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Beneficiário
    /// </summary>
    public class BeneficiarioModel
    {
        /// <summary>
        /// 0 = novo
        /// </summary>
        public long Id { get; set; }

        [Required(ErrorMessage = "Informe o CPF do beneficiário.")]
        [MaxLength(14)]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Informe o nome do beneficiário.")]
        [MaxLength(50)]
        public string Nome { get; set; }
    }
}

