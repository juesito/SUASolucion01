using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Text;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using PagedList;
using System.IO;
using System.Web.Helpers;

namespace SUAMVC.Controllers
{
    public class AcreditadosController : Controller
    {
        private suaEntities db = new suaEntities();

        private void setVariables(String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId)
        {
            if (!String.IsNullOrEmpty(plazasId))
            {
                ViewBag.pzaId = plazasId;
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                ViewBag.patId = patronesId;
            }
            if (!String.IsNullOrEmpty(clientesId))
            {
                ViewBag.cteId = clientesId;
            }
            if (!String.IsNullOrEmpty(gruposId))
            {
                ViewBag.gpoId = gruposId;
            }
            if (!String.IsNullOrEmpty(opcion))
            {
                ViewBag.opBuscador = opcion;
            }
            if (!String.IsNullOrEmpty(valor))
            {
                ViewBag.valBuscador = valor;
            }
            if (statusId != null)
            {
                ViewBag.statusId = statusId;
            }

        }
        // GET: Acreditados
        public ActionResult Index(String plazasId, String patronesId, String clientesId, 
            String gruposId, String currentPlaza, String currentPatron, String currentCliente, 
            String currentGrupo, String opcion, String valor, String statusId, int page = 1)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            setVariables(plazasId, patronesId, clientesId, gruposId,  opcion,  valor,  statusId);

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);
            
            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId
                                               where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }).Distinct(), "id", "FullName");

            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(user.Id)
                                                 orderby s.registro
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                 }).Distinct(), "id", "FullName", null);
            
            ViewBag.clientesId = new SelectList((from s in db.Clientes.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                                 orderby s.descripcion
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FUllName = s.claveCliente + " - " + s.descripcion
                                                 }).Distinct(), "id", "FullName");
         
            ViewBag.gruposId = new SelectList((from s in db.Grupos.ToList()
                                               join cli in db.Clientes on s.Id equals cli.Grupo_id
                                               join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                               where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                               orderby s.claveGrupo
                                               select new
                                               {
                                                   id = s.Id,
                                                   FUllName = s.claveGrupo + " - " + s.nombreCorto
                                               }).Distinct(), "id", "FullName");


            var acreditados = from s in db.Acreditados
                              join cli in db.Clientes on s.clienteId equals cli.Id
                              where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                    clientesAsignados.Contains(s.Cliente.Id) &&
                                    patronesAsignados.Contains(s.PatroneId)
                             select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int idPlaza = int.Parse(plazasId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int idPatron = int.Parse(patronesId.Trim());
                acreditados = acreditados.Where(s => s.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                int idCliente = int.Parse(clientesId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                int idGrupo = int.Parse(gruposId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                //TempData["buscador"] = "0";
                switch (opcion)
                {
                    case "1":
                        acreditados = acreditados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        acreditados = acreditados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        acreditados = acreditados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "4":
                        acreditados = acreditados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "5":
                        acreditados = acreditados.Where(s => s.nombreCompleto.Contains(valor));
                        break;
                    case "6":
                        acreditados = acreditados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "7":
                        acreditados = acreditados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "8":
                        acreditados = acreditados.Where(s => s.sdi.ToString().Contains(valor));
                        break;
                    case "9":
                        acreditados = acreditados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "10":
                        acreditados = acreditados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "11":
                        acreditados = acreditados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "12":
                        acreditados = acreditados.Where(s => s.Cliente.Plaza.cvecorta.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    acreditados = acreditados.Where(s => !s.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    acreditados = acreditados.Where(s => s.fechaBaja.HasValue);
                }
            }

            ViewBag.activos = acreditados.Where(s => !s.fechaBaja.HasValue).Count();
            ViewBag.registros = acreditados.Count();

            return View(acreditados.ToList());
        }

        public ActionResult UploadFile(int id)
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

            return View(acreditado);
        }

        [HttpPost]
        public ActionResult UploadPDFFile([Bind(Include = "id")] Acreditado acreditado, String answer)
        {
            if (acreditado.id > 0)
            {
                acreditado = db.Acreditados.Find(acreditado.id);
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        String path = "C:\\SUA\\Acreditados\\" + acreditado.numeroAfiliacion.Trim() + "\\" + answer + "\\"; //Path.Combine("C:\\SUA\\", uploadModel.subFolder);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        //var fileName = Path.GetFileName(file.FileName);
                        var fileName = answer + "-" + acreditado.numeroAfiliacion.Trim() + ".pdf";
                        var pathFinal = Path.Combine(path, fileName);
                        file.SaveAs(pathFinal);
                        //Move();

                        ViewBag.dbUploaded = true;

                        //Validamos la acción realizada
                        if (answer.Equals("Alta"))
                        {
                            acreditado.alta = "S";
                        }
                        else if (answer.Equals("Baja"))
                        {
                            acreditado.baja = "S";
                        }
                        else if (answer.Equals("Modificacion"))
                        {
                            acreditado.modificacion = "S";
                        }
                        else
                        {
                            acreditado.permanente = "S";
                        }

                        db.Acreditados.Add(acreditado);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult ViewAttachment(int id, String option, String carga)
        {
            if (carga != null)
            {
                Acreditado acreditado = db.Acreditados.Find(id);
                var movtosTemp = db.Movimientos.Where(x => x.acreditadoId == id
                                 && x.tipo.Equals(option)).OrderByDescending(x => x.fechaTransaccion).ToList(); 

                Movimiento movto = new Movimiento();
                if (movtosTemp != null)
                {
                    foreach (var movtosItem in movtosTemp)
                    {
                        movto = movtosItem;
                        break;
                    }//Definimos los valores para la plaza
                }

                var fileName = "C:\\SUA\\Acreditados\\" + acreditado.numeroAfiliacion + "\\" + option + "\\" + movto.nombreArchivo.Trim();

                if (System.IO.File.Exists(fileName))
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open);

                    return File(fs, "application/pdf");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Acreditados/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // GET: Acreditados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Acreditados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registroPatronal,apellidoPaterno,apellidoMaterno,nombre,nombreCompleto,CURP,RFC,ubicacion,ocupacion,idGrupo,numeroAfiliacion,numeroCredito,fechaAlta,fechaBaja,fechaInicioDescuento,fechaFinDescuento,smdv,sdi,sd,vsm,porcentaje,cuotaFija,descuentoBimestral,descuentoMensual,descuentoSemanal,descuentoCatorcenal,descuentoQuincenal,descuentoVeintiochonal,descuentoDiario,idPlaza,acuseRetencion")] Acreditado acreditado)
        {
            if (ModelState.IsValid)
            {
                db.Acreditados.Add(acreditado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(acreditado);
        }

        // GET: Acreditados/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // POST: Acreditados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registroPatronal,apellidoPaterno,apellidoMaterno,nombre,nombreCompleto,CURP,RFC,ubicacion,ocupacion,idGrupo,numeroAfiliacion,numeroCredito,fechaAlta,fechaBaja,fechaInicioDescuento,fechaFinDescuento,smdv,sdi,sd,vsm,porcentaje,cuotaFija,descuentoBimestral,descuentoMensual,descuentoSemanal,descuentoCatorcenal,descuentoQuincenal,descuentoVeintiochonal,descuentoDiario,idPlaza,acuseRetencion")] Acreditado acreditado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acreditado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(acreditado);
        }

        // GET: Acreditados/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // POST: Acreditados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Acreditado acreditado = db.Acreditados.Find(id);
            db.Acreditados.Remove(acreditado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UploadData()
        {
            ViewBag.patronesId = new SelectList(db.Patrones, "registro", "nombre");
            ViewBag.clientesId = new SelectList(db.Clientes, "id", "descripcion");
            return View();
        }

        [HttpGet]
        public void GetExcel(String plazasId, String patronesId, String clientesId, 
            String gruposId, String opcion, String valor, String statusId)
        {

            Usuario user = Session["UsuarioData"] as Usuario;
            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            List<Acreditado> allCust = new List<Acreditado>();

            var acreditados = from s in db.Acreditados
                              join cli in db.Clientes on s.clienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Cliente.Id) &&
                                   patronesAsignados.Contains(s.PatroneId)
                             select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int idPlaza = int.Parse(plazasId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int idPatron = int.Parse(patronesId.Trim());
                acreditados = acreditados.Where(s => s.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int idCliente = int.Parse(clientesId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int idGrupo = int.Parse(gruposId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                @ViewBag.opBuscador = opcion;
                @ViewBag.valBuscador = valor;
                TempData["buscador"] = "0";
                switch (opcion)
                {
                    case "1":
                        acreditados = acreditados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        acreditados = acreditados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        acreditados = acreditados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "4":
                        acreditados = acreditados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "5":
                        acreditados = acreditados.Where(s => s.nombreCompleto.Contains(valor));
                        break;
                    case "6":
                        acreditados = acreditados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "7":
                        acreditados = acreditados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "8":
                        acreditados = acreditados.Where(s => s.sdi.ToString().Contains(valor));
                        break;
                    case "9":
                        acreditados = acreditados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "10":
                        acreditados = acreditados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "11":
                        acreditados = acreditados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "12":
                        acreditados = acreditados.Where(s => s.Cliente.Plaza.cvecorta.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.statusId = statusId;
                    acreditados = acreditados.Where(s => !s.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.statusId = statusId;
                    acreditados = acreditados.Where(s => s.fechaBaja.HasValue);
                }
            }

            allCust = acreditados.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);
            List<WebGridColumn> gridColumns = new List<WebGridColumn>();

            gridColumns.Add(grid.Column("Patrone.registro", "Registro"));
            gridColumns.Add(grid.Column("apellidoPaterno", "Apellido Paterno"));
            gridColumns.Add(grid.Column("apellidoMaterno", "Apellido Materno"));
            gridColumns.Add(grid.Column("nombre", "Nombre"));
            gridColumns.Add(grid.Column("nombreCompleto", "Nombre Completo"));
            gridColumns.Add(grid.Column("curp","CURP"));
            gridColumns.Add(grid.Column("rfc", "RFC"));
            gridColumns.Add(grid.Column("ocupacion", "Ocupación"));
            gridColumns.Add(grid.Column("idGrupo", "Grupo"));
            gridColumns.Add(grid.Column("numeroAfiliacion", "Numero Afiliación"));
            gridColumns.Add(grid.Column("numeroCredito", "Numero Credito"));
            gridColumns.Add(grid.Column("fechaAlta", "Alta", format: (item) => String.Format("{0:yyyy-MM-dd}", item.fechaAlta)));
            gridColumns.Add(grid.Column("fechaBaja", "Fecha Baja", format: (item) => item.fechaBaja != null ? String.Format("{0:yyyy-MM-dd}", item.fechaBaja) : String.Empty));
            gridColumns.Add(grid.Column("Cliente.claveCliente", "Cliente"));
            gridColumns.Add(grid.Column("fechaInicioDescuento", "Inicio descuento", format: (item) => String.Format("{0:yyyy-MM-dd}", item.fechaAlta)));
            gridColumns.Add(grid.Column("fechaFinDescuento", "Fin descuento", format: (item) => item.fechaFinDescuento != null ? String.Format("{0:yyyy-MM-dd}", item.fechaFinDescuento) : String.Empty));
            gridColumns.Add(grid.Column("smdv", "SMDV"));
            gridColumns.Add(grid.Column("sdi", "SDI"));
            gridColumns.Add(grid.Column("sd", "SD"));
            gridColumns.Add(grid.Column("vsm", "VSM"));
            gridColumns.Add(grid.Column("porcentaje", "Porcentaje"));
            gridColumns.Add(grid.Column("cuotaFija", "Cuota Fija"));
            gridColumns.Add(grid.Column("descuentoBimestral", "Descuento Bimestral"));
            gridColumns.Add(grid.Column("descuentoMensual", "Descuento Mensual"));
            gridColumns.Add(grid.Column("descuentoSemanal", "Descuento Semanal"));
            gridColumns.Add(grid.Column("descuentoCatorcenal", "Descuento Catorcenal"));
            gridColumns.Add(grid.Column("descuentoQuincenal", "Descuento Quincenal"));
            gridColumns.Add(grid.Column("descuentoVeintiochonal", "Descuento Veintiochonal"));
            gridColumns.Add(grid.Column("descuentoDiario", "Descuento Diario"));
            gridColumns.Add(grid.Column("Plaza.descripcion", "Plaza"));
            /*grid.Column("Cliente.descripcion", "Cliente/Ubicación"),*/

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Acreditados-" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        public ActionResult ActivaVariable(String buscador, String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId)
        {
            if (buscador != null)
            {
                if (!buscador.Equals("1"))
                {
                    TempData["buscador"] = "1";
                }
                else
                {
                    TempData["buscador"] = "0";
                }
            }
            else {
                TempData["buscador"] = "1";
            }
            return RedirectToAction("Index", new { plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId });
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
