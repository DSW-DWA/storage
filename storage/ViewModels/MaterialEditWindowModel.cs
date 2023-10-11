using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class MaterialEditWindowModel : ReactiveObject
{
    string? Name { get; init; }
    readonly Material? _material;
    readonly MainViewModel _mainViewModel;
    public ObservableCollection<string> AvailableCategories { get; set; }
    
    public MaterialEditWindowModel(List<Category> availableCategories, MainViewModel mainView)
    {
        _mainViewModel = mainView;
        AvailableCategories = new ObservableCollection<string>(availableCategories.Select(x => x.Name));
    }
    public MaterialEditWindowModel(Material material, List<Category> availableCategories, MainViewModel mainView)
    {
        _material = material;
        _mainViewModel = mainView;
        Name = material.Name;
        AvailableCategories = new ObservableCollection<string>(availableCategories.Select(x => x.Name));
    }
    public void Save(Category category)
    {
        if (string.IsNullOrEmpty(Name)) {
            return;
        }
        if (_material == null)
        {
            _mainViewModel.MaterialAccess.Save(new Material(0, Name, category));
        }
        else
        {
            _mainViewModel.MaterialAccess.Update(new Material(_material.Id, Name, category));
        }
    }
}
