using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using SUAMVC.Models;
using System.IO;

namespace SUAMVC.Controllers
{
    public class PagosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Pagos
        public ActionResult Index()
        {
            var pagos = db.Pagos.Include(p => p.Asegurado).Include(p => p.ResumenPago);
            return View(pagos.ToList());
        }

        public ActionResult UploadPagos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(String patronesId, String periodoId, String ejercicioId)
        {
            if (!String.IsNullOrEmpty(patronesId) && !String.IsNullOrEmpty(periodoId) && !String.IsNullOrEmpty(ejercicioId))
            {
                String periodo = ejercicioId.Trim() + periodoId.Trim();
                int patronTemp = int.Parse(patronesId);
                Patrone patron = db.Patrones.Find(patronTemp);
                String path = this.UploadFile(patron.direccionArchivo);


                if (!path.Equals(""))
                {
                    Boolean existe = false;
                    SUAHelper suaHelper = new SUAHelper(path);
                    ResumenPago resumenPago = new ResumenPago();

                    //Preparamos el query del resúmen
                    String sSQL = "SELECT * FROM Registro_02" +
                        "  WHERE Registro_Patronal = '" + patron.registro + "'" +
                        "    AND Periodo_Pago = '" + periodo + "'" +
                        "ORDER BY Registro_Patronal";

                    DataTable dt = suaHelper.ejecutarSQL(sSQL);

                    foreach (DataRow rows in dt.Rows)
                    {
                        resumenPago.ip = rows["IP"].ToString().Trim();
                        resumenPago.patronId = patron.Id;
                        resumenPago.rfc = rows["RFC"].ToString().Trim();
                        resumenPago.periodoPago = periodo;
                        resumenPago.mes = periodoId;
                        resumenPago.anno = ejercicioId;
                        resumenPago.folioSUA = rows["Folio_SUA"].ToString().Trim();
                        resumenPago.razonSocial = rows["Razon_Social"].ToString().Trim();
                        resumenPago.calleColonia = rows["Calle_Colonia"].ToString().Trim();
                        resumenPago.poblacion = rows["Poblacion"].ToString().Trim();
                        resumenPago.entidadFederativa = rows["Entidad_Federativa"].ToString().Trim();
                        resumenPago.codigoPostal = rows["CP"].ToString().Trim();
                        resumenPago.primaRT = rows["Prima_RT"].ToString().Trim();
                        resumenPago.fechaPrimaRT = rows["Fecha_Prima_RT"].ToString().Trim();
                        resumenPago.actividadEconomica = rows["Actividad_Economica"].ToString().Trim();
                        resumenPago.delegacionIMSS = rows["Delegacion_IMSS"].ToString().Trim();
                        resumenPago.subDelegacionIMMS = rows["SubDelegacion_IMSS"].ToString().Trim();
                        resumenPago.zonaEconomica = rows["Zona_Economica"].ToString().Trim();
                        resumenPago.convenioReembolso = rows["Convenio_Reembolso"].ToString().Trim();
                        resumenPago.tipoCotizacion = rows["Tipo_Cotizacion"].ToString().Trim();
                        resumenPago.cotizantes = rows["Cotizantes"].ToString().Trim();
                        resumenPago.apoPat = rows["Apo_Pat"].ToString().Trim();
                        resumenPago.delSubDel = rows["Del_Subdel"].ToString().Trim();

                        existe = true;
                        db.ResumenPagoes.Add(resumenPago);
                        db.SaveChanges();
                    }

                    if (existe)
                    {

                        sSQL = "SELECT * FROM Registro_03" +
                               "  ORDER BY NSS";

                        DataTable dt2 = suaHelper.ejecutarSQL(sSQL);

                        foreach (DataRow rows in dt2.Rows)
                        {
                            Pago pago = new Pago();
                            Asegurado asegurado = new Asegurado();

                            if (!String.IsNullOrEmpty(rows["NSS"].ToString().Trim()))
                            {
                                String nss = rows["NSS"].ToString().Trim();

                                asegurado = (from s in db.Asegurados
                                             where s.PatroneId.Equals(patron.Id)
                                                && s.numeroAfiliacion.Equals(nss)
                                             select s).FirstOrDefault();

                                pago.trabajadorId = asegurado.id;
                                pago.resumenPagoId = resumenPago.id;

                                pago.ip = rows["IP"].ToString().Trim();
                                pago.NSS = rows["NSS"].ToString().Trim();
                                pago.RFC = rows["RFC"].ToString().Trim();
                                pago.CURP = rows["CURP"].ToString().Trim();
                                pago.creditoInfonavit = rows["Credito_Infonavit"].ToString().Trim();
                                pago.fid = rows["FID"].ToString().Trim();
                                pago.trabajador = rows["Trabajador"].ToString().Trim();
                                pago.sdi = (!String.IsNullOrEmpty(rows["sdi"].ToString().Trim())) ? Decimal.Parse(rows["sdi"].ToString().Trim()) : 0;
                                pago.tipoTrabajador = rows["Tipo_Trabajador"].ToString().Trim();
                                pago.jornadaSemanaReducida = rows["Jornada_Semana_Reducida"].ToString().Trim();
                                pago.diasCotizadosMes = (!String.IsNullOrEmpty(rows["Dias_Cotizados_Mes"].ToString().Trim())) ? int.Parse(rows["Dias_Cotizados_Mes"].ToString().Trim()) : 0;
                                pago.diasIncapacidad = (!String.IsNullOrEmpty(rows["Dias_Incapacidad"].ToString().Trim())) ? int.Parse(rows["Dias_Incapacidad"].ToString().Trim()) : 0;
                                pago.diasAusentismo = (!String.IsNullOrEmpty(rows["Dias_Ausentismo"].ToString().Trim())) ? int.Parse(rows["Dias_Ausentismo"].ToString().Trim()) : 0;
                                pago.cuotaFija = (!String.IsNullOrEmpty(rows["Cuota_Fija"].ToString().Trim())) ? Decimal.Parse(rows["Cuota_Fija"].ToString().Trim()) : 0;
                                pago.cuotaExcedente = (!String.IsNullOrEmpty(rows["Cuota_Excedente"].ToString().Trim())) ? Decimal.Parse(rows["Cuota_Excedente"].ToString().Trim()) : 0;
                                pago.prestacionesDinero = (!String.IsNullOrEmpty(rows["Prestaciones_Dinero"].ToString().Trim())) ? Decimal.Parse(rows["Prestaciones_Dinero"].ToString().Trim()) : 0;
                                pago.gastosMedicosPensionado = (!String.IsNullOrEmpty(rows["Gastos_Medicos_Pensionados"].ToString().Trim())) ? Decimal.Parse(rows["Gastos_Medicos_Pensionados"].ToString().Trim()) : 0;
                                pago.riesgoTrabajo = (!String.IsNullOrEmpty(rows["Riesgo_Trabajo"].ToString().Trim())) ? Decimal.Parse(rows["Riesgo_Trabajo"].ToString().Trim()) : 0;
                                pago.invalidezVida = (!String.IsNullOrEmpty(rows["Invalidez_Vida"].ToString().Trim())) ? Decimal.Parse(rows["Invalidez_Vida"].ToString().Trim()) : 0;
                                pago.guarderias = (!String.IsNullOrEmpty(rows["Guarderias"].ToString().Trim())) ? Decimal.Parse(rows["Guarderias"].ToString().Trim()) : 0;
                                pago.actRecargosIMSS = rows["Act_Recargos_IMSS"].ToString().Trim();
                                pago.diasCotizadosBimestre = (!String.IsNullOrEmpty(rows["Dias_Cotizados_Bimestre"].ToString().Trim())) ? int.Parse(rows["Dias_Cotizados_Bimestre"].ToString().Trim()) : 0;
                                pago.diasIncapacidadBimestre = (!String.IsNullOrEmpty(rows["Dias_Incapacidad_Bim"].ToString().Trim())) ? int.Parse(rows["Dias_Incapacidad_Bim"].ToString().Trim()) : 0;
                                pago.diasAusentismoBimestre = (!String.IsNullOrEmpty(rows["Dias_Ausentismo_Bim"].ToString().Trim())) ? int.Parse(rows["Dias_Ausentismo_Bim"].ToString().Trim()) : 0;
                                pago.retiro = (!String.IsNullOrEmpty(rows["Retiro"].ToString().Trim())) ? Decimal.Parse(rows["Retiro"].ToString().Trim()) : 0;
                                pago.actRecargosRetiro = rows["Act_Recargos_Retiro"].ToString().Trim();
                                pago.cesantiaVejezPatronal = (!String.IsNullOrEmpty(rows["Cesantia_Vejez_Patronal"].ToString().Trim())) ? Decimal.Parse(rows["Cesantia_Vejez_Patronal"].ToString().Trim()) : 0;
                                pago.cesantiaVejezObrera = (!String.IsNullOrEmpty(rows["Cesantia_Vejez_Obrera"].ToString().Trim())) ? Decimal.Parse(rows["Cesantia_Vejez_Obrera"].ToString().Trim()) : 0;
                                pago.actRecargosCyV = (!String.IsNullOrEmpty(rows["Act_Recargos_CyV"].ToString().Trim())) ? Decimal.Parse(rows["Act_Recargos_CyV"].ToString().Trim()) : 0;
                                pago.aportacionVoluntaria = (!String.IsNullOrEmpty(rows["Aportacion_Voluntaria"].ToString().Trim())) ? Decimal.Parse(rows["Aportacion_Voluntaria"].ToString().Trim()) : 0;
                                pago.aportacionComp = (!String.IsNullOrEmpty(rows["Aportacion_Comp"].ToString().Trim())) ? Decimal.Parse(rows["Aportacion_Comp"].ToString().Trim()) : 0;
                                pago.aportacionPatronal = (!String.IsNullOrEmpty(rows["Aportacion_Patronal"].ToString().Trim())) ? Decimal.Parse(rows["Aportacion_Patronal"].ToString().Trim()) : 0;
                                pago.amortizacion = (!String.IsNullOrEmpty(rows["Amortizacion"].ToString().Trim())) ? Decimal.Parse(rows["Amortizacion"].ToString().Trim()) : 0;
                                pago.actIMSS = (!String.IsNullOrEmpty(rows["Act_IMSS"].ToString().Trim())) ? Decimal.Parse(rows["Act_IMSS"].ToString().Trim()) : 0;
                                pago.recIMSS = (!String.IsNullOrEmpty(rows["Rec_IMSS"].ToString().Trim())) ? Decimal.Parse(rows["Rec_IMSS"].ToString().Trim()) : 0;
                                pago.actRetiro = (!String.IsNullOrEmpty(rows["Act_Retiro"].ToString().Trim())) ? Decimal.Parse(rows["Act_Retiro"].ToString().Trim()) : 0;
                                pago.actCesObr = (!String.IsNullOrEmpty(rows["Act_CesObr"].ToString().Trim())) ? Decimal.Parse(rows["Act_CesObr"].ToString().Trim()) : 0;
                                pago.cuotaExcObr = (!String.IsNullOrEmpty(rows["Cuota_ExcObr"].ToString().Trim())) ? Decimal.Parse(rows["Cuota_ExcObr"].ToString().Trim()) : 0;
                                pago.cuotaPdObr = (!String.IsNullOrEmpty(rows["Cuota_PdObr"].ToString().Trim())) ? Decimal.Parse(rows["Cuota_PdObr"].ToString().Trim()) : 0;
                                pago.cuotaGmpObr = (!String.IsNullOrEmpty(rows["Cuota_GmpObr"].ToString().Trim())) ? Decimal.Parse(rows["Cuota_GmpObr"].ToString().Trim()) : 0;
                                pago.cuotaIvObr = (!String.IsNullOrEmpty(rows["Cuota_IvObr"].ToString().Trim())) ? Decimal.Parse(rows["Cuota_IvObr"].ToString().Trim()) : 0;
                                pago.actPatIMSS = (!String.IsNullOrEmpty(rows["ActPat_IMSS"].ToString().Trim())) ? Decimal.Parse(rows["ActPat_IMSS"].ToString().Trim()) : 0;
                                pago.recPatIMSS = (!String.IsNullOrEmpty(rows["RecPat_IMSS"].ToString().Trim())) ? Decimal.Parse(rows["RecPat_IMSS"].ToString().Trim()) : 0;
                                pago.actObrIMSS = (!String.IsNullOrEmpty(rows["ActObr_IMSS"].ToString().Trim())) ? Decimal.Parse(rows["ActObr_IMSS"].ToString().Trim()) : 0;
                                pago.recObrIMSS = (!String.IsNullOrEmpty(rows["RecObr_IMSS"].ToString().Trim())) ? Decimal.Parse(rows["RecObr_IMSS"].ToString().Trim()) : 0;

                                //Guardamos el pago.
                                db.Pagos.Add(pago);
                                db.SaveChanges();
                            }//Contiene información de asegurado valida?
                        }

                    }
                }
            }
            return RedirectToAction("UploadPagos");
        }

