#!/usr/bin/env python3
"""Configura as 12 rotas do tunnel k3s-mba + DNS CNAMEs na zona dots.dev.br.
Uso: cf-routes.py <arquivo-com-api-token>
Nao imprime o token. Requer env CF_ZONE_ID_DOTS_DEV_BR."""
import json
import os
import sys
import urllib.request
import urllib.error

ACCOUNT = "0c1e6e7e68e7a8bbd17f6e41e64a3897"
TUNNEL = "db6d0974-679a-4f2b-a225-f787f764ef5d"
ZONE = os.environ["CF_ZONE_ID_DOTS_DEV_BR"].strip()

SERVICES = [
    ("mba-store", "webapp-mvc"),
    ("mba-store-bff", "bff-api"),
    ("mba-auth-api", "auth-api"),
    ("mba-aluno-api", "aluno-api"),
    ("mba-conteudo-api", "conteudo-api"),
    ("mba-financeiro-api", "pagamentos-api"),
]
ENVS = [("stg", "mba-modulo4"), ("dev", "mba-modulo4-dev")]


def req(method, path, token, body=None):
    r = urllib.request.Request("https://api.cloudflare.com/client/v4" + path,
                               data=json.dumps(body).encode() if body else None, method=method)
    r.add_header("Content-Type", "application/json")
    r.add_header("Authorization", f"Bearer {token}")
    try:
        with urllib.request.urlopen(r, timeout=25) as resp:
            return json.load(resp)
    except urllib.error.HTTPError as e:
        return json.loads(e.read().decode() or "{}")


def main():
    token = open(sys.argv[1]).read().strip()

    # Tokens de usuário verificam em /user/tokens/verify; tokens de CONTA em /accounts/{id}/tokens/verify.
    v = req("GET", "/user/tokens/verify", token)
    if not v.get("success"):
        v = req("GET", f"/accounts/{ACCOUNT}/tokens/verify", token)
    if not v.get("success"):
        print("TOKEN INVALIDO nos dois verifies:", v.get("errors"))
        sys.exit(1)
    print("token valido, status:", v["result"].get("status"))

    ingress = []
    hostnames = []
    for prefix, ns in ENVS:
        for sub, svc in SERVICES:
            host = f"{prefix}-{sub}.dots.dev.br"
            hostnames.append(host)
            ingress.append({
                "hostname": host,
                "service": f"http://{svc}.{ns}.svc.cluster.local:8080",
            })
    ingress.append({"service": "http_status:404"})

    out = req("PUT", f"/accounts/{ACCOUNT}/cfd_tunnel/{TUNNEL}/configurations", token,
              {"config": {"ingress": ingress}})
    print("tunnel config:", "OK" if out.get("success") else out.get("errors"))
    if not out.get("success"):
        sys.exit(1)

    existing = req("GET", f"/zones/{ZONE}/dns_records?per_page=100&type=CNAME", token)
    by_name = {r["name"]: r["id"] for r in existing.get("result", [])}
    target = f"{TUNNEL}.cfargotunnel.com"
    for host in hostnames:
        body = {"type": "CNAME", "name": host, "content": target, "proxied": True, "ttl": 1}
        if host in by_name:
            out = req("PUT", f"/zones/{ZONE}/dns_records/{by_name[host]}", token, body)
            action = "update"
        else:
            out = req("POST", f"/zones/{ZONE}/dns_records", token, body)
            action = "create"
        print(f"dns {action} {host}:", "OK" if out.get("success") else out.get("errors"))


if __name__ == "__main__":
    main()
