// src/DocumentProcessor.Core/Models/Section.cs
namespace DocumentProcessor.Core.Models
{
    public class Section
    {
        public string Title { get; set; } = string.Empty;
        public int Level { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<Section> Subsections { get; set; } = new List<Section>();
        public List<TableInfo> Tables { get; set; } = new List<TableInfo>();
        public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
