using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using DynamicData;
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
            newRow["Id"] = GetNextId();
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

    public Category? GetById(long id)
    {
        var categoryTable = _dataSet.Tables["Category"];

        var categoryRow = categoryTable?.Rows.Cast<DataRow>()
            .FirstOrDefault(row => (long)row["Id"] == id);

        return categoryRow == null ? null : new Category((long)categoryRow["Id"], (string)categoryRow["Name"], (string)categoryRow["MeasureUnit"]);
    }
    long GetNextId()
    {
        long? maxId = _dataSet.Tables["Category"]?.Rows.Cast<DataRow>()
            .Max(row => (long)row["Id"]);
        return maxId == null ? 0 : (long)maxId + 1;
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

        var rowsMat = _dataSet.Tables["Material"]?.Select($"CategoryId = {categoryId}");

        var rowsMatCons = new List<DataRow>();
        var rowsMatRec = new List<DataRow>();

        if (rowsMat != null)
        {
            foreach (var rowMat in rowsMat)
            {
                var range = _dataSet.Tables["MaterialConsumption"]?.Select($"MaterialId = {rowMat["Id"]}");
                if (range != null)
                {
                    rowsMatCons.AddRange(range);
                }
            }

            
            foreach (var rowMat in rowsMat)
            {
                var range = _dataSet.Tables["MaterialReceipt"]?.Select($"MaterialId = {rowMat["Id"]}");
                if (range != null)
                {
                    rowsMatRec.AddRange(range);
                }
            }

            foreach (var row in rowsMat)
            {
                _dataSet.Tables["Material"]?.Rows.Remove(row);
            }
        }
        
        foreach (var rowMatCon in rowsMatCons)
        {
            _dataSet.Tables["MaterialConsumption"]?.Rows.Remove(rowMatCon);
        }

        foreach (var rowMatRec in rowsMatRec)
        {
            _dataSet.Tables["MateriallReceipt"]?.Rows.Remove(rowMatRec);
        }
        
        DataAccess.SaveDataToXml(_dataSet);
    }
}
