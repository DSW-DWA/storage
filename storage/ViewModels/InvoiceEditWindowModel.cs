using System;
using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class InvoiceEditWindowModel : ReactiveObject
{
    public DateTimeOffset CreatedAt { get; set; }
    public TimeSpan CreatedAtTime { get; set; }
    private Invoice _invoice;
    private MainViewModel _mainViewModel;

    public InvoiceEditWindowModel(Invoice invoice, MainViewModel mainView)
    {
        _mainViewModel = mainView;
        _invoice = invoice;
        CreatedAt = new DateTimeOffset(invoice.CreatedAt);
        CreatedAtTime = invoice.CreatedAt.TimeOfDay;
    }
    public void Save()
    {
        _mainViewModel.InvoiceAccess.Update(new Invoice(_invoice.Id, CreatedAt.DateTime.Add(CreatedAtTime)));
    }
}
