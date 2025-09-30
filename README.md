# FCG_MS_Payments
Microserviço responsável por gerenciar pagamentos, integração com Stripe e operações de cobrança, visando ser usado por outras partes do ecossistema (ex: front-end, microserviços que vendem produtos).

## Principais Tecnologias

This project follows Clean Architecture with the following layers:

- .NET 8 – API estruturada em camadas (domínio, aplicação, infraestrutura, API)
- Docker (multi-stage) – build otimizado e imagem final leve
- GitHub Actions (CI/CD) – build, testes e publicação automatizada
- Stripe .NET SDK – integração com plataforma de pagamentos Stripe
- Amazon RDS (PostgreSQL) – Banco de dados persistente em nuvem
- New Relic – Observabilidade, logs e monitoramento de performance

## Funcionalidades

- Criação de produtos no Stripe
- Criação de payment intents para produtos
- Consulta de status de pagamento
- Confirmação automática

## Autenticação e Permissões

- Autenticaçaõ com API keys

## Arquitetura
 - FCG_MS_Game_Library

    - Api – Controllers, Filtros, Configuração de rotas, Program.cs

    - Application – DTOs, casos de uso, interfaces (contratos)

    - Domain – Entidades centrais (Payment, Product, Transaction), regras de negócio

    - Infrastructure – Implementação de repositórios, cliente Stripe, persistência
## 🚀 CI/CD com GitHub Actions

- CI (Pull Request):

    - Build da solução

- CD (Merge para master):

    - Construção da imagem Docker
  
    - Publicação automática no Amazon ECR com tag latest

✅ Garantindo entregas consistentes, seguras e automatizadas.

## 📊 Monitoramento com New Relic
- Agent do New Relic instalado no container em execução na EC2

- Coleta de métricas: CPU, memória, throughput e latência

- Logs estruturados em JSON enviados ao New Relic Logs

- Monitorando erros, status codes e performance em tempo real
  
## ▶️ Como Rodar

1. Atualize o arquivo appsettings.json com suas chaves da API Stripe:

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_your_publishable_key_here",
    "SecretKey": "sk_test_your_secret_key_here"
  }
}
```

2. Para produção, utilize variáveis de ambiente ou um gerenciador de configuração seguro.

## Endpoits

### Products

- `POST /api/products` - Cria um novo produto

Request body:
```json
{
  "name": "Product Name",
  "description": "Product Description",
  "price": 29.99,
  "currency": "usd"
}
```

### Payments

- `POST /api/payments/create` - Cria uma payment intent

Request body:
```json
{
  "productId": "prod_1234567890"
}
```

- `GET /api/payments/{id}` - Consulta o status do pagamento

## Executando a Aplicação

1. Navegue até o projeto da API:
```bash
cd FCG.MS.Payments.API
```

2. Execute a aplicação:
```bash
dotnet run
```

3. Acesse a documentação do Swagger em: `https://localhost:7001/swagger`

## Testes

A API está configurada para funcionar com o ambiente de testes do Stripe.
Use os números de cartões de teste fornecidos pelo Stripe para simular pagamentos.

