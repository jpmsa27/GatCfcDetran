# Projeto - API REST com Clean Architecture

## Abordagem Adotada

O projeto foi desenvolvido utilizando a arquitetura **Clean Architecture** com uma API REST.

A escolha se deve à necessidade de iniciar rapidamente um projeto em prazo reduzido (2 dias). Outras arquiteturas demandariam dias apenas para estarem completamente configuradas.

## Testes Automatizados

- Foco em testes unitários para validar os serviços centrais.
- Testes de integração ou E2E não foram priorizados, pois para o estágio atual (máximo TRL 3) o custo seria elevado frente ao benefício.

## Frameworks Utilizados

- **Entity Framework Core** para gerenciamento do banco de dados.
- Outros frameworks podem ser adicionados futuramente conforme a necessidade.
- Evitou-se incluir tecnologias desnecessárias neste estágio para não gerar over-engineering.

## Como Rodar o Projeto

### Rodar Localmente

1. Preencher as informações necessárias no `appsettings.Development.json`.
2. Comentar a linha 131 do `Program.cs`.
3. Executar: dotnet run

### Rodar via Docker-Compose

1. Preencher as informações necessárias no `docker-compose.yml`.
2. Executar: docker-compose up -d --build


O `docker-compose` foi preparado para subir todos os serviços necessários para o funcionamento do sistema.

Disponibilizado também um arquivo `.json` do Postman para facilitar testes manuais.

## Considerações

- O Front-End não foi implementado por limitação de tempo. O foco foi na entrega do Back-End.
- Um ambiente de mensageria foi configurado utilizando RabbitMQ para notificações via e-mail.
- Um cliente Refit foi pré-estruturado para futura integração com o DETRAN.
- Foi criado um diagrama simples da arquitetura como material complementar.

## Próximos Passos

- Finalizar o Front-End e executar testes de integração.
- Realizar a integração real entre o DETRAN e a API.
- Melhorar o desacoplamento dos serviços de fila e dos BackgroundServices.
- Implementar técnicas de resiliência utilizando Polly ou abordagens manuais.
- Apresentar o projeto ao cliente para obter feedback inicial.
- Realizar testes de carga para homologação em ambiente produtivo.
- Evoluir a solução conforme necessidades observadas em ambiente real.

## Extras

- `docker-compose` configurado para Postgres, RabbitMQ e aplicação.
- Script de seed do banco de dados.
- Arquivo de coleção do Postman para testes manuais.

## Notas

Este projeto foi desenvolvido como uma prova de conceito em ambiente de tempo reduzido, priorizando a entrega de qualidade no Back-End.
