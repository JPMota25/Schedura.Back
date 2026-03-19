# CLAUDE.md — Schedura Backend

Guia operacional para o Claude trabalhar neste projeto. Para detalhes arquiteturais completos, consulte `context.md`.

---

## Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 + SQL Server
- AutoMapper, FluentValidation, xUnit
- Docker (em andamento)

---

## Arquitetura em camadas

```
Api → Application.Contracts ← Application → Services → Domain ← Infra
                                    ↑___________Bootstraper____________↑
```

| Projeto                      | Responsabilidade                                              |
|------------------------------|---------------------------------------------------------------|
| `Schedura.Api`                 | HTTP: controllers e middlewares apenas                        |
| `Schedura.Application.Contracts` | DTOs públicos e interfaces de Application                  |
| `Schedura.Application`         | Orquestração de casos de uso e fronteira transacional         |
| `Schedura.Services`            | Regras de negócio e validações de domínio                     |
| `Schedura.Domain`              | Entidades, enums, interfaces (sem dependências externas)      |
| `Schedura.Infra`               | EF Core, repositórios concretos, UnitOfWork                   |
| `Schedura.Bootstraper`         | Composição de DI (único ponto de wiring)                      |

### Responsabilidades por camada — detalhado

**Application**
- Orquestra chamadas entre serviços.
- Verifica pré-condições simples que impediriam a continuidade (ex: entidade não encontrada antes de iniciar transação).
- Gerencia transações: abre, faz commit ou rollback em writes.
- Leituras (`Get*`) nunca abrem transação.
- Nunca contém regra de negócio.

**Services**
- Contém toda regra de negócio e suas validações (FluentValidation).
- Cria e atualiza entidades.
- Persiste via interfaces de repositório do Domain.
- Não sabe de transação — quem gerencia é a Application.

---

## Fluxo de dados

**Escrita (Create / Update / Delete)**
```
Controller
  → IXxxApplication (Application)   ← abre transação
    → IXxxService (Services)         ← regra de negócio + validação
      → IXxxRepository (Domain/Infra) ← persiste
    ← retorna result
  ← commit / rollback
← resposta HTTP
```

**Leitura (GetAll / GetById)**
```
Controller
  → IXxxApplication (Application)   ← sem transação
    → IXxxService (Services)
      → IXxxRepository (Domain/Infra)
    ← retorna result
← resposta HTTP
```

---

## Contratos e nomenclatura

### Hierarquia de DTOs por camada

| Camada               | Tipo de contrato     | Exemplo                          |
|----------------------|----------------------|----------------------------------|
| Api → Application    | `XxxRequest`         | `CreateUserRequest`              |
| Application → Service| `XxxInput`           | `CreateUserInput`                |
| Service → Application| `XxxResult`          | `UserResult`                     |
| Application → Api    | `XxxResponse`        | `UserResponse`, `DeleteUserResponse` |
| Application (query)  | `XxxParams`          | `GetAllUsersParams`              |

Requests podem receber campos como `string` — o AutoMapper faz o mapeamento para a entidade.

### Classes e interfaces

| Padrão                        | Exemplo                              |
|-------------------------------|--------------------------------------|
| `{Feature}{Layer}`            | `UserService`, `UserApplication`     |
| `I{Feature}{Layer}`           | `IUserService`, `IUserApplication`   |
| `{Input}{Validator}`          | `CreateUserInputValidator`           |
| `{Entity}Mappings` (Infra)    | `UserMappings`                       |
| `{Feature}ApplicationMappingProfile` | `UserApplicationMappingProfile` |

### Repositórios

- `IGenericRepository<TEntity, TId>` é a base — nunca usar diretamente nas features.
- Toda feature deve ter seu próprio repositório customizado (ex: `IUserRepository : IGenericRepository<User, string>`).

### Namespaces

Padrão: `Schedura.{Layer}.{Feature}` → ex: `Schedura.Services.Users`

---

## Criando uma nova feature

Ao adicionar uma feature (ex: `Slots`), criar pastas/arquivos simetricamente em **todos** os projetos:

```
Schedura.Domain/
  Entities/          → Slot.cs
  Interfaces/
    Repositories/    → ISlotRepository.cs
    Services/Slots/  → ISlotService.cs (inputs/results aqui)

Schedura.Services/
  Slots/             → SlotService.cs, SlotInputValidators.cs

Schedura.Application.Contracts/
  Slots/             → ISlotApplication.cs, requests, responses, params

Schedura.Application/
  Slots/             → SlotApplication.cs, SlotApplicationMappingProfile.cs

Schedura.Infra/
  Data/Mappings/     → SlotMappings.cs
  Repositories/      → SlotRepository.cs

Schedura.Bootstraper/  → registrar os novos serviços em ServiceCollectionExtensions
```

---

## Testes

- Framework: **xUnit** com **Mocks** (sem banco de dados real nos testes).
- Testes unitários por camada:
  - `Schedura.Services.Tests` → regras de negócio e validações
  - `Schedura.Application.Tests` → orquestração e controle transacional
  - `Schedura.Api.Tests` → bootstrap e endpoints HTTP básicos

---

## Regras absolutas

| Proibido                                                                 |
|--------------------------------------------------------------------------|
| Modificar arquivos de migration manualmente                              |
| Adicionar lógica de negócio em Controllers                               |
| Instalar pacotes NuGet sem solicitar permissão explícita ao usuário      |
| Acessar Infra diretamente na Application (somente via interfaces Domain) |
| Usar `IGenericRepository` diretamente nas features (sempre criar custom) |
| Adicionar dependência entre camadas que viole o sentido da arquitetura   |

---

## Decisões pendentes (não implementar até definição)

- **Autenticação/JWT**: não planejado ainda — aguardar definição de onde o middleware ficará.
- **Padrão de migrations**: comando padrão de `dotnet ef migrations add` ainda não definido.
- **Padrão de erros de API**: tratamento de erro customizado será definido conforme necessidade — não padronizar antecipadamente.
- **Docker / ambiente local**: compose em andamento — não assumir comandos de setup.
