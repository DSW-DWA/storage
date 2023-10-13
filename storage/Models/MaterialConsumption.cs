namespace storage.Models;

public class MaterialConsumption
{
    public long Id { get; init; }
    public long Count { get; init; }
    public Invoice? Invoice { get; init; }
    public Material? Material { get; init; }

    public MaterialConsumption(long id, long count, Invoice? invoice, Material? material)
    {
        Id = id;
        Count = count;
        Invoice = invoice;
        Material = material;
    }
}
