using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class MaterialReceiptAccess : IAccess<MaterialReceipt>
{
    readonly DataSet _dataSet;

    public MaterialReceiptAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
    }

    public void Save(MaterialReceipt materialReceipt)
    {
        var newRow = _dataSet.Tables["MaterialReceipt"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = materialReceipt.Id;
            newRow["Count"] = materialReceipt.Count;
            newRow["InvoiceId"] = materialReceipt.InvoiceId;
            newRow["MaterialId"] = materialReceipt.MaterialId;
            _dataSet.Tables["MaterialReceipt"]?.Rows.Add(newRow);
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public List<MaterialReceipt> GetAll()
    {
        var materialReceipts = new List<MaterialReceipt>();

        var dataRowCollection = _dataSet.Tables["MaterialReceipt"]?.Rows;
        if (dataRowCollection == null)
            return materialReceipts;
        foreach (DataRow row in dataRowCollection)
        {
            materialReceipts.Add(new MaterialReceipt(
                    (long)row["Id"],
                    (long)row["Count"],
                    (long)row["InvoiceId"],
                    (long)row["MaterialId"]
                    )
                );
        }

        return materialReceipts;
    }

    public void Update(MaterialReceipt updatedMaterialReceipt)
    {
        var rows = _dataSet.Tables["MaterialReceipt"]?.Select($"Id = {updatedMaterialReceipt.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["Count"] = updatedMaterialReceipt.Count;
            row["InvoiceId"] = updatedMaterialReceipt.InvoiceId;
            row["MaterialId"] = updatedMaterialReceipt.MaterialId;
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public void Delete(int materialReceiptId)
    {
        var rows = _dataSet.Tables["MaterialReceipt"]?.Select($"Id = {materialReceiptId}");
        if (rows != null)
            foreach (var row in rows)
            {
                _dataSet.Tables["MaterialReceipt"]?.Rows.Remove(row);
            }
        DataAccess.SaveDataToXml(_dataSet);
    }
}
