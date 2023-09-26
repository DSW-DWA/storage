namespace storage.Models;

public class MaterialConsumption
{
    public long Id { get; set; }
    public long Count { get; set; }
    public long InvoiceId { get; set; }
    public long MaterialId { get; set; }
}