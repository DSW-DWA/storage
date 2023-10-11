using System;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class InvoiceEditWindowModel : ReactiveObject
{
    DateTimeOffset CreatedAt { get; set; }
    TimeSpan CreatedAtTime { get; set; }
    readonly Invoice? _invoice;
    readonly MainViewModel _mainViewModel;

    public InvoiceEditWindowModel(MainViewModel mainView)
    {
        _mainViewModel = mainView;
    }
    public InvoiceEditWindowModel(Invoice invoice, MainViewModel mainView)
    {
        _mainViewModel = mainView;
        _invoice = invoice;
        CreatedAt = new DateTimeOffset(invoice.CreatedAt);
        CreatedAtTime = invoice.CreatedAt.TimeOfDay;
    }
    public bool Validate()
    {
        return !(CreatedAtTime == TimeSpan.Zero);
    }
    public void Save()
    {
        if (_invoice == null)
        {
            _mainViewModel.InvoiceAccess.Save(new Invoice(0, new DateTime(CreatedAt.DateTime.Year, CreatedAt.DateTime.Month, CreatedAt.DateTime.Day, CreatedAtTime.Hours, CreatedAtTime.Minutes, CreatedAtTime.Seconds)));
        }
        else
        {
            _mainViewModel.InvoiceAccess.Update(new Invoice(_invoice.Id, new DateTime(CreatedAt.DateTime.Year, CreatedAt.DateTime.Month, CreatedAt.DateTime.Day, CreatedAtTime.Hours, CreatedAtTime.Minutes, CreatedAtTime.Seconds)));
        }
    }
}
