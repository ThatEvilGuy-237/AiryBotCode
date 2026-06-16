# Caddy reverse proxy serving the two built SPAs (the main control panel and the
# standalone login app). Tailscale terminates TLS; Caddy runs plain HTTP (SPA
# serving + /api routing), so no DNS plugin / custom Caddy build is needed.

# The API base is baked into both frontends at build time so API calls and the
# Discord redirect URI target the public domain.
ARG API_BASE=https://thatevilserver.tail2a87af.ts.net

# Build the main control-panel SPA (served at /).
FROM node:22-alpine AS frontend
ARG API_BASE
WORKDIR /app
ENV VITE_API_BASE_URL=$API_BASE
# vendor/ holds the vendored @hive/ui (a file: dependency) — it must be present
# BEFORE npm ci so the local dependency resolves.
COPY AiryBotCode.Frontend/AiryWebApp/package*.json ./
COPY AiryBotCode.Frontend/AiryWebApp/vendor/ ./vendor/
RUN npm ci
COPY AiryBotCode.Frontend/AiryWebApp/ ./
RUN npm run build

# Build the standalone login SPA (served at /login).
FROM node:22-alpine AS login
ARG API_BASE
WORKDIR /app
ENV VITE_API_BASE_URL=$API_BASE
COPY AiryBotCode.Frontend/AiryLogin/package*.json ./
RUN npm ci
COPY AiryBotCode.Frontend/AiryLogin/ ./
RUN npm run build

# Final image: stock Caddy + the two built SPAs.
FROM caddy:2.10-alpine
COPY --from=frontend /app/dist /srv
COPY --from=login /app/dist /srv-login
COPY Caddyfile /etc/caddy/Caddyfile
