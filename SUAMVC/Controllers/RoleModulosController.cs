using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SUAMVC.Controllers
{
    public class RoleModulosController : Controller
    {
        private suaEntities db = new suaEntities();
        // GET: RoleModulos
        public ActionResult Index(String roleId)
        {
            ViewBag.roleId = new SelectList(db.Roles, "id", "descripcion");

            if (!String.IsNullOrEmpty(roleId))
            {
                RoleModulosModel rm = new RoleModulosModel();
                TempData["vRoleId"] = roleId;
                int id = int.Parse(roleId);
                rm.role = db.Roles.Find(id);

                //Llenamos los modulos disponibles
                rm.modulos = new List<Modulo>();
                rm.modulosByRole = new List<Modulo>();

                var roleModulos = from x in db.RoleModulos
                                  where x.roleId.Equals(id)
                                  select x.moduloId;

                List<int> rml = roleModulos.ToList();

                if (rml.Count > 0)
                {
                    var modulos = (from m in db.Modulos
                                   where !roleModulos.Contains(m.id)
                                   select m).ToList();
                    modulos.ToList().ForEach(x => rm.modulos.Add(x));
                }
                else
                {
                    var modulos = (from m in db.Modulos
                                   select m).ToList();
                    modulos.ToList().ForEach(x => rm.modulos.Add(x));
                }

                var modulos2 = (from m in db.RoleModulos
                                where m.roleId.Equals(id)
                                select m.Modulo).ToList();
                modulos2.ToList().ForEach(x => rm.modulosByRole.Add(x));

                return View(rm);

            }

            return View();
        }

        public ActionResult asignarModulo(String roleId, String[] ids)
        {

            if (!String.IsNullOrEmpty(roleId))
            {
                TempData["vRoleId"] = roleId;
                Role role = db.Roles.Find(int.Parse(roleId));

                if (ids != null && ids.Length > 0)
                {
                    DateTime date = DateTime.Now;
                    Usuario user = db.Usuarios.Find(1);

                    foreach (String moduloId in ids)
                    {
                        RoleModulo roleModulo = new RoleModulo();
                        roleModulo.Role = role;
                        roleModulo.fechaCreacion = date;
                        roleModulo.Usuario = user;

                        int idTemp = int.Parse(moduloId);
                        var modulo = db.Modulos.Find(idTemp);
                        roleModulo.Modulo = modulo;
                        db.RoleModulos.Add(roleModulo);
                        db.SaveChanges();

                    }
                }
            }
            return RedirectToAction("Index", new { roleId = roleId });
        }

        public ActionResult desasignarModulo(String roleId, String[] ids2)
        {

            if (!String.IsNullOrEmpty(roleId))
            {
                TempData["vRoleId"] = roleId;
                Role role = db.Roles.Find(int.Parse(roleId));

                if (ids2 != null && ids2.Length > 0)
                {
                    //Borramos los modulos refenciados a un rol
                    foreach (String moduloId in ids2)
                    {
                        int idRole = int.Parse(roleId);
                        int idTemp = int.Parse(moduloId);
                        var roleModulo = (from x in db.RoleModulos
                                                where x.moduloId.Equals(idTemp)
                                                 && x.roleId.Equals(idRole)
                                                    select x).FirstOrDefault();
                        if (roleModulo != null)
                        {
                            db.RoleModulos.Remove(roleModulo);
                            db.SaveChanges();
                        }
                    }

                    
                }
            }
            return RedirectToAction("Index", new { roleId = roleId});
        }
    }
}