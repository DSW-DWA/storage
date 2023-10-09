using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class MaterialConsumptionEditWindowModel : ReactiveObject
{
    public long Id { get; init; }
    public long Count { get; init; }
    public Invoice Invoice { get; init; }
    public Material Material { get; init; }

    public MaterialConsumptionEditWindowModel(MaterialConsumption materialConsumption)
    {
        Id = materialConsumption.Id;
        Count = materialConsumption.Count;
        Invoice = materialConsumption.Invoice;
        Material = materialConsumption.Material;
    }
}