        public String UploadFile(String subFolder)
        {
            String path = "";
            ParametrosHelper parameterHelper = new ParametrosHelper();
            Parametro rutaParameter = parameterHelper.getParameterByKey("SUARUTA");

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {

                    if (!subFolder.Equals(""))
                    {
                        path = Path.Combine(rutaParameter.valorString.Trim(), subFolder);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                    }
                    else
                    {
                        path = rutaParameter.valorString.Trim();
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/App_LocalResources/"), fileName);
                    var pathFinal = Path.Combine(path, fileName);
                    file.SaveAs(pathFinal);
                    ViewBag.dbUploaded = true;
                    TempData["error"] = false;
                    TempData["viewMessage"] = "Se ha realizado la actualización con exito!";
                }
            }

            return path;
        }

        // GET: Pagos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pagos.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // GET: Pagos/Create
        public ActionResult Create()
        {
            ViewBag.trabajadorId = new SelectList(db.Asegurados, "id", "numeroAfiliacion");
            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip");
            return View();
        }

        // POST: Pagos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,resumenPagoId,ip,NSS,RFC,CURP,creditoInfonavit,fid,trabajador,sdi,tipoTrabajador,jornadaSemanaReducida,diasCotizadosMes,diasIncapacidad,diasAusentismo,cuotaFija,cuotaExcedente,prestacionesDinero,gastosMedicosPensionado,riesgoTrabajo,invalidezVida,guarderias,actRecargosIMSS,diasCotizadosBimestre,diasIncapacidadBimestre,diasAusentismoBimestre,retiro,actRecargosRetiro,cesantiaVejezPatronal,cesantiaVejezObrera,actRecargosCyV,aportacionVoluntaria,aportacionComp,aportacionPatronal,amortizacion,actIMSS,recIMSS,actRetiro,recRetiro,actCesPat,recCesPat,actCesObr,recCesObr,cuotaExcObr,cuotaPdObr,cuotaGmpObr,cuotaIvObr,actPatIMSS,recPatIMSS,actObrIMSS,recObrIMSS,trabajadorId,anoPago,mesPago")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Pagos.Add(pago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.trabajadorId = new SelectList(db.Asegurados, "id", "numeroAfiliacion", pago.trabajadorId);
            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip", pago.resumenPagoId);
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pagos.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            ViewBag.trabajadorId = new SelectList(db.Asegurados, "id", "numeroAfiliacion", pago.trabajadorId);
            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip", pago.resumenPagoId);
            return View(pago);
        }

        // POST: Pagos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,resumenPagoId,ip,NSS,RFC,CURP,creditoInfonavit,fid,trabajador,sdi,tipoTrabajador,jornadaSemanaReducida,diasCotizadosMes,diasIncapacidad,diasAusentismo,cuotaFija,cuotaExcedente,prestacionesDinero,gastosMedicosPensionado,riesgoTrabajo,invalidezVida,guarderias,actRecargosIMSS,diasCotizadosBimestre,diasIncapacidadBimestre,diasAusentismoBimestre,retiro,actRecargosRetiro,cesantiaVejezPatronal,cesantiaVejezObrera,actRecargosCyV,aportacionVoluntaria,aportacionComp,aportacionPatronal,amortizacion,actIMSS,recIMSS,actRetiro,recRetiro,actCesPat,recCesPat,actCesObr,recCesObr,cuotaExcObr,cuotaPdObr,cuotaGmpObr,cuotaIvObr,actPatIMSS,recPatIMSS,actObrIMSS,recObrIMSS,trabajadorId,anoPago,mesPago")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.trabajadorId = new SelectList(db.Asegurados, "id", "numeroAfiliacion", pago.trabajadorId);
            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip", pago.resumenPagoId);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pagos.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pago pago = db.Pagos.Find(id);
            db.Pagos.Remove(pago);
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
