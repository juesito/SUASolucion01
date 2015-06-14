using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using System.Text;
using PagedList;
using System.IO;
using System.Web.Helpers;
using SUAMVC.Code52.i18n;

namespace SUAMVC.Controllers
{
    public class AseguradosController : Controller
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

        // GET: Aseguradoes
        public ActionResult Index(String plazasId, String patronesId, String clientesId,
            String gruposId, String currentPlaza, String currentPatron, String currentCliente,
            String currentGrupo, String opcion, String valor, String statusId, int page = 1, String sortOrder = null,
            String lastSortOrder = null)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            setVariables(plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId);

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var gruposAsignados = (from s in db.Grupos
                                   join cli in db.Clientes on s.Id equals cli.Grupo_id
                                   join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                   where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                   orderby s.claveGrupo
                                   select s.Id);

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

            //Query principal
            var asegurados = from s in db.Asegurados
                             join cli in db.Clientes on s.ClienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Cliente.Id) &&
                                   patronesAsignados.Contains(s.PatroneId) &&
                                   gruposAsignados.Contains(s.Cliente.Grupo_id)
                             select s;

            //Comenzamos los filtros
            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int idPlaza = int.Parse(plazasId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int idPatron = int.Parse(patronesId.Trim());
                asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int idCliente = int.Parse(clientesId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int idGrupo = int.Parse(gruposId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {

                switch (opcion)
                {
                    case "1":
                        asegurados = asegurados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        asegurados = asegurados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        asegurados = asegurados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "4":
                        asegurados = asegurados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "5":
                        asegurados = asegurados.Where(s => s.nombre.Contains(valor));
                        break;
                    case "6":
                        asegurados = asegurados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "7":
                        asegurados = asegurados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "8":
                        asegurados = asegurados.Where(s => s.salarioImss.ToString().Contains(valor.Trim()));
                        break;
                    case "9":
                        asegurados = asegurados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "10":
                        asegurados = asegurados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "11":
                        asegurados = asegurados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "12":
                        asegurados = asegurados.Where(s => s.Cliente.Plaza.cvecorta.Contains(valor));
                        break;
                    case "13":
                        asegurados = asegurados.Where(s => s.extranjero.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.statusId = statusId;
                    asegurados = asegurados.Where(s => !s.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.statusId = statusId;
                    asegurados = asegurados.Where(s => s.fechaBaja.HasValue);
                }
            }

            ViewBag.activos = asegurados.Where(s => !s.fechaBaja.HasValue).Count();
            ViewBag.registros = asegurados.Count();

            asegurados = asegurados.OrderBy(s => s.nombreTemporal);

            return View(asegurados.ToList());
        }

        // GET: Aseguradoes/Details/5
        public ActionResult Details(int id)
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
            return View(asegurado);
        }

        public ActionResult ViewAttachment(int id, String option, String carga)
        {
            if (carga != null)
            {
                Asegurado asegurado = db.Asegurados.Find(id);
                var movtosTemp = db.Movimientos.Where(x => x.aseguradoId == id
                                 && x.tipo.Equals(option)).OrderByDescending(x => x.fechaTransaccion).ToList();

                Movimiento movto = new Movimiento();
                if (movtosTemp != null && movtosTemp.Count > 0)
                {
                    foreach (var movtosItem in movtosTemp)
                    {
                        movto = movtosItem;
                        break;
                    }//Definimos los valores para la plaza

                    var fileName = "C:\\SUA\\Asegurados\\" + asegurado.numeroAfiliacion + "\\" + option + "\\" + movto.nombreArchivo.Trim();

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
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Aseguradoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Aseguradoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registroaseguradoal,numeroAfiliacion,CURP,RFC,nombre,salarioImss,salarioInfo,fechaAlta,fechaBaja,tipoTrabajo,semanaJornada,paginaInfo,tipoDescuento,valorDescuento,claveUbicacion,nombreTemporal,fechaDescuento,finDescuento,articulo33,salarioArticulo33,trapeniv,estado,claveMunicipio")] Asegurado asegurado)
        {
            if (ModelState.IsValid)
            {
                db.Asegurados.Add(asegurado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(asegurado);
        }

        // GET: Aseguradoes/Edit/5
        public ActionResult Edit(int id)
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
            return View(asegurado);
        }

        // POST: Aseguradoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registroaseguradoal,numeroAfiliacion,CURP,RFC,nombre,salarioImss,salarioInfo,fechaAlta,fechaBaja,tipoTrabajo,semanaJornada,paginaInfo,tipoDescuento,valorDescuento,claveUbicacion,nombreTemporal,fechaDescuento,finDescuento,articulo33,salarioArticulo33,trapeniv,estado,claveMunicipio")] Asegurado asegurado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asegurado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asegurado);
        }

        // GET: Aseguradoes/Delete/5
        public ActionResult DeleteMov(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            if (asegurado == null)
            {
                return HttpNotFound();
            }
            return View(asegurado);
        }

        // POST: Aseguradoes/Delete/5
        [HttpPost, ActionName("DeleteMov")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            db.MovimientosAseguradoes.Remove(asegurado);
            db.SaveChanges();
            return RedirectToAction("Index");
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

            List<Asegurado> allCust = new List<Asegurado>();

            var asegurados = from s in db.Asegurados
                             join cli in db.Clientes on s.ClienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Cliente.Id) &&
                                   patronesAsignados.Contains(s.PatroneId)
                             select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int idPlaza = int.Parse(plazasId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int idPatron = int.Parse(patronesId.Trim());
                asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int idCliente = int.Parse(clientesId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int idGrupo = int.Parse(gruposId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                @ViewBag.opBuscador = opcion;
                @ViewBag.valBuscador = valor;
                TempData["buscador"] = "0";

                switch (opcion)
                {
                    case "1":
                        asegurados = asegurados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        asegurados = asegurados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        asegurados = asegurados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "4":
                        asegurados = asegurados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "5":
                        asegurados = asegurados.Where(s => s.nombre.Contains(valor));
                        break;
                    case "6":
                        asegurados = asegurados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "7":
                        asegurados = asegurados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "8":
                        asegurados = asegurados.Where(s => s.salarioImss.ToString().Contains(valor.Trim()));
                        break;
                    case "9":
                        asegurados = asegurados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "10":
                        asegurados = asegurados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "11":
                        asegurados = asegurados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "12":
                        asegurados = asegurados.Where(s => s.Cliente.Plaza.cvecorta.Contains(valor));
                        break;
                    case "13":
                        asegurados = asegurados.Where(s => s.extranjero.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.statusId = statusId;
                    asegurados = asegurados.Where(s => !s.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.statusId = statusId;
                    asegurados = asegurados.Where(s => s.fechaBaja.HasValue);
                }
            }

            allCust = asegurados.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid.Column("Patrone.registro", "Registro "));
            gridColumns.Add(grid.Column("numeroAfiliacion", "Numero Afiliacion"));
            gridColumns.Add(grid.Column("curp", "CURP"));
            gridColumns.Add(grid.Column("rfc", "RFC"));
            gridColumns.Add(grid.Column("nombreTemporal", "Nombre"));
            gridColumns.Add(grid.Column("fechaAlta", "Fecha Alta", format: (item) => String.Format("{0:yyyy-MM-dd}", item.fechaAlta)));
            gridColumns.Add(grid.Column("fechaBaja", "Fecha Baja", format: (item) => item.fechaBaja != null ? String.Format("{0:yyyy-MM-dd}", item.fechaBaja) : String.Empty));
            gridColumns.Add(grid.Column("salarioImss", "Salario IMSS"));
            gridColumns.Add(grid.Column("Cliente.claveCliente", "Ubicación"));
            gridColumns.Add(grid.Column("Cliente.Grupos.nombreCorto", "Grupo"));
            gridColumns.Add(grid.Column("ocupacion", "Ocupación"));
            gridColumns.Add(grid.Column("Cliente.Plaza.cveCorta", "Plaza"));
            gridColumns.Add(grid.Column("extranjero", "Extranjero"));
            gridColumns.Add(grid.Column("fechaCreacion", "Fecha Creación", format: (item) => String.Format("{0:yyyy-MM-dd}", item.fechaCreacion)));
            gridColumns.Add(grid.Column("fechaModificacion", "Fecha Modificación", format: (item) => item.fechaModificacion != null ? String.Format("{0:yyyy-MM-dd}", item.fechaModificacion) : String.Empty));
            gridColumns.Add(grid.Column("alta", "Alta"));
            gridColumns.Add(grid.Column("baja", "Baja"));
            gridColumns.Add(grid.Column("modificacion", "Modificación"));
            gridColumns.Add(grid.Column("permanente", "Permanente"));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Asegurados-" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        // GET: Aseguradoes/Delete/5
        public ActionResult DeleteMovs(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            db.MovimientosAseguradoes.Remove(asegurado);
            db.SaveChanges();
            return View(asegurado);
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
            else
            {
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

