# FCG_MS_Payments
Microserviço de pagamentos desenvolvido em .NET 8, utilizando a biblioteca Stripe para processar transações financeiras. Este projeto segue os princípios da Clean Architecture, promovendo uma estrutura modular e de fácil manutenção.

## Principais Tecnologias
- .NET 8 – API estruturada em camadas de domínio, aplicação e infraestrutura

- Docker (multi-stage) – Build otimizado e imagem final baseada em aspnet:8.0

- GitHub Actions (CI/CD) – Build, testes e publicação automatizada no Amazon ECR

- AWS EC2 – Hospedagem da aplicação em container Docker

- AWS ECR – Registro das imagens do serviço de usuários

- Amazon RDS (PostgreSQL) – Banco de dados persistente em nuvem

- New Relic – Observabilidade, logs e monitoramento de performance

## Autenticação e Permissões

- Login com XApiKey

## Arquitetura

 - FCG_MS_Payments

    - Api – Controllers, Middlewares, Program.cs

    - Application – DTOs, Serviços e Interfaces

    - Domain – Entidades, Enums e Regras de Negócio

    - Infra – DbContext, Repositórios, Configurações de Persistência

✔️ Arquitetura em camadas seguindo boas práticas de DDD e REST

✔️ Injeção de dependência configurada via AddScoped

✔️ Estrutura pensada para evolução em microsserviços

## 🚀 CI/CD com GitHub Actions

- CI (Pull Request):

    - Build da solução

    - Execução dos testes unitários (dotnet test)

- CD (Merge para master):

    - Construção da imagem Docker
  
    - Publicação automática no Amazon ECR com tag latest

✅ Garantindo entregas consistentes, seguras e automatizadas.

## 📊 Monitoramento com New Relic
- Agent do New Relic instalado no container em execução na EC2

- Coleta de métricas: CPU, memória, throughput e latência

- Logs estruturados em JSON enviados ao New Relic Logs

- Dashboards monitorando erros, status codes e performance em tempo real

## ▶️ Como Rodar
1. Clone o repositório:
 ```bash
https://github.com/daniel-lopes-codbr/FCG_MS_Payments.git
 ```
2. Suba o ambiente local com Docker Compose (PostgreSQL incluso):
 ```bash
docker-compose up --build
```
3. Acesse o Swagger da API:
http://localhost:{port}/swagger/index.html

