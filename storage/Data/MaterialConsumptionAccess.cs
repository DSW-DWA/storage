using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using storage.Models;
namespace storage.Data;

public class MaterialConsumptionAccess : IAccess<MaterialConsumption>
{
    readonly DataSet _dataSet;
    readonly InvoiceAccess _invoiceAccess;
    readonly MaterialAccess _materialAccess;

    public MaterialConsumptionAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
        _invoiceAccess = new InvoiceAccess(dataSet);
        _materialAccess = new MaterialAccess(dataSet);
    }

    public void Save(MaterialConsumption materialConsumption)
    {
        var newRow = _dataSet.Tables["MaterialConsumption"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = GetNextId();
            newRow["Count"] = materialConsumption.Count;
            newRow["InvoiceId"] = materialConsumption.Invoice.Id;
            newRow["MaterialId"] = materialConsumption.Material.Id;
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
            //var invoice = _invoiceAccess.GetById((long)row["InvoiceId"]);
            //var material = _materialAccess.GetById((long)row["MaterialId"]);

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

            materialConsumptions.Add(new MaterialConsumption((long)row["Id"], (long)row["Count"], invoice, material));
        }

        return materialConsumptions;
    }

    public void RemoveAllNull()
    {
        var dataRowCollection = _dataSet.Tables["MaterialConsumption"]?.Select();
        if (dataRowCollection == null)
            return;

        foreach (DataRow row in dataRowCollection)
        {
            if (row["MaterialId"] == DBNull.Value || row["InvoiceId"] == DBNull.Value)
            {
                _dataSet.Tables["MaterialConsumption"]?.Rows.Remove(row);
            }
        }
    }

    public MaterialConsumption? GetById(long id)
    {
        var materialConsumptionTable = _dataSet.Tables["MaterialConsumption"];

        var row = materialConsumptionTable?.Rows.Cast<DataRow>()
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

        return new MaterialConsumption((long)row["Id"], (long)row["Count"], invoice, material);
    }
    long GetNextId()
    {
        long? maxId = _dataSet.Tables["MaterialConsumption"]?.Rows.Cast<DataRow>()
            .Max(row => (long)row["Id"]);
        return maxId == null ? 0 : (long)maxId + 1;
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
            row["InvoiceId"] = updatedMaterialConsumption.Invoice.Id;
            row["MaterialId"] = updatedMaterialConsumption.Material.Id;
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
