# 📚 Exemples d'utilisation - Signature PDF avec iText7 et .pfx

Ce document contient des exemples pratiques d'utilisation du service de signature PDF.

## 🎯 Cas d'usage courants

### 1. Signature de contrats

```csharp
public class ContractService
{
    private readonly PdfSignatureService _signatureService;

    public async Task<string> SignContract(string contractPath, string employeeName)
    {
        var outputPath = Path.Combine(
            Path.GetDirectoryName(contractPath), 
            $"contract_signed_{DateTime.Now:yyyyMMdd}.pdf"
        );

        var result = await _signatureService.SignPdfAsync(
            pdfInputPath: contractPath,
            pdfOutputPath: outputPath,
            certificatePath: "Certificates/hr-certificate.pfx",
            certificatePassword: "SecurePassword123",
            reason: "Signature du contrat d'emploi",
            location: "Dakar, Sénégal",
            signerName: $"Département RH - Approuvé pour {employeeName}"
        );

        if (!result.Success)
        {
            throw new Exception($"Erreur de signature: {result.Message}");
        }

        return outputPath;
    }
}
```

### 2. Signature de factures

```csharp
public class InvoiceService
{
    private readonly PdfSignatureService _signatureService;
    private readonly IConfiguration _configuration;

    public async Task<byte[]> SignInvoiceToBytes(Stream invoiceStream, string invoiceNumber)
    {
        var outputStream = new MemoryStream();
        
        var certPath = _configuration["PdfSignature:CertificatePath"];
        var certPassword = _configuration["PdfSignature:CertificatePassword"];

        var result = await _signatureService.SignPdfFromStreamAsync(
            pdfInputStream: invoiceStream,
            pdfOutputStream: outputStream,
            certificatePath: certPath,
            certificatePassword: certPassword,
            reason: $"Facture validée - N° {invoiceNumber}",
            location: "Dakar, Sénégal",
            signerName: "Service Comptabilité"
        );

        if (!result.Success)
        {
            throw new Exception($"Erreur de signature: {result.Message}");
        }

        return outputStream.ToArray();
    }
}
```

### 3. Signature batch de documents

```csharp
public class BatchSignatureService
{
    private readonly PdfSignatureService _signatureService;

    public async Task<List<SignatureResult>> SignMultipleDocuments(
        List<string> pdfPaths,
        string certificatePath,
        string certificatePassword)
    {
        var results = new List<SignatureResult>();

        foreach (var pdfPath in pdfPaths)
        {
            var fileName = Path.GetFileNameWithoutExtension(pdfPath);
            var outputPath = Path.Combine(
                Path.GetDirectoryName(pdfPath),
                $"{fileName}_signed.pdf"
            );

            var result = await _signatureService.SignPdfAsync(
                pdfInputPath: pdfPath,
                pdfOutputPath: outputPath,
                certificatePath: certificatePath,
                certificatePassword: certificatePassword,
                reason: "Signature automatique en batch",
                location: "Système automatisé",
                signerName: "Signature Électronique Batch"
            );

            results.Add(result);
        }

        return results;
    }
}
```

### 4. API Web avec validation

```csharp
[ApiController]
[Route("api/documents")]
public class DocumentController : ControllerBase
{
    private readonly PdfSignatureService _signatureService;
    private readonly ILogger<DocumentController> _logger;

    [HttpPost("sign-and-store")]
    public async Task<IActionResult> SignAndStore([FromForm] DocumentSignRequest request)
    {
        try
        {
            // Validation du fichier
            if (request.Document == null || request.Document.Length == 0)
                return BadRequest("Fichier invalide");

            if (!request.Document.ContentType.Equals("application/pdf"))
                return BadRequest("Le fichier doit être un PDF");

            // Créer des chemins temporaires
            var tempInput = Path.GetTempFileName();
            var tempOutput = Path.GetTempFileName();

            try
            {
                // Sauvegarder le fichier uploadé
                using (var stream = new FileStream(tempInput, FileMode.Create))
                {
                    await request.Document.CopyToAsync(stream);
                }

                // Signer le document
                var result = await _signatureService.SignPdfAsync(
                    pdfInputPath: tempInput,
                    pdfOutputPath: tempOutput,
                    certificatePath: "Certificates/api-certificate.pfx",
                    certificatePassword: Environment.GetEnvironmentVariable("CERT_PASSWORD"),
                    reason: request.Reason ?? "Signature via API",
                    location: "Système API - Sénégal",
                    signerName: User.Identity?.Name ?? "Utilisateur API"
                );

                if (!result.Success)
                {
                    return StatusCode(500, result.Message);
                }

                // Lire le fichier signé
                var signedBytes = await System.IO.File.ReadAllBytesAsync(tempOutput);

                // TODO: Sauvegarder dans votre système de stockage (base de données, cloud, etc.)
                // await _documentRepository.SaveSignedDocument(signedBytes, request.DocumentId);

                return File(signedBytes, "application/pdf", $"signed_{request.Document.FileName}");
            }
            finally
            {
                // Nettoyage
                if (System.IO.File.Exists(tempInput)) System.IO.File.Delete(tempInput);
                if (System.IO.File.Exists(tempOutput)) System.IO.File.Delete(tempOutput);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la signature du document");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}

public class DocumentSignRequest
{
    public IFormFile Document { get; set; }
    public string? Reason { get; set; }
}
```

