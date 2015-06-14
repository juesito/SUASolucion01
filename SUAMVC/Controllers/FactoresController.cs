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
    public class FactoresController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Factores
        public ActionResult Index()
        {
            var factores = db.Factores.Include(f => f.Usuario);
            return View(factores.ToList());
        }

        // GET: Factores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factore factore = db.Factores.Find(id);
            if (factore == null)
            {
                return HttpNotFound();
            }
            return View(factore);
        }

        // GET: Factores/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: Factores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,anosTrabajados,diasVacaciones,primaVacacional,porcentaje,diasAno,factorVacaciones,aguinaldo,diasAnoAguinaldo,factor,factorIntegracion,fechaRegistro,usuarioId")] Factore factore)
        {
            if (ModelState.IsValid)
            {
                db.Factores.Add(factore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", factore.usuarioId);
            return View(factore);
        }

        // GET: Factores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factore factore = db.Factores.Find(id);
            if (factore == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", factore.usuarioId);
            return View(factore);
        }

        // POST: Factores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,anosTrabajados,diasVacaciones,primaVacacional,porcentaje,diasAno,factorVacaciones,aguinaldo,diasAnoAguinaldo,factor,factorIntegracion,fechaRegistro,usuarioId")] Factore factore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", factore.usuarioId);
            return View(factore);
        }

        // GET: Factores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factore factore = db.Factores.Find(id);
            if (factore == null)
            {
                return HttpNotFound();
            }
            return View(factore);
        }

        // POST: Factores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Factore factore = db.Factores.Find(id);
            db.Factores.Remove(factore);
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
