#!/bin/bash

echo "🐳 Configuration de TRSB Docker"
echo "================================"

# Vérifier si .env existe
if [ -f .env ]; then
    read -p "⚠️  Le fichier .env existe déjà. Écraser?  (y/N): " -n 1 -r
    echo
    if [[ !  $REPLY =~ ^[Yy]$ ]]; then
        echo "❌ Configuration annulée."
        exit 1
    fi
fi

# Copier . env.example
cp .env.example .env

# Générer un mot de passe SQL sécurisé
SQL_PASSWORD=$(openssl rand -base64 16 | tr -d "=+/" | cut -c1-16)Aa1! 
echo "✅ Mot de passe SQL généré"

# Générer une clé JWT sécurisée
JWT_KEY=$(openssl rand -base64 64 | tr -d "\n")
echo "✅ Clé JWT générée"

# Remplacer dans .env
sed -i "s/SQL_SA_PASSWORD=. */SQL_SA_PASSWORD=${SQL_PASSWORD}/" .env
sed -i "s/JWT_SIGNING_KEY=.*/JWT_SIGNING_KEY=${JWT_KEY}/" .env

echo ""
echo "✅ Configuration terminée!"
echo ""
echo "📋 Informations importantes:"
echo "   SQL Password: ${SQL_PASSWORD}"
echo "   (Sauvegardez ce mot de passe en lieu sûr)"
echo ""
echo "🚀 Pour démarrer l'application:"
echo "   docker-compose up -d"
echo ""