### 5. Vérification et rapport de signatures

```csharp
public class SignatureVerificationService
{
    private readonly PdfSignatureService _signatureService;

    public async Task<SignatureReport> GenerateSignatureReport(string pdfPath)
    {
        var signatures = await _signatureService.VerifyPdfSignaturesAsync(pdfPath);

        var report = new SignatureReport
        {
            DocumentPath = pdfPath,
            VerificationDate = DateTime.Now,
            TotalSignatures = signatures.Count,
            ValidSignatures = signatures.Count(s => s.IsValid),
            InvalidSignatures = signatures.Count(s => !s.IsValid),
            Signatures = signatures.Select(s => new SignatureDetail
            {
                SignerName = s.SignerName,
                SignDate = s.SignDate,
                Reason = s.Reason,
                Location = s.Location,
                IsValid = s.IsValid,
                Status = s.IsValid ? "✅ Valide" : "❌ Invalide"
            }).ToList()
        };

        return report;
    }
}

public class SignatureReport
{
    public string DocumentPath { get; set; }
    public DateTime VerificationDate { get; set; }
    public int TotalSignatures { get; set; }
    public int ValidSignatures { get; set; }
    public int InvalidSignatures { get; set; }
    public List<SignatureDetail> Signatures { get; set; }
}

public class SignatureDetail
{
    public string SignerName { get; set; }
    public DateTime SignDate { get; set; }
    public string Reason { get; set; }
    public string Location { get; set; }
    public bool IsValid { get; set; }
    public string Status { get; set; }
}
```

### 6. Signature avec horodatage (Timestamp)

```csharp
// Note: Pour implémenter un horodatage TSA (Time Stamp Authority), 
// vous devrez étendre le service PdfSignatureService

public class TimestampedSignatureService
{
    private readonly PdfSignatureService _baseSignatureService;
    
    // Cette méthode montre comment vous pourriez l'étendre
    // Pour une implémentation complète, référez-vous à la documentation iText7
    public async Task<SignatureResult> SignWithTimestamp(
        string pdfPath,
        string outputPath,
        string certificatePath,
        string certificatePassword,
        string tsaUrl = "http://timestamp.digicert.com")
    {
        // Implémentation nécessiterait l'ajout d'un ITSAClient dans iText7
        // Ceci est un exemple conceptuel
        
        return await _baseSignatureService.SignPdfAsync(
            pdfPath,
            outputPath,
            certificatePath,
            certificatePassword,
            reason: "Signature avec horodatage",
            location: "Sénégal",
            signerName: "Signature Horodatée"
        );
    }
}
```

### 7. Workflow d'approbation à plusieurs signatures

