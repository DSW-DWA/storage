namespace storage.Models;

public class Category
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string MeasureUnit { get; init; }

    public Category(long id, string name, string measureUnit)
    {
        Id = id;
        Name = name;
        MeasureUnit = measureUnit;
    }
}
