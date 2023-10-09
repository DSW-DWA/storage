using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using storage.Data;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class MainWindow : Window
{
    readonly MainViewModel _model;

    public MainWindow()
    {
        InitializeComponent();
        _model = new MainViewModel();
    }
    void CreateElementClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    void EditCategoryClick(object? sender, RoutedEventArgs e)
    {
        if (CategoryGrid.SelectedItem == null)
            return;
        int categoryId = (int)GetEntityId(CategoryGrid.SelectedItem);
        var categoryToEdit = _model.CategoryAccess.GetById(categoryId);
        if (categoryToEdit == null)
            return;
        var editWindow = new CategoryEditWindow(categoryToEdit);
        editWindow.ShowDialog(this);
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
    
    void DeleteElement<T>(IAccess<T> access, DataGrid dataGrid, ICollection<T> collection)
    {
        var item = (T)dataGrid.SelectedItem;
        if (item == null) return;

        int itemId = (int)GetEntityId(item);
        access.Delete(itemId);
        var itemToRemove = collection.First(entity => GetEntityId(entity) == itemId);
        collection.Remove(itemToRemove);
        dataGrid.ItemsSource = collection;
    }
    
    async void DownloadExcelReportClick(object? sender, RoutedEventArgs e)
    {
        var excelButton = (Button)sender!;
        excelButton.IsEnabled = false;

        await Task.Run(() =>
        {
            _model.ExportToExcel();
        });

        excelButton.IsEnabled = true;
    }
    async void DownloadWordReportClick(object? sender, RoutedEventArgs e)
    {
        var wordButton = (Button)sender!;
        wordButton.IsEnabled = false;

        await Task.Run(() =>
        {
            _model.ExportToWord();
        });

        wordButton.IsEnabled = true;
    }
    
    long GetEntityId<T>(T entity)
    {
        var idProperty = entity?.GetType().GetProperty("Id");
        if (idProperty == null)
            throw new ArgumentException("Entity does not have a valid ID property.");
        object? idValue = idProperty.GetValue(entity);
        if (idValue != null) return Convert.ToInt32(idValue);
        throw new ArgumentException("Entity does not have a valid ID property.");
    }
    void EditInvoiceClick(object? sender, RoutedEventArgs e)
    {
        if (InvoiceGrid.SelectedItem == null)
            return;
        int invoiceId = (int)GetEntityId(InvoiceGrid.SelectedItem);
        var invoiceToEdit = _model.InvoiceAccess.GetById(invoiceId);
        if (invoiceToEdit == null)
            return;
        var editWindow = new InvoiceEditWindow(invoiceToEdit);
        editWindow.ShowDialog(this);
    }
    void EditMaterialClick(object? sender, RoutedEventArgs e)
    {
        if (MaterialGrid.SelectedItem == null)
            return;
        int materialId = (int)GetEntityId(MaterialGrid.SelectedItem);
        var materialToEdit = _model.MaterialAccess.GetById(materialId);
        if (materialToEdit == null)
            return;

        var availableCategories = _model.CategoryAccess.GetAll();
        var editWindow = new MaterialEditWindow(materialToEdit, availableCategories);
        editWindow.ShowDialog(this);
    }

    void EditMaterialReceiptClick(object? sender, RoutedEventArgs e)
    {
        if (MaterialReceiptGrid.SelectedItem == null)
            return;
        int materialReceiptId = (int)GetEntityId(MaterialReceiptGrid.SelectedItem);
        var materialReceiptToEdit = _model.MaterialReceiptAccess.GetById(materialReceiptId);
        if (materialReceiptToEdit == null)
            return;
        var editWindow = new MaterialReceiptEditWindow(materialReceiptToEdit);
        editWindow.ShowDialog(this);
    }

    void EditMaterialConsumptionClick(object? sender, RoutedEventArgs e)
    {
        if (MaterialConsumptionGrid.SelectedItem == null)
            return;
        int materialConsumptionId = (int)GetEntityId(MaterialConsumptionGrid.SelectedItem);
        var materialConsumptionToEdit = _model.MaterialConsumptionAccess.GetById(materialConsumptionId);
        if (materialConsumptionToEdit == null)
            return;
        var editWindow = new MaterialConsumptionEditWindow(materialConsumptionToEdit);
        editWindow.ShowDialog(this);
    }
}