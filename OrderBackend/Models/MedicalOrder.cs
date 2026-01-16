namespace OrderBackend.Models;

public class MedicalOrder
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int PatientId { get; set; }
}
