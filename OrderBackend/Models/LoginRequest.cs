namespace OrderBackend.Models;

public record LoginRequest(
    string Username,
    string Password
);
