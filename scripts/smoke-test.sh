#!/usr/bin/env bash
#
# smoke-test.sh - Smoke test ponta a ponta do fluxo de matrícula + pagamento do MBA.Modulo4.
#
# Exercita o fluxo real (caixa-preta, via HTTP) na ordem descrita no dossiê técnico:
#   registro do aluno -> login admin -> listar cursos -> matricular -> descobrir matriculaId
#   -> pagar -> polling do status até PagamentoRealizado -> passos opcionais (aula/conclusão).
#
# O script NÃO mascara os problemas conhecidos do ambiente: ele os exercita e os reporta como
# FINDINGS em tempo de execução (pegadinhas 1 a 3 do dossiê).
#
# Uso:
#   scripts/smoke-test.sh [--skip-up] [--down] [--timeout N] [--help]
#
# Flags:
#   --skip-up      Não executa "docker compose up"; assume o ambiente já no ar.
#   --down         Executa "docker compose down -v" ao final (limpeza total, inclusive volumes).
#   --timeout N    Segundos de polling do status da matrícula (padrão 60).
#   --help         Mostra esta ajuda.
#
# Sem flags: sobe o ambiente com "docker compose up -d --build", roda o fluxo e DEIXA o ambiente no ar.
#
# Código de saída: 0 = fluxo principal passou; 1 = fluxo principal falhou.
#
# URLs configuráveis por variável de ambiente (defaults entre parênteses):
#   SMOKE_AUTH_URL        (http://localhost:5020)
#   SMOKE_CONTEUDO_URL    (http://localhost:5137)
#   SMOKE_ALUNO_URL       (http://localhost:5236)
#   SMOKE_PAGAMENTOS_URL  (http://localhost:5190)
#   SMOKE_BFF_URL         (http://localhost:5293)
#
# Observação: sem "set -e" de propósito. O script coleta as falhas e reporta ao final; cada passo
# faz checagem explícita em vez de abortar no primeiro erro.

set -u -o pipefail

# =========================================================
# Configuração e estado global
# =========================================================

AUTH_URL="${SMOKE_AUTH_URL:-http://localhost:5020}"
CONTEUDO_URL="${SMOKE_CONTEUDO_URL:-http://localhost:5137}"
ALUNO_URL="${SMOKE_ALUNO_URL:-http://localhost:5236}"
PAGAMENTOS_URL="${SMOKE_PAGAMENTOS_URL:-http://localhost:5190}"
BFF_URL="${SMOKE_BFF_URL:-http://localhost:5293}"

# Credenciais fixas do admin semeado (seed) e senha forte do aluno de teste.
ADMIN_EMAIL="adm@adm.com"
ADMIN_SENHA='Adm@2026!'
ALUNO_SENHA='Smoke@2026!'

# Cartão de teste (dados fictícios de smoke, não são dados reais).
CARTAO_NUMERO="5502093788528294"
CARTAO_TITULAR="Smoke Teste"
CARTAO_VALIDADE="12/29"
CARTAO_CVV="123"

# Timeouts / retentativas.
HTTP_MAX_TIME=15          # timeout por chamada curl (segundos)
READY_TENTATIVAS=40       # tentativas de readiness por serviço
READY_INTERVALO=3         # intervalo entre tentativas de readiness (segundos)
POLL_INTERVALO=3          # intervalo do polling de status (segundos)
POLL_TIMEOUT=60           # timeout do polling de status (segundos) - sobrescrito por --timeout

# Flags de execução.
SKIP_UP=false
FLAG_DOWN=false

# Estado do fluxo.
FLUXO_OK=true
EXIT_CODE=0
RELATORIO_IMPRESSO=false
DOWN_EXECUTADO=false

# Coletores de relatório.
declare -a REPORT_STEPS=()
declare -a FINDINGS=()

# Variáveis de saída da última chamada HTTP.
HTTP_STATUS="000"
HTTP_BODY=""
HTTP_ERR=""

# Raiz do repositório (o script vive em scripts/).
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
COMPOSE_FILE="$REPO_ROOT/docker-compose.yml"

# Comando do docker compose (v2 "docker compose" ou v1 "docker-compose").
DOCKER_COMPOSE=(docker compose)

# =========================================================
# Cores (desabilitadas se stdout não for TTY)
# =========================================================

if [[ -t 1 ]]; then
	C_RESET=$'\033[0m'
	C_RED=$'\033[31m'
	C_GREEN=$'\033[32m'
	C_YELLOW=$'\033[33m'
	C_BLUE=$'\033[34m'
	C_BOLD=$'\033[1m'
else
	C_RESET=''
	C_RED=''
	C_GREEN=''
	C_YELLOW=''
	C_BLUE=''
	C_BOLD=''
fi

# =========================================================
# Helpers de log
# =========================================================

log_info()  { printf '%s\n' "${C_BLUE}[INFO]${C_RESET} $*"; }
log_ok()    { printf '%s\n' "${C_GREEN}[OK]${C_RESET}   $*"; }
log_warn()  { printf '%s\n' "${C_YELLOW}[WARN]${C_RESET} $*"; }
log_erro()  { printf '%s\n' "${C_RED}[ERRO]${C_RESET} $*" >&2; }
log_passo() { printf '%s\n' "${C_BOLD}==> $*${C_RESET}"; }

