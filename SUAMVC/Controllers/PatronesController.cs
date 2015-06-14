using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SUAMVC.Controllers
{
    public class PatronesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Patrones
        public ActionResult Index(String plazasId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var patrones = from s in db.Patrones
                           where patronesAsignados.Contains(s.Id)
                           select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaIdTemp = int.Parse(plazasId);
                patrones = patrones.Where(p => p.Plaza.id.Equals(plazaIdTemp));
            }

            patrones.OrderBy(p => p.nombre);

            return View(patrones.ToList());
        }

        // GET: Patrones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            return View(patrone);
        }

        // GET: Patrones/Create
        public ActionResult Create()
        {
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion");
            return View();
        }

        // POST: Patrones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,registro,rfc,nombre,actividad,domicilio,municipio,codigoPostal,entidad,telefono,remision,zona,delegacion,carEnt,numeroDelegacion,carDel,numSub,tipoConvenio,convenio,inicioAfiliacion,patRep,clase,fraccion,STyPS,Plaza_id,direccionArchivo")] Patrone patrone)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    patrone.nombre = patrone.nombre.ToUpper();
                    db.Patrones.Add(patrone);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            return View(patrone);
        }

        // GET: Patrones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            return View(patrone);
        }

        // POST: Patrones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,registro,rfc,nombre,actividad,domicilio,municipio,codigoPostal,entidad,telefono,remision,zona,delegacion,carEnt,numeroDelegacion,carDel,numSub,tipoConvenio,convenio,inicioAfiliacion,patRep,clase,fraccion,STyPS,Plaza_id,direccionArchivo")] Patrone patrone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patrone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            return View(patrone);
        }

        // GET: Patrones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            return View(patrone);
        }

        // POST: Patrones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patrone patrone = db.Patrones.Find(id);
            db.Patrones.Remove(patrone);
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
