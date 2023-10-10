using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Models;
using storage.ViewModels;
using System.Collections.Generic;

namespace storage.Views;

public partial class MaterialReceiptEditWindow : Window
{
    private List<Invoice> _invoices;
    private List<Material> _materials;
    public MaterialReceiptEditWindow(MaterialReceipt materialReceipt, List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        InitializeComponent();
        _invoices = invoices;
        _materials = materials;
        DataContext = new MaterialReceiptEditWindowModel(materialReceipt, invoices, materials, mainView);
        ComboBoxInvoice.SelectedIndex = invoices.FindIndex(x => x.Id == materialReceipt.Invoice.Id);
        ComboBoxMaterial.SelectedIndex = materials.FindIndex(x => x.Id == materialReceipt.Material.Id);
    }
    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("Внимание", "Вы хотитн сохранить изменения и закрыть окно?",
                ButtonEnum.YesNo);

        var result = await box.ShowAsync();

        if (result == ButtonResult.Yes && DataContext != null)
        {
            var model = (MaterialReceiptEditWindowModel)DataContext;
            model.Save(_invoices[ComboBoxInvoice.SelectedIndex], _materials[ComboBoxMaterial.SelectedIndex]);
            this.Close();
        }
    }
}

