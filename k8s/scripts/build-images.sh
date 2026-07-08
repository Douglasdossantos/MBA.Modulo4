#!/usr/bin/env bash
# Builda as 6 imagens do MBA.Modulo4 (tag :staging) e importa no containerd do k3s.
set -uo pipefail
cd /root/mba-staging

SERVICES="MBA.Auth.Api:mba-auth-api MBA.Aluno.API:mba-aluno-api MBA.Conteudo.Api:mba-conteudo-api MBA.Pagamentos.Api:mba-pagamentos-api MBA.Bff.Api:mba-bff-api MBA.WebApp.MVC:mba-webapp-mvc"

build_one() {
  local proj="${1%%:*}" img="${1##*:}"
  echo "=== BUILD $img ($proj) inicio $(date +%T) ==="
  if ! docker build -q -f "src/$proj/Dockerfile" -t "$img:staging" ./src > /dev/null 2>"/tmp/$img.err"; then
    echo "!!! FALHA no build de $img:"
    tail -40 "/tmp/$img.err"
    return 1
  fi
  docker tag "$img:staging" "$img:dev"
  echo "=== IMPORT $img $(date +%T) ==="
  docker save "$img:staging" "$img:dev" | k3s ctr -n k8s.io images import - > /dev/null || { echo "!!! FALHA no import de $img"; return 1; }
  echo "=== OK $img $(date +%T) ==="
}
export -f build_one

FAIL=0
printf '%s\n' $SERVICES | xargs -P2 -I{} bash -c 'build_one "$@"' _ {} || FAIL=1

echo "=== IMAGENS NO K3S ==="
k3s ctr -n k8s.io images ls | grep staging || true
[ "$FAIL" -eq 0 ] && echo "=== TUDO OK $(date +%T) ===" || echo "=== TERMINOU COM FALHAS $(date +%T) ==="
