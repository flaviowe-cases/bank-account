# bank-account

## Como executar o projeto

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