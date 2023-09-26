using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using storage.Models;
using storage.ViewModels;
using System.Threading;
using System.Linq;
using System;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Data;

namespace storage.Views;

public partial class MainView : UserControl
{

    readonly MainViewModel _model;

    public MainView()
    {
        InitializeComponent();

        _model = new MainViewModel();
    }

    void OnButtonClick(object sender, RoutedEventArgs e)
    {
        var t1 = new Thread(() => _model.ExportToWord());
        var t2 = new Thread(() => _model.ExportToExcel());

        t1.Start();
        t2.Start();
    }

    async void UpdateElementClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        string errorMessage = "Данные элемента имеют неверный формат";

        switch (btn.Name)
        {
            case "SaveCategory":
                var categoryElement = (Category)CategoryGrid.SelectedItem;
                if (IsValidCategory(categoryElement))
                {
                    _model.CategoryAccess.Update(categoryElement);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "SaveInvoice":
                var invoiceItem = (Invoice)InvoiceGrid.SelectedItem;
                if (IsValidInvoice(invoiceItem))
                {
                    _model.InvoiceAccess.Update(invoiceItem);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "SaveMaterial":
                var materialItem = (Material)MaterialGrid.SelectedItem;
                if (IsValidMaterial(materialItem))
                {
                    _model.MaterialAccess.Update(materialItem);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "SaveMaterialConsumption":
                var consumptionItem = (MaterialConsumption)MaterialConsumptionGrid.SelectedItem;
                if (IsValidMaterialConsumption(consumptionItem))
                {
                    _model.MaterialConsumptionAccess.Update(consumptionItem);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "SaveMaterialReceipt":
                var receiptItem = (MaterialReceipt)MaterialReceiptGrid.SelectedItem;
                if (IsValidMaterialReceipt(receiptItem))
                {
                    _model.MaterialReceiptAccess.Update(receiptItem);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
        }
    }

    bool IsValidCategory(Category? element)
    {
        return element != null && !string.IsNullOrWhiteSpace(element.Name) && !string.IsNullOrWhiteSpace(element.MeasureUnit);
    }

    bool IsValidInvoice(Invoice? element)
    {
        return element?.CreatedAt != null;
    }

    bool IsValidMaterial(Material? element)
    {
        return element != null && element.CategoryId != -1 && !string.IsNullOrWhiteSpace(element.Name);
    }

    bool IsValidMaterialConsumption(MaterialConsumption? element)
    {
        return element is { Count: > 0, InvoiceId: > 0, MaterialId: > 0 };
    }

    bool IsValidMaterialReceipt(MaterialReceipt? element)
    {
        return element is { Count: > 0, InvoiceId: > 0, MaterialId: > 0 };
    }

    async Task ShowErrorMessage(string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", message, ButtonEnum.Ok);
        await box.ShowAsync();
    }

    void DeleteElementClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;

        switch (btn.Name)
        {
            case "DeleteCategory":
                DeleteElement<Category>(_model.CategoryAccess, CategoryGrid, _model.Categories);
                break;
            case "DeleteInvoice":
                DeleteElement<Invoice>(_model.InvoiceAccess, InvoiceGrid, _model.Invoices);
                break;
            case "DeleteMaterial":
                DeleteElement<Material>(_model.MaterialAccess, MaterialGrid, _model.Materials);
                break;
            case "DeleteMaterialConsumption":
                DeleteElement<MaterialConsumption>(_model.MaterialConsumptionAccess, MaterialConsumptionGrid, _model.MaterialConsumptions);
                break;
            case "DeleteMaterialReceipt":
                DeleteElement<MaterialReceipt>(_model.MaterialReceiptAccess, MaterialReceiptGrid, _model.MaterialReceipts);
                break;
        }
    }

    void DeleteElement<T>(IAccess<T> access, DataGrid dataGrid, ObservableCollection<T> collection)
    {
        var item = (T)dataGrid.SelectedItem;
        if (item == null) return;

        int itemId = Convert.ToInt32(item.GetType().GetProperty("Id")?.GetValue(item));
        access.Delete(itemId);
        var itemToRemove = collection.First(entity => GetEntityId(entity) == itemId);
        collection.Remove(itemToRemove);
        dataGrid.ItemsSource = collection;
    }

    async void CreateElementClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;

        switch (btn.Name)
        {
            case "CreateCategory":
                int catId = GetNextId(_model.Categories);
                var category = new Category(catId, $"Категория {catId}", "шт");
                _model.CategoryAccess.Save(category);
                _model.Categories.Add(category);
                CategoryGrid.ItemsSource = _model.Categories;
                break;
            case "CreateInvoice":
                var invoice = new Invoice(GetNextId(_model.Invoices), DateTime.Now);
                _model.InvoiceAccess.Save(invoice);
                _model.Invoices.Add(invoice);
                InvoiceGrid.ItemsSource = _model.Invoices;
                break;
            case "CreateMaterial":
                int matId = GetNextId(_model.Materials);
                var material = new Material(matId, $"Материал {matId}", GetLastEntityId(_model.Categories));
                _model.MaterialAccess.Save(material);
                _model.Materials.Add(material);
                MaterialGrid.ItemsSource = _model.Materials;
                break;
            case "CreateMaterialConsumption":
                var materialConsumption = new MaterialConsumption(
                    GetNextId(_model.MaterialConsumptions), 0, GetLastEntityId(_model.Invoices), GetLastEntityId(_model.Materials));
                _model.MaterialConsumptionAccess.Save(materialConsumption);
                _model.MaterialConsumptions.Add(materialConsumption);
                MaterialConsumptionGrid.ItemsSource = _model.MaterialConsumptions;
                break;
            case "CreateMaterialReceipt":
                var materialReceipt = new MaterialReceipt(GetNextId(_model.MaterialConsumptions), 0, GetLastEntityId(_model.Invoices), GetLastEntityId(_model.Materials));
                _model.MaterialReceiptAccess.Save(materialReceipt);
                _model.MaterialReceipts.Add(materialReceipt);
                MaterialReceiptGrid.ItemsSource = _model.MaterialReceipts;
                break;
            default:
                await ShowErrorMessage("Неверная таблица");
                break;
        }
    }

    int GetEntityId<T>(T entity)
    {
        var idProperty = entity?.GetType().GetProperty("Id");
        if (idProperty == null)
            throw new ArgumentException("Entity does not have a valid ID property.");
        object? idValue = idProperty.GetValue(entity);
        if (idValue != null) return Convert.ToInt32(idValue);
        throw new ArgumentException("Entity does not have a valid ID property.");
    }

    int GetNextId<T>(ObservableCollection<T> collection)
    {
        return collection.Count > 0 ? collection.Max(entity => (int?)GetEntityId(entity)) + 1 ?? 1 : 1;
    }

    int GetLastEntityId<T>(ObservableCollection<T> collection)
    {
        return collection.Count > 0 ? collection.Max(entity => (int?)GetEntityId(entity)) ?? 1 : 1;
    }
}
