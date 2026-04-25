namespace SPS.Shared.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    // Backwards compatible alias
    public int TokenExpirationInMinutes
    {
        get => AccessTokenExpirationMinutes;
        set => AccessTokenExpirationMinutes = value;
    }

    public int RefreshTokenExpirationInDays { get; set; } = 7;
}