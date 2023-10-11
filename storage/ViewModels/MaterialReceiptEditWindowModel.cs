using ReactiveUI;
using storage.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace storage.ViewModels;

public class MaterialReceiptEditWindowModel : ReactiveObject
{
    long Id { get; set; }
    long Count { get; set; }
    ObservableCollection<string> AvailableMaterials { get; set; }
    ObservableCollection<string> AvailableInvoices { get; set; }
    readonly MainViewModel _mainViewModel;
    readonly MaterialReceipt? _materialReceipt;

    public MaterialReceiptEditWindowModel(List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        _mainViewModel = mainView;
        AvailableInvoices = new ObservableCollection<string>(invoices.Select(x => x.CreatedAt.ToString(CultureInfo.CurrentCulture)));
        AvailableMaterials = new ObservableCollection<string>(materials.Select(x => x.Name));
    }
    public MaterialReceiptEditWindowModel(MaterialReceipt materialReceipt, List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        Count = materialReceipt.Count;
        _mainViewModel = mainView;
        _materialReceipt = materialReceipt;
        AvailableInvoices = new ObservableCollection<string>(invoices.Select(x => $"{x.Id} - {x.CreatedAt.ToString(CultureInfo.CurrentCulture)}"));
        AvailableMaterials = new ObservableCollection<string>(materials.Select(x => x.Name));
    }
    public bool Validate(object? obj1, object? obj2)
    {
        return !(obj1 == null || obj2 == null || Count <= 0);
    }

    public void Save(Invoice invoice, Material material)
    {
        if (_materialReceipt == null)
        {
            _mainViewModel.MaterialReceiptAccess.Save(new MaterialReceipt(0, Count, invoice, material));
        }
        else
        {
            _mainViewModel.MaterialReceiptAccess.Update(new MaterialReceipt(_materialReceipt.Id, Count, invoice, material));
        }
    }
}
