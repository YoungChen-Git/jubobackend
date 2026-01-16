namespace OrderBackend.Models;

public record JwtSettings(
    string SecretKey,
    string Issuer,
    string Audience,
    int ExpirationMinutes
);
