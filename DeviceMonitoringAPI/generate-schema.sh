#!/bin/bash
# Генерация схемы OpenAPI
API_URL="http://localhost:5040"
OUTPUT_FILE="api-schema.json"

# Запуск API
dotnet run &
API_PID=$!

# Ждём пока API запустится
sleep 5

# Скачиваем схему
curl -s "$API_URL/swagger/v1/swagger.json" -o "$OUTPUT_FILE"
echo "API schema saved to $OUTPUT_FILE"

# Останавливаем API
kill $API_PID
echo "API stopped"