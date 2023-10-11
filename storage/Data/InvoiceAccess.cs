using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using storage.Models;
namespace storage.Data;

public class InvoiceAccess : IAccess<Invoice>
{
    readonly DataSet _dataSet;
    public InvoiceAccess(DataSet dataSet)
    {
        _dataSet = dataSet;
    }

    public void Save(Invoice invoice)
    {
        var newRow = _dataSet.Tables["Invoice"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = GetNextId();
            newRow["CreatedAt"] = invoice.CreatedAt;
            _dataSet.Tables["Invoice"]?.Rows.Add(newRow);
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public List<Invoice> GetAll()
    {
        var invoices = new List<Invoice>();

        var dataRowCollection = _dataSet.Tables["Invoice"]?.Rows;
        if (dataRowCollection == null)
            return invoices;
        foreach (DataRow row in dataRowCollection)
        {
            invoices.Add(new Invoice((long)row["Id"], (DateTime)row["CreatedAt"]));
        }

        return invoices;
    }
    
    public Invoice? GetById(long id)
    {
        var invoiceTable = _dataSet.Tables["Invoice"];

        var row = invoiceTable?.Rows.Cast<DataRow>()
            .FirstOrDefault(row => (long)row["Id"] == id);

        return row == null ? null : new Invoice((long)row["Id"], (DateTime)row["CreatedAt"]);
    }
    long GetNextId()
    {
        long? maxId = _dataSet.Tables["Invoice"]?.Rows.Cast<DataRow>()
            .Max(row => (long)row["Id"]);
        return maxId == null ? 0 : (long)maxId + 1;
    }
    public void Update(Invoice updatedInvoice)
    {
        var rows = _dataSet.Tables["Invoice"]?.Select($"Id = {updatedInvoice.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["CreatedAt"] = updatedInvoice.CreatedAt;
        }
        DataAccess.SaveDataToXml(_dataSet);
    }

    public void Delete(int invoiceId)
    {
        var rows = _dataSet.Tables["Invoice"]?.Select($"Id = {invoiceId}");
        /*if (rows != null)
            foreach (var row in rows)
            {
                _dataSet.Tables["Invoice"]?.Rows.Remove(row);
            }*/

        var rowsInv = rows;

        var rowsMatCons = new List<DataRow>();
        var rowsMatRec = new List<DataRow>();

        if (rowsInv != null)
        {
            foreach (var rowMat in rowsInv)
            {
                var range = _dataSet.Tables["MaterialConsumption"]?.Select($"InvoiceId = {rowMat["Id"]}");
                if (range != null)
                {
                    rowsMatCons.AddRange(range);
                }
            }


            foreach (var rowMat in rowsMatRec)
            {
                var range = _dataSet.Tables["MaterialReceipt"]?.Select($"InvoiceId = {rowMat["Id"]}");
                if (range != null)
                {
                    rowsMatRec.AddRange(range);
                }
            }

            foreach (var row in rowsInv)
            {
                _dataSet.Tables["Invoice"]?.Rows.Remove(row);
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
