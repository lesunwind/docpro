// src/DocumentProcessor.Core/Models/TableInfo.cs
namespace DocumentProcessor.Core.Models
{
    public class TableInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;  // e.g., "Table 1"
        public string Caption { get; set; } = string.Empty; // Full table caption
        public List<List<string>> Data { get; set; } = new List<List<string>>();
        public List<TableReference> References { get; set; } = new List<TableReference>();
        public string Section { get; set; } = string.Empty; // Section where table appears
        public bool IsDuplicate { get; set; } = false; // Track if this is a duplicate table
        public string OriginalTableId { get; set; } = string.Empty; // Link to original table if duplicate
    }

    public class TableReference
    {
        public string ReferenceText { get; set; } = string.Empty; // The text containing the reference
        public string SectionId { get; set; } = string.Empty;     // Section containing the reference
        public int Position { get; set; }                         // Position in text
        public string Context { get; set; } = string.Empty;       // Surrounding context
    }
}