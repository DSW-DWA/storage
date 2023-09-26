using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using storage.Models;
using storage.ViewModels;
using System.Threading;
using System.Linq;
using System;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace storage.Views;

public partial class MainView : UserControl
{
    
    private MainViewModel _model;

    public MainView()
    {
        InitializeComponent();

        // Initialize the MaterialInventory
        _model = new MainViewModel();
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        var t1 = new Thread(() => _model.ExportToWord());
        var t2 = new Thread(() => _model.ExportToExcel());

        t1.Start();
        t2.Start();
    }

    private async void SaveItemClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        switch (btn.Name)
        {
            case "SaveCategory":
                var item = (Category)CategoryGrid.SelectedItem;
                if (item != null && item.Name != null && item.Name != "" && item.MeasureUnit != null && item.MeasureUnit !="")
                {
                    _model.CategoryAccess.SaveCategory(item);
                } else
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Ошибка", "Данные элемента имеют не верный формат",
                    ButtonEnum.Ok);
                    var result = await box.ShowAsync();
                }
                break;
            case "SaveInvoice":
                var invoiceItem = (Invoice)InvoiceGrid.SelectedItem;
                if (invoiceItem != null && invoiceItem.CreatedAt != null)
                {
                    _model.InvoiceAccess.SaveInvoice(invoiceItem);
                } else
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Ошибка", "Данные элемента имеют не верный формат",
                    ButtonEnum.Ok);
                    var result = await box.ShowAsync();
                }

                break;
            case "SaveMaterial":
                var materialItem = (Material)MaterialGrid.SelectedItem;
                if (materialItem != null && materialItem.CategoryId != -1 && materialItem.CategoryId != null && materialItem.Name != null && materialItem.Name != "")
                {
                    _model.MaterialAccess.SaveMaterial(materialItem);
                } else
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Ошибка", "Данные элемента имеют не верный формат",
                    ButtonEnum.Ok);
                    var result = await box.ShowAsync();
                }
                break;
            case "SaveMaterialConsumption":
                var consumptionItem = (MaterialConsumption)MaterialConsmptionGrid.SelectedItem;
                if (consumptionItem != null && consumptionItem.Count > 0 && consumptionItem.Count != null && consumptionItem.InvoiceId > 0 && consumptionItem.InvoiceId != null && consumptionItem.MaterialId > 0 && consumptionItem.MaterialId != null)
                {
                    _model.MaterialConsumptionAccess.SaveMaterialConsumption(consumptionItem);
                } else
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Ошибка", "Данные элемента имеют не верный формат",
                    ButtonEnum.Ok);
                    var result = await box.ShowAsync();
                }
                break;
            case "SaveMaterialReceipt":
                var receiptItem = (MaterialReceipt)MaterialReceiptGrid.SelectedItem;
                if (receiptItem != null && receiptItem.Count > 0 && receiptItem.Count != null && receiptItem.InvoiceId > 0 && receiptItem.InvoiceId != null && receiptItem.MaterialId > 0 && receiptItem.MaterialId != null)
                {
                    _model.MaterialReceiptAccess.SaveMaterialReceipt(receiptItem);
                } else
                {
                    var box = MessageBoxManager
                    .GetMessageBoxStandard("Ошибка", "Данные элемента имеют не верный формат",
                    ButtonEnum.Ok);
                    var result = await box.ShowAsync();
                }

                break;

        }
    }

    private void DeleteItemClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        switch (btn.Name)
        {
            case "DeleteCategory":
                var item = (Category)CategoryGrid.SelectedItem;
                _model.CategoryAccess.DeleteCategory(item.Id);
                _model.Categories.Remove(item);
                CategoryGrid.ItemsSource = _model.Categories;
                break;
            case "DeleteInvoice":
                var invoiceItem = (Invoice)InvoiceGrid.SelectedItem;
                _model.InvoiceAccess.DeleteInvoice(invoiceItem.Id);
                _model.Invoices.Remove(invoiceItem);
                InvoiceGrid.ItemsSource = _model.Invoices;

                break;
            case "DeleteMaterial":
                var materialItem = (Material)MaterialGrid.SelectedItem;
                _model.MaterialAccess.DeleteMaterial(materialItem.Id); 
                _model.Materials.Remove(materialItem);
                MaterialGrid.ItemsSource = _model.Materials;
                break;
            case "DeleteMaterialConsumption":
                var consumptionItem = (MaterialConsumption)MaterialConsmptionGrid.SelectedItem;
                _model.MaterialConsumptionAccess.DeleteMaterialConsumption(consumptionItem.Id);
                _model.MaterialConsumptions.Remove(consumptionItem);
                MaterialConsmptionGrid.ItemsSource = _model.MaterialConsumptions;
                break;
            case "DeleteMaterialReceipt":
                var receiptItem = (MaterialReceipt)MaterialReceiptGrid.SelectedItem;
                _model.MaterialReceiptAccess.DeleteMaterialReceipt(receiptItem.Id);
                _model.MaterialReceipts.Remove(receiptItem);
                MaterialReceiptGrid.ItemsSource = _model.MaterialReceipts;
                break;

        }
    }
    private async void CreateItemClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        switch (btn.Name)
        {
            case "CreateCategory":
                _model.Categories.Add(new Category
                {
                    Id = _model.Categories.Select(x => x.Id).Max() + 1,
                    Name = null,
                    MeasureUnit = null
                });
                CategoryGrid.ItemsSource = _model.Categories;
                
                break;
            case "CreateInvoice":
                _model.Invoices.Add(new Invoice
                {
                    Id = _model.Invoices.Select(x => x.Id).Max() + 1,
                    CreatedAt = DateTime.Now,
                });
                InvoiceGrid.ItemsSource = _model.Invoices;


                break;
            case "CreateMaterial":
                _model.Materials.Add(new Material
                {
                    Id = _model.Materials.Select(x => x.Id).Max() + 1,
                    Name = null,
                    CategoryId = -1,
                });
                MaterialGrid.ItemsSource = _model.Materials;

                break;
            case "CreateMaterialConsmption":
                _model.MaterialConsumptions.Add(new MaterialConsumption
                {
                    Id = _model.MaterialConsumptions.Select(x => x.Id).Max() + 1,
                    Count = -1,
                    MaterialId = -1,
                    InvoiceId = -1,
                });
                MaterialConsmptionGrid.ItemsSource = _model.MaterialConsumptions;

                break;
            case "CreateMaterialReceipt":
                _model.MaterialReceipts.Add(new MaterialReceipt
                {
                    Id = _model.MaterialReceipts.Select(x => x.Id).Max() + 1,
                    Count = -1,
                    MaterialId = -1,
                    InvoiceId = -1,
                });
                MaterialReceiptGrid.ItemsSource= _model.MaterialReceipts;

                break;
            default:
                var box = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Неверная таблица",
                ButtonEnum.Ok);
                var result = await box.ShowAsync();

                break;
        }
    }
}