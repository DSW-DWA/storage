using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation.Easings;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;
using OfficeOpenXml;
using ReactiveUI;
using storage.Data;
using storage.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace storage.ViewModels;

public class MainViewModel : ReactiveObject
{
    public ObservableCollection<Category> Categories {
        get
        {
            return new ObservableCollection<Category>(CategoryAccess.GetAll());
        }
    }
    public ObservableCollection<Material> Materials 
    { 
        get
        {
            return new ObservableCollection<Material>(MaterialAccess.GetAll());
        }
    }
    public ObservableCollection<Invoice> Invoices 
    { 
        get
        {
            return new ObservableCollection<Invoice>(InvoiceAccess.GetAll());
        }
    }
    public ObservableCollection<MaterialConsumption> MaterialConsumptions
    {
        get
        {
            return new ObservableCollection<MaterialConsumption>(MaterialConsumptionAccess.GetAll());
        }
    }
    public ObservableCollection<MaterialReceipt> MaterialReceipts 
    { 
        get
        {
            return new ObservableCollection<MaterialReceipt>(MaterialReceiptAccess.GetAll());
        }
    }
    
    public DateTimeOffset ReportGenerateDateAt { get; set; }
    public TimeSpan ReportGenerateTimeAt { get; set; }
    
    public readonly DataAccess DataAccess;
    public readonly DataSet DataSet;
    public readonly CategoryAccess CategoryAccess;
    public readonly MaterialAccess MaterialAccess;
    public readonly InvoiceAccess InvoiceAccess;
    public readonly MaterialConsumptionAccess MaterialConsumptionAccess;
    public readonly MaterialReceiptAccess MaterialReceiptAccess;
    public MainViewModel()
    {
       // var lastDateTime = InvoiceAccess.GetAll().OrderBy(x => x.CreatedAt).Select(x => x.CreatedAt).FirstOrDefault();
        ReportGenerateDateAt = new DateTimeOffset(DateTime.Now);
        ReportGenerateTimeAt = DateTime.Now.TimeOfDay;

        DataAccess = new DataAccess(GetDataSet());
        DataSet = DataAccess.GetDataSet();
        CategoryAccess = new CategoryAccess(DataAccess.GetDataSet());
        MaterialAccess = new MaterialAccess(DataAccess.GetDataSet());
        InvoiceAccess = new InvoiceAccess(DataAccess.GetDataSet());
        MaterialConsumptionAccess = new MaterialConsumptionAccess(DataAccess.GetDataSet());
        MaterialReceiptAccess = new MaterialReceiptAccess(DataAccess.GetDataSet());

    }
    public DataSet GetDataSet()
    {

        var ds = new DataSet();

        var connectionString = "Data Source=storage.db;Foreign Keys=True;Mode=ReadWrite";
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            var adapter = new SQLiteDataAdapter("SELECT * FROM Material", connection);
            adapter.Fill(ds, "Material");

            adapter = new SQLiteDataAdapter("SELECT * FROM Category", connection);
            adapter.Fill(ds, "Category");

            adapter = new SQLiteDataAdapter("SELECT * FROM Invoice", connection);
            adapter.Fill(ds, "Invoice");

            adapter = new SQLiteDataAdapter("SELECT * FROM MaterialConsumption", connection);
            adapter.Fill(ds, "MaterialConsumption");

            adapter = new SQLiteDataAdapter("SELECT * FROM MaterialReceipt", connection);
            adapter.Fill(ds, "MaterialReceipt");
        }

