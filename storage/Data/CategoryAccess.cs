using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class CategoryAccess : IAccess<Category>
{
    readonly DataSet _dataSet;
    public CategoryAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
    }

    public void Save(Category element)
    {
        var newRow = _dataSet.Tables["Category"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = element.Id;
            newRow["Name"] = element.Name;
            newRow["MeasureUnit"] = element.MeasureUnit;
            _dataSet.Tables["Category"]?.Rows.Add(newRow);
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public List<Category> GetAll()
    {
        var categories = new List<Category>();

        var dataRowCollection = _dataSet.Tables["Category"]?.Rows;
        if (dataRowCollection == null)
            return categories;
        foreach (DataRow row in dataRowCollection)
        {
            categories.Add(new Category((long)row["Id"], (string)row["Name"], (string)row["MeasureUnit"]));
        }

        return categories;
    }

    public void Update(Category updatedCategory)
    {
        var rows = _dataSet.Tables["Category"]?.Select($"Id = {updatedCategory.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["Name"] = updatedCategory.Name;
            row["MeasureUnit"] = updatedCategory.MeasureUnit;
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public void Delete(int categoryId)
    {
        var rows = _dataSet.Tables["Category"]?.Select($"Id = {categoryId}");
        if (rows != null)
            foreach (var row in rows)
            {
                _dataSet.Tables["Category"]?.Rows.Remove(row);
            }
        DataAccess.SaveDataToXml(_dataSet);
    }
}
