using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class MaterialEditWindowModel : ReactiveObject
{
    public string Name { get; init; }
    public ObservableCollection<string> AvailableCategories { get; set; }
    private Material _material;
    private MainViewModel _mainViewModel;
    
    public MaterialEditWindowModel(Material material, List<Category> availableCategories, MainViewModel mainView)
    {
        _material = material;
        _mainViewModel = mainView;
        Name = material.Name;
        AvailableCategories = new ObservableCollection<string>(availableCategories.Select(x => x.Name));
    }
    public void Save(Category category)
    {
        _mainViewModel.MaterialAccess.Update(new Material(_material.Id, Name, category));
    }
}
