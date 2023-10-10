using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class InvoiceEditWindow : Window
{
    public InvoiceEditWindow(Invoice invoice, MainViewModel mainView)
    {
        InitializeComponent();
        DataContext = new InvoiceEditWindowModel(invoice, mainView);
    }

    async void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("Внимание", "Вы хотите сохранить изменения и закрыть окно?",
                ButtonEnum.YesNo);

        var result = await box.ShowAsync();

        if (result == ButtonResult.Yes && DataContext != null)
        {
            var model = (InvoiceEditWindowModel)DataContext;
            model.Save();
            this.Close();
        }
    }
}

