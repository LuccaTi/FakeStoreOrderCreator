# FakeStoreOrderCreator — Serviço Windows em .NET 8

## Visão geral
Este repositório contém um Serviço Windows construído com .NET 8 e TopShelf. O projeto já vem estruturado com camadas separadas (Host → Business → Library), configuração centralizada, logging com Serilog e execução baseada em timer.

O propósito é criar um serviço que usa a FakeStoreAPI (https://github.com/LuccaTi/FakeStoreAPI.git) para consumir e obter os dados da fakestoreapi.com, em seguida ele vai criar pedidos com base nesses dados e vai registrá-los em arquivos JSON.  

A execução do trabalho é feita de forma assíncrona para maximizar a eficiência do uso dos recursos computacionais.

Funcionalidade de exemplo disponível:
- Timer configurável que executa threads de trabalho em intervalos definidos via `appsettings.json`.
- Diretório base configurável via `appsettings.json`.
- URL da API que consome fakestoreapi.com configurável via `appsettings.json`.

## Tecnologias e bibliotecas essenciais
- .NET 8 (Console Application)
- TopShelf (hospedagem e instalação de serviços Windows)
- Serilog (logging para console e arquivo)
- Microsoft.Extensions.Configuration (leitura de `appsettings.json`)
- Microsoft.Extensions.DependencyInjection

## Estrutura do projeto
- `FakeStoreOrderCreator.Host/Program.cs`: ponto de entrada; configura TopShelf, carrega configurações e inicializa o serviço.
- `FakeStoreOrderCreator.Business/ServiceLifeCycleManager.cs`: classe principal que controla o ciclo de vida do serviço.
- `FakeStoreOrderCreator.Business/ServiceProcessingOrchestrator.cs`: classe responsável por controlar o fluxo do trabalho.
- `FakeStoreOrderCreator.Business/ServiceProcessingEngine.cs`: classe que vai obter os dados e aplicar a lógica de negócio.
- `FakeStoreOrderCreator.Business/Configuration/Config.cs`: gerenciador centralizado de configurações via `appsettings.json`.
- `FakeStoreOrderCreator.Business/Logging/Logger.cs`: inicialização e wrappers do Serilog com métodos `Debug`, `Info` e `Error`.
- `FakeStoreOrderCreator.Library/Models/`: camada para modelos de dados e outros objetos.
- `FakeStoreOrderCreator.Host/appsettings.json`: configurações da aplicação (intervalo de execução, diretório de logs, níveis de log).

## Arquitetura e padrões de projeto
- Hospedagem e ciclo de vida
    - Usa TopShelf para facilitar instalação, execução e gerenciamento como serviço Windows.
    - Executa como `LocalSystem` por padrão.
    - Nome do serviço: `FakeStoreOrderCreator`.
    - Callbacks de `WhenStarted` e `WhenStopped` para controle do ciclo de vida.

- Separação de camadas
    - **FakeStoreOrderCreator.Host**: responsável apenas pela hospedagem e bootstrap.
    - **FakeStoreOrderCreator.Business**: contém toda a lógica de negócio, configuração e logging.
    - **FakeStoreOrderCreator.Library**: camada para modelos compartilhados.

- Logging (Serilog)
    - Logs em console e arquivo rolling diário em `logs/system_log_.txt` (diretório configurável).
    - Formato padronizado: `ClassName - MethodName - Message`.
    - Em falhas na inicialização, um arquivo é escrito em `StartupErrors/` para garantir rastreabilidade mesmo antes do logger estar ativo.

- Tratamento de erros
    - Exceções no startup são capturadas e registradas em arquivo dedicado antes de encerrar com codigo de saída diferente de zero.
    - Exceções durante execução são logadas e re-lançadas para que o TopShelf gerencie adequadamente.
    - Exceções de operações canceladas são propagadas porque servem como sinalização (metodologia assíncrona).

## Configuração
Arquivo: `FakeStoreOrderCreator.Host/appsettings.json`

Seções disponíveis:
- **`AppLogging`**:
  - `LogDirectory` (string): pasta para gravação dos logs (relativa ao diretório base da aplicação).
  - `LogLevel` (string): nível mínimo de log ("Debug", "Information", "Warning", "Error").

- **`AppConfig`**:
  - `Interval` (int): intervalo em milissegundos entre execuções do timer (padrão: 10000 = 10 segundos).
  - `FakeStoreDirectory` (string): caminho para o diretório base dos arquivos da fakestore.
  - `ApiUrl` (string): url que aponta para a API responsável por consumir a fakestoreapi.com.
  - `WriteLogConsole`: flag que indica se o serviço deve registrar os logs no console assim como faz nos arquivos.

## Uso e Instalação
O serviço está configurado para consumir a API como localhost, logo precisa executar o comando "dotnet dev-certs https --trust" no terminal como administrador para que o certificado de conexão seja validado.

O código precisa ser compilado tanto em versão Debug quanto versão Release para gerar o executável, em seguida pode rodar como console ao 
usar o .exe no terminal (cmd, por exemplo), também pode ser instalado ao adicionar o argumento "install", o mesmo se aplica para 
desinstalá-lo, porém o argumento é "uninstall".
