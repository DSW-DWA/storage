using Avalonia.Controls;
using storage.Models;
using storage.ViewModels;

namespace storage.Views
{
    public partial class CategoryEditWindow : Window
    {
        private CategoryEditWindowModel _model;
        public CategoryEditWindow(Category category)
        {
            InitializeComponent();

            _model = new CategoryEditWindowModel(category);
        }
    }
}
