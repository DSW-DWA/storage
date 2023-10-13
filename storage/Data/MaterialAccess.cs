using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using storage.Models;
namespace storage.Data;

public class MaterialAccess : IAccess<Material>
{
    readonly DataSet _dataSet;
    readonly CategoryAccess _categoryAccess;
    public bool IsCascad { get; set; }

    public MaterialAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
        _categoryAccess = new CategoryAccess(dataSet);
    }
    public void Save(Material material)
    {
        var newRow = _dataSet.Tables["Material"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = GetNextId();
            newRow["Name"] = material.Name;
            newRow["CategoryId"] = material.Category.Id;
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
            var categoryId = row["CategoryId"];
            Category? category = null;

            if (categoryId != DBNull.Value) {
                category = _categoryAccess.GetById((long)categoryId);
            }

            materials.Add(new Material((long)row["Id"], (string)row["Name"], category));
        }

        return materials;
    }

    public void RemoveAllNull()
    {
        var dataRowCollection = _dataSet.Tables["Material"]?.Select();
        if (dataRowCollection == null)
            return;

        foreach (DataRow row in dataRowCollection)
        {
            if (row["CategoryId"] == DBNull.Value)
            {
                _dataSet.Tables["Material"]?.Rows.Remove(row);
            }
        }
    }
    public Material? GetById(long id)
    {
        var materialTable = _dataSet.Tables["Material"];

        var row = materialTable?.Rows.Cast<DataRow>()
            .FirstOrDefault(row => (long)row["Id"] == id);

        if (row == null)
        {
            return null;
        }

        var categoryId = row["CategoryId"];
        Category? category = null;

        if (categoryId != DBNull.Value)
        {
            category = _categoryAccess.GetById((long)categoryId);
        }

        return new Material((long)row["Id"], (string)row["Name"], category);
    }

    long GetNextId()
    {
        long? maxId = _dataSet.Tables["Material"]?.Rows.Cast<DataRow>()
            .Max(row => (long)row["Id"]);
        return maxId == null ? 0 : (long)maxId + 1;
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
            row["CategoryId"] = updatedMaterial.Category.Id;
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public void Delete(int materialId)
    {
        var rows = _dataSet.Tables["Material"]?.Select($"Id = {materialId}");
        /*if (rows != null)
            foreach (var row in rows)
            {
                _dataSet.Tables["Material"]?.Rows.Remove(row);
            }*/

        var rowsMat = rows;
        if (IsCascad)
        {
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
        } else
        {
            if (rowsMat != null)
            {
                foreach (var rowMat in rowsMat)
                {
                    var range = _dataSet.Tables["MaterialConsumption"]?.Select($"MaterialId = {rowMat["Id"]}");
                    if (range != null)
                    {
                        foreach (var ra in range)
                        {
                            ra["MaterialId"] = DBNull.Value;
                        }
                    }
                }


                foreach (var rowMat in rowsMat)
                {
                    var range = _dataSet.Tables["MaterialReceipt"]?.Select($"MaterialId = {rowMat["Id"]}");
                    if (range != null)
                    {
                        foreach (var ra in range)
                        {
                            ra["MaterialId"] = DBNull.Value;
                        }
                    }
                }

                foreach (var row in rowsMat)
                {
                    _dataSet.Tables["Material"]?.Rows.Remove(row);
                }
            }
        }

        DataAccess.SaveDataToXml(_dataSet);
    }
}
