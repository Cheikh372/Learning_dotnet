# 🎯 COMMENCEZ ICI - Système de Signature PDF Électronique

## 🌟 Bienvenue !

Vous venez d'installer un **système complet de signature électronique de documents PDF** avec un certificat .pfx en utilisant **iText7**.

## ⚡ Démarrage Ultra-Rapide (3 minutes)

### 1. Générer un certificat de test

**Windows :**
```powershell
.\generate-test-certificate.ps1
```

**Linux/Mac :**
```bash
./generate-test-certificate.sh
```

### 2. Lancer l'application

```bash
dotnet run
```

### 3. Tester

Ouvrez `test-pdf-signature.html` dans votre navigateur !

## 📚 Documentation

| 📄 Document | 🎯 Contenu | ⏱️ Temps de lecture |
|------------|-----------|-------------------|
| **[QUICK_START.md](QUICK_START.md)** | Démarrage rapide | 5 min |
| **[README_PDF_SIGNATURE.md](README_PDF_SIGNATURE.md)** | Documentation complète de l'API | 15 min |
| **[EXEMPLE_UTILISATION.md](EXEMPLE_UTILISATION.md)** | 8 exemples de code détaillés | 20 min |
| **[INSTALLATION_COMPLETE.md](INSTALLATION_COMPLETE.md)** | Tout ce qui a été installé | 10 min |
| **[FILES_CREATED.md](FILES_CREATED.md)** | Liste de tous les fichiers | 5 min |

## 🎨 Interface de test

```
📱 test-pdf-signature.html
```
Une interface web moderne pour tester toutes les fonctionnalités :
- ✅ Signer un PDF avec certificat personnalisé
- ✅ Signer avec certificat configuré
- ✅ Vérifier les signatures existantes

## 🔧 Configuration

Le fichier `appsettings.json` contient :

```json
{
  "PdfSignature": {
    "CertificatePath": "Certificates/certificate.pfx",
    "CertificatePassword": "YourPasswordHere",
    "DefaultSignerName": "Signature Électronique Sénégal",
    "DefaultLocation": "Dakar, Sénégal"
  }
}
```

## 🚀 API Endpoints

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| POST | `/api/pdfsignature/sign` | Signer avec certificat uploadé |
| POST | `/api/pdfsignature/sign-with-default-certificate` | Signer avec certificat configuré |
| POST | `/api/pdfsignature/verify` | Vérifier les signatures |
| GET | `/api/pdfsignature/certificate-info` | Info du certificat |

## 💻 Exemple de code

```csharp
// Injecter le service
private readonly PdfSignatureService _signatureService;

// Signer un PDF
var result = await _signatureService.SignPdfAsync(
    "document.pdf",
    "document_signed.pdf",
    "Certificates/test-certificate.pfx",
    "TestPassword123",
    reason: "Validation du document",
    location: "Dakar, Sénégal",
    signerName: "Jean Dupont"
);

if (result.Success)
{
    Console.WriteLine($"✅ Signé : {result.SignedFilePath}");
}
```

## 🧪 Test rapide

```bash
# Signer un PDF
curl -k -X POST "https://localhost:7000/api/pdfsignature/sign-with-default-certificate" \
  -F "PdfFile=@test.pdf" \
  -o signed.pdf

# Vérifier
curl -k -X POST "https://localhost:7000/api/pdfsignature/verify" \
  -F "pdfFile=@signed.pdf"
```

## 📦 Ce qui est inclus

✅ **Service de signature complet** (`PdfSignatureService.cs`)
✅ **API REST** (`PdfSignatureController.cs`)
✅ **Interface de test HTML** (`test-pdf-signature.html`)
✅ **Scripts de génération de certificat** (.sh et .ps1)
✅ **Documentation exhaustive** (5 fichiers .md)
✅ **Configuration** (appsettings.json)
✅ **Exemples de code** (8 scénarios)

## 🔐 Sécurité

⚠️ **Le certificat de test est auto-signé - NE PAS utiliser en production !**

Pour la production :
1. Obtenez un certificat commercial
2. Utilisez Azure Key Vault ou des variables d'environnement
3. Ne committez jamais les certificats (.gitignore configuré)

## 🎯 Prochaines étapes recommandées

1. ✅ **Testez** avec `test-pdf-signature.html`
2. 📖 **Lisez** `QUICK_START.md` pour plus de détails
3. 💡 **Consultez** `EXEMPLE_UTILISATION.md` pour des cas avancés
4. 🔒 **Préparez** la production avec un vrai certificat

## 🐛 Problème ?

Consultez la section "Résolution de problèmes" dans :
- `QUICK_START.md` - Problèmes courants
- `INSTALLATION_COMPLETE.md` - Configuration
- `README_PDF_SIGNATURE.md` - Documentation API

## 🌍 Support

- 📖 [Documentation iText7](https://kb.itextpdf.com/home)
- 🔍 [API iText7](https://api.itextpdf.com/iText7/)
- 🔐 [BouncyCastle](https://www.bouncycastle.org/documentation.html)

## ✨ Fonctionnalités

- ✅ Signature avec certificat .pfx
- ✅ Vérification des signatures
- ✅ Support fichiers et streams
- ✅ API REST complète
- ✅ Interface de test
- ✅ Documentation Swagger
- ✅ Exemples de code
- ✅ Configuré pour le Sénégal

## 📊 Architecture

```
┌─────────────────────────────────────────┐
│         API REST Controller              │
│   /api/pdfsignature/*                   │
└──────────────┬──────────────────────────┘
               │
               ↓
┌─────────────────────────────────────────┐
│      PdfSignatureService                │
│  • SignPdfAsync()                       │
│  • SignPdfFromStreamAsync()             │
│  • VerifyPdfSignaturesAsync()           │
└──────────────┬──────────────────────────┘
               │
               ↓
┌─────────────────────────────────────────┐
│         iText7 + BouncyCastle           │
│  Signature cryptographique PDF          │
└─────────────────────────────────────────┘
```

## 🎉 Prêt !

Votre système est **100% opérationnel** et prêt à signer des PDFs !

**Commencez par lire [QUICK_START.md](QUICK_START.md) pour démarrer en 5 minutes.**

---

*Créé avec ❤️ pour faciliter la signature électronique de documents PDF*

**🚀 Bon développement !**
