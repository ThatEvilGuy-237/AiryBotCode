# Caddy reverse proxy serving the two built SPAs (the main control panel and the
# standalone login app). Tailscale terminates TLS; Caddy runs plain HTTP (SPA
# serving + /api routing), so no DNS plugin / custom Caddy build is needed.

# API base for both frontends. Empty by default → the SPAs call the API
# same-origin (window.location.origin), so the panel works on ANY hostname it's
# served from (thehive.thatevilguy.com, the ts.net funnel, etc.) with no rebuild.
# Override with a build arg only to pin an absolute base.
ARG API_BASE=

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
