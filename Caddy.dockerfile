FROM node:22-alpine AS panel
WORKDIR /app
COPY AiryBotCode.Frontend/AiryPanel/package*.json ./
COPY AiryBotCode.Frontend/AiryPanel/vendor/ ./vendor/
RUN npm ci
COPY AiryBotCode.Frontend/AiryPanel/ ./
RUN npm run build

FROM caddy:2.10-alpine
COPY --from=panel /app/dist /srv
COPY Caddyfile /etc/caddy/Caddyfile
