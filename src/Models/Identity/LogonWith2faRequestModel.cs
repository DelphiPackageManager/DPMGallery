using Microsoft.AspNetCore.Mvc;

namespace DPMGallery.Models.Identity
{
    public class LogonWith2faRequestModel
    {
        public string Code { get; set; }
        
        public bool RememberMachine { get; set; }
           
        public bool RememberMe { get; set; }
    }
}
