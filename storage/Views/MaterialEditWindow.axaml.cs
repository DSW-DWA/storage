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
    private List<Category> _categories;
    public MaterialEditWindow(Material material, List<Category> availableCategories, MainViewModel mainView)
    {
        InitializeComponent();
        DataContext = new MaterialEditWindowModel(material, availableCategories, mainView);
        _categories = availableCategories;
        ComboBox.SelectedIndex = availableCategories.FindIndex(x => x.Id == material.Category.Id);
    }

    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("��������", "�� ������ ��������� ��������� � ������� ����?",
                ButtonEnum.YesNo);

        var result = await box.ShowAsync();

        if (result == ButtonResult.Yes && DataContext != null)
        {
            var model = (MaterialEditWindowModel)DataContext;
            model.Save(_categories[ComboBox.SelectedIndex]);
            this.Close();
        }
    }
}

