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
}