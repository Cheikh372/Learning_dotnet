# Signature Électronique de Documents PDF avec iText7

Cette solution permet de signer électroniquement des documents PDF avec un certificat .pfx en utilisant iText7.

## 📋 Fonctionnalités

- ✅ Signature de PDF avec certificat .pfx personnalisé
- ✅ Signature de PDF avec certificat configuré (appsettings.json)
- ✅ Vérification des signatures existantes
- ✅ Support des streams pour traiter les fichiers en mémoire
- ✅ API REST complète avec Swagger

## 🔧 Configuration

### 1. Configurer le certificat dans `appsettings.json`

```json
{
  "PdfSignature": {
    "CertificatePath": "Certificates/certificate.pfx",
    "CertificatePassword": "VotreMotDePasse",
    "DefaultSignerName": "Signature Électronique Sénégal",
    "DefaultLocation": "Dakar, Sénégal",
    "DefaultReason": "Document signé électroniquement"
  }
}
```

### 2. Placer votre certificat .pfx

Créez un dossier `Certificates` à la racine du projet et placez-y votre fichier `certificate.pfx`.

```bash
mkdir Certificates
# Copiez votre fichier .pfx dans ce dossier
```

## 🚀 Utilisation

### Démarrer l'application

```bash
dotnet run --project GRPC
```

L'API sera accessible sur : `https://localhost:7XXX` ou `http://localhost:5XXX`

### Points de terminaison de l'API

#### 1. Signer un PDF avec certificat fourni

**Endpoint:** `POST /api/pdfsignature/sign`

**Paramètres (multipart/form-data):**
- `PdfFile` (file) - Le fichier PDF à signer
- `CertificateFile` (file) - Le fichier .pfx
- `CertificatePassword` (string) - Mot de passe du certificat
- `Reason` (string, optionnel) - Raison de la signature
- `Location` (string, optionnel) - Lieu de signature
- `SignerName` (string, optionnel) - Nom du signataire

**Exemple avec cURL:**

```bash
curl -X POST "https://localhost:7XXX/api/pdfsignature/sign" \
  -H "accept: application/pdf" \
  -H "Content-Type: multipart/form-data" \
  -F "PdfFile=@document.pdf" \
  -F "CertificateFile=@certificate.pfx" \
  -F "CertificatePassword=MonMotDePasse" \
  -F "Reason=Approbation du contrat" \
  -F "Location=Dakar, Sénégal" \
  -F "SignerName=Jean Dupont" \
  --output document_signed.pdf
```

**Exemple avec PowerShell:**

```powershell
$uri = "https://localhost:7XXX/api/pdfsignature/sign"
$pdfPath = "C:\documents\document.pdf"
$certPath = "C:\certificates\certificate.pfx"
$password = "MonMotDePasse"

$form = @{
    PdfFile = Get-Item -Path $pdfPath
    CertificateFile = Get-Item -Path $certPath
    CertificatePassword = $password
    Reason = "Approbation du contrat"
    Location = "Dakar, Sénégal"
    SignerName = "Jean Dupont"
}

Invoke-RestMethod -Uri $uri -Method Post -Form $form -OutFile "document_signed.pdf"
```

#### 2. Signer un PDF avec le certificat par défaut

**Endpoint:** `POST /api/pdfsignature/sign-with-default-certificate`

**Paramètres (multipart/form-data):**
- `PdfFile` (file) - Le fichier PDF à signer
- `Reason` (string, optionnel) - Raison de la signature
- `Location` (string, optionnel) - Lieu de signature
- `SignerName` (string, optionnel) - Nom du signataire

**Exemple avec cURL:**

```bash
curl -X POST "https://localhost:7XXX/api/pdfsignature/sign-with-default-certificate" \
  -H "accept: application/pdf" \
  -H "Content-Type: multipart/form-data" \
  -F "PdfFile=@document.pdf" \
  -F "Reason=Validation finale" \
  -F "Location=Dakar" \
  -F "SignerName=Service RH" \
  --output document_signed.pdf
```

#### 3. Vérifier les signatures d'un PDF

**Endpoint:** `POST /api/pdfsignature/verify`

**Paramètres (multipart/form-data):**
- `pdfFile` (file) - Le fichier PDF à vérifier

**Exemple avec cURL:**

```bash
curl -X POST "https://localhost:7XXX/api/pdfsignature/verify" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "pdfFile=@document_signed.pdf"
```

**Réponse:**

```json
[
  {
    "signatureName": "Signature_638387234567890123",
    "signerName": "Jean Dupont",
    "signDate": "2023-12-18T10:30:00",
    "reason": "Approbation du contrat",
    "location": "Dakar, Sénégal",
    "isValid": true
  }
]
```

#### 4. Obtenir les informations du certificat configuré

**Endpoint:** `GET /api/pdfsignature/certificate-info`

**Exemple avec cURL:**

```bash
curl -X GET "https://localhost:7XXX/api/pdfsignature/certificate-info" \
  -H "accept: application/json"
```

