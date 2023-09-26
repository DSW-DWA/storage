namespace storage.Models;

public class Material
{
    public long Id { get; init; }
    public string Name { get; init; }
    public long CategoryId { get; init; }

    public Material(long id, string name, long categoryId)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
    }
}
