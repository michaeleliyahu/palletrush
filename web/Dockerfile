FROM node:24.22.0-alpine
ENV NG_CLI_ANALYTICS=false
WORKDIR /app
COPY web/package.json web/package-lock.json ./
RUN npm ci
COPY web/ ./
EXPOSE 4200
CMD ["npx", "ng", "serve", "--host", "0.0.0.0"]
