#!/usr/bin/env bash
set -euo pipefail

THEME_NAME="${KC_LOGIN_THEME:-gatewise}"
THEME_REALM="${KC_THEME_REALM:-master}"
KEYCLOAK_SERVER_URL="${KC_INTERNAL_URL:-http://localhost:8080}"

echo "Starting Keycloak with login theme '${THEME_NAME}' for realm '${THEME_REALM}'..."

/opt/keycloak/bin/kc.sh "$@" &
KEYCLOAK_PID="$!"

shutdown() {
  echo "Stopping Keycloak..."
  kill "${KEYCLOAK_PID}" 2>/dev/null || true
  wait "${KEYCLOAK_PID}" 2>/dev/null || true
}

trap shutdown TERM INT

echo "Waiting for Keycloak admin API at ${KEYCLOAK_SERVER_URL}..."

for attempt in $(seq 1 120); do
  if /opt/keycloak/bin/kcadm.sh config credentials \
    --server "${KEYCLOAK_SERVER_URL}" \
    --realm master \
    --user "${KEYCLOAK_ADMIN}" \
    --password "${KEYCLOAK_ADMIN_PASSWORD}" >/tmp/kcadm-theme-login.log 2>&1; then
    break
  fi

  if ! kill -0 "${KEYCLOAK_PID}" 2>/dev/null; then
    echo "Keycloak stopped before the admin API became available."
    cat /tmp/kcadm-theme-login.log 2>/dev/null || true
    wait "${KEYCLOAK_PID}"
  fi

  if [ "${attempt}" -eq 120 ]; then
    echo "Could not authenticate with Keycloak admin API after ${attempt} attempts."
    cat /tmp/kcadm-theme-login.log 2>/dev/null || true
    exit 1
  fi

  sleep 2
done

echo "Applying login theme '${THEME_NAME}' to realm '${THEME_REALM}'..."

/opt/keycloak/bin/kcadm.sh update "realms/${THEME_REALM}" -s "loginTheme=${THEME_NAME}"

echo "Login theme '${THEME_NAME}' applied to realm '${THEME_REALM}'."

wait "${KEYCLOAK_PID}"