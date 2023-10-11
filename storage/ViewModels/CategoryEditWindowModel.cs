using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using storage.Models;

namespace storage.ViewModels;

public class CategoryEditWindowModel : ReactiveObject
{
    string? Name { get; set; }
    string? MeasureUnit { get; set; }
    readonly Category? _category;
    readonly MainViewModel _mainViewModel;
    
    public CategoryEditWindowModel(MainViewModel mainView) 
    {
        _mainViewModel = mainView;
    }

    public CategoryEditWindowModel(Category category, MainViewModel mainView) 
    {
        _mainViewModel = mainView;
        _category = category;
        Name = category.Name;
        MeasureUnit = category.MeasureUnit;
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(MeasureUnit))
        {
            MessageBoxManager.GetMessageBoxStandard("Внимание", "Не все поля заполнены", ButtonEnum.OkAbort).ShowAsync();
            return;
        }
        if (_category == null)
        {
            _mainViewModel.CategoryAccess.Save(new Category(0, Name, MeasureUnit));            
        }
        else
        {
            _mainViewModel.CategoryAccess.Update(new Category(_category.Id, Name, MeasureUnit));
        }
    }
}
