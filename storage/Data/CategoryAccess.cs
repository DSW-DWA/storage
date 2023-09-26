using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class CategoryAccess : DataAccess
{
    public void CreateCategory(Category category)
    {
        DataRow? newRow = Ds.Tables["Category"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = category.Id;
            newRow["Name"] = category.Name;
            newRow["MeasureUnit"] = category.MeasureUnit;
            Ds.Tables["Category"]?.Rows.Add(newRow);
        }
        SaveDataToXml();
    }

    public List<Category> GetAllCategories()
    {
        var categories = new List<Category>();

        var dataRowCollection = Ds.Tables["Category"]?.Rows;
        if (dataRowCollection == null)
            return categories;
        foreach (DataRow row in dataRowCollection)
        {
            categories.Add(new Category
            {
                Id = (long)row["Id"],
                Name = (string)row["Name"],
                MeasureUnit = (string)row["MeasureUnit"]
            });
        }

        return categories;
    }
    
    public void UpdateCategory(Category updatedCategory)
    {
        var rows = Ds.Tables["Category"]?.Select($"Id = {updatedCategory.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["Name"] = updatedCategory.Name;
            row["MeasureUnit"] = updatedCategory.MeasureUnit;
        }
        SaveDataToXml();
    }
    
    public void DeleteCategory(long categoryId)
    {
        var rows = Ds.Tables["Category"]?.Select($"Id = {categoryId}");
        if (rows != null)
            foreach (var row in rows)
            {
                Ds.Tables["Category"]?.Rows.Remove(row);
            }
        SaveDataToXml();
    }
}
