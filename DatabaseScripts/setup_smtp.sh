#!/bin/bash
MAILPIT_IP=$(docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' keycloak_mailpit)

docker exec keycloak_server /opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin
docker exec keycloak_server /opt/keycloak/bin/kcadm.sh update realms/webapp-realm -s "smtpServer.host=$MAILPIT_IP" -s "smtpServer.port=1025" -s "smtpServer.from=no-reply@keycloakwebapp.local" -s "smtpServer.replyTo=no-reply@keycloakwebapp.local"
echo "SMTP Configured."
