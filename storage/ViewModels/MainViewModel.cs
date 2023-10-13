using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
    
    public static readonly DataAccess DataAccess = new DataAccess();
    public readonly DataSet DataSet = DataAccess.GetDataSet();
    public readonly CategoryAccess CategoryAccess = new CategoryAccess(DataAccess.GetDataSet());
    public readonly MaterialAccess MaterialAccess = new MaterialAccess(DataAccess.GetDataSet());
    public readonly InvoiceAccess InvoiceAccess = new InvoiceAccess(DataAccess.GetDataSet());
    public readonly MaterialConsumptionAccess MaterialConsumptionAccess = new MaterialConsumptionAccess(DataAccess.GetDataSet());
    public readonly MaterialReceiptAccess MaterialReceiptAccess = new MaterialReceiptAccess(DataAccess.GetDataSet());

    public MainViewModel()
    {
       // var lastDateTime = InvoiceAccess.GetAll().OrderBy(x => x.CreatedAt).Select(x => x.CreatedAt).FirstOrDefault();
        ReportGenerateDateAt = new DateTimeOffset(DateTime.Now);
        ReportGenerateTimeAt = DateTime.Now.TimeOfDay;
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
