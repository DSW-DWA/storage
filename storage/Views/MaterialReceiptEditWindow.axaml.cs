using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class MaterialReceiptEditWindow : Window
{
    public MaterialReceiptEditWindow(MaterialReceipt materialReceipt)
    {
        InitializeComponent();
        DataContext = new MaterialReceiptWindowModel(materialReceipt);
    }
}

