namespace DPMGallery.Configuration
{ 
    public class RateLimitingOptions
    {
        public RateLimitingOptions()
        {
                
        }

        public int PermitLimit { get;set; } = 500;

        public int Window { get;set; } = 60;
    }
}
