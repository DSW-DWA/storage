using ReactiveUI;
using storage.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace storage.ViewModels;

public class MaterialConsumptionEditWindowModel : ReactiveObject
{
    long Id { get; set; }
    long Count { get; set; }
    ObservableCollection<string> AvailableMaterials { get; set; }
    ObservableCollection<string> AvailableInvoices { get; set; }
    readonly MainViewModel _mainViewModel;
    readonly MaterialConsumption? _materialConsumption;

    public MaterialConsumptionEditWindowModel(List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        _mainViewModel = mainView;
        AvailableInvoices = new ObservableCollection<string>(invoices.Select(x => x.CreatedAt.ToString(CultureInfo.CurrentCulture)));
        AvailableMaterials = new ObservableCollection<string>(materials.Select(x => x.Name));
    }

    public MaterialConsumptionEditWindowModel(MaterialConsumption materialConsumption, List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        Count = materialConsumption.Count;
        _mainViewModel = mainView;
        _materialConsumption = materialConsumption;
        AvailableInvoices = new ObservableCollection<string>(invoices.Select(x => x.CreatedAt.ToString(CultureInfo.CurrentCulture)));
        AvailableMaterials = new ObservableCollection<string>(materials.Select(x => x.Name));
    }

    public void Save(Invoice invoice, Material material)
    {
        if (_materialConsumption == null)
        {
            _mainViewModel.MaterialConsumptionAccess.Save(new MaterialConsumption(0, Count, invoice, material));
        }
        else
        {
            _mainViewModel.MaterialConsumptionAccess.Update(new MaterialConsumption(_materialConsumption.Id, Count, invoice, material));
        }
    }
}