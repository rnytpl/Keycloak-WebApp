#!/bin/bash
echo "Authenticating..."
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin

echo "Creating backend-client..."
/opt/keycloak/bin/kcadm.sh create clients -r webapp-realm \
  -s clientId=backend-client \
  -s enabled=true \
  -s clientAuthenticatorType=client-secret \
  -s secret=backend-secret-123 \
  -s serviceAccountsEnabled=true \
  -s standardFlowEnabled=false \
  -s directAccessGrantsEnabled=false \
  -s publicClient=false

echo "Assigning manage-users role to backend-client service account..."
/opt/keycloak/bin/kcadm.sh add-roles -r webapp-realm \
  --uusername service-account-backend-client \
  --cclientid realm-management \
  --rolename manage-users

echo "Setup complete."
