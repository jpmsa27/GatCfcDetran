# Projeto - API REST com Clean Architecture

## Abordagem Adotada

O projeto foi desenvolvido utilizando a arquitetura **Clean Architecture** com uma API REST.

A escolha se deve à necessidade de iniciar rapidamente um projeto em prazo reduzido (4 dias entrarei em detalhes ao final*). Outras arquiteturas demandariam dias apenas para estarem completamente configuradas.

## Testes Automatizados

- Foco em testes unitários para validar os serviços centrais.
- Testes de integração ou E2E não foram priorizados, pois para o estágio atual (máximo TRL 3) o custo seria elevado frente ao benefício.

## Frameworks Utilizados

- **Entity Framework Core** para gerenciamento do banco de dados.
- Outros frameworks podem ser adicionados futuramente conforme a necessidade.
- Evitou-se incluir tecnologias desnecessárias neste estágio para focar nos requisitos funcionais.

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

- Houve uma extensão do tempo de produção do teste requisitado, consegui estender os 2 dias iniciais para aproximadamente 4 dias e meio de trabalho(quase 36 horas totais)
- Boa parte desse tempo extra foi gasto fazendo o front-end, vou anexar imagens para caso haja algum problema de execução a interface esteja disponível a ser visualizada.
- Um ambiente de mensageria foi configurado utilizando RabbitMQ para notificações via e-mail usando o BackgroundService do .NET.
- Um cliente Refit foi pré-estruturado para futura integração com o DETRAN(aqui tomei a liberdade de demonstrar o conhecimento de integração, mas não criei um API "Dummy" para continuar a focar no front).
- Foi criado um diagrama simples da arquitetura como material complementar, vou anexar também.

## Próximos Passos

- Finalizar o Front-End e executar testes de integração manual(feito após a extensão do prazo).
- Melhorar a tela do usuário.
- Realizar a integração real entre o DETRAN e a API.
- Melhorar o desacoplamento dos serviços de fila e dos BackgroundServices(se o serviço escalar demais).
- Implementar técnicas de resiliência utilizando Polly, kubernets, docker-swarm, etc.
- Apresentar o projeto ao cliente para obter feedback inicial.
- Realizar testes de carga para homologação em ambiente produtivo.
- Adicionar o sonarQube para validação do código e padronização.
- Pensar em utilizar um n8n para automação do deploy pelo lado da equipe tecnica para desafogar a equipe de DevOps.
- Evoluir a solução conforme necessidades observadas em ambiente real. 

## Extras

- `docker-compose` configurado para Postgres, RabbitMQ e aplicações.
- Script de seed do banco de dados.
- Arquivo de coleção do Postman para testes manuais.
- 

## Notas

- Este projeto foi desenvolvido como uma prova de conceito em ambiente de tempo reduzido(4 dias totais), priorizando a entrega de qualidade no Back-End.
- Dividi o projeto em 3 etapas: Back-end 16 horas, Front-End 16 horas, "DevOps" 5 horas.
-- Na primeira etapa como achei que não daria tempo de entregar o front, decidi focar e desenvolver o back o máximo possível para que ele conseguisse entregar todas as requisições descritas na documentação.
-- Após isso utilizei o docker para compilar e tornar o back e suas dependencias executáveis em qualque ambiente.
-- No final, o prazo foi extendido, mas minhas demandas do trabalho só me permitiram utilizar o final da sexta e o sábado para atuar neste teste.
-- Com esse prazo extra utilizei para desenvolver e entregar o front-end sem maiores problemas.
  
## Anexos

### Tela de Login
![image](https://github.com/user-attachments/assets/91513eb3-fbef-4a08-8490-339bb2b36c72)

### Telas do Super User
- Tela default
![image](https://github.com/user-attachments/assets/b7e4bc7a-ca44-4642-958d-4d661112bd91)

- Lista de usuários
![image](https://github.com/user-attachments/assets/8cebc6f2-4998-4641-a3d7-10e2d7223561)

- Dados do User, marcação de exame e progresso

![image](https://github.com/user-attachments/assets/5cea35a4-db04-4f1d-a235-f638001f05ee)

![image](https://github.com/user-attachments/assets/7f61a274-05e4-4f55-a214-422d4ec70a32)

- Criar admin e usuário

![image](https://github.com/user-attachments/assets/1f9811d9-46e7-451d-8fe6-b620e466ccf6) ![image](https://github.com/user-attachments/assets/6a6d2cf1-6e6a-4fbf-84bc-184062775f47)

### Telas do Admin
![image](https://github.com/user-attachments/assets/903af9c1-6721-4638-bcb8-09406e1728e2)

### Tela do Usuário
![image](https://github.com/user-attachments/assets/b8e037e4-0342-44d3-aa9a-5ec1320f562b)
