Write-Host "🚀 Configuration complète TRSB Docker avec HTTPS" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# Vérifier les prérequis
$commands = @("docker", "docker-compose", "dotnet")
foreach ($cmd in $commands) {
    if (!(Get-Command $cmd -ErrorAction SilentlyContinue)) {
        Write-Host "❌ $cmd n'est pas installé" -ForegroundColor Red
        exit 1
    }
}
Write-Host "✅ Prérequis vérifiés" -ForegroundColor Green

# Créer .env
if (Test-Path .env) {
    $response = Read-Host "⚠️  .env existe déjà. Écraser? (y/N)"
    if ($response -eq 'y' -or $response -eq 'Y') {
        Copy-Item .env.example .env
        Write-Host "✅ . env créé" -ForegroundColor Green
    } else {
        Write-Host "Configuration .env conservée" -ForegroundColor Yellow
    }
} else {
    Copy-Item . env.example .env
    Write-Host "✅ .env créé" -ForegroundColor Green
}

# Générer les secrets
Write-Host "🔐 Génération des secrets..." -ForegroundColor Yellow

$sqlBytes = New-Object byte[] 16
$jwtBytes = New-Object byte[] 64
$certBytes = New-Object byte[] 12
[Security.Cryptography.RNGCryptoServiceProvider]::Create().GetBytes($sqlBytes)
[Security.Cryptography.RNGCryptoServiceProvider]:: Create().GetBytes($jwtBytes)
[Security.Cryptography.RNGCryptoServiceProvider]::Create().GetBytes($certBytes)

$SQL_PASSWORD = [Convert]::ToBase64String($sqlBytes) + "Aa1!"
$JWT_KEY = [Convert]::ToBase64String($jwtBytes)
$CERT_PASSWORD = [Convert]::ToBase64String($certBytes) + "Aa1!"

(Get-Content .env) -replace 'SQL_SA_PASSWORD=.*', "SQL_SA_PASSWORD=$SQL_PASSWORD" | Set-Content . env
(Get-Content .env) -replace 'JWT_SIGNING_KEY=.*', "JWT_SIGNING_KEY=$JWT_KEY" | Set-Content .env
(Get-Content .env) -replace 'ASPNETCORE_Kestrel__Certificates__Default__Password=.*', "ASPNETCORE_Kestrel__Certificates__Default__Password=$CERT_PASSWORD" | Set-Content .env

Write-Host "✅ Secrets générés" -ForegroundColor Green

# Créer les certificats
Write-Host "📜 Génération des certificats SSL..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "certs/api" | Out-Null
New-Item -ItemType Directory -Force -Path "certs/web" | Out-Null

dotnet dev-certs https -ep certs/api/aspnetapp.pfx -p $CERT_PASSWORD --trust
Copy-Item "certs/api/aspnetapp.pfx" "certs/web/aspnetapp.pfx"

Write-Host "✅ Certificats créés" -ForegroundColor Green

# Démarrer
Write-Host "🐳 Démarrage de Docker Compose..." -ForegroundColor Cyan
docker-compose up -d --build

Write-Host ""
Write-Host "✅ Configuration terminée!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Informations de connexion:" -ForegroundColor Yellow
Write-Host "   SQL Password: $SQL_PASSWORD"
Write-Host "   Cert Password:  $CERT_PASSWORD"
Write-Host ""
Write-Host "🌐 Accès aux applications:" -ForegroundColor Cyan
Write-Host "   Web HTTPS:  https://localhost:8444"
Write-Host "   Web HTTP:   http://localhost:8081"
Write-Host "   API HTTPS:  https://localhost:8443"
Write-Host "   API HTTP:   http://localhost:8080"
Write-Host "   Swagger:     https://localhost:8443/swagger"
Write-Host ""
Write-Host "📊 Vérifier les logs:" -ForegroundColor Cyan
Write-Host "   docker-compose logs -f"
Write-Host ""