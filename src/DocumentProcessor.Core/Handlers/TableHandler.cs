// src/DocumentProcessor.Core/Handlers/TableHandler.cs
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using DocumentProcessor.Core.Models;

namespace DocumentProcessor.Core.Handlers
{
    public class TableHandler
    {
        private readonly Dictionary<string, TableInfo> tableRegistry = new();
        private readonly Dictionary<string, TableInfo> tablesByNumber = new();
        private int currentTableNumber = 1;
        private readonly Regex tableNumberPattern = new(@"Table\s+(\d+[-\d]*)\s*[:\.]?\s*(.+?)(?=\n|$)");
        private readonly Regex tableReferencePattern = new(@"Table\s+(\d+(?:-\d+)?)");

        public TableInfo ExtractTableInfo(DocumentFormat.OpenXml.Wordprocessing.Table table, string precedingText, string currentSection)
        {
            // Find table title and number from preceding text
            var match = tableNumberPattern.Match(precedingText);
            var tableNumber = match.Success ? $"Table {match.Groups[1].Value}" : $"Table {currentTableNumber++}";
            var tableTitle = match.Success ? match.Groups[2].Value.Trim() : string.Empty;

            // Check if this table already exists
            if (tablesByNumber.TryGetValue(tableNumber, out var existingTable))
            {
                return new TableInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = tableNumber,
                    Title = tableTitle,
                    Data = ExtractTableData(table),
                    Section = currentSection,
                    IsDuplicate = true,
                    OriginalTableId = existingTable.Id
                };
            }

            var tableInfo = new TableInfo
            {
                Id = Guid.NewGuid().ToString(),
                Number = tableNumber,
                Title = tableTitle,
                Caption = match.Success ? match.Value.Trim() : string.Empty,
                Data = ExtractTableData(table),
                Section = currentSection
            };

            // Register table
            tableRegistry[tableInfo.Id] = tableInfo;
            tablesByNumber[tableNumber] = tableInfo;

            return tableInfo;
        }

        private List<List<string>> ExtractTableData(DocumentFormat.OpenXml.Wordprocessing.Table table)
        {
            var data = new List<List<string>>();
            foreach (TableRow row in table.Elements<TableRow>())
            {
                var rowData = new List<string>();
                foreach (TableCell cell in row.Elements<TableCell>())
                {
                    rowData.Add(string.Join(" ", cell.Elements<Paragraph>().Select(p => p.InnerText)));
                }
                data.Add(rowData);
            }
            return data;
        }

        public void ProcessTableReferences(string content, string sectionId)
        {
            var matches = tableReferencePattern.Matches(content);
            foreach (Match match in matches)
            {
                var tableNumber = $"Table {match.Groups[1].Value}";
                if (tablesByNumber.TryGetValue(tableNumber, out var referencedTable))
                {
                    var start = Math.Max(0, match.Index - 50);
                    var length = Math.Min(100, content.Length - start);
                    var context = content.Substring(start, length);

                    referencedTable.References.Add(new TableReference
                    {
                        ReferenceText = match.Value,
                        SectionId = sectionId,
                        Position = match.Index,
                        Context = context.Trim()
                    });
                }
            }
        }

        public void ValidateTableNumbers()
        {
            // Re-number tables in sequence if needed
            var orderedTables = tablesByNumber.Values
                .Where(t => !t.IsDuplicate)
                .OrderBy(t => int.Parse(t.Number.Replace("Table ", "")))
                .ToList();

            for (int i = 0; i < orderedTables.Count; i++)
            {
                var table = orderedTables[i];
                var expectedNumber = $"Table {i + 1}";
                if (table.Number != expectedNumber)
                {
                    var oldNumber = table.Number;
                    table.Number = expectedNumber;
                    tablesByNumber.Remove(oldNumber);
                    tablesByNumber[expectedNumber] = table;
                }
            }
        }

        public void UpdateTableReferences()
        {
            foreach (var table in tablesByNumber.Values.Where(t => !t.IsDuplicate))
            {
                // Update references based on new table numbers
                foreach (var reference in table.References)
                {
                    reference.ReferenceText = reference.ReferenceText.Replace(
                        reference.ReferenceText,
                        $"Table {table.Number}");
                }
            }
        }
    }
}