```csharp
public class ApprovalWorkflowService
{
    private readonly PdfSignatureService _signatureService;

    public async Task<string> AddApprovalSignature(
        string documentPath,
        ApprovalStep step)
    {
        // Chaque approbateur ajoute sa signature au document
        var outputPath = Path.Combine(
            Path.GetDirectoryName(documentPath),
            $"document_step{step.StepNumber}.pdf"
        );

        var result = await _signatureService.SignPdfAsync(
            pdfInputPath: documentPath,
            pdfOutputPath: outputPath,
            certificatePath: step.CertificatePath,
            certificatePassword: step.CertificatePassword,
            reason: $"Approbation - Étape {step.StepNumber}: {step.StepName}",
            location: step.Location,
            signerName: step.ApproverName
        );

        if (!result.Success)
        {
            throw new Exception($"Erreur à l'étape {step.StepNumber}: {result.Message}");
        }

        return outputPath;
    }

    public async Task<WorkflowResult> ExecuteFullWorkflow(
        string initialDocumentPath,
        List<ApprovalStep> steps)
    {
        var currentPath = initialDocumentPath;
        var workflowResults = new List<SignatureResult>();

        foreach (var step in steps.OrderBy(s => s.StepNumber))
        {
            currentPath = await AddApprovalSignature(currentPath, step);
            
            var signatures = await _signatureService.VerifyPdfSignaturesAsync(currentPath);
            workflowResults.Add(new SignatureResult
            {
                Success = true,
                SignerName = step.ApproverName,
                SignatureDate = DateTime.Now,
                Message = $"Étape {step.StepNumber} complétée"
            });
        }

        return new WorkflowResult
        {
            FinalDocumentPath = currentPath,
            TotalSteps = steps.Count,
            CompletedSteps = workflowResults.Count,
            AllStepsSuccessful = workflowResults.All(r => r.Success),
            Results = workflowResults
        };
    }
}

public class ApprovalStep
{
    public int StepNumber { get; set; }
    public string StepName { get; set; }
    public string ApproverName { get; set; }
    public string CertificatePath { get; set; }
    public string CertificatePassword { get; set; }
    public string Location { get; set; }
}

public class WorkflowResult
{
    public string FinalDocumentPath { get; set; }
    public int TotalSteps { get; set; }
    public int CompletedSteps { get; set; }
    public bool AllStepsSuccessful { get; set; }
    public List<SignatureResult> Results { get; set; }
}
```

### 8. Intégration avec Azure Blob Storage

```csharp
public class AzureBlobSignatureService
{
    private readonly PdfSignatureService _signatureService;
    private readonly BlobServiceClient _blobServiceClient;

    public async Task<string> SignAndUploadToBlob(
        string containerName,
        string blobName,
        Stream pdfStream,
        string certificatePath,
        string certificatePassword)
    {
        using var outputStream = new MemoryStream();

        // Signer le document
        var result = await _signatureService.SignPdfFromStreamAsync(
            pdfInputStream: pdfStream,
            pdfOutputStream: outputStream,
            certificatePath: certificatePath,
            certificatePassword: certificatePassword,
            reason: "Document signé et archivé",
            location: "Cloud Storage - Sénégal",
            signerName: "Système de signature automatique"
        );

        if (!result.Success)
        {
            throw new Exception($"Erreur de signature: {result.Message}");
        }

        // Upload vers Azure Blob
        outputStream.Position = 0;
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient($"signed/{blobName}");

        await blobClient.UploadAsync(outputStream, overwrite: true);

        return blobClient.Uri.ToString();
    }
}
```

## 🔒 Bonnes pratiques de sécurité

1. **Ne jamais hardcoder les mots de passe**
```csharp
// ❌ Mauvais
var password = "MonMotDePasse123";

// ✅ Bon
var password = _configuration["PdfSignature:CertificatePassword"];
// ou
var password = Environment.GetEnvironmentVariable("CERT_PASSWORD");
```

2. **Utiliser Azure Key Vault en production**
```csharp
var keyVaultUrl = _configuration["KeyVault:Url"];
var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
var secret = await client.GetSecretAsync("certificate-password");
var password = secret.Value.Value;
```

3. **Nettoyer les fichiers temporaires**
```csharp
try
{
    // Traitement
}
finally
{
    if (File.Exists(tempFile))
        File.Delete(tempFile);
}
```

4. **Valider les entrées utilisateur**
```csharp
if (file.Length > 10 * 1024 * 1024) // 10 MB
    return BadRequest("Fichier trop volumineux");

if (!file.ContentType.Equals("application/pdf"))
    return BadRequest("Type de fichier invalide");
```

## 📞 Support

Pour plus d'informations sur iText7, consultez :
- [Documentation officielle iText7](https://kb.itextpdf.com/home)
- [Exemples de code iText](https://github.com/itext/itext7-dotnet-samples)
