#!/bin/bash
echo "Iniciando Elasticsearch..."
/usr/local/bin/docker-entrypoint.sh &
echo "Comando para iniciar Elasticsearch executado!"

# Espera o Elasticsearch iniciar completamente
until curl -s http://consultabeneficios.elasticsearch:9200 > /dev/null; do
  echo "Esperando o Elasticsearch iniciar..."
  sleep 60
done

# Criação do usuário kibana_system
echo "Criando usuário kibanasystem..."
curl -X POST "http://consultabeneficios.elasticsearch:9200/_security/user/kibanasystem" -H "Content-Type: application/json" -u elastic:elasticpass -d'
{
  "password" : "kibanapass",
  "roles" : [ "kibana_system" ]
}
'
echo "Usuário kibanasystem criado!"

wait