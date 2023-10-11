using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class InvoiceEditWindow : Window
{
    
    public InvoiceEditWindow(MainViewModel mainView)
    {
        InitializeComponent();
        Title = "Создание накладной";
        DataContext = new InvoiceEditWindowModel(mainView);
    }

    public InvoiceEditWindow(Invoice invoice, MainViewModel mainView)
    {
        InitializeComponent();
        Title = "Редактирование накладной";
        DataContext = new InvoiceEditWindowModel(invoice, mainView);
    }

    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var model = (InvoiceEditWindowModel)DataContext!;

        if (model.Validate() == false)
        {
            await MessageBoxManager.GetMessageBoxStandard("Внимание", "Данные некорректны", ButtonEnum.Ok).ShowAsync();
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

