using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class MaterialConsumptionAccess : IAccess<MaterialConsumption>
{
    readonly DataSet _dataSet;

    public MaterialConsumptionAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
    }

    public void Save(MaterialConsumption materialConsumption)
    {
        var newRow = _dataSet.Tables["MaterialConsumption"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = materialConsumption.Id;
            newRow["Count"] = materialConsumption.Count;
            newRow["InvoiceId"] = materialConsumption.InvoiceId;
            newRow["MaterialId"] = materialConsumption.MaterialId;
            _dataSet.Tables["MaterialConsumption"]?.Rows.Add(newRow);
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public List<MaterialConsumption> GetAll()
    {
        var materialConsumptions = new List<MaterialConsumption>();

        var dataRowCollection = _dataSet.Tables["MaterialConsumption"]?.Rows;
        if (dataRowCollection == null)
            return materialConsumptions;
        foreach (DataRow row in dataRowCollection)
        {
            materialConsumptions.Add(new MaterialConsumption
                    (
                    (long)row["Id"],
                    (long)row["Count"],
                    (long)row["InvoiceId"],
                    (long)row["MaterialId"]
                    )
                );
        }

        return materialConsumptions;
    }

    public void Update(MaterialConsumption updatedMaterialConsumption)
    {
        var rows = _dataSet.Tables["MaterialConsumption"]?.Select($"Id = {updatedMaterialConsumption.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["Count"] = updatedMaterialConsumption.Count;
            row["InvoiceId"] = updatedMaterialConsumption.InvoiceId;
            row["MaterialId"] = updatedMaterialConsumption.MaterialId;
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public void Delete(int materialConsumptionId)
    {
        var rows = _dataSet.Tables["MaterialConsumption"]?.Select($"Id = {materialConsumptionId}");
        if (rows != null)
            foreach (var row in rows)
            {
                _dataSet.Tables["MaterialConsumption"]?.Rows.Remove(row);
            }
        DataAccess.SaveDataToXml(_dataSet);
    }
}
