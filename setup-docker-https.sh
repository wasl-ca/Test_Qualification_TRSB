#!/bin/bash

echo "🚀 Configuration complète TRSB Docker avec HTTPS"
echo "================================================="

# Vérifier les prérequis
command -v docker >/dev/null 2>&1 || { echo "❌ Docker n'est pas installé"; exit 1; }
command -v docker-compose >/dev/null 2>&1 || { echo "❌ Docker Compose n'est pas installé"; exit 1; }
command -v dotnet >/dev/null 2>&1 || { echo "❌ .NET SDK n'est pas installé"; exit 1; }

echo "✅ Prérequis vérifiés"

# Créer . env
if [ -f .env ]; then
    read -p "⚠️  . env existe déjà. Écraser? (y/N): " -n 1 -r
    echo
    if [[ !  $REPLY =~ ^[Yy]$ ]]; then
        echo "Configuration . env conservée"
    else
        cp .env. example .env
        echo "✅ .env créé"
    fi
else
    cp .env.example .env
    echo "✅ .env créé"
fi

# Générer les secrets
echo "🔐 Génération des secrets..."

SQL_PASSWORD=$(openssl rand -base64 16 | tr -d "=+/" | cut -c1-16)Aa1! 
JWT_KEY=$(openssl rand -base64 64 | tr -d "\n")
CERT_PASSWORD=$(openssl rand -base64 12 | tr -d "=+/")Aa1!

sed -i "s/SQL_SA_PASSWORD=.*/SQL_SA_PASSWORD=${SQL_PASSWORD}/" .env
sed -i "s/JWT_SIGNING_KEY=.*/JWT_SIGNING_KEY=${JWT_KEY}/" .env
sed -i "s/ASPNETCORE_Kestrel__Certificates__Default__Password=.*/ASPNETCORE_Kestrel__Certificates__Default__Password=${CERT_PASSWORD}/" .env

echo "✅ Secrets générés"

# Créer les certificats
echo "📜 Génération des certificats SSL..."
mkdir -p certs/api
mkdir -p certs/web

dotnet dev-certs https -ep certs/api/aspnetapp.pfx -p "$CERT_PASSWORD" --trust
cp certs/api/aspnetapp.pfx certs/web/aspnetapp.pfx

echo "✅ Certificats créés"

# Démarrer
echo "🐳 Démarrage de Docker Compose..."
docker-compose up -d --build

echo ""
echo "✅ Configuration terminée!"
echo ""
echo "📋 Informations de connexion:"
echo "   SQL Password: ${SQL_PASSWORD}"
echo "   Cert Password: ${CERT_PASSWORD}"
echo ""
echo "🌐 Accès aux applications:"
echo "   Web HTTPS:  https://localhost:8444"
echo "   Web HTTP:   http://localhost:8081"
echo "   API HTTPS:  https://localhost:8443"
echo "   API HTTP:   http://localhost:8080"
echo "   Swagger:    https://localhost:8443/swagger"
echo ""
echo "📊 Vérifier les logs:"
echo "   docker-compose logs -f"
echo ""