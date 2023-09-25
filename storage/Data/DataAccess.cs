using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Newtonsoft.Json;
using storage.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace storage.Data
{
    public class DataAccess
    {
        private const string DataPath = @"./Data/data.json";
        public DataSet DS { get; private set; }
        
        public DataAccess() {
            var dataSet = new DataSet();

            try
            {
                var json = File.ReadAllText(DataPath);
                var root = JsonConvert.DeserializeObject<Root>(json);

                if (root != null)
                {
                    DataTable categoryTable = new DataTable("category");
                    categoryTable.Columns.Add("id", typeof(int));
                    categoryTable.Columns.Add("name", typeof(string));
                    categoryTable.Columns.Add("measure_unit", typeof(string));
                    foreach (var item in root.Category)
                    {
                        DataRow row = categoryTable.NewRow();
                        row["id"] = item.Id;
                        row["name"] = item.Name;
                        row["measure_unit"] = item.MeasureUnit;
                        categoryTable.Rows.Add(row);
                    }

                    DataTable materialTable = new DataTable("material");
                    materialTable.Columns.Add("id", typeof(int));
                    materialTable.Columns.Add("name", typeof(string));
                    materialTable.Columns.Add("category_id", typeof(int));
                    foreach (var item in root.Material)
                    {
                        DataRow row = materialTable.NewRow();
                        row["id"] = item.Id;
                        row["name"] = item.Name;
                        row["category_id"] = item.CategoryId;
                        materialTable.Rows.Add(row);
                    }

                    DataTable invoiceTable = new DataTable("invoice");
                    invoiceTable.Columns.Add("id", typeof(int));
                    invoiceTable.Columns.Add("created_at", typeof(DateTime));
                    foreach (var item in root.Invoice)
                    {
                        DataRow row = invoiceTable.NewRow();
                        row["id"] = item.Id;
                        row["created_at"] = item.CreatedAt;
                        invoiceTable.Rows.Add(row);
                    }

                    DataTable materialConsumptionTable = new DataTable("material_consumption");
                    materialConsumptionTable.Columns.Add("id", typeof(int));
                    materialConsumptionTable.Columns.Add("count", typeof(int));
                    materialConsumptionTable.Columns.Add("invoice_id", typeof(int));
                    materialConsumptionTable.Columns.Add("material_id", typeof(int));
                    foreach (var item in root.MaterialConsumption)
                    {
                        DataRow row = materialConsumptionTable.NewRow();
                        row["id"] = item.Id;
                        row["count"] = item.Count;
                        row["invoice_id"] = item.InvoiceId;
                        row["material_id"] = item.MaterialId;
                        materialConsumptionTable.Rows.Add(row);
                    }

                    DataTable materialReceiptTable = new DataTable("material_receipt");
                    materialReceiptTable.Columns.Add("id", typeof(int));
                    materialReceiptTable.Columns.Add("count", typeof(int));
                    materialReceiptTable.Columns.Add("invoice_id", typeof(int));
                    materialReceiptTable.Columns.Add("material_id", typeof(int));
                    foreach (var item in root.MaterialReceipt)
                    {
                        DataRow row = materialReceiptTable.NewRow();
                        row["id"] = item.Id;
                        row["count"] = item.Count;
                        row["invoice_id"] = item.InvoiceId;
                        row["material_id"] = item.MaterialId;
                        materialReceiptTable.Rows.Add(row);
                    }

                    dataSet.Tables.Add(categoryTable);
                    dataSet.Tables.Add(materialTable);
                    dataSet.Tables.Add(invoiceTable);
                    dataSet.Tables.Add(materialConsumptionTable);
                    dataSet.Tables.Add(materialReceiptTable);
                }
            }
            catch (FileNotFoundException e) {
                var box = MessageBoxManager
                .GetMessageBoxStandard("Database file not found", e.Message,
                ButtonEnum.Ok);

                box.ShowAsync().Wait();
            }
            catch (Exception e)
            {
                var box = MessageBoxManager
                .GetMessageBoxStandard("Error while reading database", e.Message,
                ButtonEnum.Ok);

                box.ShowAsync().Wait();
            }

            DS = dataSet;
        }

    }
}
