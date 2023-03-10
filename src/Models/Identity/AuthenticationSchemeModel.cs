namespace DPMGallery.Models.Identity
{
    public class AuthenticationSchemeModel
    {
        public AuthenticationSchemeModel(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
            
        }
        //
        // Summary:
        //     The name of the authentication scheme.
        public string Name { get; }
        //
        // Summary:
        //     The display name for the scheme. Null is valid and used for non user facing schemes.
        public string DisplayName { get; }
    }
}
