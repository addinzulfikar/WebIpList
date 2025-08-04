using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

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
            {
                int rowIndex = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    // Skip empty or whitespace-only lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    System.Diagnostics.Debug.WriteLine("Line 0: " + line);

                    var values = line.Split(',');

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

            ViewBag.Headers = headers;
            ViewBag.Rows = rows;
            return View();
        }
    }
}