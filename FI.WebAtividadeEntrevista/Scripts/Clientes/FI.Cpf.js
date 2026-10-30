var FI = window.FI || {};

FI.Cpf = (function () {

    // Tira tudo que não é dígito
    function soDigitos(valor) {
        return (valor || '').replace(/\D/g, '');
    }

    // 12345678901 -> 123.456.789-01 (aceita parcial: "1234" -> "123.4")
    function formatar(valor) {
        var d = soDigitos(valor).substring(0, 11);
        var s = d.substring(0, 3);
        if (d.length > 3) s += '.' + d.substring(3, 6);
        if (d.length > 6) s += '.' + d.substring(6, 9);
        if (d.length > 9) s += '-' + d.substring(9, 11);
        return s;
    }

    // quantidade 9 -> 1o dígito (pesos 10..2); 10 -> 2o dígito (pesos 11..2)
    function calcularDigito(d, quantidade) {
        var soma = 0;
        for (var i = 0; i < quantidade; i++)
            soma += parseInt(d.charAt(i), 10) * (quantidade + 1 - i);
        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    // Confere os dois dígitos verificadores e recusa sequências repetidas (111.111.111-11).
    // Serve para avisar o usuário cedo; o servidor valida de novo.
    function valido(valor) {
        var d = soDigitos(valor);
        if (d.length !== 11 || /^(\d)\1{10}$/.test(d))
            return false;
        return calcularDigito(d, 9) === parseInt(d.charAt(9), 10) &&
            calcularDigito(d, 10) === parseInt(d.charAt(10), 10);
    }

    // aplica a máscara a todo campo que casar com o seletor (inclusive criados depois)
    function aplicarMascara(seletor) {
        $(document).on('input', seletor, function () {
            this.value = formatar(this.value);
        });
    }

    return { soDigitos: soDigitos, formatar: formatar, valido: valido, aplicarMascara: aplicarMascara };
})();

