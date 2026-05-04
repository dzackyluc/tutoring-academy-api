namespace TutoringAcademy.Settings
{
    // This class represents the settings required for integrating with Backblaze B2 cloud storage. It includes properties such as BaseUrl, AccessKey, and SecretKey, which are necessary for authenticating and interacting with the B2 API.
    public class B2Settings
    {
        public string BaseUrl { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
    }
}