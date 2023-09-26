using System;
using System.Collections.Generic;
using System.Data;
using storage.Models;
namespace storage.Data;

public class InvoiceAccess : DataAccess
{
    public void CreateInvoice(Invoice invoice)
    {
        DataRow? newRow = Ds.Tables["Invoice"]?.NewRow();
        if (newRow != null)
        {
            newRow["Id"] = invoice.Id;
            newRow["CreatedAt"] = invoice.CreatedAt;
            Ds.Tables["Invoice"]?.Rows.Add(newRow);
        }
        SaveDataToXml();
    }

    public List<Invoice> GetAllInvoices()
    {
        var invoices = new List<Invoice>();

        var dataRowCollection = Ds.Tables["Invoice"]?.Rows;
        if (dataRowCollection == null)
            return invoices;
        foreach (DataRow row in dataRowCollection)
        {
            invoices.Add(new Invoice
            {
                Id = (long)row["Id"],
                CreatedAt = (DateTime)row["CreatedAt"]
            });
        }

        return invoices;
    }

    public void UpdateInvoice(Invoice updatedInvoice)
    {
        var rows = Ds.Tables["Invoice"]?.Select($"Id = {updatedInvoice.Id}");
        if (rows is { Length: <= 0 })
            return;
        var row = rows?[0];
        if (row != null)
        {
            row["CreatedAt"] = updatedInvoice.CreatedAt;
        }
        SaveDataToXml();
    }

    public void DeleteInvoice(long invoiceId)
    {
        var rows = Ds.Tables["Invoice"]?.Select($"Id = {invoiceId}");
        if (rows != null)
            foreach (var row in rows)
            {
                Ds.Tables["Invoice"]?.Rows.Remove(row);
            }
        SaveDataToXml();
    }
}
