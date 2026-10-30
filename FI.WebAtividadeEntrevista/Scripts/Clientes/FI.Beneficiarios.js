var FI = window.FI || {};

// Lista de beneficiários do modal. Tudo fica em memória; o servidor só recebe a lista
// quando o usuário clica em Salvar no cliente.
FI.Beneficiarios = (function () {
    var lista = [];          // [{ Id, CPF (só dígitos), Nome }]
    var indiceEdicao = -1;   // -1 = incluindo; >= 0 = alterando lista[indiceEdicao]

    // Erro do modal. Usa .text() de propósito: o texto nunca é interpretado como HTML
    function mostrarErro(msg) {
        $('#benefErro').text(msg).removeClass('hidden');
    }

    function esconderErro() {
        $('#benefErro').addClass('hidden').text('');
    }

    // Zera os campos e volta o formulário para o modo "incluir"
    function limparCampos() {
        $('#BenefCPF').val('');
        $('#BenefNome').val('');
        indiceEdicao = -1;
        $('#btnIncluirBenef').text('Incluir');
        $('#btnCancelarBenef').addClass('hidden');
    }

    // Redesenha a tabela a partir da lista. O índice vai no data-indice dos botões
    function render() {
        var $corpo = $('#benefLista').empty();

        if (lista.length === 0) {
            $corpo.append('<tr><td colspan="3" class="text-center text-muted">Nenhum beneficiário incluído.</td></tr>');
            return;
        }

        $.each(lista, function (i, b) {
            var $tr = $('<tr>');
            $('<td>').text(FI.Cpf.formatar(b.CPF)).appendTo($tr);
            $('<td>').text(b.Nome).appendTo($tr);

            var $acoes = $('<td>').appendTo($tr);
            $('<button type="button" class="btn btn-primary btn-sm js-alterar-benef">Alterar</button>')
                .attr('data-indice', i).appendTo($acoes);
            $acoes.append(' ');
            $('<button type="button" class="btn btn-primary btn-sm js-excluir-benef">Excluir</button>')
                .attr('data-indice', i).appendTo($acoes);

            $corpo.append($tr);
        });
    }

    // devolve mensagem de erro ou null
    function validar(cpf, nome) {
        if (!nome) return 'Informe o nome do beneficiário.';
        if (!FI.Cpf.valido(cpf)) return 'CPF inválido.';
        if (cpf === FI.Cpf.soDigitos($('#CPF').val())) return 'O beneficiário não pode ter o mesmo CPF do cliente.';

        var repetido = $.grep(lista, function (b, i) { return b.CPF === cpf && i !== indiceEdicao; }).length > 0;
        if (repetido) return 'Este CPF já foi incluído para o cliente.';

        return null;
    }

    // Botão Incluir/Gravar: valida e inclui na lista, ou atualiza o item em edição (mantendo o Id)
    function confirmar() {
        var cpf = FI.Cpf.soDigitos($('#BenefCPF').val());
        var nome = $.trim($('#BenefNome').val());

        var erro = validar(cpf, nome);
        if (erro) { mostrarErro(erro); return; }
        esconderErro();

        if (indiceEdicao >= 0) {
            lista[indiceEdicao].CPF = cpf;      // mantém o Id
            lista[indiceEdicao].Nome = nome;
        } else {
            lista.push({ Id: 0, CPF: cpf, Nome: nome });
        }

        limparCampos();
        render();
        $('#BenefCPF').focus();
    }

    // Joga o item nos campos e troca o botão para "Gravar"
    function iniciarEdicao(i) {
        esconderErro();
        indiceEdicao = i;
        $('#BenefCPF').val(FI.Cpf.formatar(lista[i].CPF));
        $('#BenefNome').val(lista[i].Nome);
        $('#btnIncluirBenef').text('Gravar');
        $('#btnCancelarBenef').removeClass('hidden');
        $('#BenefNome').focus();
    }

    // Remove da lista em memória; no banco só some quando o cliente for salvo
    function excluir(i) {
        lista.splice(i, 1);
        limparCampos();   // evita ficar editando um índice que mudou
        esconderErro();
        render();
    }

    // Botão Limpar: esvazia a lista depois de confirmar com o usuário
    function limparTudo() {
        if (lista.length === 0) return;
        if (!window.confirm('Remover todos os beneficiários? A remoção só vale depois de clicar em Salvar.')) return;
        lista = [];
        limparCampos();
        esconderErro();
        render();
    }

    // Chamado pelas páginas de incluir/alterar. Carrega a lista inicial e liga os eventos.
    // Crítico: os cliques usam delegação em #benefLista, porque as linhas são recriadas a cada render().
    function iniciar(itensIniciais) {
        lista = $.map(itensIniciais || [], function (b) {
            return { Id: b.Id, CPF: FI.Cpf.soDigitos(b.CPF), Nome: b.Nome };
        });
        limparCampos();
        esconderErro();
        render();

        FI.Cpf.aplicarMascara('#BenefCPF');

        $('#btnIncluirBenef').on('click', confirmar);
        $('#btnCancelarBenef').on('click', function () { limparCampos(); esconderErro(); });
        $('#btnLimparBenef').on('click', limparTudo);
        $('#benefLista').on('click', '.js-alterar-benef', function () { iniciarEdicao(parseInt($(this).attr('data-indice'), 10)); });
        $('#benefLista').on('click', '.js-excluir-benef', function () { excluir(parseInt($(this).attr('data-indice'), 10)); });

        // Enter nos campos = Incluir/Gravar (não existe <form> aqui para submeter)
        $('#BenefCPF, #BenefNome').on('keydown', function (e) {
            if (e.which === 13) { e.preventDefault(); confirmar(); }
        });

        $('#modalBeneficiarios').on('shown.bs.modal', function () { $('#BenefCPF').focus(); });
    }

    // Cópia da lista para enviar ao servidor (Id 0 = novo)
    function obter() {
        return $.map(lista, function (b) { return { Id: b.Id, CPF: b.CPF, Nome: b.Nome }; });
    }

    // usado após salvar um novo cliente (formulário volta ao zero)
    function reiniciar() {
        lista = [];
        limparCampos();
        esconderErro();
        render();
    }

    return { iniciar: iniciar, obter: obter, reiniciar: reiniciar };
})();