        return ds;
    }
    public void SetCascad(bool res)
    {
        CategoryAccess.IsCascad = res;
        InvoiceAccess.IsCascad = res;
        MaterialAccess.IsCascad = res;

        if (res)
        {
            MaterialAccess.RemoveAllNull();
            MaterialConsumptionAccess.RemoveAllNull();
            MaterialReceiptAccess.RemoveAllNull();

            var connectionString = "Data Source=storage.db;Foreign Keys=False;Mode=ReadWrite";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand(connection);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Material_temp (\r\n    Id INTEGER PRIMARY KEY,\r\n    Name TEXT,\r\n    CategoryId INTEGER,\r\n    CONSTRAINT fk_material\r\n    \tFOREIGN KEY (CategoryId) \r\n    \tREFERENCES Category(Id)\r\n    \tON DELETE CASCADE\r\n);\r\n\r\n\r\n\r\n\r\n";

                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT OR IGNORE INTO Material_temp (Id, Name, CategoryId)\r\nSELECT Id, Name, CategoryId \r\nFROM Material;\r\n\r\nDROP TABLE Material;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "ALTER TABLE Material_temp RENAME TO Material;";
                cmd.ExecuteNonQuery();

                connection.Close();
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand(connection);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS MaterialConsumption_temp (\r\n    Id INTEGER PRIMARY KEY,\r\n    Count INTEGER,\r\n    InvoiceId INTEGER,\r\n    MaterialId INTEGER,\r\n    CONSTRAINT fk_MaterialConsumprion_Invoice\r\n    \tFOREIGN KEY (InvoiceId) \r\n    \tREFERENCES Invoice(Id)\r\n    \tON DELETE CASCADE\r\n\tCONSTRAINT fk_MaterialConsumprion_Material\r\n    \tFOREIGN KEY (MaterialId) \r\n    \tREFERENCES Material(Id)\r\n    \tON DELETE CASCADE\r\n);\r\n\r\n\r\nINSERT INTO MaterialConsumption_temp (Id, Count, InvoiceId, MaterialId)\r\nSELECT Id, Count, InvoiceId, MaterialId\r\nFROM MaterialConsumption;\r\n\r\nDROP TABLE MaterialConsumption;\r\n\r\nALTER TABLE MaterialConsumption_temp RENAME TO MaterialConsumption;";
                cmd.ExecuteNonQuery();

                connection.Close();
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand(connection);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS MaterialReceipt_temp (\r\n    Id INTEGER PRIMARY KEY,\r\n    Count INTEGER,\r\n    InvoiceId INTEGER,\r\n    MaterialId INTEGER,\r\n    CONSTRAINT fk_MaterialReceipt_Invoice\r\n    \tFOREIGN KEY (InvoiceId) \r\n    \tREFERENCES Invoice(Id)\r\n    \tON DELETE CASCADE\r\n\tCONSTRAINT fk_MaterialReceipt_Material\r\n    \tFOREIGN KEY (MaterialId) \r\n    \tREFERENCES Material(Id)\r\n    \tON DELETE CASCADE\r\n);\r\n\r\n\r\nINSERT INTO MaterialReceipt_temp (Id, Count, InvoiceId, MaterialId)\r\nSELECT Id, Count, InvoiceId, MaterialId\r\nFROM MaterialReceipt;\r\n\r\nDROP TABLE MaterialReceipt;\r\n\r\nALTER TABLE MaterialReceipt_temp RENAME TO MaterialReceipt;";
                cmd.ExecuteNonQuery();

                connection.Close();
            }
        } else
        {
            var connectionString = "Data Source=storage.db;Foreign Keys=True;Mode=ReadWrite";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand(connection);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Material_temp (\r\n    Id INTEGER PRIMARY KEY,\r\n    Name TEXT,\r\n    CategoryId INTEGER,\r\n    CONSTRAINT fk_material\r\n    \tFOREIGN KEY (CategoryId) \r\n    \tREFERENCES Category(Id)\r\n    \r\n);\r\n\r\n\r\nINSERT INTO Material_temp (Id, Name, CategoryId)\r\nSELECT Id, Name, CategoryId \r\nFROM Material;\r\n\r\nDROP TABLE Material;\r\n\r\nALTER TABLE Material_temp RENAME TO Material;";
                cmd.ExecuteNonQuery();

                connection.Close();
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand(connection);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS MaterialConsumption_temp (\r\n    Id INTEGER PRIMARY KEY,\r\n    Count INTEGER,\r\n    InvoiceId INTEGER,\r\n    MaterialId INTEGER,\r\n    CONSTRAINT fk_MaterialConsumprion_Invoice\r\n    \tFOREIGN KEY (InvoiceId) \r\n    \tREFERENCES Invoice(Id)\r\n    \r\n\tCONSTRAINT fk_MaterialConsumprion_Material\r\n    \tFOREIGN KEY (MaterialId) \r\n    \tREFERENCES Material(Id)\r\n    \r\n);\r\n\r\n\r\nINSERT INTO MaterialConsumption_temp (Id, Count, InvoiceId, MaterialId)\r\nSELECT Id, Count, InvoiceId, MaterialId\r\nFROM MaterialConsumption;\r\n\r\nDROP TABLE MaterialConsumption;\r\n\r\nALTER TABLE MaterialConsumption_temp RENAME TO MaterialConsumption;";
                cmd.ExecuteNonQuery();

                connection.Close();
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand(connection);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS MaterialReceipt_temp (\r\n    Id INTEGER PRIMARY KEY,\r\n    Count INTEGER,\r\n    InvoiceId INTEGER,\r\n    MaterialId INTEGER,\r\n    CONSTRAINT fk_MaterialReceipt_Invoice\r\n    \tFOREIGN KEY (InvoiceId) \r\n    \tREFERENCES Invoice(Id)\r\n    \r\n\tCONSTRAINT fk_MaterialReceipt_Material\r\n    \tFOREIGN KEY (MaterialId) \r\n    \tREFERENCES Material(Id)\r\n    \r\n);\r\n\r\n\r\nINSERT INTO MaterialReceipt_temp (Id, Count, InvoiceId, MaterialId)\r\nSELECT Id, Count, InvoiceId, MaterialId\r\nFROM MaterialReceipt;\r\n\r\nDROP TABLE MaterialReceipt;\r\n\r\nALTER TABLE MaterialReceipt_temp RENAME TO MaterialReceipt;";
                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
    public void ExportToWord(DateTimeOffset reportGenerateDateAt, TimeSpan reportGenerateTimeAt)
    {
        const string outPutPath = @"./report.docx";
        using var doc = DocX.Create(outPutPath);
        doc.InsertParagraph("Материалы").Bold().FontSize(14d).SpacingBefore(10d).SpacingAfter(10d);
        var matTable = MaterialAccess.GetAll();
        var recTable = MaterialReceiptAccess.GetAll();

        var filteredMatTable = new List<Material>();
        var wordTable = doc.AddTable(matTable.Count + 1, 5);

        wordTable.Rows[0].Cells[0].Paragraphs[0].Append("ID");
        wordTable.Rows[0].Cells[1].Paragraphs[0].Append("Имя");
        wordTable.Rows[0].Cells[2].Paragraphs[0].Append("Категория");
        wordTable.Rows[0].Cells[3].Paragraphs[0].Append("Ед. Измерения");
        wordTable.Rows[0].Cells[4].Paragraphs[0].Append("Дата поступления на склад");

        foreach (var material in matTable)
        {
            var material1 = material;
            var relevantReceipts = recTable
                .Where(x => x.Material.Id == material1.Id &&
                    x.Invoice.CreatedAt.Date >= reportGenerateDateAt.Date && // Filter by date
                    x.Invoice.CreatedAt.TimeOfDay >= reportGenerateTimeAt)
                .OrderBy(x => x.Invoice.CreatedAt)
                .ToList();

            if (!relevantReceipts.Any())
                continue;
            filteredMatTable.Add(material);
            var time = relevantReceipts.First().Invoice.CreatedAt;
            wordTable.Rows[filteredMatTable.Count].Cells[0].Paragraphs[0].Append(material.Id.ToString());
            wordTable.Rows[filteredMatTable.Count].Cells[1].Paragraphs[0].Append(material.Name);
            wordTable.Rows[filteredMatTable.Count].Cells[2].Paragraphs[0].Append(material.Category.Name);
            wordTable.Rows[filteredMatTable.Count].Cells[3].Paragraphs[0].Append(material.Category.MeasureUnit);
            wordTable.Rows[filteredMatTable.Count].Cells[4].Paragraphs[0].Append(time.ToString(CultureInfo.CurrentCulture));
        }

        doc.InsertTable(wordTable);
        doc.InsertParagraph();

        doc.Save();
    }

    public void ExportToExcel(DateTimeOffset reportGenerateDateAt, TimeSpan reportGenerateTimeAt)
    {
        const string outPutPath = @"./report.xlsx";
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Материалы");
        var matTable = MaterialAccess.GetAll();
        var recTable = MaterialReceiptAccess.GetAll();

        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Имя";
        worksheet.Cells[1, 3].Value = "Категори";
        worksheet.Cells[1, 4].Value = "Ed. Измерения";
        worksheet.Cells[1, 5].Value = "Дата поступления на склад";

        for (int rowIndex = 0; rowIndex < matTable.Count; rowIndex++)
        {
            var time = recTable
                .Where(x => x.Material.Id == matTable[rowIndex].Id &&
                    x.Invoice.CreatedAt.Date >= reportGenerateDateAt.Date && // Filter by date
                    x.Invoice.CreatedAt.TimeOfDay >= reportGenerateTimeAt)
                .OrderBy(x => x.Invoice.CreatedAt)
                .Select(x => x.Invoice.CreatedAt)
                .FirstOrDefault();
            if (time == default(DateTime))
            {
                continue;
            }
            worksheet.Cells[rowIndex + 2, 1].Value = matTable[rowIndex].Id.ToString();
            worksheet.Cells[rowIndex + 2, 2].Value = matTable[rowIndex].Name;
            worksheet.Cells[rowIndex + 2, 3].Value = matTable[rowIndex].Category.Name;
            worksheet.Cells[rowIndex + 2, 4].Value = matTable[rowIndex].Category.MeasureUnit;
            worksheet.Cells[rowIndex + 2, 5].Value = time.ToString(CultureInfo.CurrentCulture);
            worksheet.Cells[rowIndex + 2, 5].Value = time.ToString(CultureInfo.CurrentCulture);
        }

        File.WriteAllBytes(outPutPath, package.GetAsByteArray());
    }
}
