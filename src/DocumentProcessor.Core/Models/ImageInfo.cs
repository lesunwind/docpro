// src/DocumentProcessor.Core/Models/ImageInfo.cs
using System;

namespace DocumentProcessor.Core.Models
{
    public class ImageInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;  // e.g., "image/png", "image/jpeg"
        public string Base64Data { get; set; } = string.Empty;   // Image data as Base64
        public int Width { get; set; }
        public int Height { get; set; }
        public string Description { get; set; } = string.Empty;  // Alt text or caption
    }
}
