namespace storage.Models;

public class MaterialReceipt
{
    public long Id { get; init; }
    public long Count { get; init; }
    public Invoice? Invoice { get; init; }
    public Material? Material { get; init; }

    public MaterialReceipt(long id, long count, Invoice? invoice, Material? material)
    {
        Id = id;
        Count = count;
        Invoice = invoice;
        Material = material;
    }
}
