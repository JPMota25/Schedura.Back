# Schedura - Contexto Arquitetural Atual

Este documento descreve o modelo arquitetural vigente do projeto após a refatoração.

## 1. Projetos da solução

A solução está organizada nos seguintes projetos:

- `Schedura.Api`
- `Schedura.Application.Contracts`
- `Schedura.Application`
- `Schedura.Services`
- `Schedura.Domain`
- `Schedura.Infra`
- `Schedura.Bootstraper`
- `Schedura.Api.Tests`
- `Schedura.Application.Tests`
- `Schedura.Services.Tests`

## 2. Responsabilidade por camada

### `Schedura.Api`
Camada HTTP.

Contém:
- Controllers
- Middlewares de exceção
- Configuração mínima da aplicação web

Regras:
- Não contém regra de negócio.
- Não acessa repositórios diretamente.
- Consome apenas contratos de aplicação (`IUserApplication`) e usa `AddSchedura(...)` do bootstrapper para composição.

### `Schedura.Application.Contracts`
Contratos públicos da camada de aplicação.

Contém:
- `IUserApplication`
- DTOs de entrada/saída da aplicação (requests/responses)

Regras:
- Não contém implementação.
- Não conhece infraestrutura.
- Pode ser compartilhado com a camada API sem acoplar a implementação da Application.

### `Schedura.Application`
Orquestração real de casos de uso.

Contém:
- Implementações de `IUserApplication`
- Mapeamentos entre contracts e contratos de service
- Fronteira transacional de escrita

Regras:
- Escritas (`Create/Update/Delete`) abrem/confirmam/rollback de transação via `IUnitOfWork`.
- Leituras (`Get...`) não abrem transação.
- Não acessa Infra diretamente (somente interfaces do Domain).

### `Schedura.Services`
Regra de negócio.

Contém:
- `UserService`
- Validators de entrada de negócio

Regras:
- Aplica validações e regras de domínio.
- Cria/atualiza entidades.
- Persiste via interfaces de repositório do Domain.

### `Schedura.Domain`
Núcleo do domínio.

Contém:
- Entities
- Enums
- Interfaces/portas de domínio (`Repositories`, `IUserService` e contracts de service)

Regras:
- Não depende de API, Application, Services ou Infra.
- Não contém detalhes técnicos de persistência.

### `Schedura.Infra`
Implementação técnica.

Contém:
- EF Core (`AppDbContext`, mappings, migrations)
- Repositórios concretos
- `UnitOfWork`
- Extensões de DI de infraestrutura

Regras:
- Implementa interfaces definidas no Domain.
- Não contém regra de negócio.

### `Schedura.Bootstraper`
Composição de dependências.

Contém:
- Extensão `AddSchedura(IServiceCollection, IConfiguration)`

Responsabilidades:
- Registrar Application, Services e Infra no container.
- Centralizar wiring de AutoMapper e FluentValidation.

## 3. Fluxo de dados

### Escrita (Create/Update/Delete)
Fluxo:

`Controller -> IUserApplication (Application) -> IUserService (Services) -> Repository (Infra) -> Application -> Controller`

Fronteira transacional:
- Início/commit/rollback em `Schedura.Application`.
- Não existe mais middleware transacional global.

### Leitura (GetAll/GetById)
Fluxo:

`Controller -> IUserApplication (Application) -> IUserService (Services) -> Repository (Infra) -> Application -> Controller`

Sem transação explícita para leitura.

## 4. Convenções atuais

- Contratos HTTP/aplicação em `Schedura.Application.Contracts`.
- Contratos de negócio/service no `Schedura.Domain.Interfaces.Services`.
- Implementações concretas fora do Domain.
- API sem referência direta a classes de Services/Application; usa contratos + bootstrapper.

## 5. Testes

Projetos de teste ativos:

- `Schedura.Application.Tests`
  - Valida orquestração e controle transacional na camada Application.
- `Schedura.Services.Tests`
  - Valida regras de negócio na camada Services.
- `Schedura.Api.Tests`
  - Valida bootstrap e endpoint HTTP básico com `WebApplicationFactory`.

## 6. Diretrizes para evolução

- Novos casos de uso devem expor contratos em `Schedura.Application.Contracts`.
- Orquestração e transação devem ficar em `Schedura.Application`.
- Regras de negócio devem ficar em `Schedura.Services` e/ou entidades de domínio.
- Toda dependência técnica concreta deve ficar em `Schedura.Infra`.
- A composição de DI deve ser mantida no `Schedura.Bootstraper`.
