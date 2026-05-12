// src/DocumentProcessor.Core/Handlers/ImageHandler.cs
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing;
using System.Drawing.Imaging;
using DocumentProcessor.Core.Models;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace DocumentProcessor.Core.Handlers
{
    public class ImageHandler
    {
        private const int PageWidthEMU = 12240000;  // 8.5 inches
        private const int MarginEMU = 1440000;      // 1 inch margins
        private const int MaxWidthEMU = PageWidthEMU - (2 * MarginEMU); // Available width
        private const double EMUsPerPixel = 9525;   // Conversion factor
        private const double ScaleFactor = 0.65;

        public Drawing InsertImage(string base64Data, string contentType, WordprocessingDocument doc)
        {
            try
            {
                var mainPart = doc.MainDocumentPart;
                var imageBytes = Convert.FromBase64String(base64Data);

                // Create image part and relationship
                var imagePart = mainPart.AddImagePart(contentType);
                using (var stream = new MemoryStream(imageBytes))
                {
                    imagePart.FeedData(stream);
                }
                var relationshipId = mainPart.GetIdOfPart(imagePart);

                // Calculate dimensions with scaling
                long width, height;
                using (var ms = new MemoryStream(imageBytes))
                using (var img = System.Drawing.Image.FromStream(ms))
                {
                    // Apply scaling factor to original dimensions
                    width = (long)(img.Width * EMUsPerPixel * ScaleFactor);
                    height = (long)(img.Height * EMUsPerPixel * ScaleFactor);

                    Console.WriteLine($"Original size: {img.Width}x{img.Height}");
                    Console.WriteLine($"Scaled size: {width / EMUsPerPixel}x{height / EMUsPerPixel}");
                }

                // Create drawing element
                var element = new Drawing(
                    new DW.Inline(
                        new DW.Extent() { Cx = width, Cy = height },
                        new DW.EffectExtent()
                        {
                            LeftEdge = 0L,
                            TopEdge = 0L,
                            RightEdge = 0L,
                            BottomEdge = 0L
                        },
                        new DW.DocProperties()
                        {
                            Id = (UInt32Value)1U,
                            Name = "Picture 1"
                        },
                        new DW.NonVisualGraphicFrameDrawingProperties(
                            new A.GraphicFrameLocks() { NoChangeAspect = true }
                        ),
                        new A.Graphic(
                            new A.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = "image.png"
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()
                                    ),
                                    new PIC.BlipFill(
                                        new A.Blip()
                                        {
                                            Embed = relationshipId,
                                            CompressionState = A.BlipCompressionValues.Print
                                        },
                                        new A.Stretch(new A.FillRectangle())
                                    ),
                                    new PIC.ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset() { X = 0L, Y = 0L },
                                            new A.Extents() { Cx = width, Cy = height }
                                        ),
                                        new A.PresetGeometry(
                                            new A.AdjustValueList()
                                        )
                                        { Preset = A.ShapeTypeValues.Rectangle }
                                    )
                                )
                            )
                            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                        )
                    )
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U
                    }
                );

                return element;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InsertImage: {ex.Message}");
                throw;
            }
        }
        public ImageInfo ExtractImage(DocumentFormat.OpenXml.OpenXmlElement element, MainDocumentPart mainPart)
        {
            Console.WriteLine($"Attempting to extract image from element type: {element.GetType().Name}");

            if (element is Drawing drawing)
            {
                Console.WriteLine("Found Drawing element");
                var inline = drawing.Descendants<DW.Inline>().FirstOrDefault();
                if (inline != null)
                {
                    Console.WriteLine("Found Inline element");
                    var blip = drawing.Descendants<A.Blip>().FirstOrDefault();
                    if (blip?.Embed != null)
                    {
                        Console.WriteLine($"Found Blip with Embed ID: {blip.Embed.Value}");
                        var imageId = blip.Embed.Value;
                        try
                        {
                            var imagePart = (ImagePart)mainPart.GetPartById(imageId);
                            Console.WriteLine($"Found ImagePart with ContentType: {imagePart.ContentType}");

                            using (var stream = imagePart.GetStream())
                            using (var ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                var imageBytes = ms.ToArray();
                                Console.WriteLine($"Extracted image bytes: {imageBytes.Length} bytes");

                                using (var img = System.Drawing.Image.FromStream(ms))
                                {
                                    var imageInfo = new ImageInfo
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ContentType = imagePart.ContentType,
                                        Base64Data = Convert.ToBase64String(imageBytes),
                                        Width = img.Width,
                                        Height = img.Height,
                                        Description = inline.DocProperties?.Description ?? string.Empty
                                    };
                                    Console.WriteLine($"Created ImageInfo: {imageInfo.Width}x{imageInfo.Height}");
                                    return imageInfo;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing image: {ex.Message}");
                        }
                    }
                }
            }
            return null;
        }
       
        
    }
}