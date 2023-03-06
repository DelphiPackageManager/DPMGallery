namespace DPMGallery.Models.Identity
{
    public class TwoFactorConfigModel
    {
        public bool TwoFactorEnabled {get;set; }
        public bool HasAuthenticator { get; set; }
        public int RecoveryCodesLeft { get; set; }
        public bool IsMachineRemembered { get; set; }
    }
}
