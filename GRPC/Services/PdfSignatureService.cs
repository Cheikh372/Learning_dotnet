using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Bouncycastle.Crypto;
using Org.BouncyCastle.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace GRPC.Services
{
    /// <summary>
    /// Service pour signer des documents PDF avec un certificat .pfx
    /// </summary>
    public class PdfSignatureService
    {
        private readonly ILogger<PdfSignatureService> _logger;

        public PdfSignatureService(ILogger<PdfSignatureService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Signe un document PDF avec un certificat .pfx
        /// </summary>
        /// <param name="pdfInputPath">Chemin du PDF à signer</param>
        /// <param name="pdfOutputPath">Chemin du PDF signé</param>
        /// <param name="certificatePath">Chemin du fichier .pfx</param>
        /// <param name="certificatePassword">Mot de passe du certificat</param>
        /// <param name="reason">Raison de la signature</param>
        /// <param name="location">Lieu de la signature</param>
        /// <param name="signerName">Nom du signataire</param>
        public async Task<SignatureResult> SignPdfAsync(
            string pdfInputPath,
            string pdfOutputPath,
            string certificatePath,
            string certificatePassword,
            string reason = "Document signé électroniquement",
            string location = "Sénégal",
            string signerName = "Signature Électronique")
        {
            try
            {
                _logger.LogInformation("Début de la signature du PDF: {PdfPath}", pdfInputPath);

                // Vérifier que le fichier PDF existe
                if (!File.Exists(pdfInputPath))
                {
                    throw new FileNotFoundException($"Le fichier PDF n'existe pas: {pdfInputPath}");
                }

                // Vérifier que le certificat existe
                if (!File.Exists(certificatePath))
                {
                    throw new FileNotFoundException($"Le certificat n'existe pas: {certificatePath}");
                }

                // Charger le certificat .NET
                var certificateBytes = await File.ReadAllBytesAsync(certificatePath);
                var certificate = new X509Certificate2(certificateBytes, certificatePassword, X509KeyStorageFlags.Exportable);

                // Charger avec BouncyCastle
                Pkcs12Store pkcs12Store = new Pkcs12StoreBuilder().Build();
                using (var certStream = new MemoryStream(certificateBytes))
                {
                    pkcs12Store.Load(certStream, certificatePassword.ToCharArray());
                }

                // Trouver l'alias de la clé privée
                string? alias = null;
                foreach (string currentAlias in pkcs12Store.Aliases)
                {
                    if (pkcs12Store.IsKeyEntry(currentAlias))
                    {
                        alias = currentAlias;
                        break;
                    }
                }

                if (alias == null)
                {
                    throw new InvalidOperationException("Aucune clé privée trouvée dans le certificat");
                }

                // Récupérer la clé privée et la chaîne de certificats
                var keyEntry = pkcs12Store.GetKey(alias);
                var bcPrivateKey = keyEntry.Key;
                
                // Convertir en wrapper iText
                IPrivateKey privateKey = new PrivateKeyBC(bcPrivateKey);

                // Charger la chaîne de certificats
                var certChainEntries = pkcs12Store.GetCertificateChain(alias);
                IX509Certificate[] chain = new IX509Certificate[certChainEntries.Length];
                for (int i = 0; i < certChainEntries.Length; i++)
                {
                    chain[i] = new X509CertificateBC(certChainEntries[i].Certificate);
                }

                // Créer le reader et le signer
                using (PdfReader reader = new PdfReader(pdfInputPath))
                using (FileStream outputStream = new FileStream(pdfOutputPath, FileMode.Create))
                {
                    StampingProperties stampingProperties = new StampingProperties();
                    stampingProperties.UseAppendMode();

                    PdfSigner signer = new PdfSigner(reader, outputStream, stampingProperties);

                    // Configuration du champ de signature
                    signer.SetFieldName("Signature_" + DateTime.Now.Ticks);

                    // Créer le conteneur de signature
                    IExternalSignature externalSignature = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);

                    // Signer le document
                    signer.SignDetached(externalSignature, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
                }

                _logger.LogInformation("PDF signé avec succès: {OutputPath}", pdfOutputPath);

                return new SignatureResult
                {
                    Success = true,
                    Message = "Document signé avec succès",
                    SignedFilePath = pdfOutputPath,
                    SignatureDate = DateTime.Now,
                    SignerName = signerName,
                    CertificateSubject = certificate.Subject
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la signature du PDF");
                return new SignatureResult
                {
                    Success = false,
                    Message = $"Erreur: {ex.Message}",
                    SignedFilePath = null
                };
            }
        }

        /// <summary>
        /// Signe un PDF à partir d'un stream
        /// </summary>
        public async Task<SignatureResult> SignPdfFromStreamAsync(
            Stream pdfInputStream,
            Stream pdfOutputStream,
            string certificatePath,
            string certificatePassword,
            string reason = "Document signé électroniquement",
            string location = "Sénégal",
            string signerName = "Signature Électronique")
        {
            try
            {
                _logger.LogInformation("Début de la signature du PDF depuis un stream");

                // Vérifier que le certificat existe
                if (!File.Exists(certificatePath))
                {
                    throw new FileNotFoundException($"Le certificat n'existe pas: {certificatePath}");
                }

                // Charger le certificat .NET
                var certificateBytes = await File.ReadAllBytesAsync(certificatePath);
                var certificate = new X509Certificate2(certificateBytes, certificatePassword, X509KeyStorageFlags.Exportable);

                // Charger avec BouncyCastle
                Pkcs12Store pkcs12Store = new Pkcs12StoreBuilder().Build();
                using (var certStream = new MemoryStream(certificateBytes))
                {
                    pkcs12Store.Load(certStream, certificatePassword.ToCharArray());
                }

                // Trouver l'alias
                string? alias = null;
                foreach (string currentAlias in pkcs12Store.Aliases)
                {
                    if (pkcs12Store.IsKeyEntry(currentAlias))
                    {
                        alias = currentAlias;
                        break;
                    }
                }

                if (alias == null)
                {
                    throw new InvalidOperationException("Aucune clé privée trouvée dans le certificat");
                }

                // Récupérer la clé et les certificats
                var keyEntry = pkcs12Store.GetKey(alias);
                var bcPrivateKey = keyEntry.Key;
                IPrivateKey privateKey = new PrivateKeyBC(bcPrivateKey);

                var certChainEntries = pkcs12Store.GetCertificateChain(alias);
                IX509Certificate[] chain = new IX509Certificate[certChainEntries.Length];
                for (int i = 0; i < certChainEntries.Length; i++)
                {
                    chain[i] = new X509CertificateBC(certChainEntries[i].Certificate);
                }

                // Créer le reader et le signer
                using (PdfReader reader = new PdfReader(pdfInputStream))
                {
                    StampingProperties stampingProperties = new StampingProperties();
                    stampingProperties.UseAppendMode();

                    PdfSigner signer = new PdfSigner(reader, pdfOutputStream, stampingProperties);
                    signer.SetFieldName("Signature_" + DateTime.Now.Ticks);

                    IExternalSignature externalSignature = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
                    signer.SignDetached(externalSignature, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
                }

                _logger.LogInformation("PDF signé avec succès depuis un stream");

                return new SignatureResult
                {
                    Success = true,
                    Message = "Document signé avec succès",
                    SignatureDate = DateTime.Now,
                    SignerName = signerName,
                    CertificateSubject = certificate.Subject
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la signature du PDF depuis un stream");
                return new SignatureResult
                {
                    Success = false,
                    Message = $"Erreur: {ex.Message}",
                    SignedFilePath = null
                };
            }
        }

        /// <summary>
        /// Vérifie les signatures d'un document PDF
        /// </summary>
        public async Task<List<SignatureInfo>> VerifyPdfSignaturesAsync(string pdfPath)
        {
            var signatures = new List<SignatureInfo>();

            try
            {
                _logger.LogInformation("Vérification des signatures du PDF: {PdfPath}", pdfPath);

                if (!File.Exists(pdfPath))
                {
                    throw new FileNotFoundException($"Le fichier PDF n'existe pas: {pdfPath}");
                }

                await Task.Run(() =>
                {
                    using (PdfReader reader = new PdfReader(pdfPath))
                    using (PdfDocument pdfDoc = new PdfDocument(reader))
                    {
                        SignatureUtil signatureUtil = new SignatureUtil(pdfDoc);
                        var signatureNames = signatureUtil.GetSignatureNames();

                        foreach (var name in signatureNames)
                        {
                            PdfPKCS7 pkcs7 = signatureUtil.ReadSignatureData(name);

                            signatures.Add(new SignatureInfo
                            {
                                SignatureName = name,
                                SignerName = pkcs7.GetSignName(),
                                SignDate = pkcs7.GetSignDate(),
                                Reason = pkcs7.GetReason(),
                                Location = pkcs7.GetLocation(),
                                IsValid = pkcs7.VerifySignatureIntegrityAndAuthenticity()
                            });
                        }
                    }
                });

                _logger.LogInformation("Vérification terminée. {Count} signature(s) trouvée(s)", signatures.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification des signatures");
            }

            return signatures;
        }
    }

    /// <summary>
    /// Résultat d'une opération de signature
    /// </summary>
    public class SignatureResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? SignedFilePath { get; set; }
        public DateTime SignatureDate { get; set; }
        public string? SignerName { get; set; }
        public string? CertificateSubject { get; set; }
    }

    /// <summary>
    /// Informations sur une signature
    /// </summary>
    public class SignatureInfo
    {
        public string SignatureName { get; set; } = string.Empty;
        public string? SignerName { get; set; }
        public DateTime SignDate { get; set; }
        public string? Reason { get; set; }
        public string? Location { get; set; }
        public bool IsValid { get; set; }
    }
}
