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

public partial class MaterialConsumptionEditWindow : Window
{
    private List<Invoice> _invoices;
    private List<Material> _materials;

    public MaterialConsumptionEditWindow(List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        InitializeComponent();
        _invoices = invoices;
        _materials = materials;
        DataContext = new MaterialConsumptionEditWindowModel(invoices, materials, mainView);
        ComboBoxInvoice.SelectedIndex = invoices.FindLastIndex(x => true);
        ComboBoxMaterial.SelectedIndex = materials.FindLastIndex(x => true);
    }
    public MaterialConsumptionEditWindow(MaterialConsumption materialConsumption, List<Invoice> invoices, List<Material> materials, MainViewModel mainView)
    {
        InitializeComponent();
        _invoices = invoices;
        _materials = materials;
        DataContext = new MaterialConsumptionEditWindowModel(materialConsumption, invoices, materials, mainView);
        ComboBoxInvoice.SelectedIndex = invoices.FindIndex(x => x.Id == materialConsumption.Invoice.Id);
        ComboBoxMaterial.SelectedIndex = materials.FindIndex(x => x.Id == materialConsumption.Material.Id);
    }
    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var model = (MaterialConsumptionEditWindowModel)DataContext;
        if (model.Validate(ComboBoxInvoice.SelectedItem, ComboBoxMaterial.SelectedItem) == false)
        {
            MessageBoxManager.GetMessageBoxStandard("Внимание", "Не все поля заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard("Внимание", "Вы уверены что хотите сохранить изменения?", ButtonEnum.YesNo);
        var result = await box.ShowAsync();

        if (result != ButtonResult.Yes || DataContext == null)
            return;
        
        model.Save(_invoices[ComboBoxInvoice.SelectedIndex], _materials[ComboBoxMaterial.SelectedIndex]);
        Close();
    }
}

