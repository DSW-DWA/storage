using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class MaterialAccess : DataAccess
{
    public void CreateMaterial(Material material)
    {
        DataRow? newRow = Ds.Tables["Material"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = material.Id;
            newRow["Name"] = material.Name;
            newRow["CategoryId"] = material.CategoryId;
            Ds.Tables["Material"]?.Rows.Add(newRow);
        }
        SaveDataToXml();
    }

    public List<Material> GetAllMaterials()
    {
        var materials = new List<Material>();

        var dataRowCollection = Ds.Tables["Material"]?.Rows;
        if (dataRowCollection == null)
            return materials;
        foreach (DataRow row in dataRowCollection)
        {
            materials.Add(new Material
            {
                Id = (long)row["Id"],
                Name = (string)row["Name"],
                CategoryId = (long)row["CategoryId"]
            });
        }

        return materials;
    }

    public void UpdateMaterial(Material updatedMaterial)
    {
        var rows = Ds.Tables["Material"]?.Select($"Id = {updatedMaterial.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["Name"] = updatedMaterial.Name;
            row["CategoryId"] = updatedMaterial.CategoryId;
        }
        SaveDataToXml();
    }

    public void DeleteMaterial(long materialId)
    {
        var rows = Ds.Tables["Material"]?.Select($"Id = {materialId}");
        if (rows != null)
            foreach (var row in rows)
            {
                Ds.Tables["Material"]?.Rows.Remove(row);
            }
        SaveDataToXml();
    }
}
