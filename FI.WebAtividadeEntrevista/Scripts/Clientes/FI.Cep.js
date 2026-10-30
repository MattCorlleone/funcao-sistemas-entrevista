var FI = window.FI || {};

FI.Cep = (function () {
  // Tira tudo que não é dígito
  function soDigitos(valor) {
    return (valor || "").replace(/\D/g, "");
  }

  // 08545230 -> 08545-230 (aceita parcial: "0854" -> "0854")
  function formatar(valor) {
    var d = soDigitos(valor).substring(0, 8);
    return d.length > 5 ? d.substring(0, 5) + "-" + d.substring(5) : d;
  }

  // CEP válido = exatamente 8 dígitos (não consulta se o CEP existe)
  function valido(valor) {
    return /^\d{8}$/.test(soDigitos(valor));
  }

  // aplica a máscara a todo campo que casar com o seletor (inclusive criados depois)
  function aplicarMascara(seletor) {
    $(document).on("input", seletor, function () {
      this.value = formatar(this.value);
    });
  }

  return {
    soDigitos: soDigitos,
    formatar: formatar,
    valido: valido,
    aplicarMascara: aplicarMascara,
  };
})();
