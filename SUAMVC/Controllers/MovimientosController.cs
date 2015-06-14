using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.IO;
using SUAMVC.Models;

namespace SUAMVC.Controllers
{
    public class MovimientosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Movimientos
        public ActionResult Index()
        {
            var movimientos = db.Movimientos.Include(m => m.Asegurado);
            return View(movimientos.ToList());
        }

        // GET: Movimientos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimientos.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // GET: Movimientos/Create
        public ActionResult Create()
        {
            ViewBag.aseguradoId = new SelectList(db.Asegurados, "id", "numeroAfiliacion");
            return View();
        }

        // POST: Movimientos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,aseguradoId,lote,fechaTransaccion,tipo,nombreArchivo")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                db.Movimientos.Add(movimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.aseguradoId = new SelectList(db.Asegurados, "id", "numeroAfiliacion", movimiento.aseguradoId);
            return View(movimiento);
        }

        // GET: Movimientos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimientos.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.aseguradoId = new SelectList(db.Asegurados, "id", "numeroAfiliacion", movimiento.aseguradoId);
            return View(movimiento);
        }

        // POST: Movimientos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,aseguradoId,lote,fechaTransaccion,tipo,nombreArchivo")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.aseguradoId = new SelectList(db.Asegurados, "id", "numeroAfiliacion", movimiento.aseguradoId);
            return View(movimiento);
        }

        // GET: Movimientos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimientos.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // POST: Movimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movimiento movimiento = db.Movimientos.Find(id);
            db.Movimientos.Remove(movimiento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadPDFFile([Bind(Include = "id,aseguradoId,lote,fechaTransaccion,tipo,nombreArchivo")] Movimiento movimiento, String aseguradoId)
        {
            if (!aseguradoId.Equals(""))
            {
                Asegurado asegurado = db.Asegurados.Find(Int32.Parse(aseguradoId));
                if (Request.Files.Count > 0)
                {
                    movimiento.aseguradoId = Int32.Parse(aseguradoId);
                    movimiento.Asegurado = asegurado;

                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        String path = "C:\\SUA\\Asegurados\\" + asegurado.numeroAfiliacion + "\\" + movimiento.tipo + "\\"; //Path.Combine("C:\\SUA\\", uploadModel.subFolder);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        //var fileName = Path.GetFileName(file.FileName);
                        var fileName = "Acuse" + Path.GetExtension(file.FileName.Trim());
                        var pathFinal = Path.Combine(path, fileName);
                        file.SaveAs(pathFinal);

                        movimiento.nombreArchivo = fileName;
                        //Move();
                        String answer = movimiento.tipo;
                        ViewBag.dbUploaded = true;

                        //Validamos la acción realizada
                        if (answer.Equals("A"))
                        {
                            asegurado.alta = "S";
                        }
                        else if (answer.Equals("B"))
                        {
                            asegurado.baja = "S";
                        }
                        else if (answer.Equals("M"))
                        {
                            asegurado.modificacion = "S";
                        }
                        else
                        {
                            asegurado.permanente = "S";
                        }

                        db.Entry(asegurado).State = EntityState.Modified;
                        db.Movimientos.Add(movimiento);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "Asegurados");
        }

        public ActionResult UploadFile(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asegurado asegurado = db.Asegurados.Find(id);
            if (asegurado == null)
            {
                return HttpNotFound();
            }

            Movimiento movimiento = new Movimiento();
            DateTime date = DateTime.Today;
            movimiento.Asegurado = asegurado;
            movimiento.aseguradoId = id;
            movimiento.fechaTransaccion = date;
            movimiento.tipo = "A";
            ViewBag.Asegurado = asegurado;


            return View(movimiento);
        }

        [HttpPost]
        public ActionResult UploadPDFFiles(AseguradoMovimientosModel model, String[] ids)
        {

            DateTime date = DateTime.Now;
            int[] id = null;
            if (ids != null)
            {
                id = new int[ids.Length];
                int j = 0;
                foreach (string i in ids)
                {
                    int.TryParse(i, out id[j++]);
                }
            }

            if (id != null && id.Length > 0)
            {
                List<Asegurado> allAsegurado = new List<Asegurado>();
                allAsegurado = db.Asegurados.Where(a => id.Contains(a.id)).ToList();

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    foreach (Asegurado asegurado in allAsegurado)
                    {

                        if (file != null && file.ContentLength > 0)
                        {

                            String path = "C:\\SUA\\Asegurados\\" + asegurado.numeroAfiliacion + "\\" + model.movimiento.tipo + "\\";
                            if (!System.IO.File.Exists(path))
                            {
                                System.IO.Directory.CreateDirectory(path);
                            }

                            //var fileName = Path.GetFileName(file.FileName);
                            var fileName = "Acuse" + Path.GetExtension(file.FileName.Trim());
                            var pathFinal = Path.Combine(path, fileName);
                            file.SaveAs(pathFinal);

                            model.movimiento.nombreArchivo = fileName;

                            String answer = model.movimiento.tipo;

                            //Validamos la acción realizada
                            if (answer.Equals("A"))
                            {
                                asegurado.alta = "S";
                            }
                            else if (answer.Equals("B"))
                            {
                                asegurado.baja = "S";
                            }
                            else if (answer.Equals("M"))
                            {
                                asegurado.modificacion = "S";
                            }
                            else
                            {
                                asegurado.permanente = "S";
                            }
                        }

                        model.movimiento.aseguradoId = asegurado.id;
                        model.movimiento.Asegurado = asegurado;

                        db.Entry(asegurado).State = EntityState.Modified;
                        db.Movimientos.Add(model.movimiento);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "Asegurados");
        }


        [HttpPost, ActionName("UploadFiles")]
        public ActionResult UploadFiles(String[] ids)
        {

            AseguradoMovimientosModel movimientoAsegurado = new AseguradoMovimientosModel();
            movimientoAsegurado.allAsegurado = new List<Asegurado>();

            if (ids != null)
            {
                List<Asegurado> allAsegurados = new List<Asegurado>();
                DateTime date = DateTime.Today;

                foreach (String aseguradoId in ids)
                {

                    int idTemp = int.Parse(aseguradoId);
                    var asegurado = db.Asegurados.Find(idTemp);
                    allAsegurados.Add(asegurado);

                }

                movimientoAsegurado.allAsegurado = allAsegurados;
                movimientoAsegurado.movimiento = new Movimiento();
                movimientoAsegurado.movimiento.fechaTransaccion = date;
                movimientoAsegurado.movimiento.tipo = "A";

                return View(movimientoAsegurado);
            }
            else
            {
                return RedirectToAction("Index", "Asegurados");
            }

        }


        public ActionResult UploadFileAcre(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }

            Movimiento movimiento = new Movimiento();
            DateTime date = DateTime.Now;
            movimiento.Acreditado = acreditado;
            movimiento.acreditadoId = id;
            movimiento.fechaTransaccion = date;
            movimiento.tipo = "A";
            ViewBag.Acreditado = acreditado;


            return View(movimiento);
        }

        [HttpPost]
        public ActionResult UploadPDFFileAcre([Bind(Include = "id,acreditadoId,lote,fechaTransaccion,tipo,nombreArchivo")] Movimiento movimiento, String acreditadoId)
        {
            if (!acreditadoId.Equals(""))
            {
                Acreditado acreditado = db.Acreditados.Find(Int32.Parse(acreditadoId));
                if (Request.Files.Count > 0)
                {
                    movimiento.acreditadoId = Int32.Parse(acreditadoId);
                    /*                   movimiento.Acreditado = acreditado;*/

                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        String path = "C:\\SUA\\Acreditados\\" + acreditado.numeroAfiliacion + "\\" + movimiento.tipo + "\\"; //Path.Combine("C:\\SUA\\", uploadModel.subFolder);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        //var fileName = Path.GetFileName(file.FileName);
                        var fileName = "Acuse" + Path.GetExtension(file.FileName.Trim());
                        var pathFinal = Path.Combine(path, fileName);
                        file.SaveAs(pathFinal);

                        movimiento.nombreArchivo = fileName;
                        //Move();
                        String answer = movimiento.tipo;
                        ViewBag.dbUploaded = true;

                        //Validamos la acción realizada
                        if (answer.Equals("A"))
                        {
                            acreditado.alta = "S";
                        }

                        db.Entry(acreditado).State = EntityState.Modified;
                        db.Movimientos.Add(movimiento);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "Acreditados");
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

