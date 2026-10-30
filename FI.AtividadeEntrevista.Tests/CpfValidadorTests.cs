using FI.AtividadeEntrevista.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FI.AtividadeEntrevista.Tests
{
    [TestClass]
    public class CpfValidadorTests
    {
        [TestMethod]
        [DataRow("529.982.247-25")]
        [DataRow("52998224725")]
        [DataRow("111.444.777-35")]
        [DataRow("123.456.789-09")]
        [DataRow("000.000.001-91")] // exemplos da figura do enunciado
        [DataRow("000.000.002-72")]
        public void CpfValido_Aceita(string cpf)
        {
            Assert.IsTrue(CpfValidador.EhValido(cpf));
        }

        [TestMethod]
        [DataRow("529.982.247-26")]  // 2º dígito errado
        [DataRow("529.982.247-35")]  // 1º dígito errado
        [DataRow("000.000.001-92")]
        [DataRow("111.111.111-11")]  // repetidos
        [DataRow("00000000000")]
        [DataRow("5299822472")]      // 10 dígitos
        [DataRow("529982247255")]    // 12 dígitos
        [DataRow("529.982.24725")]   // máscara parcial
        [DataRow("abc52998224725")]  // letras
        [DataRow("529 982 247 25")]  // outro separador
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void CpfInvalido_Recusa(string cpf)
        {
            Assert.IsFalse(CpfValidador.EhValido(cpf));
        }

        [TestMethod]
        public void Normalizar_RemoveMascara()
        {
            Assert.AreEqual("52998224725", CpfValidador.Normalizar("529.982.247-25"));
        }

        [TestMethod]
        public void Normalizar_Nulo_RetornaVazio()
        {
            Assert.AreEqual(string.Empty, CpfValidador.Normalizar(null));
        }
    }
}

