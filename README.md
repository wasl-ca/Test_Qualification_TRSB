# TRSB - Système de Gestion des Comptes Employés

> **Proof of concept** développé en C# / .NET 8 pour la gestion sécurisée des comptes employés.  
> Architecture Clean, authentification JWT, déploiement Docker et IIS.

---

## 📑 Table des matières

- [Vue d'ensemble](#-vue-densemble)
- [Architecture](#-architecture)
- [Démarrage rapide](#-démarrage-rapide)
- [Déploiement Docker](#-déploiement-docker)
- [Déploiement IIS](#-déploiement-iis)
- [Tests](#-tests)
- [Configuration](#-configuration)
- [Sécurité](#-sécurité)

---

## 🎯 Vue d'ensemble

### Fonctionnalités

- ✅ Création et gestion de comptes employés
- ✅ Authentification sécurisée avec JWT
- ✅ Gestion de profil utilisateur
- ✅ Interface Web responsive (Bootstrap 5)
- ✅ API REST documentée (Swagger)
- ✅ Support HTTPS
- ✅ Tests unitaires et E2E

### Technologies

- **. NET 8** - Framework principal
- **ASP.NET Core** - Web API et MVC
- **Entity Framework Core** - ORM
- **SQL Server** - Base de données
- **JWT** - Authentification
- **MediatR** - Pattern CQRS
- **Bootstrap 5** - Interface utilisateur
- **Docker** - Containerisation
- **IIS** - Hébergement Windows

---

## 🏗 Architecture

### Structure du projet

```
Test_Qualification_TRSB/
│
├── src/
│   ├── TRSB.Api/              # 🔌 API REST (Backend)
│   ├── TRSB.Web/              # 🌐 Application Web (Frontend)
│   ├── TRSB.Application/      # 💼 Logique métier (CQRS, Use Cases)
│   ├── TRSB.Domain/           # 📦 Entités et règles métier
│   └── TRSB.Infrastructure/   # 🗄 Accès aux données (EF Core)
│
├── tests/
│   ├── TRSB.Api.Tests/        # Tests API
│   ├── TRSB.Application.Tests/# Tests logique métier
│   ├── TRSB.Domain.Tests/     # Tests domaine
│   ├── TRSB.E2E.Tests/        # Tests end-to-end
│   └── TRSB.Infrastructure. Tests/
│
├── docker-compose. yml         # Configuration Docker
├── Dockerfile. api             # Image Docker API
├── Dockerfile. web             # Image Docker Web
└── README.md
```

### Architecture Clean (DDD)

```
┌─────────────────────────────────────────────┐
│              Présentation                    │
│  ┌─────────────┐      ┌─────────────┐      │
│  │  TRSB. Web   │      │  TRSB.Api   │      │
│  │    (MVC)    │─────→│    (REST)   │      │
│  └─────────────┘      └─────────────┘      │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│           TRSB.Application                   │
│  (Use Cases, CQRS, Business Logic)          │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│             TRSB.Domain                      │
│  (Entités, Value Objects, Règles métier)    │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│          TRSB.Infrastructure                 │
│  (EF Core, SQL Server, Repositories)        │
└─────────────────────────────────────────────┘
```

### Communication entre composants

```
[Utilisateur] → [TRSB.Web] → [TRSB.Api] → [Application] → [Infrastructure] → [SQL Server]
                   HTTPS         JWT         CQRS           EF Core
```

---

## ⚡ Démarrage rapide

### Prérequis

- [. NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (ou Docker)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (pour déploiement Docker)

### Développement local

```bash
# 1. Cloner le repository
git clone https://github.com/wasl-ca/Test_Qualification_TRSB.git
cd Test_Qualification_TRSB

# 2. Configurer l'environnement
cp .env.example .env
# Éditez . env avec vos valeurs

# 3. Restaurer les dépendances
dotnet restore

# 4. Appliquer les migrations
cd src/TRSB.Api
dotnet ef database update

# 5. Lancer l'application
# Terminal 1 - API
cd src/TRSB.Api
dotnet run
# → https://localhost:7001

# Terminal 2 - Web
cd src/TRSB.Web
dotnet run
# → https://localhost:7002
```

**Accès :**
- Interface Web : https://localhost:7002
- API Swagger : https://localhost:7001/swagger

---

## 🐳 Déploiement Docker

### Configuration initiale (une seule fois)

```bash
# 1. Copier le fichier d'environnement
cp .env.example .env

# 2. Générer les secrets et certificats SSL
chmod +x setup-docker-https.sh
./setup-docker-https.sh
```

**Windows (PowerShell) :**
```powershell
.\setup-docker-https.ps1
```

### Démarrage

```bash
docker-compose up -d
```

### Accès aux applications

| Application | HTTP | HTTPS |
|------------|------|-------|
| **Interface Web** | http://localhost:8081 | https://localhost:8444 |
| **API** | http://localhost:8080 | https://localhost:8443 |
| **Swagger** | http://localhost:8080/swagger | https://localhost:8443/swagger |

### Commandes utiles

```bash
# Voir les logs
docker-compose logs -f

# Arrêter
docker-compose down

# Redémarrer
docker-compose restart

# Reconstruire
docker-compose up -d --build

# Nettoyer tout (⚠️ supprime les données)
docker-compose down -v
```

### Architecture Docker

```
┌──────────────────────────────────────────────┐
│          Docker Compose Network               │
├──────────────────────────────────────────────┤
│                                               │
│  📦 trsb-web (Port 8081/8444)                │
│  └─ HTTPS:  /certs/web/aspnetapp.pfx         │
│              ↓                                │
│  📦 trsb-api (Port 8080/8443)                │
│  └─ HTTPS: /certs/api/aspnetapp.pfx         │
│              ↓                                │
│  📦 trsb-sqlserver (Port 1433)               │
│  └─ Volume: sqlserver-data                   │
│                                               │
└──────────────────────────────────────────────┘
```

### Variables d'environnement Docker

Fichier `.env` :

```dotenv
# Base de données
SQL_SA_PASSWORD=VotreMotDePasseSQL123! 
SQL_DATABASE=TRSB_DB

# Ports
API_HTTP_PORT=8080
API_HTTPS_PORT=8443
WEB_HTTP_PORT=8081
WEB_HTTPS_PORT=8444

# JWT
JWT_ISSUER=TRSB-API
JWT_AUDIENCE=TRSB-Client
JWT_SIGNING_KEY=VotreCleSecrete64Caracteres... 
JWT_EXPIRE_MINUTES=60

# Certificats SSL
ASPNETCORE_Kestrel__Certificates__Default__Password=VotreMotDePasseCert123!
ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
```

### Régénérer les certificats SSL

```bash
# Linux/Mac
./generate-certs.sh

# Windows
.\generate-certs.ps1
```

---

## 🖥 Déploiement IIS (On-Premises)

### Prérequis

1. **Windows Server 2016+** ou **Windows 10/11 Pro**
2. **IIS 10+**
3. **SQL Server 2019+**
4. **.NET 8 Hosting Bundle**

### Installation IIS et . NET

```powershell
# 1. Installer IIS (PowerShell Admin)
Install-WindowsFeature -name Web-Server -IncludeManagementTools

# 2. Télécharger et installer . NET 8 Hosting Bundle
# https://dotnet.microsoft.com/download/dotnet/8.0

# 3. Redémarrer IIS
net stop was /y
net start w3svc
```

### Configuration SQL Server

```sql
-- 1. Créer la base de données
CREATE DATABASE TRSB_DB;
GO

-- 2. Créer l'utilisateur
USE [master];
GO
CREATE LOGIN [TRSB_User] WITH PASSWORD=N'VotreMotDePasse123!';
GO

USE [TRSB_DB];
GO
CREATE USER [TRSB_User] FOR LOGIN [TRSB_User];
GO
ALTER ROLE [db_owner] ADD MEMBER [TRSB_User];
GO
```

### Déploiement de l'application

#### 1. Publier les applications

```powershell
# Publier l'API
cd src\TRSB.Api
dotnet publish -c Release -o C:\inetpub\wwwroot\TRSB\Api

# Publier le Web
cd . .\TRSB.Web
dotnet publish -c Release -o C:\inetpub\wwwroot\TRSB\Web
```

#### 2. Appliquer les migrations

```powershell
cd src\TRSB.Api
dotnet ef database update --connection "Server=localhost\SQLEXPRESS;Database=TRSB_DB;User Id=TRSB_User;Password=VotreMotDePasse123! ;TrustServerCertificate=True;"
```

#### 3. Configurer les App Settings

**API** - `C:\inetpub\wwwroot\TRSB\Api\appsettings.Production.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TRSB_DB;User Id=TRSB_User;Password=VotreMotDePasse123!;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Issuer": "TRSB-API",
    "Audience": "TRSB-Client",
    "SigningKey": "VotreCleSecrete64Caracteres...",
    "ExpireMinutes": 60
  },
  "PasswordPolicy": {
    "MinLength":  8,
    "MinSpecialChars": 1
  }
}
```

**Web** - `C:\inetpub\wwwroot\TRSB\Web\appsettings.Production.json` :

```json
{
  "Api": {
    "BaseUrl": "https://localhost:8443"
  }
}
```

#### 4. Créer les Application Pools

```powershell
# Importer le module IIS
Import-Module WebAdministration

# Pool pour l'API
New-WebAppPool -Name "TRSB-API-Pool"
Set-ItemProperty IIS:\AppPools\TRSB-API-Pool -name "managedRuntimeVersion" -value ""

# Pool pour le Web
New-WebAppPool -Name "TRSB-Web-Pool"
Set-ItemProperty IIS:\AppPools\TRSB-Web-Pool -name "managedRuntimeVersion" -value ""
```

#### 5. Créer les sites IIS

```powershell
# Site API (port 8080 HTTP, 8443 HTTPS)
New-Website -Name "TRSB-API" `
  -Port 8080 `
  -PhysicalPath "C:\inetpub\wwwroot\TRSB\Api" `
  -ApplicationPool "TRSB-API-Pool"

# Site Web (port 80 HTTP, 443 HTTPS)
New-Website -Name "TRSB-Web" `
  -Port 80 `
  -PhysicalPath "C:\inetpub\wwwroot\TRSB\Web" `
  -ApplicationPool "TRSB-Web-Pool"
```

#### 6. Configurer les permissions

```powershell
# Permissions API
icacls "C:\inetpub\wwwroot\TRSB\Api" /grant "IIS AppPool\TRSB-API-Pool:(OI)(CI)F" /T

# Permissions Web
icacls "C:\inetpub\wwwroot\TRSB\Web" /grant "IIS AppPool\TRSB-Web-Pool:(OI)(CI)F" /T
```

#### 7. Configurer HTTPS

```powershell
# Générer un certificat auto-signé (développement)
$certApi = New-SelfSignedCertificate `
  -DnsName "trsb-api.local" `
  -CertStoreLocation "cert:\LocalMachine\My"

$certWeb = New-SelfSignedCertificate `
  -DnsName "trsb-web.local" `
  -CertStoreLocation "cert:\LocalMachine\My"

# Ajouter les bindings HTTPS
New-WebBinding -Name "TRSB-API" -Protocol https -Port 8443
New-WebBinding -Name "TRSB-Web" -Protocol https -Port 443

# Lier les certificats
$bindingApi = Get-WebBinding -Name "TRSB-API" -Protocol https
$bindingApi.AddSslCertificate($certApi. Thumbprint, "my")

$bindingWeb = Get-WebBinding -Name "TRSB-Web" -Protocol https
$bindingWeb.AddSslCertificate($certWeb. Thumbprint, "my")
```

#### 8. Configurer le pare-feu

```powershell
# Autoriser les ports
New-NetFirewallRule -DisplayName "TRSB Web HTTP" -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow
New-NetFirewallRule -DisplayName "TRSB Web HTTPS" -Direction Inbound -Protocol TCP -LocalPort 443 -Action Allow
New-NetFirewallRule -DisplayName "TRSB API HTTP" -Direction Inbound -Protocol TCP -LocalPort 8080 -Action Allow
New-NetFirewallRule -DisplayName "TRSB API HTTPS" -Direction Inbound -Protocol TCP -LocalPort 8443 -Action Allow
```

#### 9. Démarrer les sites

```powershell
Start-Website -Name "TRSB-API"
Start-Website -Name "TRSB-Web"

# Vérifier
Get-Website
```

### Accès aux applications IIS

| Application | URL |
|------------|-----|
| **Interface Web** | http://localhost ou https://localhost |
| **API** | http://localhost:8080 ou https://localhost:8443 |
| **Swagger** | https://localhost:8443/swagger |

### Mise à jour de l'application

```powershell
# Arrêter les sites
Stop-Website -Name "TRSB-API"
Stop-Website -Name "TRSB-Web"
Stop-WebAppPool -Name "TRSB-API-Pool"
Stop-WebAppPool -Name "TRSB-Web-Pool"

Start-Sleep -Seconds 5

# Publier les nouvelles versions
cd C:\Projects\Test_Qualification_TRSB\src\TRSB.Api
dotnet publish -c Release -o C:\inetpub\wwwroot\TRSB\Api

cd . .\TRSB.Web
dotnet publish -c Release -o C:\inetpub\wwwroot\TRSB\Web

# Redémarrer
Start-WebAppPool -Name "TRSB-API-Pool"
Start-WebAppPool -Name "TRSB-Web-Pool"
Start-Website -Name "TRSB-API"
Start-Website -Name "TRSB-Web"
```

### Architecture IIS

```
┌──────────────────────────────────────────┐
│       Windows Server + IIS                │
├──────────────────────────────────────────┤
��                                           │
│  🌐 TRSB-Web (Port 80/443)               │
│  ├─ AppPool: TRSB-Web-Pool              │
│  ├─ Path: C:\inetpub\wwwroot\TRSB\Web   │
│  └─ SSL: trsb-web.local                 │
│              ↓                            │
│  🔌 TRSB-API (Port 8080/8443)            │
│  ├─ AppPool: TRSB-API-Pool              │
│  ├─ Path: C:\inetpub\wwwroot\TRSB\Api   │
│  └─ SSL: trsb-api.local                 │
│              ↓                            │
│  🗄 SQL Server (localhost\SQLEXPRESS)    │
│  └─ Database: TRSB_DB                    │
│                                           │
└──────────────────────────────────────────┘
```

---

## ✅ Tests

### Structure des tests

```
tests/
├── TRSB.Api.Tests/           # Tests des controllers et endpoints
├── TRSB. Application.Tests/   # Tests des use cases et handlers
├── TRSB. Domain.Tests/         # Tests des entités et règles métier
├── TRSB.E2E.Tests/            # Tests end-to-end
└── TRSB.Infrastructure.Tests/ # Tests des repositories et DB
```

### Exécuter les tests

```bash
# Tous les tests
dotnet test

# Tests avec couverture de code
dotnet test /p:CollectCoverage=true

# Tests spécifiques
dotnet test --filter "FullyQualifiedName~TRSB.Api.Tests"
dotnet test --filter "FullyQualifiedName~TRSB.E2E.Tests"

# Tests avec détails
dotnet test --logger "console;verbosity=detailed"
```

### Tests E2E

**Important :** Pour que les tests E2E fonctionnent, ajoutez cette ligne à la fin de `src/TRSB.Api/Program.cs` :

```csharp
// Permet l'accès au Program pour les tests E2E
public partial class Program { }
```

Puis exécutez :

```bash
cd tests/TRSB.E2E.Tests
dotnet test
```

### Couverture de code

```bash
# Générer un rapport HTML
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=html

# Ouvrir le rapport
# Linux/Mac
open coverage/index.html

# Windows
start coverage/index.html
```

### Tests manuels de l'API

#### Via Swagger

Accédez à `/swagger` et testez interactivement. 

#### Via cURL

```bash
# Health check
curl https://localhost:8443/health -k

# Créer un compte
curl -X POST https://localhost:8443/api/users -k \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123! ",
    "name": "John Doe"
  }'

# Login
curl -X POST https://localhost:8443/api/users/login -k \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "test@example.com",
    "password": "Password123!"
  }'

# Récupérer le profil (avec token)
curl -X GET https://localhost:8443/api/users/profile -k \
  -H "Authorization: Bearer VOTRE_TOKEN_JWT"
```

---

## ⚙️ Configuration

### Variables d'environnement

#### Développement local

Fichier `.env` à la racine :

```dotenv
# API
Api__BaseUrl=https://localhost:7001

# Database
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=TRSB_DB;User Id=sa;Password=VotreMotDePasse123!;TrustServerCertificate=True;

# JWT
Jwt__Issuer=TRSB-API
Jwt__Audience=TRSB-Client
Jwt__SigningKey=VotreCleSecrete64Caracteres... 
Jwt__ExpireMinutes=60

# Password Policy
PasswordPolicy__MinLength=8
PasswordPolicy__MinSpecialChars=1
```

#### Docker

Voir le fichier `.env` avec les variables spécifiques Docker (ports, certificats, etc.)

#### IIS

Configuration dans `appsettings.Production.json` de chaque projet.

### Politique de mots de passe

Configurable via : 

```json
"PasswordPolicy": {
  "MinLength": 8,              // Longueur minimum
  "MinSpecialChars": 1         // Nombre de caractères spéciaux minimum
}
```

### Configuration JWT

```json
"Jwt": {
  "Issuer": "TRSB-API",        // Émetteur du token
  "Audience": "TRSB-Client",   // Audience autorisée
  "SigningKey": ".. .",         // Clé de signature (min 256 bits)
  "ExpireMinutes": 60          // Durée de validité (minutes)
}
```

---

## 🔐 Sécurité

### Checklist Production

- [ ] **HTTPS activé** partout (API et Web)
- [ ] **Certificats SSL valides** (Let's Encrypt ou certificat commercial)
- [ ] **Secrets sécurisés** (Azure Key Vault, variables d'environnement)
- [ ] **Mot de passe SQL fort** (min 12 caractères, complexe)
- [ ] **Clé JWT forte** (min 256 bits, aléatoire)
- [ ] **Swagger désactivé** en production
- [ ] **CORS configuré** correctement
- [ ] **HSTS activé**
- [ ] **Firewall SQL** configuré (whitelist IPs)
- [ ] **Logs activés** (Application Insights, Serilog)
- [ ] **Sauvegardes DB** automatiques
- [ ] **Mises à jour** packages NuGet régulières

### Génération de secrets sécurisés

#### Clé JWT (256 bits minimum)

```bash
# Linux/Mac
openssl rand -base64 64

# PowerShell (Windows)
$bytes = New-Object byte[] 64
[Security.Cryptography.RNGCryptoServiceProvider]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)

# En ligne (HTTPS uniquement)
# https://generate-secret.vercel.app/64
```

#### Mot de passe SQL Server

Doit contenir : 
- Minimum 8 caractères
- Majuscules et minuscules
- Chiffres
- Caractères spéciaux

```powershell
# PowerShell
-join ((33..126) | Get-Random -Count 16 | % {[char]$_})
```

### Protection des secrets

**❌ Ne jamais commiter :**
- `.env`
- `appsettings.Production.json` avec secrets
- Certificats `.pfx`, `.crt`, `.key`
- Mots de passe en clair

**✅ À faire :**
- Utiliser `.env. example` pour documentation
- Stocker secrets dans Azure Key Vault (production)
- Utiliser des variables d'environnement
- Ajouter `.env` et `*.pfx` dans `.gitignore`

### HTTPS en production

#### Docker avec Let's Encrypt

```bash
# 1. Obtenir les certificats
certbot certonly --standalone -d api.votre-domaine.com
certbot certonly --standalone -d web.votre-domaine. com

# 2. Convertir en . pfx
openssl pkcs12 -export \
  -out certs/api/aspnetapp.pfx \
  -inkey /etc/letsencrypt/live/api. votre-domaine.com/privkey.pem \
  -in /etc/letsencrypt/live/api.votre-domaine.com/fullchain.pem \
  -password pass:VotreMotDePasseCert

# 3. Mettre à jour . env et redémarrer
docker-compose restart
```

#### IIS avec Let's Encrypt

1. Installer [win-acme](https://www.win-acme.com/)
2. Exécuter et suivre l'assistant
3. Lier le certificat dans IIS Manager

---

## 📊 Monitoring et Logs

### Logs de développement

```bash
# Docker
docker-compose logs -f trsb-api
docker-compose logs -f trsb-web

# IIS
Get-Content "C:\inetpub\wwwroot\TRSB\Api\logs\stdout_*.log" -Wait -Tail 50
```

### Logs de production

Configurer Application Insights (Azure) ou Serilog pour centraliser les logs.

---

## 🔧 Dépannage

### Problèmes courants

#### Docker :  Container ne démarre pas

```bash
# Vérifier les logs
docker-compose logs trsb-api

# Vérifier la configuration
docker-compose config

# Reconstruire
docker-compose down
docker-compose up -d --build
```

#### IIS : Erreur 502.5

```powershell
# 1. Vérifier que . NET 8 Hosting Bundle est installé
dotnet --list-runtimes

# 2. Redémarrer IIS
iisreset

# 3. Vérifier les logs
Get-Content "C:\inetpub\wwwroot\TRSB\Api\logs\stdout_*. log" -Tail 50
```

#### Erreur de certificat HTTPS

```bash
# Régénérer les certificats
./generate-certs.sh  # Linux/Mac
.\generate-certs.ps1  # Windows

# Faire confiance au certificat
dotnet dev-certs https --trust
```

#### Impossible de se connecter à SQL Server

```bash
# Vérifier que SQL Server est accessible
ping localhost

# Tester la connexion
sqlcmd -S localhost\SQLEXPRESS -U TRSB_User -P 'VotreMotDePasse123!'

# Vérifier le firewall (port 1433)
netstat -an | findstr 1433
```

---

## 📚 Documentation complémentaire

- [Documentation . NET 8](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Docker](https://docs.docker.com/)
- [IIS Hosting](https://learn.microsoft.com/aspnet/core/host-and-deploy/iis/)

---

## 📄 Licence

[Ajouter votre licence]

---

## 👥 Auteur

**Asma Elfaleh**  
Pour toute question :  [falah.asma@gmail.com]
Github Autopilot m'a aidé beaucoup pour rediger ce README.md

---

**Développé avec ❤️ en C# . NET 8**
