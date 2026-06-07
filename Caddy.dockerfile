# Caddy reverse proxy with the DuckDNS DNS plugin (for DNS-01 HTTPS challenges),
# bundling the built Svelte control panel as static files.

# Stage 1: build Caddy with the DuckDNS DNS provider module.
# 2.10-builder ships Go 1.26 (the duckdns plugin needs Go >= 1.24).
FROM caddy:2.10-builder AS caddy-builder
RUN xcaddy build --with github.com/caddy-dns/duckdns

# Stage 2: build the Svelte frontend to static files.
FROM node:22-alpine AS frontend
WORKDIR /app
COPY AiryBotCode.Frontend/AiryWebpage/package*.json ./
RUN npm ci
COPY AiryBotCode.Frontend/AiryWebpage/ ./
RUN npm run build

# Final image.
FROM caddy:2.10-alpine
COPY --from=caddy-builder /usr/bin/caddy /usr/bin/caddy
COPY --from=frontend /app/dist /srv
COPY Caddyfile /etc/caddy/Caddyfile
