using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syncfusion.Drawing;
using Syncfusion.EJ2.PdfViewer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;
using System;
using System.Collections.Generic;

using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace PdfViewerService2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PdfViewerController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        //Initialize the memory cache object   
        public IMemoryCache _cache;
        public PdfViewerController(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            Console.WriteLine("PdfViewerController initialized");
        }

        [HttpPost("Load")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Load")]
        //Post action for Loading the PDF documents   
        public IActionResult Load([FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Load called");
            //Initialize the PDF viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            MemoryStream stream = new MemoryStream();
            object jsonResult = new object();
            if (jsonObject != null && jsonObject.ContainsKey("document"))
            {
                if (bool.Parse(jsonObject["isFileName"]))
                {
                    string documentPath = GetDocumentPath(jsonObject["document"]);
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(documentPath);
                        stream = new MemoryStream(bytes);
                    }
                    else
                    {
                        return this.Content(jsonObject["document"] + " is not found");
                    }
                }
                else
                {
                    byte[] bytes = Convert.FromBase64String(jsonObject["document"]);
                    stream = new MemoryStream(bytes);
                }
            }
            jsonResult = pdfviewer.Load(stream, jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("Bookmarks")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Bookmarks")]
        //Post action for processing the bookmarks from the PDF documents
        public IActionResult Bookmarks([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            var jsonResult = pdfviewer.GetBookmarks(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderPdfPages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderPdfPages")]
        //Post action for processing the PDF documents  
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetPage(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderThumbnailImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderThumbnailImages")]
        //Post action for rendering the ThumbnailImages
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object result = pdfviewer.GetThumbnailImages(jsonObject);
            return Content(JsonConvert.SerializeObject(result));
        }      
        [AcceptVerbs("Post")]
        [HttpPost("RenderAnnotationComments")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderAnnotationComments")]
        //Post action for rendering the annotations
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetAnnotationComments(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        [AcceptVerbs("Post")]
        [HttpPost("ExportAnnotations")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ExportAnnotations")]
        //Post action to export annotations
        public IActionResult ExportAnnotations([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = pdfviewer.GetAnnotations(jsonObject);
            return Content(jsonResult);
        }
        [AcceptVerbs("Post")]
        [HttpPost("ImportAnnotations")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ImportAnnotations")]
        //Post action to import annotations
        public IActionResult ImportAnnotations([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = string.Empty;
            if (jsonObject != null && jsonObject.ContainsKey("fileName"))
            {
                string documentPath = GetDocumentPath(jsonObject["fileName"]);
                if (!string.IsNullOrEmpty(documentPath))
                {
                    jsonResult = System.IO.File.ReadAllText(documentPath);
                }
                else
                {
                    return this.Content(jsonObject["document"] + " is not found");
                }
            }
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ExportFormFields")]
        public IActionResult ExportFormFields([FromBody] Dictionary<string, string> jsonObject)

        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = pdfviewer.ExportFormFields(jsonObject);
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ImportFormFields")]
        public IActionResult ImportFormFields([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.ImportFormFields(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Unload")]
        //Post action for unloading and disposing the PDF document resources  
        public IActionResult Unload([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            pdfviewer.ClearCache(jsonObject);
            return this.Content("Document cache is cleared");
        }


        [HttpPost("Download")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Download")]
        public IActionResult Download([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            string contentType = "application/pdf";
            MemoryStream stream = new MemoryStream();
            //Define the file name

            string fileName = "ProcessedForm.pdf";

            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
            string base64String = documentBase.Split(new string[] { "data:application/pdf;base64," }, StringSplitOptions.None)[1];
            if (base64String != null || base64String != string.Empty)
            {

                byte[] byteArray = Convert.FromBase64String(base64String);

                PdfLoadedDocument document = new PdfLoadedDocument(byteArray);

                X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                //Find the certificate using thumb print.
                X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, "F85E1C5D93115CA3F969DA3ABC8E0E9547FCCF5A", true);
                X509Certificate2 digitalID = collection[0];

                //Load X509Certificate2.
                PdfCertificate certificate = new PdfCertificate(digitalID);



                for (int i = 0; i < document.PageCount; i++)
                {
                    PdfLoadedAnnotationCollection annotationCollection = document.Pages[i].Annotations;

                    for (int j = annotationCollection.Count - 1; j >= 0; j--)
                    {
                        var ann = annotationCollection[j];

                        if (ann.Subject == "Sign Here")
                        {
                            PdfSignature signature = new PdfSignature(document, document.Pages[i], certificate, "DigitalSignature");

                            //Changing the digital signature standard and hashing algorithm.
                            signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
                            signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;
                            signature.Bounds = new Syncfusion.Drawing.RectangleF(ann.Bounds.X, ann.Bounds.Y, ann.Bounds.Width, ann.Bounds.Height);
                            signature.Certificated = true;

                           // signature.DocumentPermissions = PdfCertificationFlags.ForbidChanges;
                            //  signature.DocumentPermissions = PdfCertificationFlags.AllowFormFill | PdfCertificationFlags.AllowComments;
                            //Flatten all the annotations in the page
                            document.Pages[i].Annotations.Flatten = false;
                            annotationCollection.RemoveAt(j);
                        }
                    }
                }

                //document.Save("Output.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Open);
                document.Save(stream);
                stream.Position = 0;
                document.Close();

                documentBase = string.Concat("data:application/pdf;base64,", Convert.ToBase64String(stream.ToArray()));


            }

            return Content(documentBase);
        }
        //public IActionResult Download([FromBody] Dictionary<string, string> jsonObject)
        //{
        //    PdfRenderer pdfviewer = new PdfRenderer(_cache);
        //    //  FileStream docStream = new FileStream("D:\\JaiProject\\project\\DotNetCore-Web\\AspDotnetCorePDFSignature\\Sample(14) (1).pdf", FileMode.Open, FileAccess.Read);
        //    X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
        //    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        //    X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
        //    //Find the certificate using thumb print.
        //    X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, "F85E1C5D93115CA3F969DA3ABC8E0E9547FCCF5A", true);
        //    X509Certificate2 digitalID = collection[0];

        //    //PdfRubberStampAnnotation

        //   // PdfLoadedDocument document = new PdfLoadedDocument(docStream);
        //    PdfCertificate certificate = new PdfCertificate(digitalID);
        //   // PdfLoadedAnnotationCollection annotationCollection = document.Pages[0].Annotations;
        //    //var requiredAnnotation= annotationCollection.se
        //    var content = jsonObject["stampAnnotations"];
        //  List<PdfRubberStampAnnotation> objPdfRubberStampAnnotation = JsonConvert.DeserializeObject< List<PdfRubberStampAnnotation>>(content);
        //   // PdfLoadedRubberStampAnnotation objPdfRubberStampAnnotation = JsonConvert.DeserializeObject<PdfLoadedRubberStampAnnotation>(content);

        //    //PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");
        //    PdfPage page =(PdfPage) pdfviewer.GetPage(jsonObject);
        //    PdfSignature signature = new PdfSignature(page, certificate, "DigitalSignature");

        //    //Changing the digital signature standard and hashing algorithm.
        //    signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
        //    signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;
        //    signature.Bounds = new Syncfusion.Drawing.RectangleF(objPdfRubberStampAnnotation[0].Bounds.X, 
        //        objPdfRubberStampAnnotation[0].Bounds.Y, signature.Bounds.Width, objPdfRubberStampAnnotation[0].Bounds.Height);


        //    //objPdfRubberStampAnnotation.Bounds.
        //    //foreach (PdfLoadedAnnotation ann in annotationCollection)
        //    //{
        //    //    PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");

        //    //    //Changing the digital signature standard and hashing algorithm.
        //    //    signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
        //    //    signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;
        //    //    signature.Bounds = new Syncfusion.Drawing.RectangleF(ann.Bounds.X, ann.Bounds.Y, ann.Bounds.Width, ann.Bounds.Height);
        //    //    //Flatten all the annotations in the page
        //    //    //   loadedPage.Annotations.Flatten = true;

        //    //}

        //    //PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");

        //    //Changing the digital signature standard and hashing algorithm.
        //    //signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
        //    //signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;
        //    //signature.Bounds = new Syncfusion.Drawing.RectangleF(ann.Bounds.X, ann.Bounds.Y, ann.Bounds.Width, ann.Bounds.Height);
        //    jsonObject["stampAnnotations"] = JsonConvert.SerializeObject(signature);





        //    string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
        //    return Content(documentBase);
        //}

        //Post action for downloading the PDF documents

        //public IActionResult Download([FromBody] Dictionary<string, string> jsonObject)
        //{
        //    //Initialize the PDF Viewer

        //    PdfViewerController pdfViewer = new PdfViewerController(_hostingEnvironment, _cache);
        //    //Initialize the PDF Viewer


        //    //Loads the PDF document in PDF Viewer

        //    // pdfViewer.Load(jsonObject);
        //    // pdfViewer.ProcessForm(jsonObject);

        //    //Gets the filename of loaded PDF document

        //    //  string fileName = pdfViewer.DocumentInformation.FileName;

        //    //Gets the file path of loaded PDF document

        //    // string filePath = pdfViewer.DocumentInformation.FilePath;

        //    PdfRenderer pdfviewer = new PdfRenderer(_cache);
        //    string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
        //    object page = pdfviewer.GetPage(jsonObject);

        //    string FileName = jsonObject["documentId"];
        //    FileInfo f = new FileInfo(FileName);
        //    string fullname = f.FullName;
        //    string documentPath = GetDocumentPath(fullname);
        //    FileStream docStream = new FileStream(documentBase, FileMode.Open, FileAccess.Read);
        //    X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
        //    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        //    X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
        //    //Find the certificate using thumb print.
        //    X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, "F85E1C5D93115CA3F969DA3ABC8E0E9547FCCF5A", true);
        //    X509Certificate2 digitalID = collection[0];

        //    //Load existing PDF document.
        //    // FileStream docStream = new FileStream("Vendor.pdf", FileMode.Open, FileAccess.Read);

        //    PdfLoadedDocument document = new PdfLoadedDocument(docStream);

        //    //Load X509Certificate2.
        //    PdfCertificate certificate = new PdfCertificate(digitalID);
        //    PdfLoadedAnnotationCollection annotationCollection =  document.Pages[0].Annotations;
        //    foreach (PdfLoadedAnnotation ann in annotationCollection)
        //    {
        //        PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");

        //        //Changing the digital signature standard and hashing algorithm.
        //        signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
        //        signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;
        //        signature.Bounds = new Syncfusion.Drawing.RectangleF(ann.Bounds.X, ann.Bounds.Y, ann.Bounds.Width, ann.Bounds.Height);
        //        //Flatten all the annotations in the page
        //        //   loadedPage.Annotations.Flatten = true;

        //    }
        //    MemoryStream stream = new MemoryStream();
        //    document.Save(stream);
        //    stream.Position = 0;
        //    ////Closes the document
        //    document.Close(true);
        //    ////Defining the ContentType for pdf file
        //    string contentType = "application/pdf";
        //    //Define the file name
        //    string fileName = "ProcessedForm.pdf";
        //    //Creates a FileContentResult object by using the file contents, content type, and file name
        //    // return File(stream, contentType, fileName);






        //    return Content(documentBase);
        //}

        [HttpPost("ProcessForm")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
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

        [HttpPost("PrintImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/PrintImages")]
        //Post action for printing the PDF documents
        public IActionResult PrintImages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object pageImage = pdfviewer.GetPrintImage(jsonObject);
            return Content(JsonConvert.SerializeObject(pageImage));
        }

        //Gets the path of the PDF document
        private string GetDocumentPath(string document)
        {
            string documentPath = string.Empty;
            if (!System.IO.File.Exists(document))
            {
                var path = _hostingEnvironment.ContentRootPath;
                if (System.IO.File.Exists(path + "/Data/" + document))
                    documentPath = path + "/Data/" + document;
            }
            else
            {
                documentPath = document;
            }
            Console.WriteLine(documentPath);
            return documentPath;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost("Flatern")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Flatern")]
        public IActionResult Flatern([FromBody] Dictionary<string, string> jsonObject)
        {
            //Load the PDF document

            // FileStream docStream = new FileStream("D:\\JaiProject\\project\\DotNetCore-Web\\AspDotnetCorePDFSignature\\Sample.pdf", FileMode.Open, FileAccess.Read);
            FileStream docStream = new FileStream(GetDocumentPath(jsonObject["fileName"]), FileMode.Open, FileAccess.Read);

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

        //public Object GetDocument(jsonObjects jsonObject)
        //{
        //    var jsonData = JsonConverter(jsonObject);
        //    byte[] docBytes = System.IO.File.ReadAllBytes(GetDocumentPath(jsonData["documentName"]));
        //    var docBase64 = "data:application/pdf;base64," + Convert.ToBase64String(docBytes);
        //    return (docBase64);

        //}

    }
}