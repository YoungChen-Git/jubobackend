namespace OrderBackend.Models;

public class Patient
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    
    // Navigation property - 一個病人可以有多個醫囑
    public ICollection<MedicalOrder> MedicalOrders { get; set; } = new List<MedicalOrder>();
}
