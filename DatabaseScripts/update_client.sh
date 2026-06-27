#!/bin/bash
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin
CID=$(/opt/keycloak/bin/kcadm.sh get clients -r webapp-realm -q clientId=nextjs-client | grep -oP '(?<="id" : ")[^"]*')
echo "Updating client: $CID"
/opt/keycloak/bin/kcadm.sh update clients/$CID -r webapp-realm \
  -s 'redirectUris=["http://localhost:3000/api/auth/callback/keycloak", "http://localhost:3000", "http://localhost:3000/*"]' \
  -s 'webOrigins=["http://localhost:3000", "+"]'
echo "Update complete."
