using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using storage.Models;
using Xceed.Document.NET;

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

            Categories = new List<Category>();
            Materials = new List<Material>();
            Invoices = new List<Invoice>();
            MaterialConsumptions = new List<MaterialConsumption>();
            MaterialReceipts = new List<MaterialReceipt>();

            try
            {
                dataSet.ReadXmlSchema(@"./Data/schema.xml");
                dataSet.ReadXml(@"./Data/data.xml");

                foreach (DataRow row in dataSet.Tables["Category"].Rows)
                { 
                    Categories.Add(new Category
                    {
                        Id = (long)row["Id"],
                        Name = (string)row["Name"],
                        MeasureUnit = (string)row["MeasureUnit"],
                    });
                }

                foreach (DataRow row in dataSet.Tables["Material"].Rows)
                {

                    Materials.Add(new Material
                    {
                        Id = (long)row["Id"],
                        Name = (string)row["Name"],
                        CategoryId = (long)row["CategoryId"],
                    });
                }

                foreach (DataRow row in dataSet.Tables["Invoice"].Rows)
                {
                    Invoices.Add(new Invoice
                    {
                        Id = (long)row["Id"],
                        CreatedAt = (DateTime)row["CreatedAt"]
                    });
                }

                foreach (DataRow row in dataSet.Tables["MaterialConsumption"].Rows)
                {

                    MaterialConsumptions.Add(new MaterialConsumption
                    {
                        Id = (long)row["Id"],
                        Count = (long)row["count"],
                        InvoiceId = (long)row["InvoiceId"],
                        MaterialId = (long)row["MaterialId"],
                    });
                }

                foreach (DataRow row in dataSet.Tables["MaterialReceipt"].Rows)
                {

                    MaterialReceipts.Add(new MaterialReceipt
                    {
                        Id = (long)row["Id"],
                        Count = (long)row["count"],
                        InvoiceId = (long)row["InvoiceId"],
                        MaterialId = (long)row["MaterialId"],
                    });
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
