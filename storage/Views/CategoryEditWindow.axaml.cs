using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class CategoryEditWindow : Window
{
    public CategoryEditWindow(MainViewModel mainView)
    {
        InitializeComponent();
        DataContext = new CategoryEditWindowModel(mainView);
    }

    public CategoryEditWindow(Category category, MainViewModel mainView)
    {
        InitializeComponent();
        DataContext = new CategoryEditWindowModel(category, mainView);
    }

    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var model = (CategoryEditWindowModel)DataContext;

        if (model.Validate() == false)
        {
            MessageBoxManager.GetMessageBoxStandard("Внимание", "Не все поля заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard("Внимание", "Вы уверены что хотите сохранить изменения?", ButtonEnum.YesNo);
        var result = await box.ShowAsync();

        if (result != ButtonResult.Yes || DataContext == null)
            return;
        
        model.Save();
        Close();
    }
}
