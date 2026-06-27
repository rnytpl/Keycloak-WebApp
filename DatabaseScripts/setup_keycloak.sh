#!/bin/bash
echo "Authenticating..."
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin

echo "Creating Realm..."
/opt/keycloak/bin/kcadm.sh create realms -s realm=webapp-realm -s enabled=true

echo "Creating Client..."
/opt/keycloak/bin/kcadm.sh create clients -r webapp-realm \
  -s clientId=nextjs-client \
  -s enabled=true \
  -s clientAuthenticatorType=client-secret \
  -s secret=nextjs-secret-123 \
  -s standardFlowEnabled=true \
  -s directAccessGrantsEnabled=true \
  -s publicClient=false \
  -s "redirectUris=[\"http://localhost:3000/api/auth/callback/keycloak\"]" \
  -s "webOrigins=[\"http://localhost:3000\"]"

echo "Creating User..."
/opt/keycloak/bin/kcadm.sh create users -r webapp-realm -s username=testuser -s enabled=true -s firstName=Test -s lastName=User -s email=testuser@example.com -s emailVerified=true

echo "Setting Password..."
/opt/keycloak/bin/kcadm.sh set-password -r webapp-realm --username testuser --new-password password

echo "Setup complete."
