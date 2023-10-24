using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Data;
using storage.Models;
using storage.ViewModels;
using System.Threading;
using Avalonia.Controls.Shapes;
using Path = System.IO.Path;

namespace storage.Views;

public partial class MainWindow : Window
{
    readonly MainViewModel _model;
    private bool _askCascad = true;
    public MainWindow()
    {
        InitializeComponent();
        _model = new MainViewModel();
        DataContext = _model;
    }
    void UpdateAllGrids()
    {
        CategoryGrid.ItemsSource = _model.Categories;
        InvoiceGrid.ItemsSource = _model.Invoices;
        MaterialGrid.ItemsSource = _model.Materials;
        MaterialConsumptionGrid.ItemsSource = _model.MaterialConsumptions;
        MaterialReceiptGrid.ItemsSource = _model.MaterialReceipts;        
    }

    async void DeleteElementClick(object sender, RoutedEventArgs e)
    {
        if (_askCascad)
        {
            var box1 = MessageBoxManager
            .GetMessageBoxStandard("Внимание", "Выключить каскадное удланение?",
                ButtonEnum.YesNo);

            var result1 = await box1.ShowAsync();

            if (result1 == ButtonResult.Yes)
            {
                _model.SetCascad(false);
            }
            else
            {
                _model.SetCascad(true);
            }

            _askCascad = false;
        }

        var btn = (Button)sender;

        var box = MessageBoxManager
            .GetMessageBoxStandard("Внимание", "Вы уверены что хотите удалить элемент?",
                ButtonEnum.YesNo);

        var result = await box.ShowAsync();
        if (result == ButtonResult.No)
        {
            return;
        }
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
        UpdateAllGrids();
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


        var date = ReportDate.SelectedDate;
        var time = ReportTime.SelectedTime;

        if (date != null && time != null)
        {
            await Task.Run(() =>
            {
                _model.ExportToExcel((DateTimeOffset)date, (TimeSpan)time);
            });
        }

        excelButton.IsEnabled = true;
    }
    async void DownloadWordReportClick(object? sender, RoutedEventArgs e)
    {
        var wordButton = (Button)sender!;
        wordButton.IsEnabled = false;

        var date = ReportDate.SelectedDate;
        var time = ReportTime.SelectedTime;

        if (date != null && time != null)
        {
            await Task.Run(() =>
            {
                _model.ExportToWord((DateTimeOffset)date, (TimeSpan)time);
            });
        }

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
    async void EditCategoryClick(object? sender, RoutedEventArgs e)
    {
        if (CategoryGrid.SelectedItem == null)
            return;
        int categoryId = (int)GetEntityId(CategoryGrid.SelectedItem);
        var categoryToEdit = _model.CategoryAccess.GetById(categoryId);
        if (categoryToEdit == null)
            return;
        var editWindow = new CategoryEditWindow(categoryToEdit, _model);
        var task = editWindow.ShowDialog(this);

        await task;
        UpdateAllGrids();
    }
    async void EditInvoiceClick(object? sender, RoutedEventArgs e)
    {
        if (InvoiceGrid.SelectedItem == null)
            return;
        int invoiceId = (int)GetEntityId(InvoiceGrid.SelectedItem);
        var invoiceToEdit = _model.InvoiceAccess.GetById(invoiceId);
        if (invoiceToEdit == null)
            return;
        var editWindow = new InvoiceEditWindow(invoiceToEdit, _model);
        var task = editWindow.ShowDialog(this);

        await task;
        UpdateAllGrids();
    }
    async void EditMaterialClick(object? sender, RoutedEventArgs e)
    {
        if (MaterialGrid.SelectedItem == null)
            return;
        int materialId = (int)GetEntityId(MaterialGrid.SelectedItem);
        var materialToEdit = _model.MaterialAccess.GetById(materialId);
        if (materialToEdit == null)
            return;

        var availableCategories = _model.CategoryAccess.GetAll();
        var editWindow = new MaterialEditWindow(materialToEdit, availableCategories, _model);
        var task = editWindow.ShowDialog(this);

        await task;
        UpdateAllGrids();
    }

    async void EditMaterialReceiptClick(object? sender, RoutedEventArgs e)
    {
        if (MaterialReceiptGrid.SelectedItem == null)
            return;
        int materialReceiptId = (int)GetEntityId(MaterialReceiptGrid.SelectedItem);
        var materialReceiptToEdit = _model.MaterialReceiptAccess.GetById(materialReceiptId);
        if (materialReceiptToEdit == null)
            return;

        var availableInvoices = _model.InvoiceAccess.GetAll();
        var availableMaterials = _model.MaterialAccess.GetAll();
        var editWindow = new MaterialReceiptEditWindow(materialReceiptToEdit, availableInvoices, availableMaterials, _model);
        var task = editWindow.ShowDialog(this);

        await task;
        UpdateAllGrids();
    }

    async void EditMaterialConsumptionClick(object? sender, RoutedEventArgs e)
    {
        if (MaterialConsumptionGrid.SelectedItem == null)
            return;
        int materialConsumptionId = (int)GetEntityId(MaterialConsumptionGrid.SelectedItem);
        var materialConsumptionToEdit = _model.MaterialConsumptionAccess.GetById(materialConsumptionId);
        if (materialConsumptionToEdit == null)
            return;

        var availableInvoices = _model.InvoiceAccess.GetAll();
        var availableMaterials = _model.MaterialAccess.GetAll();
        var editWindow = new MaterialConsumptionEditWindow(materialConsumptionToEdit, availableInvoices, availableMaterials, _model);
        var task = editWindow.ShowDialog(this);

        await task;
        UpdateAllGrids();
    }
    async void CreateCategoryClick(object? sender, RoutedEventArgs e)
    {
        var saveWindow = new CategoryEditWindow(_model);
        await saveWindow.ShowDialog(this);
        UpdateAllGrids();
    }
    async void CreateInvoiceClick(object? sender, RoutedEventArgs e)
    {
        var saveWindow = new InvoiceEditWindow(_model);
        await saveWindow.ShowDialog(this);
        UpdateAllGrids();
    }
    async void CreateMaterialClick(object? sender, RoutedEventArgs e)
    {
        var availableCategories = _model.CategoryAccess.GetAll();
        var saveWindow = new MaterialEditWindow(availableCategories, _model);
        await saveWindow.ShowDialog(this);
        UpdateAllGrids();
    }
    async void CreateMaterialConsumptionClick(object? sender, RoutedEventArgs e)
    {
        var availableInvoices = _model.InvoiceAccess.GetAll();
        var availableMaterials = _model.MaterialAccess.GetAll();
        var editWindow = new MaterialConsumptionEditWindow(availableInvoices, availableMaterials, _model);
        await editWindow.ShowDialog(this);
        UpdateAllGrids();
    }
    async void CreateMaterialReceiptClick(object? sender, RoutedEventArgs e)
    {
        var availableInvoices = _model.InvoiceAccess.GetAll();
        var availableMaterials = _model.MaterialAccess.GetAll();
        var editWindow = new MaterialReceiptEditWindow(availableInvoices, availableMaterials, _model);
        await editWindow.ShowDialog(this);
        UpdateAllGrids();
    }
    async void SaveSchemaClick(object? sender, RoutedEventArgs e)
    {
        string filename = "./Data/saved_data_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".db";
        string path = Path.GetFullPath(filename);
        File.Copy("storage.db", filename, true);
        await MessageBoxManager.GetMessageBoxStandard("Внимание", "Data saved to " + path + "").ShowAsync();
    }
}