using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class MaterialAccess : IAccess<Material>
{
    readonly DataSet _dataSet;

    public MaterialAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
    }

    public void Save(Material material)
    {
        var newRow = _dataSet.Tables["Material"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = material.Id;
            newRow["Name"] = material.Name;
            newRow["CategoryId"] = material.CategoryId;
            _dataSet.Tables["Material"]?.Rows.Add(newRow);
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public List<Material> GetAll()
    {
        var materials = new List<Material>();

        var dataRowCollection = _dataSet.Tables["Material"]?.Rows;
        if (dataRowCollection == null)
            return materials;
        foreach (DataRow row in dataRowCollection)
        {
            materials.Add(new Material((long)row["Id"], (string)row["Name"], (long)row["CategoryId"]));
        }

        return materials;
    }

    public void Update(Material updatedMaterial)
    {
        var rows = _dataSet.Tables["Material"]?.Select($"Id = {updatedMaterial.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["Name"] = updatedMaterial.Name;
            row["CategoryId"] = updatedMaterial.CategoryId;
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public void Delete(int materialId)
    {
        var rows = _dataSet.Tables["Material"]?.Select($"Id = {materialId}");
        if (rows != null)
            foreach (var row in rows)
            {
                _dataSet.Tables["Material"]?.Rows.Remove(row);
            }
        DataAccess.SaveDataToXml(_dataSet);
    }
}
