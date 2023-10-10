using ReactiveUI;
using storage.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace storage.ViewModels;

public class MaterialReceiptEditWindowModel : ReactiveObject
{
    public long Id { get; set; }
    public long Count { get; set; }
    public ObservableCollection<string> AvailableMaterials { get; set; }
    public ObservableCollection<string> AvailableInvoicies { get; set; }
    private MainViewModel _mainViewModel;
    private MaterialReceipt _materialReceipt;

    public MaterialReceiptEditWindowModel(MaterialReceipt materialReceipt, List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        Count = materialReceipt.Count;
        _mainViewModel = mainView;
        _materialReceipt = materialReceipt;
        AvailableInvoicies = new ObservableCollection<string>(invoices.Select(x => x.CreatedAt.ToString()));
        AvailableMaterials = new ObservableCollection<string>(materials.Select(x => x.Name));
    }

    public void Save(Invoice invoice, Material material)
    {
        _mainViewModel.MaterialReceiptAccess.Update(new MaterialReceipt(_materialReceipt.Id, Count, invoice, material));
    }
}
