using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SUAMVC.Controllers
{
    public class RoleFuncionesController : Controller
    {
        private suaEntities db = new suaEntities();
        // GET: RoleFunciones
        public ActionResult Index(String roleId, String moduleId)
        {
            ViewBag.roleId = new SelectList(db.Roles, "id", "descripcion");
            ViewBag.moduleId = new SelectList(db.Modulos, "id", "descripcionCorta");
            if (!String.IsNullOrEmpty(roleId) && !String.IsNullOrEmpty(moduleId))
            {
                RoleFuncionesModel roleFuncion = new RoleFuncionesModel();

                TempData["vRoleId"] = roleId;
                TempData["vModuleId"] = moduleId;
                int id = int.Parse(roleId);
                int lmoduleId = int.Parse(moduleId);
                roleFuncion.role = db.Roles.Find(id);

                roleFuncion.funciones = new List<Funcion>();
                roleFuncion.funcionesByRole = new List<Funcion>();

                var roleFuncionesIds = (from x in db.RoleFuncions
                                        where x.roleId.Equals(id)
                                        select x.funcionId);

                List<int> rfl = roleFuncionesIds.ToList();

                if (rfl.Count > 0)
                {
                    var funciones = (from m in db.Funcions
                                     where m.moduloId.Equals(lmoduleId)
                                     && !roleFuncionesIds.Contains(m.id)
                                     select m).ToList();
                    funciones.ToList().ForEach(x => roleFuncion.funciones.Add(x));
                }
                else
                {
                    var funciones = (from m in db.Funcions
                                     where m.moduloId.Equals(lmoduleId)
                                     select m).ToList();
                    funciones.ToList().ForEach(x => roleFuncion.funciones.Add(x));
                }

                var funciones2 = (from m in db.RoleFuncions
                                  where m.roleId.Equals(id)
                                    && m.Funcion.moduloId.Equals(lmoduleId)
                                  select m.Funcion).ToList();
                funciones2.ToList().ForEach(x => roleFuncion.funcionesByRole.Add(x));

                return View(roleFuncion);
            }

            return View();
        }

        //Asignamos funciones al rol
        public ActionResult asignarFuncion(String roleId,  String moduleId, String[] ids)
        {

            if (!String.IsNullOrEmpty(roleId))
            {
                TempData["vRoleId"] = roleId;
                TempData["vModuleId"] = moduleId;
                Role role = db.Roles.Find(int.Parse(roleId));

                if (ids != null && ids.Length > 0)
                {
                    DateTime date = DateTime.Now;
                    Usuario user = db.Usuarios.Find(1);

                    foreach (String moduloId in ids)
                    {
                        RoleFuncion roleFuncion = new RoleFuncion();
                        roleFuncion.Role = role;
                        roleFuncion.fechaCreacion = date;
                        roleFuncion.Usuario = user;

                        int idTemp = int.Parse(moduloId);
                        var funcion = db.Funcions.Find(idTemp);
                        roleFuncion.Funcion = funcion;
                        db.RoleFuncions.Add(roleFuncion);
                        db.SaveChanges();

                    }
                }
            }
            return RedirectToAction("Index", new { roleId = roleId, moduleId = moduleId });
        }

        //Desasignamos funciones al rol seleccionado
        public ActionResult desasignarFuncion(String roleId, String moduleId, String[] ids2)
        {

            if (!String.IsNullOrEmpty(roleId))
            {
                TempData["vRoleId"] = roleId;
                TempData["vModuleId"] = moduleId;
                Role role = db.Roles.Find(int.Parse(roleId));

                if (ids2 != null && ids2.Length > 0)
                {
                    //Borramos los modulos refenciados a un rol
                    foreach (String funcionId in ids2)
                    {
                        int idRole = int.Parse(roleId);
                        int idTemp = int.Parse(funcionId);
                        var roleFuncion = (from x in db.RoleFuncions
                                           where x.funcionId.Equals(idTemp)
                                            && x.roleId.Equals(idRole)
                                           select x).FirstOrDefault();
                        if (roleFuncion != null)
                        {
                            db.RoleFuncions.Remove(roleFuncion);
                            db.SaveChanges();
                        }
                    }


                }
            }
            return RedirectToAction("Index", new { roleId = roleId, moduleId = moduleId });
        }
    }
}