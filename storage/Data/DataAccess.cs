using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using storage.Models;
namespace storage.Data
{
    public class DataAccess
    {
        private const string DataPath = @"./Data/data.json";
        public DataSet DS { get; private set; }

        public List<Category> Categories { get; private set; }
        public List<Invoice> Invoices { get; private set; }
        public List<Material> Materials { get; private set; }
        public List<MaterialConsumption> MaterialConsumptions { get; private set; }
        public List<MaterialReceipt> MaterialReceipts { get; private set; }
        
        public DataAccess() {
            var dataSet = new DataSet();

            /*dataSet.ReadXmlSchema(@"./Data/schema.xml");
            dataSet.ReadXml(@"./Data/data.xml");*/

            Categories = new List<Category>();
            Materials = new List<Material>();
            Invoices = new List<Invoice>();
            MaterialConsumptions = new List<MaterialConsumption>();
            MaterialReceipts = new List<MaterialReceipt>();

            try
            {
                var json = File.ReadAllText(DataPath);
                var root = JsonConvert.DeserializeObject<Root>(json);

                if (root != null)
                {
                    DataTable categoryTable = new DataTable("Category");
                    categoryTable.Columns.Add("Id", typeof(int));
                    categoryTable.Columns.Add("Name", typeof(string));
                    categoryTable.Columns.Add("MeasureUnit", typeof(string));
                    foreach (var item in root.Category)
                    {
                        DataRow row = categoryTable.NewRow();
                        row["Id"] = item.Id;
                        row["Name"] = item.Name;
                        row["MeasureUnit"] = item.MeasureUnit;
                        categoryTable.Rows.Add(row);

                        Categories.Add(new Category
                        {
                            Id = item.Id,
                            Name = item.Name,
                            MeasureUnit = item.MeasureUnit,
                        });
                    }

                    DataTable materialTable = new DataTable("Material");
                    materialTable.Columns.Add("Id", typeof(int));
                    materialTable.Columns.Add("Name", typeof(string));
                    materialTable.Columns.Add("CategoryId", typeof(int));
                    foreach (var item in root.Material)
                    {
                        DataRow row = materialTable.NewRow();
                        row["Id"] = item.Id;
                        row["Name"] = item.Name;
                        row["CategoryId"] = item.CategoryId;
                        materialTable.Rows.Add(row);

                        Materials.Add(new Material
                        {
                            Id = item.Id,
                            Name = item.Name,
                            CategoryId = item.CategoryId,
                        });
                    }

                    DataTable invoiceTable = new DataTable("Invoice");
                    invoiceTable.Columns.Add("Id", typeof(int));
                    invoiceTable.Columns.Add("CreatedAt", typeof(DateTime));
                    foreach (var item in root.Invoice)
                    {
                        DataRow row = invoiceTable.NewRow();
                        row["Id"] = item.Id;
                        row["CreatedAt"] = item.CreatedAt;
                        invoiceTable.Rows.Add(row);

                        Invoices.Add(new Invoice
                        {
                            Id = item.Id,
                            CreatedAt = item.CreatedAt
                        });
                    }

                    DataTable materialConsumptionTable = new DataTable("MaterialConsumption");
                    materialConsumptionTable.Columns.Add("Id", typeof(int));
                    materialConsumptionTable.Columns.Add("count", typeof(int));
                    materialConsumptionTable.Columns.Add("InvoiceId", typeof(int));
                    materialConsumptionTable.Columns.Add("MaterialId", typeof(int));
                    foreach (var item in root.MaterialConsumption)
                    {
                        DataRow row = materialConsumptionTable.NewRow();
                        row["Id"] = item.Id;
                        row["count"] = item.Count;
                        row["InvoiceId"] = item.InvoiceId;
                        row["MaterialId"] = item.MaterialId;
                        materialConsumptionTable.Rows.Add(row);

                        MaterialConsumptions.Add(new MaterialConsumption
                        {
                            Id = item.Id,
                            Count = item.Count,
                            InvoiceId = item.InvoiceId,
                            MaterialId = item.MaterialId,
                        });
                    }

                    DataTable materialReceiptTable = new DataTable("MaterialReceipt");
                    materialReceiptTable.Columns.Add("Id", typeof(int));
                    materialReceiptTable.Columns.Add("count", typeof(int));
                    materialReceiptTable.Columns.Add("InvoiceId", typeof(int));
                    materialReceiptTable.Columns.Add("MaterialId", typeof(int));
                    foreach (var item in root.MaterialReceipt)
                    {
                        DataRow row = materialReceiptTable.NewRow();
                        row["Id"] = item.Id;
                        row["count"] = item.Count;
                        row["InvoiceId"] = item.InvoiceId;
                        row["MaterialId"] = item.MaterialId;
                        materialReceiptTable.Rows.Add(row);

                        MaterialReceipts.Add(new MaterialReceipt
                        {
                            Id = item.Id,
                            Count = item.Count,
                            InvoiceId = item.InvoiceId,
                            MaterialId = item.MaterialId,
                        });
                    }

                    dataSet.Tables.Add(categoryTable);
                    dataSet.Tables.Add(materialTable);
                    dataSet.Tables.Add(invoiceTable);
                    dataSet.Tables.Add(materialConsumptionTable);
                    dataSet.Tables.Add(materialReceiptTable);

                    dataSet.AcceptChanges();
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
