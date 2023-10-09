using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class MaterialReceiptWindowModel : ReactiveObject
{
    public long Id { get; init; }
    public long Count { get; init; }
    public Invoice Invoice { get; init; }
    public Material Material { get; init; }

    public MaterialReceiptWindowModel(MaterialReceipt materialConsumption)
    {
        Id = materialConsumption.Id;
        Count = materialConsumption.Count;
        Invoice = materialConsumption.Invoice;
        Material = materialConsumption.Material;
    }
}
