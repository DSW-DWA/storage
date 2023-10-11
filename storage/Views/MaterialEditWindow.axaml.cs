using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class MaterialEditWindow : Window
{
    readonly List<Category> _categories;
    public MaterialEditWindow(List<Category> availableCategories, MainViewModel mainView)
    {
        InitializeComponent();
        DataContext = new MaterialEditWindowModel(availableCategories, mainView);
        _categories = availableCategories;
        ComboBox.SelectedIndex = availableCategories.FindLastIndex(x => true);
    }
    public MaterialEditWindow(Material material, List<Category> availableCategories, MainViewModel mainView)
    {
        InitializeComponent();
        DataContext = new MaterialEditWindowModel(material, availableCategories, mainView);
        _categories = availableCategories;
        ComboBox.SelectedIndex = availableCategories.FindIndex(x => x.Id == material.Category.Id);
    }

    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Внимание", "Вы уверены что хотите сохранить изменения?", ButtonEnum.YesNo);
        var result = await box.ShowAsync();

        if (result != ButtonResult.Yes || DataContext == null)
            return;
        var model = (MaterialEditWindowModel)DataContext;
        model.Save(_categories[ComboBox.SelectedIndex]);
        Close();
    }
}

