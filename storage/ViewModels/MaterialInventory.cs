using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using ReactiveUI;
using storage.Data;

namespace storage.ViewModels;

public class MaterialInventory : ReactiveObject
{
    private const string DataPath = @"./Data/data.json";

    public DataSet? MaterialInventoryDataset { get; private set; } = LoadMaterialInventoryDataSet();

    private static DataSet? LoadMaterialInventoryDataSet()
    {
        var dataSet = new DataSet();

        var json = File.ReadAllText(DataPath);
        var root = JsonConvert.DeserializeObject<Root>(json);

        if (root == null) return dataSet;

        DataTable categoryTable = new DataTable("category");
        categoryTable.Columns.Add("id", typeof(int));
        categoryTable.Columns.Add("name", typeof(string));
        categoryTable.Columns.Add("measure_unit", typeof(string));
        foreach (var item in root.category)
        {
            DataRow row = categoryTable.NewRow();
            row["id"] = item.id;
            row["name"] = item.name;
            row["measure_unit"] = item.measure_unit;
            categoryTable.Rows.Add(row);
        }

        DataTable materialTable = new DataTable("material");
        materialTable.Columns.Add("id", typeof(int));
        materialTable.Columns.Add("name", typeof(string));
        materialTable.Columns.Add("category_id", typeof(int));
        foreach (var item in root.material)
        {
            DataRow row = materialTable.NewRow();
            row["id"] = item.id;
            row["name"] = item.name;
            row["category_id"] = item.category_id;
            materialTable.Rows.Add(row);
        }

        DataTable invoiceTable = new DataTable("invoice");
        invoiceTable.Columns.Add("id", typeof(int));
        invoiceTable.Columns.Add("created_at", typeof(DateTime));
        foreach (var item in root.invoice)
        {
            DataRow row = invoiceTable.NewRow();
            row["id"] = item.id;
            row["created_at"] = item.created_at;
            invoiceTable.Rows.Add(row);
        }

        DataTable materialConsumptionTable = new DataTable("material_consumption");
        materialConsumptionTable.Columns.Add("id", typeof(int));
        materialConsumptionTable.Columns.Add("count", typeof(int));
        materialConsumptionTable.Columns.Add("invoice_id", typeof(int));
        materialConsumptionTable.Columns.Add("material_id", typeof(int));
        foreach (var item in root.material_consumption)
        {
            DataRow row = materialConsumptionTable.NewRow();
            row["id"] = item.id;
            row["count"] = item.count;
            row["invoice_id"] = item.invoice_id;
            row["material_id"] = item.material_id;
            materialConsumptionTable.Rows.Add(row);
        }

        DataTable materialReceiptTable = new DataTable("material_receipt");
        materialReceiptTable.Columns.Add("id", typeof(int));
        materialReceiptTable.Columns.Add("count", typeof(int));
        materialReceiptTable.Columns.Add("invoice_id", typeof(int));
        materialReceiptTable.Columns.Add("material_id", typeof(int));
        foreach (var item in root.material_receipt)
        {
            DataRow row = materialReceiptTable.NewRow();
            row["id"] = item.id;
            row["count"] = item.count;
            row["invoice_id"] = item.invoice_id;
            row["material_id"] = item.material_id;
            materialReceiptTable.Rows.Add(row);
        }

        dataSet.Tables.Add(categoryTable);
        dataSet.Tables.Add(materialTable);
        dataSet.Tables.Add(invoiceTable);
        dataSet.Tables.Add(materialConsumptionTable);
        dataSet.Tables.Add(materialReceiptTable);
        
        return dataSet;
    }
}