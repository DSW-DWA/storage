namespace storage.Models;

public class MaterialReceipt
{
    public int Id { get; set; }
    public int Count { get; set; }
    public int InvoiceId { get; set; }
    public int MaterialId { get; set; }
}