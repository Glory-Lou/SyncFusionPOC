using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspDotnetCorePDFSignature.Models;
using Syncfusion.Pdf.Parsing;
using System.IO;
using Syncfusion.EJ2.PdfViewer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Security;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace AspDotnetCorePDFSignature.Controllers
{
    public class HomeController : Controller
    {
       // private IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            //_hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        //private string GetDocumentPath(string document)
        //{
        //    string documentPath = string.Empty;
        //    if (!System.IO.File.Exists(document))
        //    {
        //        var path = _hostingEnvironment.ContentRootPath;
        //        if (System.IO.File.Exists(path + "/Data/" + document))
        //            documentPath = path + "/Data/" + document;
        //    }
        //    else
        //    {
        //        documentPath = document;
        //    }
        //    Console.WriteLine(documentPath);
        //    return documentPath;
        //}
       

        [HttpPost("Flatern")]
        [Route("[controller]/Flatern")]
        public IActionResult Flatern()
        {
            //Load the PDF document

             FileStream docStream = new FileStream("D:\\JaiProject\\project\\DotNetCore-Web\\AspDotnetCorePDFSignature\\Sample.pdf", FileMode.Open, FileAccess.Read);
           // FileStream docStream = new FileStream(GetDocumentPath(jsonObject["fileName"]), FileMode.Open, FileAccess.Read);

            PdfLoadedDocument lDoc = new PdfLoadedDocument(docStream);

            //Gets the first page from the document

            PdfLoadedPage page = lDoc.Pages[0] as PdfLoadedPage;

            //Gets the annotation collection

            PdfLoadedAnnotationCollection annotations = page.Annotations;

            //Gets the first annotation and modify the properties

            PdfLoadedPopupAnnotation popUp = annotations[0] as PdfLoadedPopupAnnotation;

            popUp.Border = new PdfAnnotationBorder(4, 0, 0);

            popUp.Color = new PdfColor(Color.Red);

            popUp.Text = "Modified annotation";

            //Save the document into stream

            MemoryStream stream = new MemoryStream();

            lDoc.Save(stream);

            stream.Position = 0;

            //Closes the document

            lDoc.Close(true);

            //Defining the ContentType for pdf file

            string contentType = "application/pdf";

            //Define the file name

            string fileName = "sample.pdf";

            //Creates a FileContentResult object by using the file contents, content type, and file name

            return File(stream, contentType, fileName);
        }

        [HttpPost("ProcessForm")]
        [Route("[controller]/ProcessForm")]
        public IActionResult ProcessForm([FromBody] Dictionary<string, string> jsonObject)
        {

            FileStream docStream = new FileStream("D:\\JaiProject\\project\\DotNetCore-Web\\AspDotnetCorePDFSignature\\Sample(14) (1).pdf", FileMode.Open, FileAccess.Read);
            X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            //Find the certificate using thumb print.
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, "F85E1C5D93115CA3F969DA3ABC8E0E9547FCCF5A", true);
            X509Certificate2 digitalID = collection[0];

            //Load existing PDF document.
            // FileStream docStream = new FileStream("Vendor.pdf", FileMode.Open, FileAccess.Read);

            PdfLoadedDocument document = new PdfLoadedDocument(docStream);

            //Load X509Certificate2.
            PdfCertificate certificate = new PdfCertificate(digitalID);

            //Create a revision 2 signature with loaded digital ID.
            //PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");

            ////Changing the digital signature standard and hashing algorithm.
            //signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
            //signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;
            PdfLoadedAnnotationCollection annotationCollection = document.Pages[0].Annotations;
            foreach (PdfLoadedAnnotation ann in annotationCollection)
            {
                PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");

                //Changing the digital signature standard and hashing algorithm.
                signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
                signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;
                signature.Bounds = new Syncfusion.Drawing.RectangleF(ann.Bounds.X, ann.Bounds.Y, ann.Bounds.Width, ann.Bounds.Height);
                //Flatten all the annotations in the page
                //   loadedPage.Annotations.Flatten = true;

            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            ////Closes the document

            document.Close(true);

            ////Defining the ContentType for pdf file

            string contentType = "application/pdf";

            //Define the file name

            string fileName = "ProcessedForm.pdf";

            //Creates a FileContentResult object by using the file contents, content type, and file name

            return File(stream, contentType, fileName);




        }

        [HttpPost("AddMultipleSign")]
        [Route("[controller]/AddMultipleSign")]
        public IActionResult AddMultipleSign()
        {
            
            FileStream docStream = new FileStream("D:\\JaiProject\\project\\DotNetCore-Web\\AspDotnetCorePDFSignature\\Sample (8).pdf", FileMode.Open, FileAccess.Read);
           
            // FileStream docStream = new FileStream("Sample.pdf", FileMode.Open, FileAccess.Read);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
           
             PdfDocument document = new PdfDocument();

            string contentType = "application/pdf";

            //Set margin
            document.PageSettings.Margins.All = 0;

            //Add page
            PdfPage page = document.Pages.Add();

            for (int _i = 0; _i < 2; _i++)
            {
                //Create PDF Signature field
                PdfSignatureField signatureField = new PdfSignatureField(page, "Signature");
                //Set bounds
                signatureField.Bounds = new Syncfusion.Drawing.RectangleF(72 + (_i * 100), (float)(100 * 0.7), 90, 30);
                signatureField.ToolTip = "Signature";
                signatureField.Required = true;
                signatureField.BorderStyle = PdfBorderStyle.Solid;
                //Add the form field to the document.
                document.Form.Fields.Add(signatureField);
                // End Signature Field

                //Draw text in below of each signature box 
                page.Graphics.DrawString("(John)", new PdfStandardFont(PdfFontFamily.Helvetica, 10), PdfBrushes.Black, new PointF(72 + (_i * 100) + 25, (float)(100 * 0.7) + 40));
            }
            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            ////Closes the document

            document.Close(true);

           

            //Define the file name

            string fileName = "AddMultipleSign.pdf";

            //Creates a FileContentResult object by using the file contents, content type, and file name

            return File(stream, contentType, fileName);
        }

        [HttpPost("Save")]
        [Route("[controller]/Save")]
        public IActionResult Save()
        {
            FileStream docStream = new FileStream("D:\\JaiProject\\project\\DotNetCore-Web\\AspDotnetCorePDFSignature\\Sample (8).pdf", FileMode.Open, FileAccess.Read);
            //Load the PDF document
            // FileStream docStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
            //Get the first page of the document
            PdfPageBase page = loadedDocument.Pages[0];
            //Create an instance for named destination.
            PdfNamedDestination destination = new PdfNamedDestination("Jai Shree Ram");
            destination.Destination = new PdfDestination(page);
            //Set the location
            destination.Destination.Location = new PointF(0, 500);
            //Set zoom factor to 400 percentage
            destination.Destination.Zoom = 4;
            loadedDocument.NamedDestinationCollection.Add(destination);
            //Save the document into stream
            MemoryStream stream = new MemoryStream();
            loadedDocument.Save(stream);
            stream.Position = 0;
            //Closes the document
            loadedDocument.Close(true);
            //Defining the ContentType for pdf file
            string contentType = "application/pdf";
            //Define the file name
            string fileName = "Output.pdf";
            //Creates a FileContentResult object by using the file contents, content type, and file name
            return File(stream, contentType, fileName);
        }
        //public IActionResult Save()
        //{




        //     FileStream docStream = new FileStream("D:\\JaiProject\\project\\DotNetCore-Web\\AspDotnetCorePDFSignature\\Sample.pdf", FileMode.Open, FileAccess.Read);
        //    //Load the PDF document

        //  //  FileStream docStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read);

        //    PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);

        //    //Gets the first page of the document

        //    PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;

        //    //Gets the first signature field of the PDF document

        //    PdfLoadedSignatureField field = loadedDocument.Form.Fields[0] as PdfLoadedSignatureField;

        //    //Creates a certificate

        //    FileStream certificateStream = new FileStream("PDF.pfx", FileMode.Open, FileAccess.Read);

        //    PdfCertificate certificate = new PdfCertificate(certificateStream, "syncfusion");

        //    field.Signature = new PdfSignature(loadedDocument, page, certificate, "Signature", field);

        //    //Save the document into stream

        //    MemoryStream stream = new MemoryStream();

        //    loadedDocument.Save(stream);

        //    stream.Position = 0;

        //    //Close the documents

        //    loadedDocument.Close(true);

        //    //Defining the ContentType for pdf file

        //    string contentType = "application/pdf";

        //    //Define the file name

        //    string fileName = "Output.pdf";

        //    //Creates a FileContentResult object by using the file contents, content type, and file name

        //    return File(stream, contentType, fileName);







        //    // PdfLoadedDocument document = new PdfLoadedDocument(docStream);

        //    // //Creates a rectangle

        //    // RectangleF rectangle = new RectangleF(10, 40, 30, 30);

        //    // //Creates a new popup annotation.

        //    // PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle, "Test popup annotation");

        //    // popupAnnotation.Border.Width = 4;

        //    // popupAnnotation.Border.HorizontalRadius = 20;

        //    // popupAnnotation.Border.VerticalRadius = 30;

        //    // //Sets the pdf popup icon.

        //    // popupAnnotation.Icon = PdfPopupIcon.NewParagraph;

        //    // //Adds the annotation to loaded page

        //    // document.Pages[0].Annotations.Add(popupAnnotation);

        //    // ///////////////////////////
        //    // ///

        //    // //PdfDocument document = new PdfDocument();

        //    // //Creates a new page 

        //    // PdfPage page =(PdfPage) document.Pages.Add();

        //    // //Creates a rectangle

        //    // RectangleF rectangle1 = new RectangleF(10, 40, 30, 30);



        //    // PdfSignatureField signatureField = new PdfSignatureField(page, "SignatureField");

        //    // signatureField.Bounds = new RectangleF(200, 300, 100, 100);

        //    // signatureField.Signature = new PdfSignature();

        //    //// Adds certificate to the signature field

        //    //  FileStream certificateStream = new FileStream("PDF.pfx", FileMode.Open, FileAccess.Read);

        //    // signatureField.Signature.Certificate = new PdfCertificate(certificateStream, "syncfusion");

        //    // signatureField.Signature.Reason = "I am author of this document";

        //    // //Adds the field
        //    // document.Form.Fields.Add(signatureField);




        //    // //Save the document into stream

        //    // MemoryStream stream = new MemoryStream();

        //    // document.Save(stream);

        //    // stream.Position = 0;

        //    // //Closes the document

        //    // document.Close(true);

        //    // //Defining the ContentType for pdf file

        //    // string contentType = "application/pdf";

        //    // //Define the file name

        //    // string fileName = "PopupAnnotation.pdf";

        //    // //Creates a FileContentResult object by using the file contents, content type, and file name

        //    // return File(stream, contentType, fileName);






        //    // //Load the PDF document
        //    //// FileStream docStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read);
        //    // PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
        //    // //Get the first page of the document
        //    // PdfPageBase page = loadedDocument.Pages[0];
        //    // //Create an instance for named destination.
        //    // PdfNamedDestination destination = new PdfNamedDestination("Jai Shree Ram");
        //    // destination.Destination = new PdfDestination(page);
        //    // //Set the location
        //    // destination.Destination.Location = new PointF(0, 500);
        //    // //Set zoom factor to 400 percentage
        //    // destination.Destination.Zoom = 4;
        //    // loadedDocument.NamedDestinationCollection.Add(destination);
        //    // //Save the document into stream
        //    // MemoryStream stream = new MemoryStream();
        //    // loadedDocument.Save(stream);
        //    // stream.Position = 0;
        //    // //Closes the document
        //    // loadedDocument.Close(true);
        //    // //Defining the ContentType for pdf file
        //    // string contentType = "application/pdf";
        //    // //Define the file name
        //    // string fileName = "Output.pdf";
        //    // //Creates a FileContentResult object by using the file contents, content type, and file name
        //    // return File(stream, contentType, fileName);




        //    // FileStream docStream = new FileStream("Sample.pdf", FileMode.Open, FileAccess.Read);
        //    // PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
        //    // //To-Do some manipulation
        //    // //To-Do some manipulation
        //    // //Save the document into stream
        //    // MemoryStream stream = new MemoryStream();
        //    // loadedDocument.Save(stream);
        //    // //If the position is not set to '0' then the PDF will be empty.
        //    // stream.Position = 0;
        //    // //Close the document.
        //    // loadedDocument.Close(true);
        //    // //Defining the ContentType for pdf file.
        //    // string contentType = "application/pdf";
        //    // //Define the file name.
        //    // string fileName = "Output.pdf";
        //    // //Creates a FileContentResult object by using the file contents, content type, and file name.
        //    // return File(stream, contentType, fileName);
        //    //// return View();
        //    ///



        //    //FileStream docStream = new FileStream("C:\\Users\\hp\\Downloads\\Sample (3).pdf", FileMode.Open, FileAccess.Read);

        //    //PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);

        //    ////Gets the page

        //    //PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;

        //    ////Creates a signature field

        //    //PdfSignatureField signatureField = new PdfSignatureField(page, "SignatureField");

        //    //signatureField.Bounds = new RectangleF(200, 300, 100, 100);

        //    //signatureField.Signature = new PdfSignature();

        //    //Adds certificate to the signature field

        //    //// FileStream certificateStream = new FileStream("PDF.pfx", FileMode.Open, FileAccess.Read);

        //    // signatureField.Signature.Certificate = new PdfCertificate(certificateStream, "syncfusion");

        //    // signatureField.Signature.Reason = "I am author of this document";

        //    //Adds the field

        //    //loadedDocument.Form.Fields.Add(signatureField);

        //    ////Save the document into stream

        //    //MemoryStream stream = new MemoryStream();

        //    //loadedDocument.Save(stream);

        //    //stream.Position = 0;

        //    ////Close the documents

        //    //loadedDocument.Close(true);

        //    ////Defining the ContentType for pdf file

        //    //string contentType = "application/pdf";

        //    ////Define the file name

        //    //string fileName = "Output.pdf";

        //    ////Creates a FileContentResult object by using the file contents, content type, and file name

        //    //return File(stream, contentType, fileName);


        //    //  FileStream docStream = new FileStream("Sample.pdf", FileMode.Open, FileAccess.Read);
        //    // PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
        //    //Create PDF document 
        //    // PdfDocument document = new PdfDocument();

        //    // string contentType = "application/pdf";

        //    // //Set margin
        //    // document.PageSettings.Margins.All = 0;

        //    // //Add page
        //    // PdfPage page = document.Pages.Add();

        //    // for (int _i = 0; _i < 2; _i++)
        //    // {
        //    //     //Create PDF Signature field
        //    //     PdfSignatureField signatureField = new PdfSignatureField(page, "Signature");
        //    //     //Set bounds
        //    //     signatureField.Bounds = new Syncfusion.Drawing.RectangleF(72 + (_i * 100), (float)(100 * 0.7), 90, 30);
        //    //     signatureField.ToolTip = "Signature";
        //    //     signatureField.Required = true;
        //    //     signatureField.BorderStyle = PdfBorderStyle.Solid;
        //    //     //Add the form field to the document.
        //    //     document.Form.Fields.Add(signatureField);
        //    //     // End Signature Field

        //    //     //Draw text in below of each signature box 
        //    //     page.Graphics.DrawString("(John)", new PdfStandardFont(PdfFontFamily.Helvetica, 10), PdfBrushes.Black, new PointF(72 + (_i * 100) + 25, (float)(100 * 0.7) + 40));
        //    // }
        //    // MemoryStream stream = new MemoryStream();
        //    //// loadedDocument.Save(stream);
        //    // //If the position is not set to '0' then the PDF will be empty.
        //    // stream.Position = 0;
        //    // document.Close(true);
        //    // //Save the document
        //    // MemoryStream ms = new MemoryStream();
        //    // //document.Save(ms);
        //    // ms.Position = 0;
        //    // //Save the document 
        //    // FileStream outStream = System.IO.File.OpenWrite("Sample.pdf");
        //    // ms.WriteTo(outStream);
        //    // outStream.Flush();
        //    //// string fileName = "Output.pdf";
        //    // return File(stream, contentType, "Sample.pdf");
        //}

        public void AddShape()
        {
            PdfViewer pdfviewer = new PdfViewer();
            
            //System.Windows.Shapes.Path signature = new System.Windows.Shapes.Path();
            ////Note: Set your signature appearance as Data for the path here.
            ////Add signature in the first page of the PDF document at the location: X= 450, Y=750 and with the size: Width=100, Height=100.
            //pdfViewer.AddHandwrittenSignature(signature, 1, new RectangleF(450, 750, 100, 100));


            System.Windows.Shapes signature = new System.Windows.Shapes();
            //Note: Set your signature appearance using `Data` property of the path here.
            Dictionary<int, List<RectangleF>> bounds = new Dictionary<int, List<RectangleF>>();
            //Add signature bounds for the first page as: X= 450, Y=750, Width=100, Height=100.
            bounds.Add(1, new List<RectangleF>() { new RectangleF(250, 230, 100, 100) });
            //Add signature bounds for the second page as: X= 350, Y=550, Width=200, Height=200
            //  bounds.Add(2, new List<RectangleF>() { new RectangleF(350, 550, 200, 200) });
            //pdfviewer.AnnotationSettings.EnableHandwrittenSignature(signature, bounds);
            pdfviewer.RectangleSettings = new PdfViewerRectangleSettings()
            { };
            pdfviewer.HandWrittenSignatureSettings = new PdfViewerHandWrittenSignatureSettings()
            { 
                
            };

            

        }

        [HttpPost]
        public void CreateWritFile()
        {
            System.IO.File.WriteAllText("d:\\myFiletest", "Jai Mata Di");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
