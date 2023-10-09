using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class MaterialEditWindow : Window
{
    public MaterialEditWindow(Material material, List<Category> availableCategories)
    {
        InitializeComponent();
        DataContext = new MaterialEditWindowModel(material, availableCategories);
    }
}

