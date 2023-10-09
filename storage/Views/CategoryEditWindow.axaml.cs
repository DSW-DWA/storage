using Avalonia.Controls;
using storage.Models;
using storage.ViewModels;

namespace storage.Views;

public partial class CategoryEditWindow : Window
{
    public CategoryEditWindow(Category category)
    {
        InitializeComponent();
        DataContext = new CategoryEditWindowModel(category);
    }
}
