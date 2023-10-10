using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class MaterialConsumptionEditWindow : Window
{
    public MaterialConsumptionEditWindow(MaterialConsumption materialConsumption, MainViewModel mainView)
    {
        InitializeComponent();
        DataContext = new MaterialConsumptionEditWindowModel(materialConsumption);
    }
}

