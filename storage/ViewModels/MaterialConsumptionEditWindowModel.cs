using ReactiveUI;
using storage.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace storage.ViewModels;

public class MaterialConsumptionEditWindowModel : ReactiveObject
{
    public long Id { get; set; }
    public long Count { get; set; }
    public ObservableCollection<string> AvailableMaterials { get; set; }
    public ObservableCollection<string> AvailableInvoicies { get; set; }
    private MainViewModel _mainViewModel;
    private MaterialConsumption _MaterialConsumption;

    public MaterialConsumptionEditWindowModel(MaterialConsumption MaterialConsumption, List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        Count = MaterialConsumption.Count;
        _mainViewModel = mainView;
        _MaterialConsumption = MaterialConsumption;
        AvailableInvoicies = new ObservableCollection<string>(invoices.Select(x => x.CreatedAt.ToString()));
        AvailableMaterials = new ObservableCollection<string>(materials.Select(x => x.Name));
    }

    public void Save(Invoice invoice, Material material)
    {
        _mainViewModel.MaterialConsumptionAccess.Update(new MaterialConsumption(_MaterialConsumption.Id, Count, invoice, material));
    }
}