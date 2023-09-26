namespace storage.Models;

public class MaterialConsumption
{
    public long Id { get; init; }
    public long Count { get; init; }
    public long InvoiceId { get; init; }
    public long MaterialId { get; init; }

    public MaterialConsumption(long id, long count, long invoiceId, long materialId)
    {
        Id = id;
        Count = count;
        InvoiceId = invoiceId;
        MaterialId = materialId;
    }
}
