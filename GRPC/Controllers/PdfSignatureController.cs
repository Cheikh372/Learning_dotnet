using GRPC.Services;
using Microsoft.AspNetCore.Mvc;

namespace GRPC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfSignatureController : ControllerBase
    {
        private readonly PdfSignatureService _pdfSignatureService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PdfSignatureController> _logger;

        public PdfSignatureController(
            PdfSignatureService pdfSignatureService,
            IConfiguration configuration,
            ILogger<PdfSignatureController> logger)
        {
            _pdfSignatureService = pdfSignatureService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Signe un document PDF avec un certificat .pfx
        /// </summary>
        [HttpPost("sign")]
        [ProducesResponseType(typeof(SignatureResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignPdf([FromForm] SignPdfRequest request)
        {
            try
            {
                if (request.PdfFile == null || request.PdfFile.Length == 0)
                {
                    return BadRequest("Aucun fichier PDF fourni");
                }

                if (request.CertificateFile == null || request.CertificateFile.Length == 0)
                {
                    return BadRequest("Aucun certificat fourni");
                }

                if (string.IsNullOrWhiteSpace(request.CertificatePassword))
                {
                    return BadRequest("Mot de passe du certificat requis");
                }

                // Créer un dossier temporaire pour les fichiers
                string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFolder);

                try
                {
                    // Sauvegarder le PDF en entrée
                    string inputPdfPath = Path.Combine(tempFolder, "input.pdf");
                    using (var stream = new FileStream(inputPdfPath, FileMode.Create))
                    {
                        await request.PdfFile.CopyToAsync(stream);
                    }

                    // Sauvegarder le certificat
                    string certificatePath = Path.Combine(tempFolder, "certificate.pfx");
                    using (var stream = new FileStream(certificatePath, FileMode.Create))
                    {
                        await request.CertificateFile.CopyToAsync(stream);
                    }

                    // Chemin pour le PDF signé
                    string outputPdfPath = Path.Combine(tempFolder, "signed.pdf");

                    // Signer le document
                    var result = await _pdfSignatureService.SignPdfAsync(
                        inputPdfPath,
                        outputPdfPath,
                        certificatePath,
                        request.CertificatePassword,
                        request.Reason ?? "Document signé électroniquement",
                        request.Location ?? "Sénégal",
                        request.SignerName ?? "Signature Électronique"
                    );

                    if (!result.Success)
                    {
                        return BadRequest(result.Message);
                    }

                    // Retourner le PDF signé
                    var fileBytes = await System.IO.File.ReadAllBytesAsync(outputPdfPath);
                    var fileName = Path.GetFileNameWithoutExtension(request.PdfFile.FileName) + "_signed.pdf";

                    return File(fileBytes, "application/pdf", fileName);
                }
                finally
                {
                    // Nettoyer les fichiers temporaires
                    try
                    {
                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossible de supprimer le dossier temporaire: {TempFolder}", tempFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la signature du PDF");
                return StatusCode(500, $"Erreur interne: {ex.Message}");
            }
        }

        /// <summary>
        /// Signe un document PDF avec le certificat configuré dans appsettings
        /// </summary>
        [HttpPost("sign-with-default-certificate")]
        [ProducesResponseType(typeof(SignatureResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignPdfWithDefaultCertificate([FromForm] SignPdfWithDefaultRequest request)
        {
            try
            {
                if (request.PdfFile == null || request.PdfFile.Length == 0)
                {
                    return BadRequest("Aucun fichier PDF fourni");
                }

                // Récupérer la configuration du certificat
                var certificatePath = _configuration["PdfSignature:CertificatePath"];
                var certificatePassword = _configuration["PdfSignature:CertificatePassword"];
                var defaultSignerName = _configuration["PdfSignature:DefaultSignerName"] ?? "Signature Électronique";
                var defaultLocation = _configuration["PdfSignature:DefaultLocation"] ?? "Sénégal";

                if (string.IsNullOrWhiteSpace(certificatePath))
                {
                    return BadRequest("Chemin du certificat non configuré dans appsettings");
                }

                if (string.IsNullOrWhiteSpace(certificatePassword))
                {
                    return BadRequest("Mot de passe du certificat non configuré dans appsettings");
                }

                if (!System.IO.File.Exists(certificatePath))
                {
                    return BadRequest($"Le certificat n'existe pas: {certificatePath}");
                }

                // Créer un dossier temporaire pour les fichiers
                string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFolder);

                try
                {
                    // Sauvegarder le PDF en entrée
                    string inputPdfPath = Path.Combine(tempFolder, "input.pdf");
                    using (var stream = new FileStream(inputPdfPath, FileMode.Create))
                    {
                        await request.PdfFile.CopyToAsync(stream);
                    }

                    // Chemin pour le PDF signé
                    string outputPdfPath = Path.Combine(tempFolder, "signed.pdf");

                    // Signer le document
                    var result = await _pdfSignatureService.SignPdfAsync(
                        inputPdfPath,
                        outputPdfPath,
                        certificatePath,
                        certificatePassword,
                        request.Reason ?? "Document signé électroniquement",
                        request.Location ?? defaultLocation,
                        request.SignerName ?? defaultSignerName
                    );

                    if (!result.Success)
                    {
                        return BadRequest(result.Message);
                    }

                    // Retourner le PDF signé
                    var fileBytes = await System.IO.File.ReadAllBytesAsync(outputPdfPath);
                    var fileName = Path.GetFileNameWithoutExtension(request.PdfFile.FileName) + "_signed.pdf";

                    return File(fileBytes, "application/pdf", fileName);
                }
                finally
                {
                    // Nettoyer les fichiers temporaires
                    try
                    {
                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossible de supprimer le dossier temporaire: {TempFolder}", tempFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la signature du PDF");
                return StatusCode(500, $"Erreur interne: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie les signatures d'un document PDF
        /// </summary>
        [HttpPost("verify")]
        [ProducesResponseType(typeof(List<SignatureInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyPdfSignatures([FromForm] IFormFile pdfFile)
        {
            try
            {
                if (pdfFile == null || pdfFile.Length == 0)
                {
                    return BadRequest("Aucun fichier PDF fourni");
                }

                // Créer un dossier temporaire
                string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFolder);

                try
                {
                    // Sauvegarder le PDF
                    string pdfPath = Path.Combine(tempFolder, "document.pdf");
                    using (var stream = new FileStream(pdfPath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(stream);
                    }

                    // Vérifier les signatures
                    var signatures = await _pdfSignatureService.VerifyPdfSignaturesAsync(pdfPath);

                    return Ok(signatures);
                }
                finally
                {
                    // Nettoyer
                    try
                    {
                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossible de supprimer le dossier temporaire: {TempFolder}", tempFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification des signatures");
                return StatusCode(500, $"Erreur interne: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtient les informations sur le certificat configuré
        /// </summary>
        [HttpGet("certificate-info")]
        [ProducesResponseType(typeof(CertificateInfo), StatusCodes.Status200OK)]
        public IActionResult GetCertificateInfo()
        {
            try
            {
                var certificatePath = _configuration["PdfSignature:CertificatePath"];
                var defaultSignerName = _configuration["PdfSignature:DefaultSignerName"];
                var defaultLocation = _configuration["PdfSignature:DefaultLocation"];

                return Ok(new CertificateInfo
                {
                    IsConfigured = !string.IsNullOrWhiteSpace(certificatePath),
                    CertificateExists = !string.IsNullOrWhiteSpace(certificatePath) && System.IO.File.Exists(certificatePath),
                    DefaultSignerName = defaultSignerName,
                    DefaultLocation = defaultLocation
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des informations du certificat");
                return StatusCode(500, $"Erreur interne: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Requête pour signer un PDF avec certificat fourni
    /// </summary>
    public class SignPdfRequest
    {
        public IFormFile PdfFile { get; set; } = null!;
        public IFormFile CertificateFile { get; set; } = null!;
        public string CertificatePassword { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Location { get; set; }
        public string? SignerName { get; set; }
    }

    /// <summary>
    /// Requête pour signer un PDF avec le certificat par défaut
    /// </summary>
    public class SignPdfWithDefaultRequest
    {
        public IFormFile PdfFile { get; set; } = null!;
        public string? Reason { get; set; }
        public string? Location { get; set; }
        public string? SignerName { get; set; }
    }

    /// <summary>
    /// Informations sur le certificat configuré
    /// </summary>
    public class CertificateInfo
    {
        public bool IsConfigured { get; set; }
        public bool CertificateExists { get; set; }
        public string? DefaultSignerName { get; set; }
        public string? DefaultLocation { get; set; }
    }
}
