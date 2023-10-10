using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

    static readonly DataAccess DataAccess = new DataAccess();
    public readonly CategoryAccess CategoryAccess = new CategoryAccess(DataAccess.GetDataSet());
    public readonly MaterialAccess MaterialAccess = new MaterialAccess(DataAccess.GetDataSet());
    public readonly InvoiceAccess InvoiceAccess = new InvoiceAccess(DataAccess.GetDataSet());
    public readonly MaterialConsumptionAccess MaterialConsumptionAccess = new MaterialConsumptionAccess(DataAccess.GetDataSet());
    public readonly MaterialReceiptAccess MaterialReceiptAccess = new MaterialReceiptAccess(DataAccess.GetDataSet());

    public MainViewModel()
    {
        /*Categories = new ObservableCollection<Category>(CategoryAccess.GetAll());
        Materials = new ObservableCollection<Material>(MaterialAccess.GetAll());
        Invoices = new ObservableCollection<Invoice>(InvoiceAccess.GetAll());
        MaterialConsumptions = new ObservableCollection<MaterialConsumption>(MaterialConsumptionAccess.GetAll());
        MaterialReceipts = new ObservableCollection<MaterialReceipt>(MaterialReceiptAccess.GetAll());*/
    }
    public void ExportToWord()
    {
        var dataset = DataAccess.GetDataSet();
        var outputpath = @"./report.docx";
        using (var doc = DocX.Create(outputpath))
        {
            doc.InsertParagraph("Материалы").Bold().FontSize(14d).SpacingBefore(10d).SpacingAfter(10d);
            var MatTable = MaterialAccess.GetAll();
            var RecTable = MaterialReceiptAccess.GetAll();

            var wordTable = doc.AddTable(MatTable.Count+1, 5);

            wordTable.Rows[0].Cells[0].Paragraphs[0].Append("ID");
            wordTable.Rows[0].Cells[1].Paragraphs[0].Append("Имя");
            wordTable.Rows[0].Cells[2].Paragraphs[0].Append("Категория");
            wordTable.Rows[0].Cells[3].Paragraphs[0].Append("Ед. Измерения");
            wordTable.Rows[0].Cells[4].Paragraphs[0].Append("Дата поступления на склад");
            
            for(var rowIndex = 0; rowIndex < MatTable.Count; rowIndex++)
            {
                var time = RecTable.Where(x => x.Material.Id == MatTable[rowIndex].Id).OrderBy(x => x.Invoice.CreatedAt).Select(x => x.Invoice.CreatedAt).FirstOrDefault();
                
                wordTable.Rows[rowIndex + 1].Cells[0].Paragraphs[0].Append(MatTable[rowIndex].Id.ToString());
                wordTable.Rows[rowIndex + 1].Cells[1].Paragraphs[0].Append(MatTable[rowIndex].Name);
                wordTable.Rows[rowIndex + 1].Cells[2].Paragraphs[0].Append(MatTable[rowIndex].Category.Name);
                wordTable.Rows[rowIndex + 1].Cells[3].Paragraphs[0].Append(MatTable[rowIndex].Category.MeasureUnit);
                wordTable.Rows[rowIndex + 1].Cells[4].Paragraphs[0].Append(time.ToString());
            }

            doc.InsertTable(wordTable);
            doc.InsertParagraph();

            doc.Save();
        }
    }

    public void ExportToExcel()
    {
        var dataSet = DataAccess.GetDataSet();
        var outputPath = @"./report.xlsx";
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Материалы");
            var MatTable = MaterialAccess.GetAll();
            var RecTable = MaterialReceiptAccess.GetAll();

            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Имя";
            worksheet.Cells[1, 3].Value = "Категори";
            worksheet.Cells[1, 4].Value = "Ed. Измерения";
            worksheet.Cells[1, 5].Value = "Дата поступления на склад";

            for (var rowIndex = 0; rowIndex < MatTable.Count; rowIndex++)
            {
                var time = RecTable.Where(x => x.Material.Id == MatTable[rowIndex].Id).OrderBy(x => x.Invoice.CreatedAt).Select(x => x.Invoice.CreatedAt).FirstOrDefault();

                worksheet.Cells[rowIndex + 2, 1].Value = MatTable[rowIndex].Id.ToString();
                worksheet.Cells[rowIndex + 2, 2].Value = MatTable[rowIndex].Name;
                worksheet.Cells[rowIndex + 2, 3].Value = MatTable[rowIndex].Category.Name;
                worksheet.Cells[rowIndex + 2, 4].Value = MatTable[rowIndex].Category.MeasureUnit;
                worksheet.Cells[rowIndex + 2, 5].Value = time.ToString();
            }

            File.WriteAllBytes(outputPath, package.GetAsByteArray());
        }
    }
}
