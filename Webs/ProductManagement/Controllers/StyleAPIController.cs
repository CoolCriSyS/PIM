using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    public class StyleAPIController : ApiController
    {
        private PIMContext db = new PIMContext();

        // GET api/StyleInfoAPI
        public IEnumerable<Style> GetStyles()
        {
            //TODO: How do I filter by brand?
            var styles = db.Styles.Include(s => s.Pattern);
            foreach (var style in styles)
            {
                style.PatternName = style.Pattern.PatternName;
                style.BrandId = style.Pattern.BrandId;
            }
            return styles.AsEnumerable<Style>();
        }

        // GET api/StyleInfoAPI/5
        public Style GetStyle(int id)
        {
            Style style = db.Styles.Find(id);
            if (style == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return style;
        }

        // PUT api/StyleInfoAPI/5
        public HttpResponseMessage PutStyle(int id, Style style)
        {
            if (style.BrandId == 0 || string.IsNullOrEmpty(style.PatternName) || string.IsNullOrEmpty(style.MarketingDescription))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "All fields except Tech Bullets are required.");
            
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != style.StyleId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var pattern = db.Patterns.SingleOrDefault(p => p.PatternName == style.PatternName);
            style.PatternId = pattern.PatternId;
            db.Entry(style).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/StyleInfoAPI
        public HttpResponseMessage PostStyle(Style style)
        {
            if (style.BrandId == 0 || string.IsNullOrEmpty(style.PatternName) || string.IsNullOrEmpty(style.MarketingDescription))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "All fields except Tech Bullets are required.");
            
            if (ModelState.IsValid)
            {
                var style2 = db.Styles.SingleOrDefault(s => s.StockNumber == style.StockNumber);
                if (style2 != null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Style number already exists. Please use update.");
                
                var pattern = db.Patterns.SingleOrDefault(p => p.PatternName == style.PatternName);
                if (pattern == null) pattern = new Pattern();
                pattern.BrandId = style.BrandId;
                pattern.PatternName = style.PatternName;
                pattern.LastUpdated = DateTime.Now;
                db.Entry(pattern).State = pattern.PatternId == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges(); // Need to save the pattern first to get an id

                style.Pattern = pattern;
                db.Styles.Add(style);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, style);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = style.StyleId }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/StyleInfoAPI/5
        public HttpResponseMessage DeleteStyle(int id)
        {
            Style style = db.Styles.Find(id);
            if (style == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Styles.Remove(style);

            // Remove pattern if no other styles are using it
            if (db.Styles.Count(s => s.PatternId == style.PatternId) <= 1)
            {
                db.Patterns.Remove(db.Patterns.Find(style.PatternId));
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, style);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}