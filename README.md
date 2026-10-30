# FI.WebAtividadeEntrevista

Sistema de manutenção de clientes em ASP.NET MVC, feito para o teste prático da Função Sistemas. Além do cadastro que já existia, agora cada cliente tem CPF e uma lista de beneficiários.

## O que o sistema faz

- Cadastra, altera e lista clientes.
- Pede CPF do cliente: obrigatório, com máscara `999.999.999-99`, dígitos verificadores conferidos e sem repetição entre clientes.
- Abre um modal de beneficiários (CPF e nome) com inclusão, alteração e exclusão. Nada vai ao banco enquanto o cliente não for salvo.
- Valida o CPF de cada beneficiário e não aceita o mesmo CPF duas vezes no mesmo cliente. O beneficiário também não pode ter o CPF do titular.
- Aplica máscara e validação em CEP (`99999-999`) e telefone (`(99) 99999-9999`).

## Tecnologias

- .NET Framework 4.8, ASP.NET MVC 5, C# 7.3
- jQuery, Bootstrap 3, jTable
- SQL Server LocalDB, com acesso só por stored procedures
- MSTest para os testes unitários

## Como rodar

Pré-requisitos: Visual Studio 2022 com a carga de trabalho ASP.NET e o SQL Server LocalDB.

1. Abra `FI.WebAtividadeEntrevista.sln`.
2. Defina `FI.WebAtividadeEntrevista` como projeto de inicialização.
3. Prepare o banco (veja abaixo).
4. Aperte `F5`.

### Banco de dados

O banco fica em `FI.WebAtividadeEntrevista\App_Data\BancoDeDados.mdf`, usando a connection string `BancoDeDados` do `Web.config`.

Antes da primeira execução, rode no banco o script `FI.AtividadeEntrevista\DAL\Scripts\01_CPF_Beneficiarios.sql`. Ele cria a coluna `CPF`, a tabela `BENEFICIARIOS`, os índices de unicidade e as procedures novas. As procedures também estão separadas em `DAL\Clientes\Procedures`.

## Estrutura

```
FI.WebAtividadeEntrevista       Web: controllers, views, scripts, models
FI.AtividadeEntrevista          Regra e dados
  DML/                          entidades (Cliente, Beneficiario)
  BLL/                          regras de negócio e validadores (CPF, CEP, telefone)
  DAL/                          acesso a dados via stored procedures
FI.AtividadeEntrevista.Tests    testes unitários
```

Fluxo de uma gravação: tela, `FI.Clientes.js` ou `FI.AltClientes.js` (POST em JSON), `ClienteController`, `BoCliente`, DAOs, stored procedures.

## Decisões de projeto

- **O servidor é a barreira real.** O JavaScript valida só para dar retorno rápido; o `BoCliente` valida tudo de novo, e o banco tem índices únicos como última proteção.
- **CPF, CEP e telefone são guardados só com dígitos.** A máscara existe apenas na tela.
- **Cliente e beneficiários são gravados juntos**, numa `TransactionScope`. Se algo falhar, nada fica salvo.
- **Os beneficiários são sincronizados por Id:** Id 0 é inserido, Id existente é atualizado e quem sumiu da lista é excluído. Um Id de outro cliente é recusado.
- **Erros de regra de negócio** viram `RegraDeNegocioException` e chegam à tela como HTTP 400.

## Testes

```bash
dotnet test FI.AtividadeEntrevista.Tests/FI.AtividadeEntrevista.Tests.csproj
```

Ou use **Test > Test Explorer** no Visual Studio. Hoje os testes cobrem o `CpfValidador`.

CPFs válidos para testar manualmente: `529.982.247-25`, `111.444.777-35`, `123.456.789-09`, `000.000.001-91`.

## Limites conhecidos

- O CEP é validado só pelo formato; o sistema não confirma que ele existe.
- Trocar os CPFs entre dois beneficiários já salvos, num único Salvar, esbarra no índice único.
- A tela depende de JavaScript; sem ele, o formulário não envia.
