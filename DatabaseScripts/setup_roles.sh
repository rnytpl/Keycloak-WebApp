#!/bin/bash
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin

echo "Creating roles..."
/opt/keycloak/bin/kcadm.sh create roles -r webapp-realm -s name=admin || true
/opt/keycloak/bin/kcadm.sh create roles -r webapp-realm -s name=moderator || true
/opt/keycloak/bin/kcadm.sh create roles -r webapp-realm -s name=user || true

echo "Assigning admin role to testuser..."
/opt/keycloak/bin/kcadm.sh add-roles -r webapp-realm --uusername testuser --rolename admin

echo "Adding user role to default-roles-webapp-realm..."
USER_ROLE_ID=$(/opt/keycloak/bin/kcadm.sh get roles/user -r webapp-realm | grep -oP '(?<="id" : ")[^"]*')
DEFAULT_ROLE_ID=$(/opt/keycloak/bin/kcadm.sh get roles/default-roles-webapp-realm -r webapp-realm | grep -oP '(?<="id" : ")[^"]*')

/opt/keycloak/bin/kcadm.sh create rolesById/$DEFAULT_ROLE_ID/composites -r webapp-realm -f - <<EOF
[
  {
    "id": "$USER_ROLE_ID",
    "name": "user"
  }
]
EOF

echo "Done"
