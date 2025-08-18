# bank-account

## Como executar o projeto

### Docker Compose

1. Verficar se suas portas `5005` e `5006` estão disponiveis
2. Rodar commando
    ```sh
    docker compose up -d
    ```
3. Abrir api de contas em [Account Api](http://localhost:5005/swagger)
4. Abrir api de transações em [Transaction Api](http://localhost:5006/swagger)


### Visual Studio
1. Abra o Visual Studio.
2. Selecione `File > Open > Project/Solution` e escolha o arquivo `Bank.sln` na pasta `src`.
3. Selecione o projeto de inicialização desejado (ex: `Bank.Accounts.Api`).
4. Pressione F5 para rodar em modo debug ou Ctrl+F5 para rodar sem debug.

### Visual Studio Code
1. Abra a pasta do projeto no VS Code.
2. Instale a extensão C# (caso não tenha).
3. Abra o terminal integrado e execute:
	```sh
	dotnet restore
	dotnet run --project src/Bank.Accounts.Api/Bank.Accounts.Api.csproj
	```
4. Para rodar outros projetos, altere o caminho do `.csproj` conforme necessário.

### JetBrains Rider
1. Abra o Rider.
2. Selecione `Open` e escolha o arquivo `Bank.sln` na pasta `src`.
3. Selecione o projeto de inicialização desejado.
4. Clique em Run ou Debug para iniciar o projeto.

## Arquitetura do Projeto

### Account Api

Account api possui as seguintes ações:

1. Buscar conta por `id` ou `accountNumber`, retornar uma única conta
2. Buscar contas de forma páginada
3. Cadastrar contas `(ao cadastrar uma conta é realizado uma requisição no microservico de transações para registrar o pagamento)`

### Transacion Api

Transaction api possui as seguintes ações:

1. Listar o histórico de transações da conta  por `(accountNumber)` 
2. Listar o saldo das contas passada como parametro
3. Realizar uma transção

`Obs.: Transaction api faz requisição para Account Api, afim de validar se realmente a conta existe`


### Testes Unitários

Foi realizado os testes unitarios com as seguintes libs

1. NSubstitue: para `mockar` procedimentos
2. Fixture: para auto gerar dados fake
3. FluentAssertions: reponsável por eliminar logica nos testes

### Testes Integrados

Testes integrados foi utilziado o WebApplicationFactory para simular setup das duas apis, foram realizados alguns testes