namespace OrderBackend.Models;

public record MedicalOrderCreateRequest(
    string Message,
    int PatientId
);
