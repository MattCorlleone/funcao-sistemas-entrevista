
// Tela de alteração: preenche o formulário com o cliente (obj, injetado pela view),
// liga as máscaras e envia tudo no Salvar.
$(document).ready(function () {
    if (obj) {
        $('#formCadastro #Nome').val(obj.Nome);
        $('#formCadastro #CPF').val(FI.Cpf.formatar(obj.CPF));
        $('#formCadastro #CEP').val(FI.Cep.formatar(obj.CEP));
        $('#formCadastro #Email').val(obj.Email);
        $('#formCadastro #Sobrenome').val(obj.Sobrenome);
        $('#formCadastro #Nacionalidade').val(obj.Nacionalidade);
        $('#formCadastro #Estado').val(obj.Estado);
        $('#formCadastro #Cidade').val(obj.Cidade);
        $('#formCadastro #Logradouro').val(obj.Logradouro);
        $('#formCadastro #Telefone').val(FI.Telefone.formatar(obj.Telefone));
    }

    FI.Beneficiarios.iniciar(obj ? obj.Beneficiarios : []);
    FI.Cpf.aplicarMascara('#CPF');
    FI.Cep.aplicarMascara('#CEP');
    FI.Telefone.aplicarMascara('#Telefone');

    $('#formCadastro').submit(function (e) {
        e.preventDefault();

        // Validação só por conforto: o servidor valida tudo de novo e é quem decide.
        if (!FI.Cpf.valido($(this).find("#CPF").val())) {
            ModalDialog("Ocorreu um erro", "CPF inválido.");
            return;
        }
        if (!FI.Cep.valido($(this).find("#CEP").val())) {
            ModalDialog("Ocorreu um erro", "CEP inválido.");
            return;
        }
        if (!FI.Telefone.valido($(this).find("#Telefone").val())) {
            ModalDialog("Ocorreu um erro", "Telefone inválido.");
            return;
        }

        $.ajax({
            url: urlPost,
            method: "POST",
            // JSON é obrigatório: a lista de beneficiários só chega ao MVC nesse formato
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                "Id": obj.Id,
                "Nome": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "CPF": $(this).find("#CPF").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "Beneficiarios": FI.Beneficiarios.obter()
            }),
            error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
            success:
            function (r) {
                ModalDialog("Sucesso!", r)
                $("#formCadastro")[0].reset();                                
                window.location.href = urlRetorno;
            }
        });
    })
    
})

// Abre um modal de aviso. O texto entra como HTML, então nunca passe nele
// algo digitado pelo usuário.
function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}
