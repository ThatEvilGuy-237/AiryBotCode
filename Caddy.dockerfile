# Caddy reverse proxy serving the built Vue control panel (AiryWebApp).
# Tailscale terminates TLS; Caddy runs plain HTTP (password gate + SPA + /api),
# so no DNS plugin / custom Caddy build is needed.

# Build the Vue frontend to static files. The API base is baked in at build time
# so API calls and the Discord redirect URI target the public domain.
FROM node:22-alpine AS frontend
WORKDIR /app
ENV VITE_API_BASE_URL=https://thatevilserver.tail2a87af.ts.net
COPY AiryBotCode.Frontend/AiryWebApp/package*.json ./
RUN npm ci
COPY AiryBotCode.Frontend/AiryWebApp/ ./
RUN npm run build

# Final image: stock Caddy + the built SPA.
FROM caddy:2.10-alpine
COPY --from=frontend /app/dist /srv
COPY Caddyfile /etc/caddy/Caddyfile
