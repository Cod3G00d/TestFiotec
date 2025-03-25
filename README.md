# API INFODENGUE

## Descrição
A API INFODENGUE é uma aplicação desenvolvida para consultar dados epidemiológicos de arboviroses (Dengue, Zika e Chikungunya) em diferentes municípios. A API permite gerar relatórios comparativos entre municípios e consultar dados específicos de acordo com os parâmetros fornecidos.

## Funcionalidades
- Consultar dados epidemiológicos de arboviroses por município.
- Gerar relatórios comparativos entre múltiplos municípios.
- Gerar relatórios epidemiológicos para as três arboviroses em um único município.
- Autenticação via JWT para acesso seguro aos endpoints.

## Tecnologias Utilizadas
- .NET 8
- ASP.NET Core
- Entity Framework Core
- Swagger para documentação da API
- JWT para autenticação

## Como Iniciar o Projeto

### Pré-requisitos
- .NET 8 SDK
- SQL Server

### Passos para Configuração

1. **Clone o repositório:**
 
 
 
2. **Configurar a string de conexão do banco de dados:**
   No arquivo `appsettings.json`, configure a string de conexão para o seu banco de dados SQL Server:
   

3. **Restaurar as dependências:**
   

4. **Aplicar as migrações do banco de dados:**
   


5. **Executar a aplicação:**
   


### Endpoints Principais

- **Consultar Dados Epidemiológicos:**
  

  Parâmetros:
  - `geocode`: Código IBGE da cidade
  - `disease`: Tipo de doença (Dengue, Chikungunya, Zika)
  - `format`: Formato de saída (json, csv)
  - `ewStart`: Semana epidemiológica inicial (1-53)
  - `ewEnd`: Semana epidemiológica final (1-53)
  - `eyStart`: Ano inicial
  - `eyEnd`: Ano final

- **Gerar Relatório Comparativo entre Municípios:**
  

  Parâmetros:
  - `geocodes`: Lista de códigos IBGE separados por vírgula (ex: "3550308,3304557")
  - `disease`: Tipo de doença (Dengue, Chikungunya, Zika)
  - `format`: Formato de saída (json, csv)
  - `ewStart`: Semana epidemiológica inicial (1-53)
  - `ewEnd`: Semana epidemiológica final (1-53)
  - `eyStart`: Ano inicial
  - `eyEnd`: Ano final

- **Gerar Relatório Epidemiológico por Arbovirose:**
  

  Parâmetros:
  - `geocode`: Código IBGE da cidade
  - `format`: Formato de saída (json, csv)
  - `ewStart`: Semana epidemiológica inicial (1-53)
  - `ewEnd`: Semana epidemiológica final (1-53)
  - `eyStart`: Ano inicial
  - `eyEnd`: Ano final

## Autenticação
A API utiliza JWT para autenticação. Para acessar os endpoints protegidos, é necessário incluir o token JWT no cabeçalho da requisição:



## Documentação da API
A documentação da API pode ser acessada através do Swagger, disponível na URL:
  
