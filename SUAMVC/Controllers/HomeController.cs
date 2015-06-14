using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SUAMVC.Controllers
{
    public class HomeController : Controller
    {
        private suaEntities db = new suaEntities();
        // GET: Home
        public ActionResult Index()
        {

            SetupModel setup = new SetupModel();
            setup.setUpSystem();

            return View();
        }

        public ActionResult Home() {
            Usuario usuario = Session["UsuarioData"] as Usuario;

            List<Menu> menuModel = createMenu(usuario);
            Session["menuList"] = menuModel;

            return View();
        }

        //Creamos el menu del usuario de acuerdo a sus permisos
        public List<Menu> createMenu(Usuario user) {
            List<Menu> menuCompleto = new List<Menu>();

            int role = user.roleId;
            var modulosPorUsuario = (from ru in db.RoleModulos
                                    where ru.roleId.Equals(role)
                                    select ru.Modulo).ToList();

            if (modulosPorUsuario.Count > 0) {
                foreach (Modulo modulo in modulosPorUsuario) {

                    Menu menu = new Menu();
                    menu.Id = modulo.id;
                    menu.Name = modulo.descripcionCorta.Trim();

                    var funcionesPorUsuario = (from fu in db.RoleFuncions
                                                   where fu.roleId.Equals(role)
                                                      && fu.Funcion.moduloId.Equals(modulo.id)
                                                      && fu.Funcion.tipo.Trim().Equals("M")
                                                   select fu.Funcion).ToList();

                    if (funcionesPorUsuario.Count > 0) {
                        foreach(Funcion funcion in funcionesPorUsuario){
                            MenuItem menuItem = new MenuItem();
                            menuItem.Id = funcion.id;
                            menuItem.Name = funcion.descripcionCorta.Trim();
                            menuItem.ControllerName = funcion.controlador.Trim();
                            menuItem.ActionName = funcion.accion.Trim();
                            menuItem.ParentMenu = menu;
                            menuItem.Url = funcion.descripcionLarga.Trim();
                            menuItem.UserName = user.nombreUsuario.Trim();
                            menu.MenuItems.Add(menuItem);
                        }
                    }

                    menuCompleto.Add(menu);
                }
            }

            return menuCompleto;
        }
    }
}