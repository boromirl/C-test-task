# Генерация схемы OpenAPI
$apiUrl = "http://localhost:5040"
$outputFile = "api-schema.json"

# Запуск API
$process = Start-Process dotnet -ArgumentList "run" -PassThru

# Ждём пока API запустится
Start-Sleep -Seconds 5

# Скачиваем схему
try {
    Invoke-WebRequest -Uri "$apiUrl/swagger/v1/swagger.json" -OutFile $outputFile
    Write-Host "API schema saved to $outputFile"
}
catch {
    Write-Host "Failed to download schema: $($_.Exception.Message)"
}

# Останавливаем API
Stop-Process -Id $process.Id
Write-Host "API stopped"