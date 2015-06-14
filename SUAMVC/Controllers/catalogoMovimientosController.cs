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
    public class catalogoMovimientosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: catalogoMovimientos
        public ActionResult Index()
        {
            return View(db.catalogoMovimientos.ToList());
        }

        // GET: catalogoMovimientos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogoMovimiento catalogoMovimiento = db.catalogoMovimientos.Find(id);
            if (catalogoMovimiento == null)
            {
                return HttpNotFound();
            }
            return View(catalogoMovimiento);
        }

        // GET: catalogoMovimientos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: catalogoMovimientos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,tipo,descripcion,fechaCreacion")] catalogoMovimiento catalogoMovimiento)
        {
            if (ModelState.IsValid)
            {
                db.catalogoMovimientos.Add(catalogoMovimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalogoMovimiento);
        }

        // GET: catalogoMovimientos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogoMovimiento catalogoMovimiento = db.catalogoMovimientos.Find(id);
            if (catalogoMovimiento == null)
            {
                return HttpNotFound();
            }
            return View(catalogoMovimiento);
        }

        // POST: catalogoMovimientos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,tipo,descripcion,fechaCreacion")] catalogoMovimiento catalogoMovimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogoMovimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalogoMovimiento);
        }

        // GET: catalogoMovimientos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogoMovimiento catalogoMovimiento = db.catalogoMovimientos.Find(id);
            if (catalogoMovimiento == null)
            {
                return HttpNotFound();
            }
            return View(catalogoMovimiento);
        }

        // POST: catalogoMovimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catalogoMovimiento catalogoMovimiento = db.catalogoMovimientos.Find(id);
            db.catalogoMovimientos.Remove(catalogoMovimiento);
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
