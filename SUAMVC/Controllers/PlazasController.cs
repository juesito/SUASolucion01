using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;

namespace SUAMVC.Controllers
{
    public class PlazasController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Plazas
        public ActionResult Index()
        {
            return View(db.Plazas.OrderBy(s => s.descripcion).ToList());
        }

        // GET: Plazas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plaza plaza = db.Plazas.Find(id);
            if (plaza == null)
            {
                return HttpNotFound();
            }
            return View(plaza);
        }

        // GET: Plazas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Plazas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,cveCorta,indicador")] Plaza plaza)
        {
            if (ModelState.IsValid)
            {
                plaza.descripcion = plaza.descripcion.ToUpper();
                plaza.cvecorta = plaza.cvecorta.ToUpper();
                db.Plazas.Add(plaza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(plaza);
        }

        // GET: Plazas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plaza plaza = db.Plazas.Find(id);
            if (plaza == null)
            {
                return HttpNotFound();
            }
            return View(plaza);
        }

        // POST: Plazas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,cveCorta,indicador")] Plaza plaza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plaza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(plaza);
        }

        // GET: Plazas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plaza plaza = db.Plazas.Find(id);
            if (plaza == null ) 
            {
                return HttpNotFound();
            }
            return View(plaza);
        }

        // POST: Plazas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Plaza plaza = db.Plazas.Find(id);
            db.Plazas.Remove(plaza);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
