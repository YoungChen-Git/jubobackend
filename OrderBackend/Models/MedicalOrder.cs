namespace OrderBackend.Models;

public class MedicalOrder
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    
    // Navigation property
    public Patient? Patient { get; set; }
}
