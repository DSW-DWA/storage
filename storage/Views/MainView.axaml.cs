using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeOpenXml;
using storage.ViewModels;
using System.Data;
using System.IO;
using Xceed.Words.NET;
using OfficeOpenXml;

namespace storage.Views;

public partial class MainView : UserControl
{
    private MainViewModel _model;
    public MainView()
    {
        InitializeComponent();
        _model = new MainViewModel();
    }
    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        //ExportToWord(_model.Ds, @"./output.docx");
        ExportToExcel(_model.Ds, @"./output.xlsx");
    }
    public void ExportToWord(DataSet dataSet, string outputPath)
    {
        using (var doc = DocX.Create(outputPath))
        {
            foreach (DataTable table in dataSet.Tables)
            {
                doc.InsertParagraph(table.TableName).Bold().FontSize(14d).SpacingBefore(10d).SpacingAfter(10d);

                var wordTable = doc.AddTable(table.Rows.Count + 1, table.Columns.Count);

                for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                {
                    wordTable.Rows[0].Cells[colIndex].Paragraphs[0].Append(table.Columns[colIndex].ColumnName);
                }

                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                    {
                        wordTable.Rows[rowIndex + 1].Cells[colIndex].Paragraphs[0].Append(table.Rows[rowIndex][colIndex].ToString());
                    }
                }

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
                for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                {
                    worksheet.Cells[1, colIndex + 1].Value = table.Columns[colIndex].ColumnName;
                }

                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                    {
                        worksheet.Cells[rowIndex + 2, colIndex + 1].Value = table.Rows[rowIndex][colIndex];
                    }
                }
            }

            File.WriteAllBytes(outputPath, package.GetAsByteArray());
        }
    }
}
