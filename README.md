# Minimal API

Este projeto é uma **Minimal API** desenvolvido com .NET, focado em simplicidade, performance e fácil manutenção.

---

## Visão Geral

Este projeto implementa uma MinimalAPI para um sistema de gestão de veículos com autenticação e autorização.

## Visualização do Fluxo:

1. Usuário (Admin/Editor) → envia login (email, senha) → API (autenticação) → retorna token JWT

2. Usuário com token → requisita endpoints da API (veículos) → recebe dados conforme permissões.

## Tecnologias Utilizadas

- **.NET 9.0**: Plataforma principal para desenvolvimento da API.
- **ASP.NET Core Minimal API**: Estrutura para criação dos endpoints.
- **Entity Framework Core**: ORM para acesso ao banco de dados MySQL.
- **Autenticação JWT**: Segurança dos endpoints via tokens JWT.
- **Swagger/OpenAPI**: Documentação automática dos endpoints.
- **MSTest**: Framework de testes unitários.

## Testes

O projeto possui testes automatizados que cobrem:

- **Testes de unidade**: Validação de entidades e serviços.
- **Testes de integração**: Verificação dos endpoints HTTP usando WebApplicationFactory e HttpClient.
- **Request/Mocks**: Serviços mockados para simular cenários e facilitar o isolamento dos testes.

Os testes podem ser executados via Visual Studio ou CLI do .NET:

> dotnet test Test/

## Como executar

1. Clone o repositório.
2. Configure a string de conexão do MySQL em `appsettings.json`.
3. Execute o projeto via Visual Studio ou CLI:

> dotnet run --project API/minimal-api.csproj

---

:space_invader: [Wisecode](https://github.com/wisecoden)
