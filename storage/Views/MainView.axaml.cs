using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using storage.Models;
using storage.ViewModels;
using System.Threading;

namespace storage.Views;

public partial class MainView : UserControl
{
    public ObservableCollection<Category> Categories { get; }
    private MaterialInventory _inventory;

    public MainView()
    {
        InitializeComponent();

        // Initialize the MaterialInventory
        _inventory = new MaterialInventory();
        var dataSet = _inventory.MaterialInventoryDataset;

        var categories = new List<Category>();
        
        foreach (DataRowView rowView in dataSet.Tables["category"].DefaultView)
        {
            DataRow row = rowView.Row;
            categories.Add(new Category
            {
                Id = (int)row["id"],
                Name = (string)row["name"],
                MeasureUnit = (string)row["measure_unit"]
            });
        }

        Categories = new ObservableCollection<Category>(categories);
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        var t1 = new Thread(() => _inventory.ExportToWord());
        var t2 = new Thread(() => _inventory.ExportToExcel());

        t1.Start();
        t2.Start();
    }
}