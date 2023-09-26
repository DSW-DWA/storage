using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class MaterialReceiptAccess : DataAccess
{
    public void CreateMaterialReceipt(MaterialReceipt materialReceipt)
    {
        DataRow? newRow = Ds.Tables["MaterialReceipt"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = materialReceipt.Id;
            newRow["Count"] = materialReceipt.Count;
            newRow["InvoiceId"] = materialReceipt.InvoiceId;
            newRow["MaterialId"] = materialReceipt.MaterialId;
            Ds.Tables["MaterialReceipt"]?.Rows.Add(newRow);
        }
        SaveDataToXml();
    }

    public List<MaterialReceipt> GetAllMaterialReceipts()
    {
        var materialReceipts = new List<MaterialReceipt>();

        var dataRowCollection = Ds.Tables["MaterialReceipt"]?.Rows;
        if (dataRowCollection == null)
            return materialReceipts;
        foreach (DataRow row in dataRowCollection)
        {
            materialReceipts.Add(new MaterialReceipt
            {
                Id = (long)row["Id"],
                Count = (long)row["Count"],
                InvoiceId = (long)row["InvoiceId"],
                MaterialId = (long)row["MaterialId"]
            });
        }

        return materialReceipts;
    }

    public void DeleteMaterialReceipt(long materialReceiptId)
    {
        var rows = Ds.Tables["MaterialReceipt"]?.Select($"Id = {materialReceiptId}");
        if (rows != null)
            foreach (var row in rows)
            {
                Ds.Tables["MaterialReceipt"]?.Rows.Remove(row);
            }
        SaveDataToXml();
    }

    public void SaveMaterialReceipt(MaterialReceipt materialReceipt)
    {
    }
}
