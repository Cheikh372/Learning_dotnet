# Script PowerShell pour générer un certificat .pfx de test
# Ce certificat est auto-signé et ne doit être utilisé qu'à des fins de test

Write-Host "🔐 Génération d'un certificat de test pour la signature PDF" -ForegroundColor Cyan
Write-Host ""

# Définir les variables
$certPassword = "TestPassword123"
$certDir = "Certificates"
$certName = "test-certificate"

# Créer le dossier s'il n'existe pas
if (!(Test-Path $certDir)) {
    New-Item -ItemType Directory -Path $certDir | Out-Null
}

# Générer le certificat avec PowerShell
Write-Host "1️⃣ Génération du certificat..." -ForegroundColor Yellow

$cert = New-SelfSignedCertificate `
    -Subject "CN=Signature Test Senegal, O=Test Organisation, L=Dakar, S=Dakar, C=SN" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -KeyExportPolicy Exportable `
    -KeySpec Signature `
    -KeyLength 2048 `
    -KeyAlgorithm RSA `
    -HashAlgorithm SHA256 `
    -NotAfter (Get-Date).AddYears(1) `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.4")

Write-Host "2️⃣ Export du certificat en .pfx..." -ForegroundColor Yellow

# Convertir le mot de passe en SecureString
$securePassword = ConvertTo-SecureString -String $certPassword -Force -AsPlainText

# Exporter le certificat
$pfxPath = Join-Path $certDir "$certName.pfx"
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $securePassword | Out-Null

# Supprimer le certificat du store
Write-Host "3️⃣ Nettoyage..." -ForegroundColor Yellow
Remove-Item -Path "Cert:\CurrentUser\My\$($cert.Thumbprint)" -Force

Write-Host ""
Write-Host "✅ Certificat généré avec succès!" -ForegroundColor Green
Write-Host ""
Write-Host "📁 Emplacement: $pfxPath" -ForegroundColor White
Write-Host "🔑 Mot de passe: $certPassword" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  ATTENTION: Ce certificat est auto-signé et destiné uniquement aux tests." -ForegroundColor Red
Write-Host "   N'utilisez PAS ce certificat en production!" -ForegroundColor Red
Write-Host ""
Write-Host "Pour utiliser ce certificat, mettez à jour votre appsettings.json:" -ForegroundColor Cyan
Write-Host ""
Write-Host @"
{
  "PdfSignature": {
    "CertificatePath": "Certificates/test-certificate.pfx",
    "CertificatePassword": "TestPassword123",
    "DefaultSignerName": "Signature Test Sénégal",
    "DefaultLocation": "Dakar, Sénégal"
  }
}
"@ -ForegroundColor White
Write-Host ""
