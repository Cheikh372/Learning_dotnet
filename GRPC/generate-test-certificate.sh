#!/bin/bash

# Script pour générer un certificat .pfx de test
# Ce certificat est auto-signé et ne doit être utilisé qu'à des fins de test

echo "🔐 Génération d'un certificat de test pour la signature PDF"
echo ""

# Définir les variables
CERT_PASSWORD="TestPassword123"
CERT_DIR="Certificates"
CERT_NAME="test-certificate"

# Créer le dossier s'il n'existe pas
mkdir -p "$CERT_DIR"

# Générer une clé privée RSA
echo "1️⃣ Génération de la clé privée..."
openssl genrsa -out "$CERT_DIR/$CERT_NAME.key" 2048

# Générer une demande de certificat (CSR)
echo "2️⃣ Génération de la demande de certificat..."
openssl req -new -key "$CERT_DIR/$CERT_NAME.key" -out "$CERT_DIR/$CERT_NAME.csr" \
    -subj "/C=SN/ST=Dakar/L=Dakar/O=Test Organisation/OU=IT/CN=Signature Test Senegal/emailAddress=test@example.sn"

# Générer un certificat auto-signé valide 1 an
echo "3️⃣ Génération du certificat auto-signé..."
openssl x509 -req -days 365 \
    -in "$CERT_DIR/$CERT_NAME.csr" \
    -signkey "$CERT_DIR/$CERT_NAME.key" \
    -out "$CERT_DIR/$CERT_NAME.crt"

# Créer le fichier .pfx
echo "4️⃣ Création du fichier .pfx..."
openssl pkcs12 -export \
    -out "$CERT_DIR/$CERT_NAME.pfx" \
    -inkey "$CERT_DIR/$CERT_NAME.key" \
    -in "$CERT_DIR/$CERT_NAME.crt" \
    -password pass:$CERT_PASSWORD

# Nettoyer les fichiers intermédiaires
echo "5️⃣ Nettoyage des fichiers temporaires..."
rm "$CERT_DIR/$CERT_NAME.key"
rm "$CERT_DIR/$CERT_NAME.csr"
rm "$CERT_DIR/$CERT_NAME.crt"

echo ""
echo "✅ Certificat généré avec succès!"
echo ""
echo "📁 Emplacement: $CERT_DIR/$CERT_NAME.pfx"
echo "🔑 Mot de passe: $CERT_PASSWORD"
echo ""
echo "⚠️  ATTENTION: Ce certificat est auto-signé et destiné uniquement aux tests."
echo "   N'utilisez PAS ce certificat en production!"
echo ""
echo "Pour utiliser ce certificat, mettez à jour votre appsettings.json:"
echo ""
echo '{
  "PdfSignature": {
    "CertificatePath": "Certificates/test-certificate.pfx",
    "CertificatePassword": "TestPassword123",
    "DefaultSignerName": "Signature Test Sénégal",
    "DefaultLocation": "Dakar, Sénégal"
  }
}'
echo ""
