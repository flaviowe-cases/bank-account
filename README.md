
# Bank Account Microservices

Este repositório contém uma solução de microsserviços para gerenciamento de contas bancárias e transações, desenvolvida em .NET 9.0. O projeto é composto por duas APIs principais, um worker, testes unitários e integrados, além de estar configurado para gerar dados de telemetria.


## Como executar o projeto

### Requerimentos

Para rodar a aplicação, é necessario executar as duas apis ao mesmo tempo.

### Para debug
1. [.NET 9](https://dotnet.microsoft.com/pt-br/download/dotnet/9.0) ou maior
2. Uma das seguintes IDE
	- [Visual Studio](https://visualstudio.microsoft.com/pt-br/)
	- [Visual Studio Code](https://code.visualstudio.com)
	- [Rider JetBrains](https://www.jetbrains.com/rider/)

### Para executar standalone
1. [Docker](https://www.docker.com)

## Executar

### Docker Compose

1. Verifique se as portas `5005` e `5006` estão disponíveis.
2. Execute o comando:
	```sh
	docker compose up -d
	```
3. Acesse a documentação das APIs:
	- [Account Api Scalar](http://localhost:5005/scalar)
	- [Transaction Api Scalar](http://localhost:5006/scalar)


### Visual Studio
1. Abra o Visual Studio.
2. Selecione `File > Open > Project/Solution` e escolha o arquivo `Bank.sln` na pasta `src`.
3. Defina o projeto de inicialização (ex: `Bank.Accounts.Api`).
4. Pressione F5 para rodar em modo debug ou Ctrl+F5 para rodar sem debug.


### Visual Studio Code
1. Abra a pasta do projeto no VS Code.
2. Instale a extensão C#.
3. No terminal integrado, execute:
	```sh
	dotnet restore
	dotnet run --project src/Bank.Accounts.Api/Bank.Accounts.Api.csproj
	```
4. Para rodar outros projetos, altere o caminho do `.csproj` conforme necessário.


### JetBrains Rider
1. Abra o Rider.
2. Selecione `Open` e escolha o arquivo `Bank.sln` na pasta `src`.
3. Defina o projeto de inicialização.
4. Clique em Run ou Debug para iniciar o projeto.


## Arquitetura do Projeto


### Account Api

Principais endpoints:
1. Buscar conta por `id` ou `accountNumber` (retorna uma única conta)
2. Buscar contas de forma paginada
3. Cadastrar contas (ao cadastrar, é feita uma requisição ao microserviço de transações para registrar o pagamento)


### Transaction Api

Principais endpoints:
1. Listar o histórico de transações da conta por `accountNumber`
2. Listar o saldo das contas passadas como parâmetro
3. Realizar uma transação

Obs.: Transaction Api faz requisição para Account Api para validar se a conta existe.



## Testes

### Testes Unitários
Os testes unitários utilizam as seguintes bibliotecas:
1. NSubstitute: para mockar procedimentos
2. Fixture: para auto gerar dados fake
3. FluentAssertions: para simplificar asserções

### Testes Integrados
Utilizado o WebApplicationFactory para simular o setup das duas APIs e validar cenários de integração.

## Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.