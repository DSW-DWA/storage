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
            newRow["Id"] = invoice.Id;
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
        if (rows != null)
            foreach (var row in rows)
            {
                _dataSet.Tables["Invoice"]?.Rows.Remove(row);
            }
        DataAccess.SaveDataToXml(_dataSet);
    }
}
