using ProductManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new PIMModel { Brands = GetBrands() };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(PIMModel model, HttpPostedFileBase file)
        {
            model.Brands = GetBrands();
            
            if (ModelState.IsValid)
            {
                int added = 0;
                int updated = 0;

                if (file != null && file.ContentLength > 0 && file.FileName.Contains("xls"))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Files/upload"), fileName);
                    file.SaveAs(path);
                    var dt = ReadSpreadsheetIntoDataTable(path, "ProductInfo");
                    System.IO.File.Delete(path);

                    using (var db = new PIMContext())
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (string.IsNullOrEmpty(row["Style Number"].ToString().Trim()))
                            {
                                ModelState.AddModelError("StyleError", "A style number is missing. Please correct and re-upload.");
                                return View(model);
                            }
                            string patternName = row["Pattern Name"].ToString().Trim();
                            var pattern = db.Patterns.SingleOrDefault(p => p.PatternName == patternName);
                            if (pattern == null) pattern = new Pattern();
                            pattern.BrandId = model.BrandId;
                            pattern.PatternName = patternName;
                            pattern.LastUpdated = DateTime.Now;
                            // Add if it doesn't exist otherwise just updates the existing pattern that was loaded
                            db.Entry(pattern).State = pattern.PatternId == 0 ? EntityState.Added : EntityState.Modified;
                            db.SaveChanges(); // Need to save the pattern first to get an id

                            string styleNumber = row["Style Number"].ToString().Trim();
                            var style = db.Styles.SingleOrDefault(s => s.StockNumber == styleNumber);
                            if (style == null) style = new Style();
                            style.PatternId = pattern.PatternId;
                            style.StockNumber = styleNumber;
                            style.MarketingDescription = row["Marketing Description"].ToString();
                            style.TechBullets = !string.IsNullOrEmpty(row["Tech Bullets"].ToString()) ?
                                row["Tech Bullets"].ToString() : null;
                            style.LastUpdated = DateTime.Now;
                            style.LastUpdatedBy = User.Identity.Name;

                            db.Entry(style).State = style.StyleId == 0 ? EntityState.Added : EntityState.Modified;
                            if (style.StyleId == 0)
                                added++;
                            else
                                updated++;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("XlsError", "Please upload a valid Excel file.");
                    return View(model);
                }

                return RedirectToAction("ViewProducts", new { b = model.BrandId, a = added, u = updated });
            }

            return View(model);
        }

        public ActionResult ViewProducts(int a = 0, int u = 0)
        {
            var model = new PIMModel { Brands = GetBrands() };
            ViewBag.Added = a;
            ViewBag.Updated = u;

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult Purge()
        {
            using (var db = new PIMContext())
            {
                var ctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext;
                ctx.ExecuteStoreCommand("TRUNCATE TABLE [Style]");
                ctx.ExecuteStoreCommand("DELETE FROM [Pattern]");
            }

            ViewBag.Purged = "The [Style] and [Pattern] tables have been purged.";
            return View("Admin");
        }

        private SelectList GetBrands()
        {            
            using (var db = new PIMContext())
            {
                var query = from b in db.Brands
                            select b;

                return new SelectList(query.ToList(), "BrandId", "BrandName");
            }
        }

        private DataTable ReadSpreadsheetIntoDataTable(string path, string sheet)
        {
            string sql = "SELECT * FROM [" + sheet + "$]";
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path +
                                      ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;\"";

            using (var connection = new OleDbConnection(connectionString))
            using (var command = new OleDbCommand(sql, connection))
            using (var adapter = new OleDbDataAdapter(command))
            {
                var dataTable = new DataTable { Locale = CultureInfo.CurrentCulture };
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}
