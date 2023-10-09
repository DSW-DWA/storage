using System;
using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class InvoiceEditWindowModel : ReactiveObject
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public InvoiceEditWindowModel(Invoice invoice)
    {
        Id = invoice.Id;
        CreatedAt = invoice.CreatedAt;
    }
}
