# Сборка проекта
FROM node:18-alpine AS build
WORKDIR /app

# Копируем модули и устанавливаем зависимости
COPY package*.json ./
RUN npm install

# Копируем исходный код и собираем проект
COPY . .
RUN npm run build -- --configuration production

# runtime с nginx
FROM nginx:alpine

# копируем собранное приложение в папку nginx
COPY --from=build /app/dist/device-monitoring-ui /usr/share/nginx/html

# копируем файл конфигурации nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80