using Avalonia.Controls;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class InvoiceEditWindow : Window
{
    public InvoiceEditWindow(Invoice invoice)
    {
        InitializeComponent();
        DataContext = new InvoiceEditWindowModel(invoice);
    }
}

