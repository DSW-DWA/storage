using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using storage.Models;
using storage.ViewModels;
using System.Threading;
using System.Linq;

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

    // Category Save and Delete
    private void SaveCategoryClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to save changes for the selected category

    }

    private void DeleteCategoryClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to delete the selected category
    }

    // Invoice Save and Delete
    private void SaveInvoiceClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to save changes for the selected invoice
    }

    private void DeleteInvoiceClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to delete the selected invoice
    }

    // Material Save and Delete
    private void SaveMaterialClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to save changes for the selected material
    }

    private void DeleteMaterialClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to delete the selected material
    }

    // MaterialConsumption Save and Delete
    private void SaveConsumptionClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to save changes for the selected consumption
    }

    private void DeleteConsumptionClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to delete the selected consumption
    }

    // MaterialReceipt Save and Delete
    private void SaveReceiptClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to save changes for the selected receipt
    }

    private void DeleteReceiptClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to delete the selected receipt
    }

    // Create button functions
    private void CreateCategoryClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to create a new category
        var btn = (Button)sender;
        switch (btn.Name)
        {
            case "AddCategory":
                _model.Categories.Add(new Category
                {
                    Id = _model.Categories.Select(x => x.Id).Max() + 1,
                    Name = null,
                    MeasureUnit = null
                });
                Category.ItemsSource = _model.Categories;
                
                break;
            default:
                break;
        }
    }

    private void CreateInvoiceClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to create a new invoice
    }

    private void CreateMaterialClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to create a new material
    }

    private void CreateConsumptionClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to create a new consumption
    }

    private void CreateReceiptClick(object sender, RoutedEventArgs e)
    {
        // Implement the logic to create a new receipt
    }
}