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
    public class ClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Clientes
        public ActionResult Index(String plazasId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientes = from p in db.Clientes
                           where plazasAsignadas.Contains(p.Plaza.id)
                           select p;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaIdTemp = int.Parse(plazasId);
                clientes = clientes.Where(p => p.Plaza.id.Equals(plazaIdTemp));
            }

            ViewBag.PlazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId
                                               where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }).Distinct(), "id", "FullName");
            return View(clientes.ToList().OrderBy(p => p.descripcion));
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion");
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto");
            return View();
        }

        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,claveCliente,claveSua,rfc,descripcion,Plaza_id,Grupo_id,ejecutivo")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                Cliente clienteTemp = db.Clientes.Where(p => p.rfc.Equals(cliente.rfc.Trim())).FirstOrDefault();

                if (clienteTemp == null)
                {
                    cliente.descripcion = cliente.descripcion.ToUpper();
                    cliente.rfc = cliente.rfc.ToUpper();
                    cliente.ejecutivo = cliente.ejecutivo.ToUpper();
                    db.Clientes.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion", cliente.Plaza_id);
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto", cliente.Grupo_id);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion", cliente.Plaza_id);
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto", cliente.Grupo_id);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,claveCliente,claveSua,rfc,descripcion,Plaza_id,Grupo_id,ejecutivo")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.descripcion = cliente.descripcion.ToUpper();
                cliente.rfc = cliente.rfc.ToUpper();
                cliente.ejecutivo = cliente.ejecutivo.ToUpper();
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion", cliente.Plaza_id);
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto", cliente.Grupo_id);
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Clientes.Find(id);
            db.Clientes.Remove(cliente);
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