**Réponse:**

```json
{
  "isConfigured": true,
  "certificateExists": true,
  "defaultSignerName": "Signature Électronique Sénégal",
  "defaultLocation": "Dakar, Sénégal"
}
```

## 📝 Utilisation programmatique du service

### Exemple 1: Signer un PDF avec un fichier

```csharp
public class MonService
{
    private readonly PdfSignatureService _pdfSignatureService;

    public MonService(PdfSignatureService pdfSignatureService)
    {
        _pdfSignatureService = pdfSignatureService;
    }

    public async Task SignerDocument()
    {
        var result = await _pdfSignatureService.SignPdfAsync(
            pdfInputPath: "document.pdf",
            pdfOutputPath: "document_signed.pdf",
            certificatePath: "certificate.pfx",
            certificatePassword: "MonMotDePasse",
            reason: "Approbation du document",
            location: "Dakar, Sénégal",
            signerName: "Jean Dupont"
        );

        if (result.Success)
        {
            Console.WriteLine($"Document signé avec succès: {result.SignedFilePath}");
            Console.WriteLine($"Date: {result.SignatureDate}");
            Console.WriteLine($"Signataire: {result.SignerName}");
        }
        else
        {
            Console.WriteLine($"Erreur: {result.Message}");
        }
    }
}
```

### Exemple 2: Signer un PDF avec des streams

```csharp
public async Task SignerDocumentAvecStream()
{
    using var inputStream = File.OpenRead("document.pdf");
    using var outputStream = File.Create("document_signed.pdf");

    var result = await _pdfSignatureService.SignPdfFromStreamAsync(
        pdfInputStream: inputStream,
        pdfOutputStream: outputStream,
        certificatePath: "certificate.pfx",
        certificatePassword: "MonMotDePasse",
        reason: "Validation électronique",
        location: "Dakar",
        signerName: "Service Administratif"
    );

    if (result.Success)
    {
        Console.WriteLine("PDF signé avec succès!");
    }
}
```

### Exemple 3: Vérifier les signatures

```csharp
public async Task VerifierSignatures()
{
    var signatures = await _pdfSignatureService.VerifyPdfSignaturesAsync("document_signed.pdf");

    foreach (var signature in signatures)
    {
        Console.WriteLine($"Signature: {signature.SignatureName}");
        Console.WriteLine($"Signataire: {signature.SignerName}");
        Console.WriteLine($"Date: {signature.SignDate}");
        Console.WriteLine($"Raison: {signature.Reason}");
        Console.WriteLine($"Lieu: {signature.Location}");
        Console.WriteLine($"Valide: {signature.IsValid}");
        Console.WriteLine("---");
    }
}
```

## 🔒 Créer un certificat de test

Pour créer un certificat .pfx de test, vous pouvez utiliser OpenSSL:

```bash
# Générer une clé privée
openssl genrsa -out private.key 2048

# Générer une demande de certificat
openssl req -new -key private.key -out certificate.csr \
  -subj "/C=SN/ST=Dakar/L=Dakar/O=MonEntreprise/CN=Signature Test"

# Générer un certificat auto-signé
openssl x509 -req -days 365 -in certificate.csr \
  -signkey private.key -out certificate.crt

# Créer le fichier .pfx
openssl pkcs12 -export -out certificate.pfx \
  -inkey private.key -in certificate.crt \
  -password pass:MonMotDePasse
```

## 📱 Interface de test HTML

Un fichier `test-pdf-signature.html` est fourni pour tester l'API via une interface web simple.

Ouvrez simplement le fichier dans un navigateur et :
1. Sélectionnez votre fichier PDF
2. Sélectionnez votre certificat .pfx
3. Entrez le mot de passe
4. Remplissez les informations optionnelles
5. Cliquez sur "Signer le PDF"

## 🔍 Swagger UI

Accédez à la documentation interactive Swagger :

```
https://localhost:7XXX/swagger
```

Vous pourrez y tester tous les endpoints directement depuis votre navigateur.

## ⚠️ Notes de sécurité

1. **Ne jamais committer le certificat ou le mot de passe** dans le contrôle de version
2. Utiliser des **variables d'environnement** ou **Azure Key Vault** en production
3. Le certificat doit être stocké de manière sécurisée
4. Utiliser HTTPS pour toutes les communications
5. Valider et nettoyer les entrées utilisateur

## 🛠️ Dépendances

- iText7 (8.0.2) - Bibliothèque pour manipuler les PDF
- BouncyCastle - Cryptographie (inclus avec iText7)
- ASP.NET Core 7.0

## 📄 Licence

Ce code est fourni à titre d'exemple. Assurez-vous d'avoir les licences appropriées pour iText7 si vous l'utilisez en production commerciale.

## 🤝 Support

Pour toute question ou problème, veuillez consulter :
- [Documentation iText7](https://kb.itextpdf.com/home)
- [BouncyCastle Documentation](https://www.bouncycastle.org/documentation.html)
