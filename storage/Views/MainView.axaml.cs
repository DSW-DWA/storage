using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeOpenXml;
using storage.Data;
using storage.ViewModels;
using Xceed.Words.NET;

namespace storage.Views;

public partial class MainView : UserControl
{
    public ObservableCollection<Category> Categories { get; }

    public MainView()
    {
        InitializeComponent();

        // Initialize the MaterialInventory
        var materialInventory = new MaterialInventory();
        var dataSet = materialInventory.MaterialInventoryDataset;

        var categories = new List<Category>();
        
        foreach (DataRowView rowView in dataSet.Tables["category"].DefaultView)
        {
            DataRow row = rowView.Row;
            categories.Add(new Category
            {
                id = (int)row["id"],
                name = (string)row["name"],
                measure_unit = (string)row["measure_unit"]
            });
        }

        Categories = new ObservableCollection<Category>(categories);
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        //ExportToWord(_model.Ds, @"./output.docx");
        //ExportToExcel(_model.Ds, @"./output.xlsx");
    }

    public void ExportToWord(DataSet dataSet, string outputPath)
    {
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

    public void ExportToExcel(DataSet dataSet, string outputPath)
    {
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