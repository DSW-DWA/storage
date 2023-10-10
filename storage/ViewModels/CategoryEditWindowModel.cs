using ReactiveUI;
using storage.Models;

namespace storage.ViewModels;

public class CategoryEditWindowModel : ReactiveObject
{
    public string Name { get; set; }
    public string MeasureUnit { get; set; }
    private Category _category;
    private MainViewModel _mainViewModel;

    public CategoryEditWindowModel(Category category, MainViewModel mainView) 
    {
        _mainViewModel = mainView;
        _category = category;
        Name = category.Name;
        MeasureUnit = category.MeasureUnit;
    }

    public void Save()
    {
        _mainViewModel.CategoryAccess.Update(new Category(_category.Id, Name, MeasureUnit));
    }
}
