using System.Collections.Generic;
using ReactiveUI;
using storage.Models;
namespace storage.ViewModels;

public class MaterialEditWindowModel : ReactiveObject
{
    public long Id { get; init; }
    public string Name { get; init; }
    public Category Category { get; init; }
    public List<Category> AvailableCategories { get; set; }
    
    public MaterialEditWindowModel(Material material, List<Category> availableCategories)
    {
        Id = material.Id;
        Name = material.Name;
        Category = material.Category;
        AvailableCategories = availableCategories;
    }
}
