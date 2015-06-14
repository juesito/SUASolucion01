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
using System.Web.Security;

namespace SUAMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private suaEntities db = new suaEntities();
        private UsuarioModel usuarioModel = new UsuarioModel();

        // GET: Usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios.Include(u => u.Plaza).Include(u => u.Role);
            return View(usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            ViewBag.plazaId = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion");
            ViewBag.roleId = new SelectList(db.Roles, "id", "descripcion");
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,nombreUsuario,contrasena,claveUsuario,email,apellidoMaterno,apellidoPaterno,estatus,fechaIngreso,roleId,plazaId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.nombreUsuario = usuario.nombreUsuario.ToUpper();
                usuario.apellidoMaterno = usuario.apellidoMaterno.ToUpper();
                usuario.apellidoPaterno = usuario.apellidoPaterno.ToUpper();
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.plazaId = new SelectList((from s in db.Plazas.ToList()
                                              where s.indicador.Equals("U")
                                              orderby s.descripcion
                                              select new
                                              {
                                                  id = s.id,
                                                  descripcion = s.descripcion
                                              }), "id", "descripcion", usuario.plazaId);
            ViewBag.roleId = new SelectList(db.Roles, "id", "descripcion", usuario.roleId);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.plazaId = new SelectList((from s in db.Plazas.ToList()
                                              where s.indicador.Equals("U")
                                              orderby s.descripcion
                                              select new
                                              {
                                                  id = s.id,
                                                  descripcion = s.descripcion
                                              }), "id", "descripcion", usuario.plazaId);
            ViewBag.roleId = new SelectList(db.Roles, "id", "descripcion", usuario.roleId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,nombreUsuario,contrasena,claveUsuario,email,apellidoMaterno,apellidoPaterno,estatus,fechaIngreso,roleId,plazaId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.nombreUsuario = usuario.nombreUsuario.ToUpper();
                usuario.apellidoMaterno = usuario.apellidoMaterno.ToUpper();
                usuario.apellidoPaterno = usuario.apellidoPaterno.ToUpper();
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.plazaId = new SelectList((from s in db.Plazas.ToList()
                                              where s.indicador.Equals("U")
                                              orderby s.descripcion
                                              select new
                                              {
                                                  id = s.id,
                                                  descripcion = s.descripcion
                                              }), "id", "descripcion", usuario.plazaId);
            ViewBag.roleId = new SelectList(db.Roles, "id", "descripcion", usuario.roleId);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.UsuarioModel user)
        {
            Boolean bFounded = false;

            if (ModelState.IsValid)
            {

                if (user.IsValid(user.UserName, user.Password))
                {
                    Usuario usuario = user.extraeUsuario(user.UserName, user.Password);
                    usuario.contrasena = "XXXXX";
                    Session["UsuarioData"] = usuario;

                    //Llenamos los permisos del usuario
                    SecurityUserModel.llenarPermisos(usuario.roleId);
                    FormsAuthentication.SetAuthCookie(usuario.nombreUsuario, user.RememberMe);
                    bFounded = true;
                }
                else
                {
                    TempData["CustomError"] = "Datos de acceso incorrectos!";

                }

            }

            if (bFounded)
            {
                return RedirectToAction("Home", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }
        public ActionResult Logout()
        {
            Session["UsuarioData"] = null;
            SecurityUserModel.limpiarListaDePermisos();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
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