# =========================================================
# Coletores de relatório
# =========================================================

# registrar_passo STATUS DESCRICAO   (STATUS = PASS|FAIL|WARN)
registrar_passo() {
	REPORT_STEPS+=("$1|$2")
}

registrar_finding() {
	FINDINGS+=("$1")
	log_warn "FINDING: $1"
}

marcar_falha() {
	FLUXO_OK=false
}

# =========================================================
# Ajuda
# =========================================================

mostrar_ajuda() {
	cat <<'AJUDA'
smoke-test.sh - Smoke test ponta a ponta do fluxo de matrícula + pagamento (MBA.Modulo4)

USO:
  scripts/smoke-test.sh [--skip-up] [--down] [--timeout N] [--help]

FLAGS:
  --skip-up      Não executa "docker compose up"; assume o ambiente já no ar.
  --down         Executa "docker compose down -v" ao final (remove containers e volumes).
  --timeout N    Segundos de polling do status da matrícula (padrão 60).
  --help         Mostra esta ajuda e sai.

SEM FLAGS:
  Sobe o ambiente com "docker compose up -d --build", roda o fluxo e DEIXA o ambiente no ar.

CÓDIGO DE SAÍDA:
  0  fluxo principal passou
  1  fluxo principal falhou

VARIÁVEIS DE AMBIENTE (URLs base, defaults entre parênteses):
  SMOKE_AUTH_URL        (http://localhost:5020)
  SMOKE_CONTEUDO_URL    (http://localhost:5137)
  SMOKE_ALUNO_URL       (http://localhost:5236)
  SMOKE_PAGAMENTOS_URL  (http://localhost:5190)
  SMOKE_BFF_URL         (http://localhost:5293)

A saída final traz sempre um bloco RELATORIO (cada passo como PASS/FAIL/WARN) e um bloco
FINDINGS (divergências detectadas em tempo de execução, ex.: pegadinhas 1 a 3 do dossiê).
AJUDA
}

# =========================================================
# Parse de argumentos
# =========================================================

parse_args() {
	while [[ $# -gt 0 ]]; do
		case "$1" in
			--skip-up)
				SKIP_UP=true
				shift
				;;
			--down)
				FLAG_DOWN=true
				shift
				;;
			--timeout)
				if [[ $# -lt 2 ]]; then
					log_erro "--timeout requer um valor numérico (segundos)."
					exit 2
				fi
				if ! [[ "$2" =~ ^[0-9]+$ ]] || [[ "$2" -eq 0 ]]; then
					log_erro "--timeout inválido: '$2'. Informe um inteiro positivo de segundos."
					exit 2
				fi
				POLL_TIMEOUT="$2"
				shift 2
				;;
			--timeout=*)
				local valor="${1#*=}"
				if ! [[ "$valor" =~ ^[0-9]+$ ]] || [[ "$valor" -eq 0 ]]; then
					log_erro "--timeout inválido: '$valor'. Informe um inteiro positivo de segundos."
					exit 2
				fi
				POLL_TIMEOUT="$valor"
				shift
				;;
			--help|-h)
				mostrar_ajuda
				exit 0
				;;
			*)
				log_erro "Argumento desconhecido: '$1'. Use --help para ver as opções."
				exit 2
				;;
		esac
	done
}

# =========================================================
# Checagem de dependências
# =========================================================

verificar_dependencias() {
	local faltando=()

	command -v curl >/dev/null 2>&1 || faltando+=("curl")
	command -v jq   >/dev/null 2>&1 || faltando+=("jq")
	command -v docker >/dev/null 2>&1 || faltando+=("docker")

	if [[ ${#faltando[@]} -gt 0 ]]; then
		log_erro "Dependências ausentes: ${faltando[*]}"
		log_erro "Instale-as antes de rodar o smoke test (ex.: apt-get install ${faltando[*]})."
		exit 2
	fi

	# Detecta o subcomando do docker compose (v2) ou o binário legado docker-compose (v1).
	if docker compose version >/dev/null 2>&1; then
		DOCKER_COMPOSE=(docker compose)
	elif command -v docker-compose >/dev/null 2>&1; then
		DOCKER_COMPOSE=(docker-compose)
	else
		log_erro "Não encontrei 'docker compose' (v2) nem 'docker-compose' (v1)."
		exit 2
	fi

	if [[ ! -f "$COMPOSE_FILE" ]]; then
		log_erro "docker-compose.yml não encontrado em: $COMPOSE_FILE"
		exit 2
	fi
}

# =========================================================
# Wrapper de docker compose (aponta para o compose da raiz)
# =========================================================

compose() {
	"${DOCKER_COMPOSE[@]}" -f "$COMPOSE_FILE" "$@"
}

# =========================================================
# Chamada HTTP (captura status e body separadamente)
#
# Uso: http_call METODO URL [TOKEN] [BODY_JSON]
# Define: HTTP_STATUS, HTTP_BODY, HTTP_ERR
# =========================================================

http_call() {
	local metodo="$1"
	local url="$2"
	local token="${3:-}"
	local corpo="${4:-}"

	local -a args=(--silent --show-error --max-time "$HTTP_MAX_TIME" -X "$metodo" -H "Accept: application/json")
	if [[ -n "$token" ]]; then
		args+=(-H "Authorization: Bearer $token")
	fi
	if [[ -n "$corpo" ]]; then
		args+=(-H "Content-Type: application/json" --data "$corpo")
	fi

	local tmp_body tmp_err
	tmp_body="$(mktemp)"
	tmp_err="$(mktemp)"

	# %{http_code} vai para stdout; body para arquivo; erros de transporte para o arquivo de erro.
	HTTP_STATUS="$(curl "${args[@]}" -o "$tmp_body" -w '%{http_code}' "$url" 2>"$tmp_err")" || HTTP_STATUS="000"
	HTTP_BODY="$(cat "$tmp_body")"
	HTTP_ERR="$(cat "$tmp_err")"

	rm -f "$tmp_body" "$tmp_err"
}

# Imprime o body (e o erro de transporte, se houver) para diagnóstico.
diagnostico_http() {
	log_erro "HTTP status: ${HTTP_STATUS}"
	if [[ -n "$HTTP_ERR" ]]; then
		log_erro "curl: ${HTTP_ERR}"
	fi
	if [[ -n "$HTTP_BODY" ]]; then
		log_erro "Body: ${HTTP_BODY}"
	fi
}

# =========================================================
# Readiness
# =========================================================

# Espera GET {base}/health/live responder 200.
esperar_health() {
	local nome="$1"
	local base="$2"
	local i
	for ((i = 1; i <= READY_TENTATIVAS; i++)); do
		http_call GET "${base}/health/live"
		if [[ "$HTTP_STATUS" == "200" ]]; then
			log_ok "Serviço '${nome}' pronto (health/live 200) após ${i} tentativa(s)."
			return 0
		fi
		sleep "$READY_INTERVALO"
	done
	log_erro "Serviço '${nome}' não respondeu health/live 200 em ${base} após $((READY_TENTATIVAS * READY_INTERVALO))s."
	return 1
}

# Espera funcional da Conteúdo API (PEGADINHA 1: sem /health).
# Qualquer resposta HTTP (status != 000) em GET /api/Curso significa que o serviço está vivo.
esperar_conteudo() {
	local base="$1"
	local i
	for ((i = 1; i <= READY_TENTATIVAS; i++)); do
		http_call GET "${base}/api/Curso"
		if [[ "$HTTP_STATUS" != "000" ]]; then
			log_ok "Conteúdo API viva (GET /api/Curso respondeu HTTP ${HTTP_STATUS}) após ${i} tentativa(s)."
			return 0
		fi
		sleep "$READY_INTERVALO"
	done
	log_erro "Conteúdo API não respondeu em ${base}/api/Curso após $((READY_TENTATIVAS * READY_INTERVALO))s."
	return 1
}

# Verifica se o container da Conteúdo API está 'unhealthy' (PEGADINHA 1).
conteudo_unhealthy() {
	local status
	status="$(docker inspect --format '{{if .State.Health}}{{.State.Health.Status}}{{else}}none{{end}}' mba-conteudo-api 2>/dev/null || echo "desconhecido")"
	[[ "$status" == "unhealthy" ]]
}

# =========================================================
# Preparação do ambiente
# =========================================================

preparar_ambiente() {
	log_passo "Preparação do ambiente"

	if [[ "$SKIP_UP" == true ]]; then
		log_info "--skip-up: assumindo ambiente já no ar (não executo docker compose up)."
	else
		log_info "Subindo ambiente: ${DOCKER_COMPOSE[*]} up -d --build (pode levar alguns minutos no build)."
		# Não aborta em caso de erro: bff-api/webapp-mvc podem falhar por causa da PEGADINHA 1
		# (conteudo-api unhealthy). O contorno com --no-deps é feito logo abaixo.
		if compose up -d --build; then
			log_ok "docker compose up concluído (serviços independentes iniciados)."
		else
			log_warn "docker compose up retornou erro (esperado se bff/webapp dependerem da conteudo-api unhealthy)."
		fi
	fi

	# Readiness dos serviços com /health/live necessários ao fluxo.
	local prep_ok=true

	if esperar_health "Auth API" "$AUTH_URL"; then
		registrar_passo "PASS" "Readiness: Auth API (health/live)"
	else
		registrar_passo "FAIL" "Readiness: Auth API (health/live)"
		prep_ok=false
	fi

	if esperar_health "Aluno API" "$ALUNO_URL"; then
		registrar_passo "PASS" "Readiness: Aluno API (health/live)"
	else
		registrar_passo "FAIL" "Readiness: Aluno API (health/live)"
		prep_ok=false
	fi

	if esperar_health "Pagamentos API" "$PAGAMENTOS_URL"; then
		registrar_passo "PASS" "Readiness: Pagamentos API (health/live)"
	else
		registrar_passo "FAIL" "Readiness: Pagamentos API (health/live)"
		prep_ok=false
	fi

	# Conteúdo API: checagem funcional (PEGADINHA 1).
	if esperar_conteudo "$CONTEUDO_URL"; then
		registrar_passo "PASS" "Readiness: Conteúdo API (checagem funcional GET /api/Curso)"
	else
		registrar_passo "FAIL" "Readiness: Conteúdo API (checagem funcional GET /api/Curso)"
		prep_ok=false
	fi

	# PEGADINHA 1: detectar conteudo-api unhealthy e contornar a subida do BFF/WebApp.
	if conteudo_unhealthy; then
		registrar_finding "PEGADINHA 1 confirmada: o container 'mba-conteudo-api' está 'unhealthy' (Conteúdo API sem endpoint /health/live). O bff-api (depends_on service_healthy) não sobe normalmente."
		if [[ "$SKIP_UP" == false ]]; then
			log_info "Contorno: ${DOCKER_COMPOSE[*]} up -d --no-deps bff-api webapp-mvc (ignora a dependência unhealthy)."
			if compose up -d --no-deps bff-api webapp-mvc; then
				log_ok "Contorno aplicado: bff-api e webapp-mvc iniciados com --no-deps."
				registrar_passo "PASS" "Contorno PEGADINHA 1: bff-api/webapp-mvc via --no-deps"
			else
				log_erro "Falha ao aplicar o contorno --no-deps para bff-api/webapp-mvc."
				registrar_passo "FAIL" "Contorno PEGADINHA 1: bff-api/webapp-mvc via --no-deps"
				prep_ok=false
			fi
		else
			log_warn "--skip-up ativo: não aplico o contorno --no-deps. Se o BFF não estiver no ar, a readiness abaixo falhará."
		fi
	fi

	# BFF: readiness após eventual contorno (o pagamento faz RPC síncrono cujo responder está no BFF).
	if esperar_health "BFF API" "$BFF_URL"; then
		registrar_passo "PASS" "Readiness: BFF API (health/live)"
	else
		registrar_passo "FAIL" "Readiness: BFF API (health/live)"
		prep_ok=false
	fi

	if [[ "$prep_ok" == true ]]; then
		return 0
	fi
	return 1
}

# =========================================================
# Fluxo E2E
# =========================================================

# Variáveis compartilhadas entre os passos do fluxo.
ALUNO_TOKEN=""
ALUNO_ID=""
ALUNO_EMAIL=""
ADMIN_TOKEN=""
CURSO_ID=""
CURSO_NOME=""
CURSO_VALOR=""
AULA_ID=""
MATRICULA_ID=""

# Passo 1 - Registro do aluno (Auth API): cria usuário E aluno (RPC RabbitMQ -> Aluno API).
passo_registro() {
	log_passo "Passo 1 - Registro do aluno (Auth API)"
	local ts corpo
	ts="$(date +%s)"
	ALUNO_EMAIL="smoke+${ts}@teste.com"
	corpo="$(jq -n \
		--arg nomeUsuario "SmokeTeste${ts}" \
		--arg email "$ALUNO_EMAIL" \
		--arg senha "$ALUNO_SENHA" \
		'{nomeUsuario: $nomeUsuario, email: $email, senha: $senha, senhaConfirmacao: $senha, administrador: false}')"

	http_call POST "${AUTH_URL}/api/identidade/nova-conta" "" "$corpo"
	if [[ "$HTTP_STATUS" != "200" && "$HTTP_STATUS" != "201" ]]; then
		log_erro "Registro do aluno falhou."
		diagnostico_http
		registrar_passo "FAIL" "Passo 1 - Registro do aluno (Auth) [HTTP ${HTTP_STATUS}]"
		marcar_falha
		return 1
	fi

	# Resposta SEM envelope (objeto direto): accessToken + usuarioToken.id.
	ALUNO_TOKEN="$(printf '%s' "$HTTP_BODY" | jq -r '.accessToken // empty')"
	ALUNO_ID="$(printf '%s' "$HTTP_BODY" | jq -r '.usuarioToken.id // empty')"

	if [[ -z "$ALUNO_TOKEN" || -z "$ALUNO_ID" ]]; then
		log_erro "Registro respondeu ${HTTP_STATUS} mas sem accessToken/usuarioToken.id."
		diagnostico_http
		registrar_passo "FAIL" "Passo 1 - Registro do aluno (Auth) [sem token/id]"
		marcar_falha
		return 1
	fi

	log_ok "Aluno registrado: ${ALUNO_EMAIL} (alunoId=${ALUNO_ID})."
	registrar_passo "PASS" "Passo 1 - Registro do aluno (Auth)"
	return 0
}

# Passo 2 - Login do admin (Auth API).
passo_login_admin() {
	log_passo "Passo 2 - Login do admin (Auth API)"
	local corpo
	corpo="$(jq -n \
		--arg email "$ADMIN_EMAIL" \
		--arg senha "$ADMIN_SENHA" \
		'{email: $email, senha: $senha}')"

	http_call POST "${AUTH_URL}/api/identidade/autenticar" "" "$corpo"
	if [[ "$HTTP_STATUS" != "200" ]]; then
		log_erro "Login do admin falhou."
		diagnostico_http
		registrar_passo "FAIL" "Passo 2 - Login do admin (Auth) [HTTP ${HTTP_STATUS}]"
		marcar_falha
		return 1
	fi

	ADMIN_TOKEN="$(printf '%s' "$HTTP_BODY" | jq -r '.accessToken // empty')"
	if [[ -z "$ADMIN_TOKEN" ]]; then
		log_erro "Login do admin respondeu 200 mas sem accessToken."
		diagnostico_http
		registrar_passo "FAIL" "Passo 2 - Login do admin (Auth) [sem token]"
		marcar_falha
		return 1
	fi

	log_ok "Admin autenticado (${ADMIN_EMAIL})."
	registrar_passo "PASS" "Passo 2 - Login do admin (Auth)"
	return 0
}

# Passo 3 - Listar cursos (Conteúdo API) com token ADMIN.
passo_listar_cursos() {
	log_passo "Passo 3 - Listar cursos (Conteúdo API, token admin)"

	http_call GET "${CONTEUDO_URL}/api/Curso" "$ADMIN_TOKEN"
	if [[ "$HTTP_STATUS" != "200" ]]; then
		log_erro "Listagem de cursos falhou."
		diagnostico_http
		registrar_passo "FAIL" "Passo 3 - Listar cursos (Conteúdo) [HTTP ${HTTP_STATUS}]"
		marcar_falha
		return 1
	fi

	# Envelope: { success, type, result:[ { id, nome, valor, aulas:[{id,...}] } ] }
	CURSO_ID="$(printf '%s' "$HTTP_BODY" | jq -r '.result[0].id // empty')"
	CURSO_NOME="$(printf '%s' "$HTTP_BODY" | jq -r '.result[0].nome // empty')"
	CURSO_VALOR="$(printf '%s' "$HTTP_BODY" | jq -r '.result[0].valor // empty')"
	AULA_ID="$(printf '%s' "$HTTP_BODY" | jq -r '.result[0].aulas[0].id // empty')"

	if [[ -z "$CURSO_ID" || -z "$CURSO_VALOR" ]]; then
		log_erro "Não consegui extrair cursoId/valor do primeiro curso."
		diagnostico_http
		registrar_passo "FAIL" "Passo 3 - Listar cursos (Conteúdo) [curso inválido]"
		marcar_falha
		return 1
	fi

	log_ok "Curso selecionado: '${CURSO_NOME}' (cursoId=${CURSO_ID}, valor=${CURSO_VALOR}, aulaId=${AULA_ID:-<nenhuma>})."
	registrar_passo "PASS" "Passo 3 - Listar cursos (Conteúdo)"
	return 0
}

# Passo 4 - Matricular (Aluno API) - sem auth efetiva.
passo_matricular() {
	log_passo "Passo 4 - Matricular aluno (Aluno API)"
	local corpo
	corpo="$(jq -n \
		--arg cursoId "$CURSO_ID" \
		--arg alunoId "$ALUNO_ID" \
		'{cursoId: $cursoId, alunoId: $alunoId}')"

	http_call POST "${ALUNO_URL}/api/Aluno/matricular-aluno" "$ALUNO_TOKEN" "$corpo"
	if [[ "$HTTP_STATUS" != "200" && "$HTTP_STATUS" != "201" ]]; then
		if [[ "$HTTP_BODY" == *"403"* || "$HTTP_STATUS" == "403" ]]; then
			registrar_finding "Matrícula com token de ALUNO falhou (HTTP ${HTTP_STATUS}): a validação de curso na Aluno API repassa o token do chamador (AuthorizationForwardingHandler) para a Conteúdo API, cujo endpoint de curso exige a claim Cursos/VI que aluno comum não possui. Aluno não consegue se matricular sozinho. Retentando com token ADMIN para validar o restante do fluxo."
			log_warn "Matrícula com token de aluno negada; retentando com token admin (diagnóstico)."
			http_call POST "${ALUNO_URL}/api/Aluno/matricular-aluno" "$ADMIN_TOKEN" "$corpo"
		fi
		if [[ "$HTTP_STATUS" != "200" && "$HTTP_STATUS" != "201" ]]; then
			log_erro "Matrícula falhou."
			diagnostico_http
			registrar_passo "FAIL" "Passo 4 - Matricular aluno (Aluno) [HTTP ${HTTP_STATUS}]"
			marcar_falha
			return 1
		fi
		log_ok "Matrícula criada com token admin (fallback diagnóstico)."
		registrar_passo "PASS" "Passo 4 - Matricular aluno (Aluno) [via fallback admin]"
		return 0
	fi

	log_ok "Matrícula criada (status inicial esperado: PendentePagamento)."
	registrar_passo "PASS" "Passo 4 - Matricular aluno (Aluno)"
	return 0
}

# Passo 5 - Descobrir matriculaId (Aluno API) via GET {alunoId}/PorId, filtrando por cursoId.
passo_descobrir_matricula() {
	log_passo "Passo 5 - Descobrir matriculaId (Aluno API, PorId)"

	http_call GET "${ALUNO_URL}/api/Aluno/${ALUNO_ID}/PorId" "$ALUNO_TOKEN"
	if [[ "$HTTP_STATUS" != "200" ]]; then
		log_erro "Consulta PorId falhou."
		diagnostico_http
		registrar_passo "FAIL" "Passo 5 - Descobrir matriculaId (Aluno) [HTTP ${HTTP_STATUS}]"
		marcar_falha
		return 1
	fi

	MATRICULA_ID="$(printf '%s' "$HTTP_BODY" | jq -r --arg cid "$CURSO_ID" '.result.matriculas[]? | select(.cursoId == $cid) | .id' | head -n1)"
	if [[ -z "$MATRICULA_ID" ]]; then
		log_erro "Não encontrei matrícula para cursoId=${CURSO_ID} na resposta PorId."
		diagnostico_http
		registrar_passo "FAIL" "Passo 5 - Descobrir matriculaId (Aluno) [não encontrada]"
		marcar_falha
		return 1
	fi

	log_ok "matriculaId descoberto: ${MATRICULA_ID}."
	registrar_passo "PASS" "Passo 5 - Descobrir matriculaId (Aluno)"
	return 0
}

# Passo 6 - Pagar (Pagamentos API). PEGADINHA 3: pode dar 403 com o token do aluno.
passo_pagar() {
	log_passo "Passo 6 - Registrar pagamento (Pagamentos API)"
	local data_matricula corpo
	data_matricula="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
	# Monta o corpo JSON com jq -n (--arg/--argjson) para escapar corretamente todos os valores dinâmicos.
	corpo="$(jq -n \
		--arg alunoId "$ALUNO_ID" \
		--arg cursoId "$CURSO_ID" \
		--arg matriculaCursoId "$MATRICULA_ID" \
		--arg nomeCurso "$CURSO_NOME" \
		--arg dataMatricula "$data_matricula" \
		--argjson valor "$CURSO_VALOR" \
		--arg numeroCartao "$CARTAO_NUMERO" \
		--arg nomeTitularCartao "$CARTAO_TITULAR" \
		--arg validadeCartao "$CARTAO_VALIDADE" \
		--arg cvvCartao "$CARTAO_CVV" \
		'{
			alunoId: $alunoId,
			cursoId: $cursoId,
			matriculaCursoId: $matriculaCursoId,
			pagamentoPodeSerRealizado: true,
			nomeCurso: $nomeCurso,
			dataMatricula: $dataMatricula,
			dataConclusao: null,
			estadoMatricula: "PendentePagamento",
			valor: $valor,
			numeroCartao: $numeroCartao,
			nomeTitularCartao: $nomeTitularCartao,
			validadeCartao: $validadeCartao,
			cvvCartao: $cvvCartao
		}')"

	# Tentativa 1: com o token do aluno (fluxo desenhado).
	http_call POST "${PAGAMENTOS_URL}/api/Faturamento/${ALUNO_ID}/registrar-pagamento" "$ALUNO_TOKEN" "$corpo"

	if [[ "$HTTP_STATUS" == "403" ]]; then
		registrar_finding "PEGADINHA 3 confirmada: POST registrar-pagamento retornou 403 com o token do aluno (o endpoint exige Administrador/PG E Alunos/PG; o aluno comum não possui Administrador/PG). Refazendo com o token admin para validar o restante do pipeline."
		log_warn "Pagamento 403 com o token do aluno. Refazendo com o token do admin."
		http_call POST "${PAGAMENTOS_URL}/api/Faturamento/${ALUNO_ID}/registrar-pagamento" "$ADMIN_TOKEN" "$corpo"
	fi

	if [[ "$HTTP_STATUS" != "200" && "$HTTP_STATUS" != "201" ]]; then
		log_erro "Registro de pagamento falhou (inclusive após eventual retentativa com o token admin)."
		diagnostico_http
		# Rotula o finding conforme o status recebido:
		#   403         => problema de autorização do endpoint (claims Administrador/PG E Alunos/PG) - PEGADINHA 3;
		#   401/400/500 => provável falha da validação interna na Aluno API sem esquema JWT - PEGADINHA 2.
		if [[ "$HTTP_STATUS" == "403" ]]; then
			registrar_finding "PEGADINHA 3: o registro de pagamento retornou 403 mesmo após a retentativa com o token admin. O endpoint exige as claims Administrador/PG E Alunos/PG; nem o token do aluno nem o do admin satisfazem ambas as exigências de autorização."
		elif [[ "$HTTP_STATUS" == "401" || "$HTTP_STATUS" == "400" || "$HTTP_STATUS" == "500" ]]; then
			registrar_finding "PEGADINHA 2 provável: o registro de pagamento não completou (HTTP ${HTTP_STATUS}). A Pagamentos API valida a matrícula na Aluno API (GET api/aluno/matricula/{id}/status), que tende a falhar porque a Aluno API não registra esquema de autenticação JWT."
		fi
		registrar_passo "FAIL" "Passo 6 - Registrar pagamento (Pagamentos) [HTTP ${HTTP_STATUS}]"
		marcar_falha
		return 1
	fi

	log_ok "Pagamento aceito para processamento (HTTP ${HTTP_STATUS}). A confirmação é assíncrona."
	registrar_passo "PASS" "Passo 6 - Registrar pagamento (Pagamentos)"
	return 0
}

# Passo 7 - Polling do status até PagamentoRealizado (2) com timeout configurável.
passo_confirmar_status() {
	log_passo "Passo 7 - Polling do status da matrícula (até PagamentoRealizado, timeout ${POLL_TIMEOUT}s)"
	local tentativas status ultimo_status="<desconhecido>" i
	tentativas=$(( (POLL_TIMEOUT + POLL_INTERVALO - 1) / POLL_INTERVALO ))
	[[ "$tentativas" -lt 1 ]] && tentativas=1

	for ((i = 1; i <= tentativas; i++)); do
		http_call GET "${ALUNO_URL}/api/Aluno/${ALUNO_ID}/PorId" "$ALUNO_TOKEN"
		if [[ "$HTTP_STATUS" == "200" ]]; then
			status="$(printf '%s' "$HTTP_BODY" | jq -r --arg cid "$CURSO_ID" '.result.matriculas[]? | select(.cursoId == $cid) | .status' | head -n1)"
			[[ -n "$status" ]] && ultimo_status="$status"
			# Aceita o número 2 OU a string "PagamentoRealizado".
			if [[ "$status" == "2" || "$status" == "PagamentoRealizado" ]]; then
				log_ok "Status confirmado: PagamentoRealizado após ${i} verificação(ões)."
				registrar_passo "PASS" "Passo 7 - Confirmação assíncrona (status PagamentoRealizado)"
				return 0
			fi
			# Recusa explícita: encerra sem esperar todo o timeout.
			if [[ "$status" == "5" || "$status" == "PagamentoRecusado" ]]; then
				log_erro "Status final: PagamentoRecusado (${status})."
				registrar_finding "Pagamento RECUSADO (status ${status}). Possível PEGADINHA 2: falha na validação da matrícula na Aluno API (autenticação JWT não registrada) ou valor divergente."
				registrar_passo "FAIL" "Passo 7 - Confirmação assíncrona (status PagamentoRecusado)"
				marcar_falha
				return 1
			fi
		fi
		sleep "$POLL_INTERVALO"
	done

	log_erro "Timeout de ${POLL_TIMEOUT}s sem atingir PagamentoRealizado. Último status observado: ${ultimo_status}."
	registrar_finding "PEGADINHA 2 provável: após ${POLL_TIMEOUT}s o status permaneceu em '${ultimo_status}' (esperado 2/PagamentoRealizado). A confirmação assíncrona depende da validação na Aluno API, que pode estar quebrada por ausência de esquema de autenticação JWT."
	registrar_passo "FAIL" "Passo 7 - Confirmação assíncrona [timeout, último status: ${ultimo_status}]"
	marcar_falha
	return 1
}

# Passo 8 - Opcional/diagnóstico: aula assistida + conclusão (PEGADINHA 2 => provável 403). WARN, nunca FAIL.
passo_opcional_aula() {
	log_passo "Passo 8 - Opcional: aula assistida e conclusão (diagnóstico, PEGADINHA 2)"

	if [[ -z "$AULA_ID" ]]; then
		log_warn "Nenhuma aulaId disponível; pulando o registro de aula assistida."
		registrar_passo "WARN" "Passo 8a - Aula assistida (sem aulaId disponível)"
	else
		local corpo_aula
		corpo_aula="$(jq -n \
			--arg alunoId "$ALUNO_ID" \
			--arg matriculaId "$MATRICULA_ID" \
			--arg aulaId "$AULA_ID" \
			'{alunoId: $alunoId, matriculaId: $matriculaId, aulaId: $aulaId}')"
		http_call POST "${ALUNO_URL}/api/Aluno/registrar-aula-assistida" "$ALUNO_TOKEN" "$corpo_aula"
		if [[ "$HTTP_STATUS" == "200" || "$HTTP_STATUS" == "201" ]]; then
			log_ok "Aula assistida registrada (HTTP ${HTTP_STATUS})."
			registrar_passo "WARN" "Passo 8a - Aula assistida OK (HTTP ${HTTP_STATUS}) [passo opcional]"
		else
			log_warn "Aula assistida retornou HTTP ${HTTP_STATUS} (não afeta o fluxo principal)."
			if [[ "$HTTP_STATUS" == "401" || "$HTTP_STATUS" == "403" ]]; then
				registrar_finding "PEGADINHA 2 possível: registrar-aula-assistida retornou ${HTTP_STATUS} (o endpoint exige UserId==alunoId; a Aluno API pode não validar o JWT corretamente)."
			fi
			registrar_passo "WARN" "Passo 8a - Aula assistida (HTTP ${HTTP_STATUS}) [passo opcional]"
		fi
	fi

	local corpo_conclusao
	corpo_conclusao="$(jq -n \
		--arg alunoId "$ALUNO_ID" \
		--arg matriculaId "$MATRICULA_ID" \
		'{alunoId: $alunoId, matriculaId: $matriculaId}')"
	http_call PUT "${ALUNO_URL}/api/Aluno/concluir-curso" "$ALUNO_TOKEN" "$corpo_conclusao"
	if [[ "$HTTP_STATUS" == "200" || "$HTTP_STATUS" == "201" || "$HTTP_STATUS" == "204" ]]; then
		log_ok "Conclusão de curso aceita (HTTP ${HTTP_STATUS})."
		registrar_passo "WARN" "Passo 8b - Conclusão de curso OK (HTTP ${HTTP_STATUS}) [passo opcional]"
	else
		log_warn "Conclusão de curso retornou HTTP ${HTTP_STATUS} (não afeta o fluxo principal)."
		if [[ "$HTTP_STATUS" == "401" || "$HTTP_STATUS" == "403" ]]; then
			registrar_finding "PEGADINHA 2 possível: concluir-curso retornou ${HTTP_STATUS} (o endpoint exige UserId==alunoId; a Aluno API pode não validar o JWT corretamente)."
		fi
		registrar_passo "WARN" "Passo 8b - Conclusão de curso (HTTP ${HTTP_STATUS}) [passo opcional]"
	fi

	return 0
}

# Orquestra os passos do fluxo. Cada passo crítico interrompe a sequência principal em caso de falha,
# mas os passos opcionais (8) são sempre executados quando há dados suficientes.
executar_fluxo() {
	passo_registro          || return 1
	passo_login_admin       || return 1
	passo_listar_cursos     || return 1
	passo_matricular        || return 1
	passo_descobrir_matricula || return 1

	# Pagamento e confirmação: mesmo em falha, seguimos para os passos opcionais de diagnóstico.
	passo_pagar
	passo_confirmar_status

	passo_opcional_aula
	return 0
}

# =========================================================
# Relatório final
# =========================================================

imprimir_relatorio() {
	RELATORIO_IMPRESSO=true
	local item status desc cor

	printf '\n'
	printf '%s\n' "${C_BOLD}================= RELATORIO =================${C_RESET}"
	if [[ ${#REPORT_STEPS[@]} -eq 0 ]]; then
		printf '  %s\n' "Nenhum passo executado."
	else
		for item in "${REPORT_STEPS[@]}"; do
			status="${item%%|*}"
			desc="${item#*|}"
			case "$status" in
				PASS) cor="$C_GREEN" ;;
				FAIL) cor="$C_RED" ;;
				WARN) cor="$C_YELLOW" ;;
				*)    cor="$C_RESET" ;;
			esac
			printf '  [%s%-4s%s] %s\n' "$cor" "$status" "$C_RESET" "$desc"
		done
	fi

	printf '\n'
	printf '%s\n' "${C_BOLD}================= FINDINGS =================${C_RESET}"
	if [[ ${#FINDINGS[@]} -eq 0 ]]; then
		printf '  %s\n' "Nenhuma divergência detectada em tempo de execução."
	else
		for item in "${FINDINGS[@]}"; do
			printf '  %s- %s\n' "$C_YELLOW" "${item}${C_RESET}"
		done
	fi

	printf '\n'
	if [[ "$FLUXO_OK" == true ]]; then
		printf '%s\n' "${C_GREEN}${C_BOLD}RESULTADO: fluxo principal PASSOU (exit 0).${C_RESET}"
	else
		printf '%s\n' "${C_RED}${C_BOLD}RESULTADO: fluxo principal FALHOU (exit 1).${C_RESET}"
	fi
}

# =========================================================
# Teardown / saída
# =========================================================

executar_down() {
	if [[ "$DOWN_EXECUTADO" == true ]]; then
		return 0
	fi
	DOWN_EXECUTADO=true
	log_passo "Teardown: ${DOCKER_COMPOSE[*]} down -v"
	if compose down -v; then
		log_ok "Ambiente removido (containers e volumes)."
	else
		log_warn "Falha ao executar docker compose down -v."
	fi
}

on_exit() {
	local rc=$?
	if [[ "$RELATORIO_IMPRESSO" == false ]]; then
		imprimir_relatorio
	fi
	# Teardown APENAS com a flag --down. Em caso de falha sem --down, os containers ficam no ar
	# de propósito, para que o passo de coleta de logs do CI consiga inspecioná-los.
	if [[ "$FLAG_DOWN" == true ]]; then
		executar_down
	fi
	exit "$rc"
}

# =========================================================
# Main
# =========================================================

main() {
	parse_args "$@"
	verificar_dependencias

	log_info "Alvos: auth=${AUTH_URL} conteudo=${CONTEUDO_URL} aluno=${ALUNO_URL} pagamentos=${PAGAMENTOS_URL} bff=${BFF_URL}"
	log_info "Opções: skip-up=${SKIP_UP} down=${FLAG_DOWN} timeout(polling)=${POLL_TIMEOUT}s"

	# A partir daqui, garante o relatório e o eventual teardown na saída.
	trap on_exit EXIT

	if preparar_ambiente; then
		executar_fluxo
	else
		log_erro "A preparação do ambiente falhou; abortando o fluxo E2E."
		marcar_falha
	fi

	imprimir_relatorio

	if [[ "$FLUXO_OK" == true ]]; then
		EXIT_CODE=0
	else
		EXIT_CODE=1
	fi

	# on_exit (trap) roda o teardown se solicitado e preserva o código de saída.
	exit "$EXIT_CODE"
}

main "$@"
