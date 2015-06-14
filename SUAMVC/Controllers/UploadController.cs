using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SUAMVC.Controllers
{

    public class UploadController : Controller
    {

        private suaEntities db = new suaEntities();

        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    ParametrosHelper parameterHelper = new ParametrosHelper();
                    Parametro rutaParameter = parameterHelper.getParameterByKey("SUARUTA");
                    String path = rutaParameter.valorString.Trim(); 
                    if (!System.IO.File.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    var pathFinal = Path.Combine(path, fileName);
                    file.SaveAs(pathFinal);

                    if (RefreshBoss() == 0)
                    {
                        ViewBag.dbUploaded = false;
                    }
                    else {
                        ViewBag.dbUploaded = true;
                    }

                    
                }
            }

            return RedirectToAction("Index");

        }

        /**
         * Actualizamos los patrones 
         * 
         */
        
        public int RefreshBoss() {
            SUAHelper sua = null;
            int count = 0;
            Boolean isError = false;
            try
            {               
                //Realizamos la conexion
                sua = new SUAHelper();

                String sSQL = "SELECT REG_PAT, RFC_PAT, NOM_PAT, ACT_PAT, DOM_PAT, " +
                              "       MUN_PAT, CPP_PAT, ENT_PAT, TEL_PAT, REM_PAT, " +
                              "       ZON_PAT, DEL_PAT, CAR_ENT, NUM_DEL, CAR_DEL, " +
                              "       NUM_SUB, CAR_SUB, TIP_CON, CON_VEN, INI_AFIL," +
                              "       PAT_REP, CLASE  , FRACCION, STyPS  " +
                              "  FROM Patron   " +
                              "  ORDER BY REG_PAT ";

                //Ejecutamos nuestra consulta
                DataTable dt = sua.ejecutarSQL(sSQL);

                foreach (DataRow rows in dt.Rows)
                {
                    //Revisamos la existencia del registro

                    String patronDescripcion = rows["REG_PAT"].ToString();
                    Patrone patron = new Patrone();
                    if (!patronDescripcion.Equals(""))
                    {
                        var patronTemp = from b in db.Patrones
                                         where b.registro.Equals(patronDescripcion.Trim())
                                         select b;


                        if (patronTemp != null && patronTemp.Count() > 0)
                        {
                            foreach (var patronItem in patronTemp)
                            {
                                patron = patronItem;
                                break;
                            }//Definimos los valores para la plaza
                        }
                        else
                        {
                            patron.registro = "";
                        }

                    }


                    if (!patron.registro.Equals(""))
                    {
                        String plazaDescripcion = rows["CAR_ENT"].ToString();
                        if (!plazaDescripcion.Equals(""))
                        {
                            var plazaTemp = from b in db.Plazas
                                                 where b.descripcion.Equals(plazaDescripcion.Trim())
                                                 select b;

                            Plaza plaza = new Plaza();

                            if (plazaTemp.Count() > 0)
                            {
                                foreach (var plazaItem in plazaTemp)
                                {
                                    plaza.id = plazaItem.id;
                                    plaza.descripcion = plazaItem.descripcion;
                                    plaza.indicador = "P";
                                    break;
                                }//Definimos los valores para la plaza
                            }
                            else {
                                plaza.descripcion = plazaDescripcion.Trim();
                                plaza.indicador = "P";
                                db.Plazas.Add(plaza);
                                db.SaveChanges();

                            }//Ya existen datos con esta plaza?                            

                            //Modificamos los datos del patron existente
                            patron.telefono = rows["TEL_PAT"].ToString();
                            patron.domicilio = rows["DOM_PAT"].ToString();
                            patron.patRep = rows["PAT_REP"].ToString();
/*                          patron.registro = rows["REG_PAT"].ToString();
                            patron.rfc = rows["RFC_PAT"].ToString();
                            patron.nombre = rows["NOM_PAT"].ToString();
                            patron.actividad = rows["ACT_PAT"].ToString();
                            patron.municipio = rows["MUN_PAT"].ToString();
                            patron.codigoPostal = rows["CPP_PAT"].ToString();
                            patron.entidad = rows["ENT_PAT"].ToString();
                            patron.remision = ((Boolean.Parse(rows["REM_PAT"].ToString()) == true) ? "V" : "F");
                            patron.zona = rows["ZON_PAT"].ToString();
                            patron.delegacion = rows["DEL_PAT"].ToString();
                            patron.carEnt = rows["CAR_ENT"].ToString();
                            patron.numeroDelegacion = Int32.Parse(rows["NUM_DEL"].ToString());
                            patron.carDel = rows["CAR_DEL"].ToString();
                            patron.numSub = Int32.Parse(rows["NUM_SUB"].ToString());
                            patron.Plaza_id = plaza.id;
                            patron.tipoConvenio = Decimal.Parse(rows["TIP_CON"].ToString());
                            patron.convenio = rows["CON_VEN"].ToString();
                            patron.inicioAfiliacion = rows["INI_AFIL"].ToString();
                            patron.clase = rows["CLASE"].ToString();
                            patron.fraccion = rows["FRACCION"].ToString();
                            patron.STyPS = rows["STyPS"].ToString();            */

                            //Ponemos la entidad en modo modficada y guardamos cambios
                            try
                            {
                                db.Entry(patron).State = EntityState.Modified;
                                db.SaveChanges();
                                count++;
                            }
                            catch (DbEntityValidationException ex)
                            {
                                StringBuilder sb = new StringBuilder();

                                foreach (var failure in ex.EntityValidationErrors)
                                {
                                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                    foreach (var error in failure.ValidationErrors)
                                    {
                                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                        sb.AppendLine();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        String plazaDescripcion = rows["CAR_ENT"].ToString();
                        if (!plazaDescripcion.Equals(""))
                        {
                            var plazaTemp = from b in db.Plazas
                                            where b.descripcion.Equals(plazaDescripcion.Trim())
                                            select b;

                            Plaza plaza = new Plaza();

                            if (plazaTemp.Count() > 0)
                            {
                                foreach (var plazaItem in plazaTemp)
                                {
                                    plaza.id = plazaItem.id;
                                    plaza.descripcion = plazaItem.descripcion;
                                    plaza.indicador = "P";
                                    break;
                                }//Definimos los valores para la plaza
                            }
                            else
                            {
                                plaza.descripcion = plazaDescripcion.Trim();
                                plaza.indicador = "P";
                                db.Plazas.Add(plaza);
                                db.SaveChanges();

                            }//Ya existen datos con esta plaza?
                            //Creamos el nuevo patron
                            patron = new Patrone();

                            patron.registro = rows["REG_PAT"].ToString();
                            patron.rfc = rows["RFC_PAT"].ToString();
                            patron.nombre = rows["NOM_PAT"].ToString();
                            patron.actividad = rows["ACT_PAT"].ToString();
                            patron.domicilio = rows["DOM_PAT"].ToString();
                            patron.municipio = rows["MUN_PAT"].ToString();
                            patron.codigoPostal = rows["CPP_PAT"].ToString();
                            patron.entidad = rows["ENT_PAT"].ToString();
                            patron.telefono = rows["TEL_PAT"].ToString();
                            patron.remision = ((Boolean.Parse(rows["REM_PAT"].ToString()) == true) ? "V" : "F");
                            patron.zona = rows["ZON_PAT"].ToString();
                            patron.delegacion = rows["DEL_PAT"].ToString();
                            patron.carEnt = rows["CAR_ENT"].ToString();
                            patron.numeroDelegacion = Int32.Parse(rows["NUM_DEL"].ToString());
                            patron.carDel = rows["CAR_DEL"].ToString();
                            patron.numSub = Int32.Parse(rows["NUM_SUB"].ToString());
                            patron.Plaza_id = plaza.id;
                            patron.tipoConvenio = Decimal.Parse(rows["TIP_CON"].ToString());
                            patron.convenio = rows["CON_VEN"].ToString();
                            patron.inicioAfiliacion = rows["INI_AFIL"].ToString();
                            patron.patRep = rows["PAT_REP"].ToString();
                            patron.clase = rows["CLASE"].ToString();
                            patron.fraccion = rows["FRACCION"].ToString();
                            patron.STyPS = rows["STyPS"].ToString();

                            //Guardamos el patron
                            try
                            {
                                db.Patrones.Add(patron);
                                db.SaveChanges();
                                count++;
                            }
                            catch (DbEntityValidationException ex)
                            {
                                StringBuilder sb = new StringBuilder();

                                foreach (var failure in ex.EntityValidationErrors)
                                {
                                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                    foreach (var error in failure.ValidationErrors)
                                    {
                                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                        sb.AppendLine();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                if (ex.Source != null)
                {
                    Console.WriteLine(ex.Source);
                    isError = true;
                }
            }
            finally
            {
                if (sua != null)
                {
                    sua.cerrarConexion();
                }
            }

            if (isError)
            {
                TempData["error"] = isError;
                TempData["viewMessage"] = "Ocurrio un error al intentar cargar el archivo";
            }
            else {
                TempData["error"] = isError;
                TempData["viewMessage"] = "Se ha realizado la actualización de los Patrones con exito!";
            }

            return count;
        }


        public ActionResult GoAcreditados(String plazasId)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId
                                               where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }).Distinct(), "id", "descripcion");

            if (String.IsNullOrEmpty(plazasId))
            {
                var plazaTemp = from s in db.Plazas.ToList()
                                join top in db.TopicosUsuarios on s.id equals top.topicoId
                                where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                orderby s.descripcion
                                select s;

                Plaza plaza = new Plaza();
                if (plazaTemp != null && plazaTemp.Count() > 0)
                {
                    foreach (var plazaItem in plazaTemp)
                    {
                        plaza = plazaItem;
                        break;
                    }
                    plazasId = plaza.id.ToString();
                }
            }
            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(user.Id) &&
                                                       !s.direccionArchivo.Trim().Equals(null) && !s.direccionArchivo.Trim().Equals(String.Empty)  &&
                                                       s.Plaza.id.Equals(plazasId)
                                                 orderby s.registro
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                  }).Distinct(), "id", "FullName", null);
  
            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var patrones = from p in db.Patrones
                           where !p.direccionArchivo.Trim().Equals(null) && !p.direccionArchivo.Trim().Equals(String.Empty)
                           && patronesAsignados.Contains(p.Id)
                           && p.Plaza.id.ToString().Equals(plazasId)
                           select new
                           {
                               id = p.Id,
                               DisplayText = p.registro.ToString() + " " + p.nombre.ToString()
                           };


            ViewData["patronesId"] = new SelectList(patrones, "Id", "DisplayText");
            return View("UploadAcreditados");
        }


        public ActionResult GoOnlyUploadFile(String plazasId)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId
                                               where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }).Distinct(), "id", "descripcion");

            if (String.IsNullOrEmpty(plazasId))
            {
                var plazaTemp = from s in db.Plazas.ToList()
                                join top in db.TopicosUsuarios on s.id equals top.topicoId
                                where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                orderby s.descripcion
                                select s;

                Plaza plaza = new Plaza();
                if (plazaTemp != null && plazaTemp.Count() > 0)
                {
                    foreach (var plazaItem in plazaTemp)
                    {
                        plaza = plazaItem;
                        break;
                    }
                    plazasId = plaza.id.ToString();
                }
            }
            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(user.Id) &&
                                                       !s.direccionArchivo.Trim().Equals(null) && !s.direccionArchivo.Trim().Equals(String.Empty) &&
                                                       s.Plaza.id.Equals(plazasId)
                                                 orderby s.registro
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                 }).Distinct(), "id", "FullName", null);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var patrones = from p in db.Patrones
                           where !p.direccionArchivo.Trim().Equals(null) && !p.direccionArchivo.Trim().Equals(String.Empty)
                           && patronesAsignados.Contains(p.Id)
                           && p.Plaza.id.ToString().Equals(plazasId)
                           select new
                           {
                               id = p.Id,
                               DisplayText = p.registro.ToString() + " " + p.nombre.ToString()
                           };


            ViewData["patronesId"] = new SelectList(patrones, "Id", "DisplayText");
            return View("OnlyUpLoadFile");
        }

        
        [HttpPost]
        public ActionResult uploadFile(UploadModel uploadModel, int patronesId)
        {

            Patrone patron = db.Patrones.Find(patronesId);

            String path = this.Upload(uploadModel, patron.direccionArchivo);
            if (!path.Equals("")) {
                this.uploadAcreditado(path);
                this.uploadAsegurado(path);
            }

            return View("UploadAcreditados");
        }

        public ActionResult uploadFile2(UploadModel uploadModel, int patronesId)
        {

            Patrone patron = db.Patrones.Find(patronesId);

            String path = this.Upload(uploadModel, patron.direccionArchivo);

            return View("OnlyUploadFile");
        }

        public String Upload(UploadModel uploadModel, String subFolder)
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


        /**
         * Realizamos la carga de los acreditados
         */
        public void uploadAcreditado(String path)
        {
            SUAHelper sua = null;
            Boolean isError = false;
            try
            {
                sua = new SUAHelper(path);

                ParametrosHelper parameterHelper = new ParametrosHelper();
            
                Parametro smdfParameter = parameterHelper.getParameterByKey("SMDF");
                Parametro sinfonParameter = parameterHelper.getParameterByKey("SINFON");

                //Preparamos la consulta
                String sSQL = "SELECT a.REG_PATR , a.NUM_AFIL, a.CURP      , a.RFC_CURP, a.NOM_ASEG, " +
                              "       a.SAL_IMSS , a.SAL_INFO, a.FEC_ALT   , a.FEC_BAJ , a.TIP_TRA , " +
                              "       a.SEM_JORD , a.PAG_INFO, a.TIP_DSC   , a.VAL_DSC , a.CVE_UBC , " +
                              "       a.TMP_NOM  , a.FEC_DSC , a.FEC_FinDsc, a.ARTI_33 , a.SAL_AR33," +
                              "       a.TRA_PENIV, a.ESTADO  , a.CVE_MUN   , b.OCUPA   , b.LUG_NAC  " +
                              "  FROM Asegura a LEFT JOIN Afiliacion b  " +
                              "    ON a.REG_PATR = b.REG_PATR AND  a.NUM_AFIL = b.NUM_AFIL " +
                              "  WHERE a.PAG_INFO <> '' " +
                              "  ORDER BY a.REG_PATR, a.NUM_AFIL ";


                DataTable dt = sua.ejecutarSQL(sSQL);

                foreach (DataRow rows in dt.Rows)
                {
                    String patronDescripcion = rows["REG_PATR"].ToString();
                    Patrone patron = new Patrone();
                    if (!patronDescripcion.Equals(""))
                    {
                        var patronTemp = from b in db.Patrones
                                         where b.registro.Equals(patronDescripcion.Trim())
                                         select b;


                        if (patronTemp != null && patronTemp.Count() > 0)
                        {
                            foreach (var patronItem in patronTemp)
                            {
                                patron = patronItem;
                                break;
                            }//Definimos los valores para la plaza
                        }
                        else
                        {
                            patron.registro = "";
                        }

                    }

                    if (!patron.registro.Trim().Equals(""))
                    {
                        Boolean bExist = false;

                        //Creamos el nuevo asegurado      
                        Acreditado acreditado = new Acreditado();
                        String numAfil = rows["NUM_AFIL"].ToString().Trim();
                        String numCred = rows["PAG_INFO"].ToString().Trim();

                        //Revisamos la existencia del registro
                        var acreditadoExist = from b in db.Acreditados
                                              where b.Patrone.registro.Equals(patron.registro.Trim())
                                                && b.numeroAfiliacion.Equals(numAfil)
                                                && b.numeroCredito.Equals(numCred)
                                              select b;

                        if (acreditadoExist != null && acreditadoExist.Count() > 0)
                        {
                            foreach (var acred in acreditadoExist)
                            {
                                acreditado = acred;
                                bExist = true;
                                break;
                            }//Borramos cada registro.
                        }//Ya existen datos con este patron?

                        String tipoDescuento = rows["TIP_DSC"].ToString();

                        acreditado.PatroneId = patron.Id;
                        acreditado.Patrone = patron;
                        acreditado.numeroAfiliacion = rows["NUM_AFIL"].ToString();
                        acreditado.CURP = rows["CURP"].ToString();
                        acreditado.RFC = rows["RFC_CURP"].ToString();

                        String cliente = rows["CVE_UBC"].ToString();

                        var clienteTemp = db.Clientes.Where(b => b.claveCliente == cliente.Trim()).FirstOrDefault();

                        if (clienteTemp != null)
                        {
                            acreditado.Cliente = (Cliente)clienteTemp;
                            acreditado.clienteId = clienteTemp.Id;
                        }
                        else
                        {
                            Cliente clienteNuevo = new Cliente();
                            clienteNuevo.claveCliente = cliente;
                            clienteNuevo.rfc = "PENDIENTE";
                            clienteNuevo.claveSua = "PENDIENTE";
                            clienteNuevo.descripcion = "PENDIENTE";
                            clienteNuevo.ejecutivo = "PENDIENTE";
                            clienteNuevo.Plaza_id = 1;
                            clienteNuevo.Grupo_id = 4;

                            try
                            {
                                db.Clientes.Add(clienteNuevo);
                                db.SaveChanges();
                                acreditado.clienteId = clienteNuevo.Id;
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

                        }

                        String nombrePattern = rows["NOM_ASEG"].ToString();
                        nombrePattern = nombrePattern.Replace("$", ",");

                        string[] substrings = Regex.Split(nombrePattern, ",");

                        acreditado.nombre = substrings[2];
                        acreditado.apellidoPaterno = substrings[0];
                        acreditado.apellidoMaterno = substrings[1];
                        acreditado.nombreCompleto = substrings[0] + " " + substrings[1] + " " + substrings[2];
                        acreditado.ocupacion = rows["OCUPA"].ToString();
                        acreditado.fechaAlta = DateTime.Parse(rows["FEC_ALT"].ToString());

                        if (rows["FEC_BAJ"].ToString().Equals(""))
                        {
                            acreditado.fechaBaja = null;
                        }
                        else
                        {
                            acreditado.fechaBaja = DateTime.Parse(rows["FEC_BAJ"].ToString());
                        }//Trae fecha valida?

                        acreditado.idGrupo = "";
                        acreditado.numeroCredito = rows["PAG_INFO"].ToString();

                        if (rows["FEC_DSC"].ToString().Equals(""))
                        {
                            acreditado.fechaInicioDescuento = null;
                        }
                        else
                        {
                            acreditado.fechaInicioDescuento = DateTime.Parse(rows["FEC_DSC"].ToString());
                        }//Trae fecha valida?

                        if (rows["FEC_FinDsc"].ToString().Equals(""))
                        {
                            acreditado.fechaFinDescuento = null;
                        }
                        else
                        {
                            acreditado.fechaFinDescuento = DateTime.Parse(rows["FEC_FinDsc"].ToString());
                        }//Trae fecha valida?

                        DateTime date = DateTime.Now;

                        //Validamos que el ultimo movimiento no sea por baja o suspención.
                        if (sua.esValidoActualizarPorMovimiento(acreditado.Patrone.registro, acreditado.numeroAfiliacion))
                        {
                            //Validamos que el valor de los parametros sea mayor a cero.
                            if (sinfonParameter.valorMoneda > 0 && smdfParameter.valorMoneda > 0)
                            {
                                
                                if (bExist) { 
                                    if(acreditado.fechaUltimoCalculo != null){
                                //Validamos que se haya modificado el valor de los parametros para el calculo
                                        if (DateTime.Compare((DateTime)acreditado.fechaUltimoCalculo, smdfParameter.fechaCreacion) <= 0
                                        && DateTime.Compare((DateTime)acreditado.fechaUltimoCalculo, sinfonParameter.fechaCreacion) <= 0)
                                        {
                                            acreditado = calcularInfonavitInfo(acreditado, rows, tipoDescuento, Decimal.Parse(sinfonParameter.valorMoneda.ToString()), Decimal.Parse(smdfParameter.valorMoneda.ToString()));

                                        } //Se ha cambiado los parametros desde la ultima actualización ?
                                    }else{
                                        acreditado = calcularInfonavitInfo(acreditado, rows, tipoDescuento, Decimal.Parse(sinfonParameter.valorMoneda.ToString()), Decimal.Parse(smdfParameter.valorMoneda.ToString()));
                                    }
                                }
                                else
                                {
                                    acreditado = calcularInfonavitInfo(acreditado, rows, tipoDescuento, Decimal.Parse(sinfonParameter.valorMoneda.ToString()), Decimal.Parse(smdfParameter.valorMoneda.ToString()));
                                }
                            }//Los parametros son mayores a cero en su valor moneda ?
                        }//El movimiento es por baja o suspención ?
                        
                        acreditado.Plaza_id = patron.Plaza_id;

                        if (!bExist)
                        {
                            acreditado.fechaCreacion = date;
                        }
                        else {
                            acreditado.fechaModificacion = date;
                        }

                        //Guardamos el asegurado
                        try
                        {
                            if (!bExist)
                            {
                                db.Acreditados.Add(acreditado);
                            }
                            else {
                                db.Entry(acreditado).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (var failure in ex.EntityValidationErrors)
                            {
                                sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                foreach (var error in failure.ValidationErrors)
                                {
                                    sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                    sb.AppendLine();
                                }
                            }
                        }
                    }
                }

            }
            catch (OleDbException ex)
            {
                if (ex.Source != null)
                {
                    Console.WriteLine(ex.Source);
                    isError = true;
                }
            }
            finally
            {
                if (isError)
                {
                    TempData["error"] = isError;
                    TempData["viewMessage"] = "Ocurrio un error al intentar cargar el archivo de los Acreditados";
                }
                else
                {
                    TempData["error"] = isError;
                    TempData["viewMessage"] = "Se ha realizado la actualización con exito!";
                }
                if (sua != null)
                {
                    sua.cerrarConexion();
                }
            }
        }

        private Acreditado calcularInfonavitInfo(Acreditado acreditado, DataRow rows, String tipoDescuento, Decimal sinfon, Decimal smdf)
        {
            DateTime date = DateTime.Now;

            Decimal valueToCalculate = Decimal.Parse(rows["VAL_DSC"].ToString());
            acreditado.sdi = Double.Parse(rows["SAL_IMSS"].ToString());
            Decimal sdi = Decimal.Parse(rows["SAL_IMSS"].ToString());
            acreditado.smdv = Double.Parse(smdf.ToString());

            TempData["sinfon"] = sinfon.ToString();

            Decimal newValue = Decimal.Parse("0.0");
            //Empezamos con los calculos
/*            if (tipoDescuento.Trim().Equals("1"))
            {

                // Descuento tipo porcentaje
                acreditado.sd = 0;
                acreditado.cuotaFija = 0;
                acreditado.vsm = 0;
                acreditado.porcentaje = valueToCalculate / 100;


                newValue = (sdi * 60);
                newValue = newValue * (valueToCalculate / 100);
                newValue = newValue + sinfon;

                acreditado.descuentoBimestral = newValue;

            }
            else */
            if (tipoDescuento.Trim().Equals("2"))
            {
                // Descuento tipo cuota fija
                acreditado.sd = 0;
                acreditado.cuotaFija = valueToCalculate;
                acreditado.vsm = 0;
                acreditado.porcentaje = 0;

                newValue = valueToCalculate * 2;
                acreditado.descuentoBimestral = newValue;


            }
            else if (tipoDescuento.Trim().Equals("3"))
            {
                // Descuento tipo VSM
                acreditado.sd = 0;
                acreditado.cuotaFija = 0;
                acreditado.vsm = Math.Round(valueToCalculate, 3);
                acreditado.porcentaje = 0;
                newValue = valueToCalculate * smdf * 2;
                newValue = newValue + sinfon;
                newValue = Math.Round(newValue, 3);
                acreditado.descuentoBimestral = newValue;
            }

            acreditado.descuentoMensual = Math.Round(acreditado.descuentoBimestral / 2, 3);
            Decimal newValue2 = acreditado.descuentoMensual * Decimal.Parse((7 / 30.4).ToString());
            newValue2 = Math.Round(newValue2, 3);
            acreditado.descuentoSemanal = newValue2;

            newValue2 = acreditado.descuentoMensual * Decimal.Parse((14 / 30.4).ToString());
            newValue2 = Math.Round(newValue2, 3);
            acreditado.descuentoCatorcenal = newValue2;
            acreditado.descuentoQuincenal = Math.Round(acreditado.descuentoBimestral / 4, 3);
            acreditado.descuentoVeintiochonal = Math.Round(acreditado.descuentoMensual * Decimal.Parse((28 / 30.4).ToString()), 3);
            acreditado.descuentoDiario = Math.Round(acreditado.descuentoBimestral / Decimal.Parse("60.1"), 3);
            acreditado.fechaUltimoCalculo = date.Date;

            return acreditado;
        }

        /**
         *  Hacemos la carga de los asegurados
         * */
        public void uploadAsegurado(String path)
        {
            SUAHelper sua = null;
            Boolean isError = false;
            try
            {
                //Realizamos la conexión
                sua = new SUAHelper(path);

                String sSQL = "SELECT a.REG_PATR , a.NUM_AFIL, a.CURP      , a.RFC_CURP, a.NOM_ASEG, " +
                              "       a.SAL_IMSS , a.SAL_INFO, a.FEC_ALT   , a.FEC_BAJ , a.TIP_TRA , " +
                              "       a.SEM_JORD , a.PAG_INFO, a.TIP_DSC   , a.VAL_DSC , a.CVE_UBC , " +
                              "       a.TMP_NOM  , a.FEC_DSC , a.FEC_FinDsc, a.ARTI_33 , a.SAL_AR33," +
                              "       a.TRA_PENIV, a.ESTADO  , a.CVE_MUN   , b.OCUPA   , b.LUG_NAC  " +
                              "  FROM Asegura a LEFT JOIN Afiliacion b  " +
                              "    ON a.REG_PATR = b.REG_PATR AND  a.NUM_AFIL = b.NUM_AFIL " +
                              "  ORDER BY a.NUM_AFIL ";

                //Ejecutamos la consulta
                DataTable dt = sua.ejecutarSQL(sSQL);
                foreach (DataRow rows in dt.Rows)
                {

                    String patronDescripcion = rows["REG_PATR"].ToString();
                    Patrone patron = new Patrone();
                    if (!patronDescripcion.Equals(""))
                    {
                        var patronTemp = from b in db.Patrones
                                         where b.registro.Equals(patronDescripcion.Trim())
                                         select b;


                        if (patronTemp != null && patronTemp.Count() > 0)
                        {
                            foreach (var patronItem in patronTemp)
                            {
                                patron = patronItem;
                                break;
                            }//Definimos los valores para la plaza
                        }
                        else
                        {
                            patron.registro = "";
                        }

                    }

                    if (!patron.registro.Trim().Equals(""))
                    {
                        Boolean bExist = false;

                        //Creamos el nuevo asegurado      
                        Asegurado asegurado = new Asegurado();
                        String numAfil = rows["NUM_AFIL"].ToString().Trim();

                        //Revisamos la existencia del registro
                        var aseguradoExist = from b in db.Asegurados
                                              where b.Patrone.registro.Equals(patron.registro.Trim())
                                                && b.numeroAfiliacion.Equals(numAfil)
                                              select b;

                        if (aseguradoExist.Count() > 0)
                        {
                            foreach (var aseg in aseguradoExist)
                            {
                                asegurado = aseg;
                                bExist = true;
                                break;
                            }//Borramos cada registro.
                        }//Ya existen datos con este patron?


                        //Creamos el nuevo asegurado                 

                        asegurado.PatroneId = patron.Id;
                        asegurado.numeroAfiliacion = rows["NUM_AFIL"].ToString();
                        asegurado.CURP = rows["CURP"].ToString();
                        asegurado.RFC = rows["RFC_CURP"].ToString();
                        asegurado.nombre = rows["NOM_ASEG"].ToString();
                        asegurado.salarioImss = Decimal.Parse(rows["SAL_IMSS"].ToString());
                        if (rows["SAL_INFO"].ToString().Equals(""))
                        {
                            asegurado.salarioInfo = 0;
                        }
                        else
                        {
                            asegurado.salarioInfo = Decimal.Parse(rows["SAL_INFO"].ToString());
                        }

                        asegurado.fechaAlta = DateTime.Parse(rows["FEC_ALT"].ToString());

                        if (rows["FEC_BAJ"].ToString().Equals(""))
                        {
                            asegurado.fechaBaja = null;
                        }
                        else
                        {
                            asegurado.fechaBaja = DateTime.Parse(rows["FEC_BAJ"].ToString());
                        }//Trae fecha valida?
                        asegurado.tipoTrabajo = rows["TIP_TRA"].ToString();
                        asegurado.semanaJornada = rows["SEM_JORD"].ToString();
                        asegurado.paginaInfo = rows["PAG_INFO"].ToString();
                        asegurado.tipoDescuento = rows["TIP_DSC"].ToString();
                        asegurado.valorDescuento = Decimal.Parse(rows["VAL_DSC"].ToString());

                        String cliente = rows["CVE_UBC"].ToString();

                        var clienteTemp = db.Clientes.Where(b => b.claveCliente == cliente.Trim()).FirstOrDefault();

                        if (clienteTemp != null){
                            asegurado.Cliente = (Cliente)clienteTemp;
                            asegurado.ClienteId = clienteTemp.Id;
                        }else
                        {
                            Cliente clienteNuevo = new Cliente();
                            clienteNuevo.claveCliente = cliente;
                            clienteNuevo.rfc = "PENDIENTE";
                            clienteNuevo.claveSua = "PENDIENTE";
                            clienteNuevo.descripcion = "PENDIENTE";
                            clienteNuevo.ejecutivo = "PENDIENTE";
                            clienteNuevo.Plaza_id = 1;
                            clienteNuevo.Grupo_id = 4;

                            try
                            {
                                db.Clientes.Add(clienteNuevo);
                                db.SaveChanges();
                                asegurado.ClienteId = clienteNuevo.Id;
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

                        }

                        asegurado.nombreTemporal = rows["TMP_NOM"].ToString();

                        if (rows["FEC_DSC"].ToString().Equals(""))
                        {
                            asegurado.fechaDescuento = null;
                        }
                        else
                        {
                            asegurado.fechaDescuento = DateTime.Parse(rows["FEC_DSC"].ToString());
                        }//Trae fecha valida?

                        if (rows["FEC_FinDsc"].ToString().Equals(""))
                        {
                            asegurado.finDescuento = null;
                        }
                        else
                        {
                            asegurado.finDescuento = DateTime.Parse(rows["FEC_FinDsc"].ToString());
                        }//Trae fecha valida?
                        asegurado.articulo33 = rows["ARTI_33"].ToString();
                        if (rows["SAL_AR33"].ToString().Equals(""))
                        {
                            asegurado.salarioArticulo33 = 0;
                        }
                        else
                        {
                            asegurado.salarioArticulo33 = Decimal.Parse(rows["SAL_AR33"].ToString());
                        }
                        asegurado.trapeniv = rows["TRA_PENIV"].ToString();
                        asegurado.estado = rows["ESTADO"].ToString();
                        asegurado.claveMunicipio = rows["CVE_MUN"].ToString();
                        asegurado.Plaza_id = patron.Plaza_id;
                        asegurado.ocupacion = rows["OCUPA"].ToString();
                        if (rows["OCUPA"].ToString().Equals("EXTRANJERO"))
                        {
                            asegurado.extranjero = "SI";
                        }
                        else {
                            asegurado.extranjero = "NO";
                        }

                        DateTime date = DateTime.Now;
                        if (!bExist)
                        {
                            asegurado.fechaCreacion = date;
                        }
                        else
                        {
                            asegurado.fechaModificacion = date;
                        }


                        //Guardamos el asegurado
                        try
                        {
                            if (bExist)
                            {
                                db.Entry(asegurado).State = EntityState.Modified;
                            }
                            else
                            {
                                db.Asegurados.Add(asegurado);
                            }
                            db.SaveChanges();
                            if (asegurado.id > 0)
                            {
                                uploadIncapacidades(asegurado.Patrone.registro, asegurado.numeroAfiliacion, asegurado.id, path);
                                uploadMovimientos(asegurado.Patrone.registro, asegurado.numeroAfiliacion, asegurado.id, path);
                                accionesAdicionalesAsegurados(asegurado);
                            }
                        }
                        catch (DbEntityValidationException ex)
                        {
                            isError = true;
                            StringBuilder sb = new StringBuilder();

                            foreach (var failure in ex.EntityValidationErrors)
                            {
                                sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                foreach (var error in failure.ValidationErrors)
                                {
                                    sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                    sb.AppendLine();
                                }
                            }
                        }
                    }
                }

            }
            catch (OleDbException ex)
            {
                isError = true;
                if (ex.Source != null)
                {
                    Console.WriteLine(ex.Source);
                }
            }
            finally
            {
                if (isError)
                {
                    TempData["error"] = isError;
                    TempData["viewMessage"] = "Ocurrio un error al intentar cargar el archivo de Asegurados";
                }
                else
                {
                    TempData["error"] = isError;
                    TempData["viewMessage"] = "Se ha realizado la actualización con exito!";
                }
                if (sua != null)
                {
                    sua.cerrarConexion();
                }
            }
        }

        public void uploadIncapacidades(String registro, String numeroAfiliacion, int aseguradoId, String path)
        {
            SUAHelper sua = null;
            try
            {
                //Realizamos la conexión
                sua = new SUAHelper(path);

                String sSQL = "SELECT a.REG_PAT  , a.NUM_AFI      , a.FEC_ACC   , a.FOL_INC , a.GRUPO_INC , " +
                              "       a.REC_REV  , a.CONSECUENCIA , a.TIP_RIE   , a.RAM_SEG , a.SECUELA   , " +
                              "       a.CON_INC  , a.DIA_SUB      , a.POR_INC   , a.IND_DEF , a.FEC_TER     " +
                              "  FROM Incapacidades a  " +
                              "  WHERE a.REG_PAT = '" + registro + "'" +
                              "    AND a.NUM_AFI = '" + numeroAfiliacion + "'" +
                              "  ORDER BY a.NUM_AFI ";

                //Ejecutamos la consulta
                DataTable dt = sua.ejecutarSQL(sSQL);
                foreach (DataRow rows in dt.Rows)
                {

                    Boolean bExist = false;

                    String folio = rows["FOL_INC"].ToString();
                    Incapacidade incapacidad = new Incapacidade();
                    var incapacidadTemp = from b in db.Incapacidades
                                          where b.folioIncapacidad.Equals(folio.Trim())
                                          select b;

                    if (incapacidadTemp != null && incapacidadTemp.Count() > 0)
                    {
                        foreach (var incapacidadItem in incapacidadTemp)
                        {
                            incapacidad = incapacidadItem;
                            bExist = true;
                            break;
                        }//Definimos los valores para la plaza
                    }

                    //Creamos la nueva incapacidad     
                    if (!bExist)
                    {
                        incapacidad.aseguradoId = aseguradoId;
                        incapacidad.folioIncapacidad = rows["FOL_INC"].ToString();
                    }
                    incapacidad.fechaAcc = DateTime.Parse(rows["FEC_ACC"].ToString());
                    incapacidad.grupoIncapacidad = rows["GRUPO_INC"].ToString();
                    incapacidad.recRev = rows["REC_REV"].ToString();
                    incapacidad.consecuencia = rows["CONSECUENCIA"].ToString();

                    incapacidad.tieRie = rows["TIP_RIE"].ToString();
                    incapacidad.ramSeq = rows["RAM_SEG"].ToString();
                    incapacidad.secuela = rows["SECUELA"].ToString();
                    incapacidad.conInc = rows["CON_INC"].ToString();
                    incapacidad.diaSub = int.Parse(rows["DIA_SUB"].ToString());
                    incapacidad.porcentajeIncapacidad = Decimal.Parse(rows["POR_INC"].ToString());
                    incapacidad.indDef = rows["IND_DEF"].ToString();
                    incapacidad.fecTer = DateTime.Parse(rows["FEC_TER"].ToString());

                    //Guardamos la incapacidad
                    try
                    {
                        if (bExist)
                        {
                            db.Entry(incapacidad).State = EntityState.Modified;
                        }
                        else
                        {
                            db.Incapacidades.Add(incapacidad);
                        }
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (var failure in ex.EntityValidationErrors)
                        {
                            sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                            foreach (var error in failure.ValidationErrors)
                            {
                                sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                sb.AppendLine();
                            }
                        }
                    }
                }

            }
            catch (OleDbException ex)
            {
                if (ex.Source != null)
                {
                    Console.WriteLine(ex.Source);
                }
            }
            finally
            {
                if (sua != null)
                {
                    sua.cerrarConexion();
                }
            }
        }

        public void uploadMovimientos(String registro, String numeroAfiliacion, int aseguradoId, String path)
        {
            SUAHelper sua = null;
            try
            {
                //Realizamos la conexión
                sua = new SUAHelper(path);

                String sSQL = "SELECT a.REG_PATR , a.NUM_AFIL , a.TIP_MOVS  , a.FEC_INIC , a.CON_SEC  , " +
                              "       a.NUM_DIAS , a.SAL_MOVT , a.SAL_MOVT2 , a.MES_ANO  , a.FEC_REAL , " +
                              "       a.FOL_INC  , a.CVE_MOVS , a.SAL_MOVT3 , a.TIP_INC  , a.EDO_MOV  , " +
                              "       a.FEC_EXT  , a.SAL_ANT1 , a.SAL_ANT2  , a.SAL_ANT3 , a.ART_33   , " +
                              "       a.TIP_SAL  , a.TIP_RIE  , a.TIP_REC   , a.NUM_CRE  , a.VAL_DES  , " +
                              "       a.TIP_DES  , a.TAB_DISM " +
                              "  FROM Movtos a  " +
                              "  WHERE a.REG_PATR = '" + registro + "'" +
                              "    AND a.NUM_AFIL = '" + numeroAfiliacion + "'" +
                              "  ORDER BY a.NUM_AFIL ";

                //Ejecutamos la consulta
                DataTable dt = sua.ejecutarSQL(sSQL);
                foreach (DataRow rows in dt.Rows)
                {

                    String folio = rows["FOL_INC"].ToString();
                    MovimientosAsegurado movimiento = new MovimientosAsegurado();

                    movimiento.fechaInicio = DateTime.Parse(rows["FEC_INIC"].ToString());
                    movimiento.aseguradoId = aseguradoId;
                    movimiento.sdi = rows["SAL_MOVT"].ToString();
                    String tipoMov = "01";
                    if (!string.IsNullOrEmpty(rows["TIP_MOVS"].ToString().Trim()))
                    {
                        tipoMov = rows["TIP_MOVS"].ToString().Trim();
                    }

                    //Validamos que ese movimiento no se haya guardado anteriormente
                    var movTemp = (from s in db.MovimientosAseguradoes
                                  .Where(s => s.aseguradoId.Equals(aseguradoId)
                                  && s.catalogoMovimiento.tipo.Equals(tipoMov.Trim())
                                  && s.fechaInicio.Equals(movimiento.fechaInicio))
                                   select s).FirstOrDefault();
                    
                    if (movTemp == null)
                    {

                        if (rows["NUM_DIAS"].ToString() != null && !rows["NUM_DIAS"].ToString().Equals(""))
                        {
                            movimiento.numeroDias = int.Parse(rows["NUM_DIAS"].ToString());
                        }

                        if (folio != null && !folio.Equals(""))
                        {

                            var incapacidadTemp = from b in db.Incapacidades
                                                  where b.folioIncapacidad.Equals(folio.Trim())
                                                  select b;

                            if (incapacidadTemp != null && incapacidadTemp.Count() > 0)
                            {
                                foreach (var incapacidadItem in incapacidadTemp)
                                {
                                    movimiento.Incapacidade = incapacidadItem;
                                    movimiento.incapacidadId = incapacidadItem.id;
                                    break;
                                }//Definimos los valores para la plaza
                            }
                        }

                        var tipoTemp = db.catalogoMovimientos.Where(b => b.tipo == tipoMov).FirstOrDefault();

                        if (tipoTemp != null)
                        {
                            movimiento.catalogoMovimiento = (catalogoMovimiento)tipoTemp;
                        }
                        else
                        {
                            catalogoMovimiento catMov = new catalogoMovimiento();
                            catMov.id = 1;
                            catMov.tipo = "01";
                            movimiento.catalogoMovimiento = catMov;
                        }

                        movimiento.credito = rows["NUM_CRE"].ToString();
                        movimiento.estatus = rows["EDO_MOV"].ToString();

                        //Guardamos el movimiento
                        try
                        {

                            db.MovimientosAseguradoes.Add(movimiento);
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (var failure in ex.EntityValidationErrors)
                            {
                                sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                foreach (var error in failure.ValidationErrors)
                                {
                                    sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                    sb.AppendLine();
                                }
                            }
                        }
                    }
                }//Ya existe ese movimiento for fecha y tipo?
            }
            catch (OleDbException ex)
            {
                if (ex.Source != null)
                {
                    Console.WriteLine(ex.Source);
                }
            }
            finally
            {
                if (sua != null)
                {
                    sua.cerrarConexion();
                }
            }
        }

        /**
         * Realizamos el calculo del salario diario y la fecha de entrada 
         */
        private void accionesAdicionalesAsegurados(Asegurado asegurado)
        {

            int aseguradoId = asegurado.id;
            DateTime ahora = DateTime.Now;

            var ahora2 = 0;
            if (asegurado.numeroAfiliacion.Equals("81048003131"))
            { 
                ahora2 = 0;

            }
            //obtenemos el ultimo reingreso, si existe.
            var movTemp = (from s in db.MovimientosAseguradoes
                                  .Where(s => s.aseguradoId.Equals(aseguradoId)
                                   && s.catalogoMovimiento.tipo.Equals("08"))
                                  .OrderByDescending(s => s.fechaInicio)
                           select s).FirstOrDefault();

            if (movTemp != null)
            {
                asegurado.fechaAlta = movTemp.fechaInicio;
            }

            if (asegurado.salarioDiario == null)
            {
                asegurado.salarioDiario = 0;
            }

            var movTemp2 = (from s in db.MovimientosAseguradoes
                            where s.aseguradoId.Equals(aseguradoId) &&
                                 (s.catalogoMovimiento.tipo.Equals("01") || s.catalogoMovimiento.tipo.Equals("02") ||
                                  s.catalogoMovimiento.tipo.Equals("07") || s.catalogoMovimiento.tipo.Equals("08") ||
                                  s.catalogoMovimiento.tipo.Equals("13"))
                            orderby s.fechaInicio descending
                            select s).ToList();

            MovimientosAsegurado movto = new MovimientosAsegurado();
            if (movTemp2 != null && movTemp2.Count() > 0)
            {
                foreach (var movItem in movTemp2)
                {
                    movto = movItem;
                    break;
                }

                if (movto.catalogoMovimiento.tipo.Trim().Equals("08"))
                {
                    asegurado.salarioDiario = Decimal.Parse(movto.sdi.ToString());
                    asegurado.salarioImss = Decimal.Parse(movto.sdi.ToString());
                }
                else if (movto.catalogoMovimiento.tipo.Trim().Equals("01") || movto.catalogoMovimiento.tipo.Trim().Equals("07") ||
                         movto.catalogoMovimiento.tipo.Trim().Equals("13"))
                {
                    long annos = DatesHelper.DateDiffInYears(asegurado.fechaAlta, ahora);
                    if (annos.Equals(0))
                    {
                        annos = 1;
                    }
                    Factore factor = (db.Factores.Where(x => x.anosTrabajados == annos).FirstOrDefault());
                    if (factor != null)
                    {
                        asegurado.salarioDiario = Decimal.Parse(movto.sdi.Trim()) / factor.factorIntegracion;
                        asegurado.salarioImss = Decimal.Parse(movto.sdi.ToString());
                    }
                    else
                    {
                        asegurado.salarioDiario = 0;
                    }
                }
                else if (movto.catalogoMovimiento.tipo.Trim().Equals("02"))
                {
                    asegurado.salarioDiario = 0;
                    asegurado.salarioImss = 0;

                }
            }
            else
            {
                long annos = DatesHelper.DateDiffInYears(asegurado.fechaAlta, ahora);
                if (annos.Equals(0))
                {
                    annos = 1;
                }
                Factore factor = (db.Factores.Where(x => x.anosTrabajados == annos).FirstOrDefault());
                if (factor != null)
                {
                    asegurado.salarioDiario = asegurado.salarioImss / factor.factorIntegracion;
                }
                else
                {
                    asegurado.salarioDiario = 0;
                }
            }
            if (asegurado.fechaBaja.HasValue)
            {
                asegurado.salarioDiario = 0;
                asegurado.salarioImss = 0;
            }
            db.Entry(asegurado).State = EntityState.Modified;
            db.SaveChanges();

            Acreditado acreditado = (from s in db.Acreditados
                                     where s.PatroneId == asegurado.PatroneId
                                        && s.numeroAfiliacion.Equals(asegurado.numeroAfiliacion)
                                     select s).FirstOrDefault();

            if (acreditado != null)
            {
                acreditado.fechaAlta = asegurado.fechaAlta;
                acreditado.sd = Decimal.Parse(asegurado.salarioDiario.ToString());
                acreditado.sdi = Double.Parse(asegurado.salarioImss.ToString());

                //calcular el descuento tipo uno que ocupa sdi
                DateTime date = DateTime.Now;
                Decimal valueToCalculate = Decimal.Parse(asegurado.valorDescuento.ToString());
                Decimal newValue = Decimal.Parse("0.0");


                if (asegurado.tipoDescuento.Trim().Equals("1"))
                {

                    try
                    {
                        ParametrosHelper parameterHelper = new ParametrosHelper();

                        Parametro sinfonParameter = parameterHelper.getParameterByKey("SINFON");

                        decimal sinfon = decimal.Parse(sinfonParameter.valorMoneda.ToString());
                        // Descuento tipo porcentaje
                        acreditado.cuotaFija = 0;
                        acreditado.vsm = 0;
                        acreditado.porcentaje = valueToCalculate / 100;


                        newValue = (Decimal.Parse(acreditado.sdi.ToString()) * 60);
                        newValue = newValue * (valueToCalculate / 100);
                        newValue = newValue + sinfon;

                        acreditado.descuentoBimestral = newValue;

                        acreditado.descuentoMensual = Math.Round(acreditado.descuentoBimestral / 2, 3);
                        Decimal newValue2 = acreditado.descuentoMensual * Decimal.Parse((7 / 30.4).ToString());
                        newValue2 = Math.Round(newValue2, 3);
                        acreditado.descuentoSemanal = newValue2;

                        newValue2 = acreditado.descuentoMensual * Decimal.Parse((14 / 30.4).ToString());
                        newValue2 = Math.Round(newValue2, 3);
                        acreditado.descuentoCatorcenal = newValue2;
                        acreditado.descuentoQuincenal = Math.Round(acreditado.descuentoBimestral / 4, 3);
                        acreditado.descuentoVeintiochonal = Math.Round(acreditado.descuentoMensual * Decimal.Parse((28 / 30.4).ToString()), 3);
                        acreditado.descuentoDiario = Math.Round(acreditado.descuentoBimestral / Decimal.Parse("60.1"), 3);
                        acreditado.fechaUltimoCalculo = date.Date;
                    }
                    catch (OleDbException ex)
                    {
                        if (ex.Source != null)
                        {
                            Console.WriteLine(ex.Source);
                        }
                    }
                }


                db.Entry(acreditado).State = EntityState.Modified;
                db.SaveChanges();
            }

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