using ReactiveUI;
namespace storage.ViewModels;

public class CategoryEditViewModel
{
    public Interaction<MainViewModel, CategoryEditViewModel?> ShowDialog { get; }
}
