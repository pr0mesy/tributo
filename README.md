# Tributo

API de cálculo de impostos brasileiros para ICMS, ISS e IPI — construída com ASP.NET Core, SQL Server e Clean Architecture.

---

## Visão geral

O Tributo calcula impostos brasileiros para qualquer produto ou serviço, dado um estado de origem, estado de destino e tipo de produto. Aplica alíquotas reais de ICMS 2026 para operações internas e interestaduais, extraídas da tabela oficial por estado, alíquota fixa de ISS para serviços e IPI para produtos industrializados.

Desenvolvido como projeto de portfólio para demonstrar design orientado a domínio, arquitetura em camadas, testes unitários e design de API REST no contexto do complexo sistema tributário brasileiro.

---

## Funcionalidades

- Cálculo de ICMS com tabela real de alíquotas 2026 (27 estados × 27 estados)
- Cálculo de ISS para serviços
- Cálculo de IPI para produtos industrializados
- Breakdown completo de impostos na resposta
- Frontend servido na mesma origem da API
- Testes unitários xUnit com Moq para todos os calculadores

---

## Arquitetura

O projeto segue Clean Architecture com quatro camadas, cada uma com responsabilidade única e regras estritas de dependência.

```
TaxCalculatorBR/
├── TaxCalculatorBR.Domain/          # Entidades, enums, interfaces — sem dependências externas
├── TaxCalculatorBR.Application/     # Regras de negócio, DTOs, mappers — depende do Domain
├── TaxCalculatorBR.Infrastructure/  # EF Core, SQL Server, repositórios — depende do Domain
├── TaxCalculatorBR.API/             # Controllers, DI, arquivos estáticos — depende de tudo
└── TaxCalculatorBR.Tests/           # Testes unitários xUnit com Moq
```

**Regra de dependência:** camadas externas dependem de camadas internas. O Domain não possui nenhuma dependência externa.

**Calculadores de imposto** (`IcmsCalculator`, `IssCalculator`, `IpiCalculator`) implementam a interface `ITaxCalculator`, seguindo os princípios de Responsabilidade Única e Inversão de Dependência. O `TaxCalculatorService` os orquestra sem conhecer seus detalhes de implementação.

**Alíquotas** são armazenadas no SQL Server em vez de hardcodadas no código, portanto alterações de alíquota exigem apenas uma atualização de dados — sem necessidade de redeploy.

---

## Stack

| Camada | Tecnologia |
|---|---|
| API | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Banco de dados | SQL Server 2022 |
| Testes | xUnit + Moq |
| Frontend | HTML, CSS, Vanilla JS |

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/)
- [dotnet-ef CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

Instale a ferramenta EF CLI caso ainda não tenha:

```bash
dotnet tool install --global dotnet-ef
export PATH="$HOME/.dotnet/tools:$PATH"
```

---

## Executando localmente

### 1. Clone o repositório

```bash
git clone https://github.com/pr0mesy/tributo.git
cd tributo
```

### 2. Suba o SQL Server via Docker

```bash
docker run -e 'ACCEPT_EULA=Y' \
           -e 'MSSQL_SA_PASSWORD=SuaSenha@123' \
           -p 1433:1433 \
           --name sqlserver \
           --hostname sqlserver \
           -d mcr.microsoft.com/mssql/server:2022-latest
```

Aguarde alguns segundos para o SQL Server inicializar antes do próximo passo.

### 3. Aplique as migrations

```bash
cd TaxCalculatorBR.API

dotnet ef database update \
  --project ../TaxCalculatorBR.Infrastructure \
  --startup-project .
```

Isso cria o banco de dados, a tabela `Aliquotas` e insere todas as alíquotas de ICMS 2026.

### 4. Execute a API

```bash
dotnet run --project TaxCalculatorBR.API
```

Acesse [http://localhost:5022](http://localhost:5022) para abrir o frontend.  
Acesse [http://localhost:5022/swagger](http://localhost:5022/swagger) para explorar a API.

---

## API

### POST /api/tax/calculate

Calcula os impostos para um produto e uma operação interestadual.

**Corpo da requisição**

```json
{
  "productName": "Notebook",
  "price": 5000.00,
  "type": 0,
  "originState": "SP",
  "destinationState": "BA"
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `productName` | string | Nome do produto ou serviço |
| `price` | decimal | Preço base em BRL |
| `type` | inteiro | `0` = Produto, `1` = Serviço, `2` = Industrializado |
| `originState` | string | Sigla do estado de origem (ex: `SP`) |
| `destinationState` | string | Sigla do estado de destino (ex: `BA`) |

**Resposta**

```json
{
  "originState": "SP",
  "destinationState": "BA",
  "icmsValue": 350.00,
  "issValue": 0.00,
  "ipiValue": 0.00,
  "totalTax": 350.00,
  "finalPrice": 5350.00
}
```

**Regras de cálculo**

| Imposto | Incide sobre | Alíquota |
|---|---|---|
| ICMS | Produtos e industrializados | Variável por estado (7% a 23%) |
| ISS | Somente serviços | 5% fixo |
| IPI | Somente industrializados | 10% fixo |

Para ICMS interestadual, estados do Sul e Sudeste (SP, MG, RJ, PR, RS, SC) aplicam 7% nas saídas para Norte, Nordeste, Centro-Oeste e ES. Todas as demais operações interestaduais aplicam 12%.

---

## Executando os testes

```bash
dotnet test
```

---

## Licença

MIT