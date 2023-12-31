using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using storage.Models;
namespace storage.Data;

public class MaterialReceiptAccess : IAccess<MaterialReceipt>
{
    readonly DataSet _dataSet;
    readonly InvoiceAccess _invoiceAccess;
    readonly MaterialAccess _materialAccess;

    public MaterialReceiptAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
        _invoiceAccess = new InvoiceAccess(dataSet);
        _materialAccess = new MaterialAccess(dataSet);
    }

    public void Save(MaterialReceipt materialReceipt)
    {
        var newRow = _dataSet.Tables["MaterialReceipt"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = GetNextId();
            newRow["Count"] = materialReceipt.Count;
            newRow["InvoiceId"] = materialReceipt.Invoice.Id;
            newRow["MaterialId"] = materialReceipt.Material.Id;
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
            /*var invoice = _invoiceAccess.GetById((long)row["InvoiceId"]);
            var material = _materialAccess.GetById((long)row["MaterialId"]);*/

            var invoiceId = row["InvoiceId"];
            var materialId = row["MaterialId"];
            Invoice? invoice = null;
            Material? material = null;

            if (invoiceId != DBNull.Value)
            {
                invoice = _invoiceAccess.GetById((long)invoiceId);
            }

            if (materialId != DBNull.Value)
            {
                material = _materialAccess.GetById((long)materialId);
            }

            materialReceipts.Add(new MaterialReceipt((long)row["Id"], (long)row["Count"], invoice, material));
        }

        return materialReceipts;
    }

    public void RemoveAllNull()
    {
        var dataRowCollection = _dataSet.Tables["MaterialReceipt"]?.Select();
        if (dataRowCollection == null)
            return;

        foreach (DataRow row in dataRowCollection)
        {
            if (row["MaterialId"] == DBNull.Value || row["InvoiceId"] == DBNull.Value)
            {
                _dataSet.Tables["MaterialReceipt"]?.Rows.Remove(row);
            }
        }
    }
    public MaterialReceipt? GetById(long id)
    {
        var materialReceiptTable = _dataSet.Tables["MaterialReceipt"];

        var row = materialReceiptTable?.Rows.Cast<DataRow>()
            .FirstOrDefault(row => (long)row["Id"] == id);
        if (row == null) 
            return null;

        /*var invoice = _invoiceAccess.GetById((long)row["InvoiceId"]);
        var material = _materialAccess.GetById((long)row["MaterialId"]);*/
        var invoiceId = row["InvoiceId"];
        var materialId = row["MaterialId"];
        Invoice? invoice = null;
        Material? material = null;

        if (invoiceId != DBNull.Value)
        {
            invoice = _invoiceAccess.GetById((long)invoiceId);
        }

        if (materialId != DBNull.Value)
        {
            material = _materialAccess.GetById((long)materialId);
        }

        return new MaterialReceipt((long)row["Id"], (long)row["Count"], invoice, material);
    }

    long GetNextId()
    {
        long? maxId = _dataSet.Tables["MaterialReceipt"]?.Rows.Cast<DataRow>()
            .Max(row => (long)row["Id"]);
        return maxId == null ? 0 : (long)maxId + 1;
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
            row["InvoiceId"] = updatedMaterialReceipt.Invoice.Id;
            row["MaterialId"] = updatedMaterialReceipt.Material.Id;
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
