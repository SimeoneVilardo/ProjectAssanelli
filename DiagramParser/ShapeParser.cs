using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace DiagramParser
{
    public class ShapeParser
    {
        public void a(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(
    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
    ImageLockMode.ReadWrite, bitmap.PixelFormat);

            bitmap.UnlockBits(bitmapData);
        }

        private void fixBackground(BitmapData bitmapData)
        {
            ColorFiltering colorFilter = new ColorFiltering();
            colorFilter.Red = new IntRange(0, 64);
            colorFilter.Green = new IntRange(0, 64);
            colorFilter.Blue = new IntRange(0, 64);
            colorFilter.FillOutsideRange = false;
            colorFilter.ApplyInPlace(bitmapData);
        }

        private Blob[] locateObjects(BitmapData bitmapData)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            return blobs;
        }

        public void b(Blob[] blobs, BlobCounter blobCounter)
        {
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                AForge.Point center;
                float radius;

                // is circle ?
                if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                {
                    
                }
                else
                {
                    List<IntPoint> corners;

                    // is triangle or quadrilateral
                    if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                    {
                        // get sub-type
                        PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
                        if (subType == PolygonSubType.Unknown)
                        {
                            //pen = (corners.Count == 4) ? redPen : bluePen;
                        }
                        else
                        {
                           // pen = (corners.Count == 4) ? brownPen : greenPen;
                        }

                        //g.DrawPolygon(pen, ToPointsArray(corners));
                    }
                }
            }
        }

        public string ExtractText()
        {
            string testImagePath = @"..\..\sample.png";
            using (var tEngine = new TesseractEngine(@"..\..\tesseract-master", "ita", EngineMode.Default)) //creating the tesseract OCR engine with English as the language
            {
                using (var img = Pix.LoadFromFile(testImagePath)) // Load of the image file from the Pix object which is a wrapper for Leptonica PIX structure
                {
                    using (var page = tEngine.Process(img)) //process the specified image
                    {
                        string text = page.GetText(); //Gets the image's content as plain text.
                        return text;
                    }
                }
            }
        }
    }
}
