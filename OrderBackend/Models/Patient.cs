namespace OrderBackend.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation property - 一個病人可以有多個醫囑
    public ICollection<MedicalOrder> MedicalOrders { get; set; } = new List<MedicalOrder>();
}
