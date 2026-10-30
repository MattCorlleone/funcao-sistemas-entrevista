var FI = window.FI || {};

FI.Telefone = (function () {
  // Tira tudo que não é dígito
  function soDigitos(valor) {
    return (valor || "").replace(/\D/g, "");
  }

  // 1120203030 -> (11) 2020-3030 | 11920203030 -> (11) 92020-3030 (aceita parcial)
  // móvel começa com 9 (9 dígitos após o DDD); fixo começa com 2-5 (8 dígitos)
  function formatar(valor) {
    var d = soDigitos(valor).substring(0, 11);
    if (d.length === 0) return "";
    var s = "(" + d.substring(0, 2);
    if (d.length <= 2) return s;
    var numero = d.substring(2);
    var corte = numero.charAt(0) === "9" ? 5 : 4;
    s += ") " + numero.substring(0, corte);
    if (numero.length > corte) s += "-" + numero.substring(corte);
    return s;
  }

  // DDD 11..99 (sem zero) + fixo (8 dígitos, 2-5 no início) ou móvel (9 dígitos, 9 no início)
  function valido(valor) {
    return /^[1-9][1-9]([2-5]\d{7}|9\d{8})$/.test(soDigitos(valor));
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
