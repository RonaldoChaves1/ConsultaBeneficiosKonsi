# Desafio - Consultar dados de benefícios (Konsi)

## Sobre o desafio
O desafio foi proposto pela empresa Konsi sugerindo a criação de uma aplicação que permita a um beneficiário consultar os tipos de benefícios que ele possui a partir do CPF.
Para isso, deve haver uma carga de CPF diretamente numa fila do RabbitMQ, onde a aplicação consome essa fila e ao encontrar os dados associados ao CPF, indexa o mesmo no ElasticSearch.
Nesse intermédio, para evitar o consumo excessido da API promovida pela Konsi, quando um CPF é consultado, as informações são guardadas em cache, no Redis.

É possível acompanhar todo o processo através das aplicações de gerenciamento do RabbitMQ, onde é inserido um registro na fila, a mesma é consumida e através do Kibana, pode-se ver os índices criados.

## Preparando o ambiente e executando o projeto
### Pré-requisitos
- Visual Studio 2022 (com o componente .Net 8);
- Docker
- Git

### Passos para configuração
Tendo os pré-requisitos instalados e devidamente configurados, basta executar os seguintes passos:
1. Abrir o Visual Studio 2022 e no popup inicial, selecionar a opção "Clonar um repositório";
2. No campo "Local ou repositório", inserir a seguinte informação:
```
https://github.com/RonaldoChaves1/ConsultaBeneficiosKonsi.git
```
3. Finalizado o processo de clonagem, será necessário localizar o arquivo "appsettings.json" no projeto, e configurar as propriedades do "KonsiClientConfig", conforme fornecida pela Konsi. Ex.:
```
{
  [...]
  "KonsiClientConfig": {
    "BaseUrl": "URL_FORNECIDA_PELA_KONSI",
    "Usuario": "USUARIO_FORNECIDO_PELA_KONSI",
    "Senha": "SENHA_FORNECIDA_PELA_KONSI"
  },
  [...]
}
```
4. Ainda no arquivo "appsettings.json", é possível efetuar outras configurações, tal como do RabbitMQ, ElasticSearch e Redis (propriedade "cache"), entretanto, é possível manter as configurações padrão e assim, o docker-compose montará todo o ambiente já preparado para o projeto.

O Visual Studio já se encarrega de baixar as dependências e criar os containers, sendo necessário apenas executar a aplicação através da opção "Docker Compose".

## Utilizando a aplicação
### Links relevantes
Ao executar a aplicação, a página do Swagger será aberta automaticamente: https://localhost:61675/swagger/index.html
As seguintes aplicações podem complementar o uso da aplicação de forma visual:
- RabbitMQ
    - URL: http://localhost:15672/
    - Usuário: guest
    - Senha: guest
- Kibana
    - URL: http://localhost:5601/
    - Usuário: elastic
    - Senha: elasticpass

### Passos para utilização da aplicação
Inicialmente, a aplicação fica aguardando que uma nova mensagem seja enviada para a fila "cpf-beneficiarios". A mensagem deve conter apenas o CPF a ser consultado.

Após enviar um CPF para a fila do RabbitMQ, a aplicação se encarrega de efetuar a busca no endpoint fornecido pela Konsi e retornar indexar as informações no ElasticSearch.
Caso não seja encontrado nenhum dado para o CPF informado, a aplicação reportará no log.

Pode-se utilizar o endpoint abaixo para efetuar uma busca pelo CPF:
```
https://localhost:61675/api/v1/Beneficiario/BuscarBeneficiario/{cpf}
```

Ou utilizar o endpoint abaixo, para consultar toda a listagem de CPF indexados:
```
https://localhost:61675/api/v1/Beneficiario/ListarTodosBeneficiarios
```