namespace storage.Models;

public class Material
{
    public long Id { get; init; }
    public string Name { get; init; }
    public Category? Category { get; init; }

    public Material(long id, string name, Category? category)
    {
        Id = id;
        Name = name;
        Category = category;
    }
}
