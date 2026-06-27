#!/bin/bash
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin

DEFAULT_ROLE_ID=$(/opt/keycloak/bin/kcadm.sh get roles/default-roles-webapp-realm -r webapp-realm | grep -oP '(?<="id" : ")[^"]*')
USER_ROLE_ID=$(/opt/keycloak/bin/kcadm.sh get roles/user -r webapp-realm | grep -oP '(?<="id" : ")[^"]*')

echo "Adding user role ($USER_ROLE_ID) to default role composite ($DEFAULT_ROLE_ID)"

/opt/keycloak/bin/kcadm.sh create roles-by-id/$DEFAULT_ROLE_ID/composites -r webapp-realm -f - <<EOF
[
  {
    "id": "$USER_ROLE_ID",
    "name": "user"
  }
]
EOF

echo "Default role set."
