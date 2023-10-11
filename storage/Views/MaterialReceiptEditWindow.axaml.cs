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
    public MaterialReceiptEditWindow(List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        InitializeComponent();
        Title = "Создание поступления на материал";
        _invoices = invoices;
        _materials = materials;
        DataContext = new MaterialReceiptEditWindowModel(invoices, materials, mainView);
        ComboBoxInvoice.SelectedIndex = invoices.FindLastIndex(x => true);
        ComboBoxMaterial.SelectedIndex = materials.FindLastIndex(x => true);
    }
    public MaterialReceiptEditWindow(MaterialReceipt materialReceipt, List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        InitializeComponent();
        Title = "Редактирование поступления на материал";
        _invoices = invoices;
        _materials = materials;
        DataContext = new MaterialReceiptEditWindowModel(materialReceipt, invoices, materials, mainView);
        ComboBoxInvoice.SelectedIndex = invoices.FindIndex(x => x.Id == materialReceipt.Invoice.Id);
        ComboBoxMaterial.SelectedIndex = materials.FindIndex(x => x.Id == materialReceipt.Material.Id);
    }
    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var model = (MaterialReceiptEditWindowModel)DataContext!;
        if (model.Validate(ComboBoxInvoice.SelectedItem, ComboBoxMaterial.SelectedItem) == false)
        {
            await MessageBoxManager.GetMessageBoxStandard("Внимание", "Данные некорректны", ButtonEnum.Ok).ShowAsync();
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard("Внимание", "Вы уверены что хотите сохранить изменения?", ButtonEnum.YesNo);
        var result = await box.ShowAsync();

        if (result != ButtonResult.Yes || DataContext == null)
            return;
        
        model.Save(_invoices[ComboBoxInvoice.SelectedIndex], _materials[ComboBoxMaterial.SelectedIndex]);
        this.Close();
    }
}
