using UnityEngine.Networking;

public class CustomCertificateHandler : CertificateHandler
{
    // Custom handler that always returns true, effectively bypassing the certificate validation
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}