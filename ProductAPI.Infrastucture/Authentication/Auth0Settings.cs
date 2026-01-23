namespace ProductAPI.Infrastructure.Authentication;

public class Auth0Settings
{
    //Defines the SectionName that contains the Auth0 Settings in AppSettings
    public const string SectionName = "Auth0";
    
    public string Domain { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}