using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using Xceed.Document.NET;
using Xceed.Words.NET;
using OfficeOpenXml;
using System.Data;
using System.IO;

namespace PresenterLibrary
{
    public partial class PreviewWindow : Window
    {
        public string FileName;
        private List<Dictionary<string, string>> PreviewData;

        public PreviewWindow(List<Dictionary<string, string>> previewData, string tableName)
        {
            InitializeComponent();
            FileNameTextBox.Text = tableName;
            PreviewData = previewData;
            LoadExcelPreview();
            LoadWordPreview();
        }

        private void LoadExcelPreview()
        {
            var dataTable = new System.Data.DataTable();
            if (PreviewData.Count > 0)
            {
                foreach (var header in PreviewData[0].Keys)
                    dataTable.Columns.Add(header);

                foreach (var row in PreviewData)
                {
                    var rowData = new object[row.Count];
                    int i = 0;
                    foreach (var value in row.Values)
                        rowData[i++] = value;
                    dataTable.Rows.Add(rowData);
                }
            }

            PreviewDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void LoadWordPreview()
        {
            var paragraph = new System.Windows.Documents.Paragraph();
            paragraph.Inlines.Add(new Bold(new System.Windows.Documents.Run(FileNameTextBox.Text)));
            paragraph.Inlines.Add(new LineBreak());

            if (PreviewData.Count > 0)
            {
                foreach (var row in PreviewData)
                {
                    foreach (var kvp in row)
                    {
                        paragraph.Inlines.Add(new Bold(new System.Windows.Documents.Run(kvp.Key + ": ")));
                        paragraph.Inlines.Add(new System.Windows.Documents.Run(kvp.Value));
                        paragraph.Inlines.Add(new LineBreak());
                    }
                    paragraph.Inlines.Add(new LineBreak());
                }
            }

            PreviewRichTextBox.Document.Blocks.Clear();
            PreviewRichTextBox.Document.Blocks.Add(paragraph);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FileName = FileNameTextBox.Text;
            if (string.IsNullOrEmpty(FileName))
            {
                MessageBox.Show("Пожалуйста, введите имя файла.");
                return;
            }

            if (PreviewTabControl.SelectedIndex == 0) // Excel Preview
            {
                var dataTable = ((DataView)PreviewDataGrid.ItemsSource).ToTable();
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                    // Выделение заголовков жирным шрифтом
                    using (var range = worksheet.Cells[1, 1, 1, dataTable.Columns.Count])
                    {
                        range.Style.Font.Bold = true;
                    }
                    worksheet.Cells.AutoFitColumns();
                    var file = new FileInfo(FileName + ".xlsx");
                    package.SaveAs(file);
                }

                MessageBox.Show("Документ сохранён в Excel: " + FileName + ".xlsx");
            }
            else if (PreviewTabControl.SelectedIndex == 1) // Word Preview
            {
                var doc = DocX.Create(FileName + ".docx");
                var paragraph = doc.InsertParagraph();
                paragraph.AppendLine(FileNameTextBox.Text).Bold().FontSize(14);
                paragraph.AppendLine();

                if (PreviewData.Count > 0)
                {
                    foreach (var row in PreviewData)
                    {
                        foreach (var kvp in row)
                        {
                            paragraph.AppendLine(kvp.Key + ": ").Bold();
                            paragraph.Append(kvp.Value);
                        }
                        paragraph.AppendLine();
                    }
                }

                doc.Save();
                MessageBox.Show("Документ сохранен в Word: " + FileName + ".docx");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void PreviewTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SaveButton != null)
            {
                if (PreviewTabControl.SelectedIndex == 0) // Excel Preview
                    SaveButton.Content = "Сохранить в Excel";
                else if (PreviewTabControl.SelectedIndex == 1) // Word Preview
                    SaveButton.Content = "Сохранить в Word";
            }
        }
    }
}
