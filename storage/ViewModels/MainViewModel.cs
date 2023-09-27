using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Animation.Easings;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;
using OfficeOpenXml;
using ReactiveUI;
using storage.Data;
using storage.Models;
using Xceed.Words.NET;

namespace storage.ViewModels;

public class MainViewModel : ReactiveObject
{
    public ObservableCollection<Category> Categories { get; }
    public ObservableCollection<Material> Materials { get; }
    public ObservableCollection<Invoice> Invoices { get; }
    public ObservableCollection<MaterialConsumption> MaterialConsumptions { get; }
    public ObservableCollection<MaterialReceipt> MaterialReceipts { get; }

    static readonly DataAccess DataAccess = new DataAccess();
    public readonly CategoryAccess CategoryAccess = new CategoryAccess(DataAccess.GetDataSet());
    public readonly MaterialAccess MaterialAccess = new MaterialAccess(DataAccess.GetDataSet());
    public readonly InvoiceAccess InvoiceAccess = new InvoiceAccess(DataAccess.GetDataSet());
    public readonly MaterialConsumptionAccess MaterialConsumptionAccess = new MaterialConsumptionAccess(DataAccess.GetDataSet());
    public readonly MaterialReceiptAccess MaterialReceiptAccess = new MaterialReceiptAccess(DataAccess.GetDataSet());

    public MainViewModel()
    {
        Categories = new ObservableCollection<Category>(CategoryAccess.GetAll());
        Materials = new ObservableCollection<Material>(MaterialAccess.GetAll());
        Invoices = new ObservableCollection<Invoice>(InvoiceAccess.GetAll());
        MaterialConsumptions = new ObservableCollection<MaterialConsumption>(MaterialConsumptionAccess.GetAll());
        MaterialReceipts = new ObservableCollection<MaterialReceipt>(MaterialReceiptAccess.GetAll());
    }

    public void ExportToWord()
    {
        Thread.Sleep(5000);
        var dataset = DataAccess.GetDataSet();
        var outputpath = @"./report.docx";
        using (var doc = DocX.Create(outputpath))
        {
            doc.InsertParagraph("Материалы").Bold().FontSize(14d).SpacingBefore(10d).SpacingAfter(10d);
            var MatTable = MaterialAccess.GetAll();
            var CatTable = CategoryAccess.GetAll();
            var RecTable = MaterialReceiptAccess.GetAll();
            var InvTable = InvoiceAccess.GetAll();

            var wordTable = doc.AddTable(MatTable.Count+1, 5);

            wordTable.Rows[0].Cells[0].Paragraphs[0].Append("ID");
            wordTable.Rows[0].Cells[1].Paragraphs[0].Append("Имя");
            wordTable.Rows[0].Cells[2].Paragraphs[0].Append("Категория");
            wordTable.Rows[0].Cells[3].Paragraphs[0].Append("Ед. Измерения");
            wordTable.Rows[0].Cells[4].Paragraphs[0].Append("Дата поступления на склад");
            
            for(var rowIndex = 0; rowIndex < MatTable.Count; rowIndex++)
            {
                var recId = RecTable.Where(x => x.MaterialId == MatTable[rowIndex].Id).Select(x => x.Id).ToList();

                wordTable.Rows[rowIndex + 1].Cells[0].Paragraphs[0].Append(MatTable[rowIndex].Id.ToString());
                wordTable.Rows[rowIndex + 1].Cells[1].Paragraphs[0].Append(MatTable[rowIndex].Name.ToString());
                wordTable.Rows[rowIndex + 1].Cells[2].Paragraphs[0].Append(CatTable.Where(x => x.Id == MatTable[rowIndex].Id).FirstOrDefault()?.Name);
                wordTable.Rows[rowIndex + 1].Cells[3].Paragraphs[0].Append(CatTable.Where(x => x.Id == MatTable[rowIndex].Id).FirstOrDefault()?.MeasureUnit);
                wordTable.Rows[rowIndex + 1].Cells[4].Paragraphs[0].Append(InvTable.Where(x => recId.Contains(x.Id)).OrderBy(x => x.CreatedAt).FirstOrDefault()?.ToString());
            }

            doc.InsertTable(wordTable);
            doc.InsertParagraph();

            doc.Save();
        }
    }

    public void ExportToExcel()
    {
        Thread.Sleep(5000);
        /*var dataset = _dataaccess.ds;
        var outputpath = @"./report.xlsx";
        excelpackage.licensecontext = licensecontext.noncommercial;
        using (var package = new excelpackage())
        {
            foreach (datatable table in dataset.tables)
            {
                var worksheet = package.workbook.worksheets.add(table.tablename);
                for (var colindex = 0; colindex < table.columns.count; colindex++)
                    worksheet.cells[1, colindex + 1].value = table.columns[colindex].columnname;

                for (var rowindex = 0; rowindex < table.rows.count; rowindex++)
                    for (var colindex = 0; colindex < table.columns.count; colindex++)
                        worksheet.cells[rowindex + 2, colindex + 1].value = table.rows[rowindex][colindex];
            }

            file.writeallbytes(outputpath, package.getasbytearray());
        }*/
    }
}
