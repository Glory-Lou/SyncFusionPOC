using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Syncfusion.EJ2.PdfViewer;
using System.IO;
using Newtonsoft.Json;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;

namespace AspDotnetCorePDFSignature.Controllers
{
    public class PdfViewerController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public PdfViewerController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Load")]
        public IActionResult Load([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
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
        [HttpPost]
        [Route("api/[controller]/RenderPdfPages")]
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.GetPage(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/RenderAnnotationComments")]
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.GetAnnotationComments(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Unload")]
        public IActionResult Unload([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            pdfviewer.ClearCache(jsonObject);
            return this.Content("Document cache is cleared");
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/RenderThumbnailImages")]
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object result = pdfviewer.GetThumbnailImages(jsonObject);
            return Content(JsonConvert.SerializeObject(result));
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Bookmarks")]
        public IActionResult Bookmarks([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.GetBookmarks(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Download")]
        public IActionResult Download([FromBody] Dictionary<string, string> jsonObject)
        {
            RedirectToAction("ProcessForm", "Home", jsonObject);
            PdfRenderer pdfviewer = new PdfRenderer();
            string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
            return Content(documentBase);
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/PrintImages")]
        public IActionResult PrintImages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object pageImage = pdfviewer.GetPrintImage(jsonObject);
            return Content(JsonConvert.SerializeObject(pageImage));
        }

        private string GetDocumentPath(string document)
        {
            string documentPath = string.Empty;
            if (!System.IO.File.Exists(document))
            {
                string basePath = _hostingEnvironment.WebRootPath;
                string dataPath = string.Empty;
                dataPath = basePath + @"/fileuploads/";
                if (System.IO.File.Exists(dataPath + document))
                    documentPath = dataPath + document;
            }
            else
            {
                documentPath = document;
            }
            return documentPath;
        }

        [AcceptVerbs("Post")]
        [HttpPost("Flatern")]
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

    }
}
