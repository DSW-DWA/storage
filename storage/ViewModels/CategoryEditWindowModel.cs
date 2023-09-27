using ReactiveUI;
using storage.Models;

namespace storage.ViewModels;

public class CategoryEditWindowModel
{
    public string Name { get; set; }
    public string MeasureUnit { get; set; }

    public CategoryEditWindowModel(Category category) 
    {
        Name = category.Name;
        MeasureUnit = category.MeasureUnit;
    }
}
