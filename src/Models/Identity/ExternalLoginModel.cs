namespace DPMGallery.Models.Identity
{
    public class ExternalLoginModel
    {
        public ExternalLoginModel(string loginProvider, string providerKey, string displayName)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = displayName;
           
        }

        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user identity user provided by the login provider.
        /// </summary>
        /// <value>
        /// The unique identifier for the user identity user provided by the login provider.
        /// </value>
        /// <remarks>
        /// This would be unique per provider, examples may be @microsoft as a Twitter provider key.
        /// </remarks>
        public string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the display name for the provider.
        /// </summary>
        /// <value>
        /// The display name for the provider.
        /// </value>
        /// <remarks>
        /// Examples of the display name may be local, FACEBOOK, Google, etc.
        /// </remarks>
        public string ProviderDisplayName { get; set; }
    }
}
