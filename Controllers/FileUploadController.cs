using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualBasic.FileIO;

namespace YourNamespace.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.Error = "Please select a CSV file.";
                return View();
            }

            List<string[]> rows = new List<string[]>();
            string[] headers = null;

            using (var reader = new StreamReader(file.InputStream, Encoding.UTF8))
            using (var parser = new TextFieldParser(reader))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                int rowIndex = 0;
                while (!parser.EndOfData)
                {
                    var values = parser.ReadFields();
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(values[i]))
                        {
                            values[i] = values[i].Trim();
                        }
                    }

                    if (rowIndex == 0)
                    {
                        headers = values;
                        if (headers.Length != 28)
                        {
                            ViewBag.Error = $"Header must have exactly 28 columns. Found: {headers.Length}";
                            return View();
                        }
                    }
                    else
                    {
                        if (values.Length != 28)
                        {
                            Array.Resize(ref values, 28);
                        }
                        rows.Add(values);
                    }
                    rowIndex++;
                }
            }

            TempData["Headers"] = headers;
            TempData["Rows"] = rows;
            return RedirectToAction("DataReview");
        }

        [HttpGet]
        public ActionResult DataReview()
        {
            var headers = TempData["Headers"] as string[];
            var rows = TempData["Rows"] as List<string[]>;
            if (headers == null || rows == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Headers = headers;
            ViewBag.Rows = rows;
            return View();
        }
    }
}