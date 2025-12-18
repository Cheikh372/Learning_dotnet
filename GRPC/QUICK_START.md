# 🚀 Démarrage Rapide - Signature PDF

## Installation complète en 5 minutes

### Étape 1 : Générer un certificat de test

**Sur Windows (PowerShell) :**
```powershell
.\generate-test-certificate.ps1
```

**Sur Linux/Mac (Bash) :**
```bash
./generate-test-certificate.sh
```

### Étape 2 : Configurer l'application

Le fichier `appsettings.json` est déjà configuré avec :
```json
{
  "PdfSignature": {
    "CertificatePath": "Certificates/test-certificate.pfx",
    "CertificatePassword": "TestPassword123",
    "DefaultSignerName": "Signature Électronique Sénégal",
    "DefaultLocation": "Dakar, Sénégal"
  }
}
```

### Étape 3 : Lancer l'application

```bash
dotnet run --project GRPC
```

### Étape 4 : Tester avec l'interface HTML

1. Ouvrez `test-pdf-signature.html` dans votre navigateur
2. Modifiez l'URL de l'API si nécessaire (par défaut : `https://localhost:7000`)
3. Sélectionnez un fichier PDF à signer
4. Cliquez sur "Signer le PDF"

## 📋 Endpoints disponibles

### 1. Signer avec certificat personnalisé
```
POST /api/pdfsignature/sign
```
Uploadez un PDF + un certificat .pfx

### 2. Signer avec certificat par défaut
```
POST /api/pdfsignature/sign-with-default-certificate
```
Uploadez uniquement le PDF (utilise le certificat configuré)

### 3. Vérifier les signatures
```
POST /api/pdfsignature/verify
```
Uploadez un PDF pour vérifier ses signatures

### 4. Info certificat
```
GET /api/pdfsignature/certificate-info
```
Obtenir les informations du certificat configuré

## 🧪 Test rapide avec cURL

```bash
# Définir l'URL de l'API
API_URL="https://localhost:7000"

# Signer un PDF avec le certificat par défaut
curl -X POST "$API_URL/api/pdfsignature/sign-with-default-certificate" \
  -F "PdfFile=@votre-document.pdf" \
  -F "SignerName=Test User" \
  -F "Reason=Test de signature" \
  --output document_signed.pdf \
  --insecure

# Vérifier les signatures
curl -X POST "$API_URL/api/pdfsignature/verify" \
  -F "pdfFile=@document_signed.pdf" \
  --insecure | json_pp
```

## 🔍 Swagger UI

Accédez à la documentation interactive :
```
https://localhost:7000/swagger
```

## 📱 Interface Web de Test

L'interface HTML `test-pdf-signature.html` offre :
- ✅ 3 modes de signature (personnalisé, par défaut, vérification)
- ✅ Interface moderne et intuitive
- ✅ Gestion des erreurs
- ✅ Téléchargement automatique du PDF signé
- ✅ Affichage des signatures existantes

## ⚡ Utilisation programmatique

### Exemple simple C#

```csharp
// Injecter le service
private readonly PdfSignatureService _signatureService;

// Signer un PDF
var result = await _signatureService.SignPdfAsync(
    "input.pdf",
    "output_signed.pdf",
    "Certificates/test-certificate.pfx",
    "TestPassword123",
    "Mon document signé",
    "Dakar",
    "Jean Dupont"
);

if (result.Success)
{
    Console.WriteLine($"✅ PDF signé : {result.SignedFilePath}");
}
```

## 🐛 Résolution de problèmes

### Erreur : "Certificat non trouvé"
- Vérifiez que le fichier .pfx existe dans `Certificates/`
- Vérifiez le chemin dans `appsettings.json`

### Erreur : "Mot de passe incorrect"
- Si vous avez généré le certificat avec le script, le mot de passe est `TestPassword123`
- Vérifiez le mot de passe dans `appsettings.json`

### Erreur HTTPS/SSL
- Pour les tests, utilisez `--insecure` avec cURL
- Dans le navigateur, acceptez le certificat auto-signé
- En production, utilisez un certificat SSL valide

### Le PDF ne se télécharge pas
- Vérifiez la console du navigateur pour les erreurs CORS
- Vérifiez que l'URL de l'API est correcte
- Vérifiez que l'application ASP.NET Core est démarrée

## 📚 Documentation complète

- `README_PDF_SIGNATURE.md` - Documentation complète de l'API
- `EXEMPLE_UTILISATION.md` - Exemples de code avancés
- `test-pdf-signature.html` - Interface de test

## 🔒 Sécurité - Points importants

1. ⚠️ Le certificat de test est **auto-signé** - NE PAS utiliser en production
2. ⚠️ Ne jamais committer les certificats dans Git
3. ⚠️ Utiliser des variables d'environnement pour les mots de passe en production
4. ⚠️ Toujours utiliser HTTPS en production

## 🎯 Prochaines étapes

1. Tester avec vos propres documents PDF
2. Obtenir un certificat commercial pour la production
3. Configurer l'authentification pour l'API
4. Intégrer dans votre application existante

## 📞 Besoin d'aide ?

- Consultez la documentation iText7 : https://kb.itextpdf.com/home
- Vérifiez les exemples dans `EXEMPLE_UTILISATION.md`
- Testez avec Swagger UI pour comprendre l'API

---

**C'est prêt ! Vous pouvez maintenant signer vos documents PDF électroniquement. 🎉**
