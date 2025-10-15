namespace AuthServiceApp.API.Models.Requets
{
    public class RefreshTokensRequest
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
