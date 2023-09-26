using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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

    private DataAccess _dataAccess;

    public MainViewModel()
    {
        _dataAccess = new DataAccess();

        Categories = new ObservableCollection<Category>(_dataAccess.Categories);
        Materials = new ObservableCollection<Material>(_dataAccess.Materials);
        Invoices = new ObservableCollection<Invoice>(_dataAccess.Invoices);
        MaterialConsumptions = new ObservableCollection<MaterialConsumption>(_dataAccess.MaterialConsumptions);
        MaterialReceipts = new ObservableCollection<MaterialReceipt>(_dataAccess.MaterialReceipts);

    }
    public void AddRow()
    {
        Categories.Add(new Category{
            Id = 5,
            Name = "Тест",
            MeasureUnit = "кг"
        });
    }
    public void ExportToWord()
    {
        var dataSet = _dataAccess.DS;
        var outputPath = @"./report.docx";
        using (var doc = DocX.Create(outputPath))
        {
            foreach (DataTable table in dataSet.Tables)
            {
                doc.InsertParagraph(table.TableName).Bold().FontSize(14d).SpacingBefore(10d).SpacingAfter(10d);

                var wordTable = doc.AddTable(table.Rows.Count + 1, table.Columns.Count);

                for (var colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                    wordTable.Rows[0].Cells[colIndex].Paragraphs[0].Append(table.Columns[colIndex].ColumnName);

                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                    for (var colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                        wordTable.Rows[rowIndex + 1].Cells[colIndex].Paragraphs[0]
                            .Append(table.Rows[rowIndex][colIndex].ToString());

                doc.InsertTable(wordTable);
                doc.InsertParagraph();
            }

            doc.Save();
        }
    }

    public void ExportToExcel()
    {
        var dataSet = _dataAccess.DS;
        var outputPath = @"./report.xlsx";
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            foreach (DataTable table in dataSet.Tables)
            {
                var worksheet = package.Workbook.Worksheets.Add(table.TableName);
                for (var colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                    worksheet.Cells[1, colIndex + 1].Value = table.Columns[colIndex].ColumnName;

                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                    for (var colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                        worksheet.Cells[rowIndex + 2, colIndex + 1].Value = table.Rows[rowIndex][colIndex];
            }

            File.WriteAllBytes(outputPath, package.GetAsByteArray());
        }
    }
}