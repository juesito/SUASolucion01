using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SUAMVC.Controllers
{
    public class TopicosUsuarioController : Controller
    {
        private suaEntities db = new suaEntities();
        // GET: TopicosUsuario
        public ActionResult Index(String usuarioId, String topico)
        {

            ViewBag.usuarioId = new SelectList((from s in db.Usuarios.ToList()
                                                orderby s.nombreUsuario
                                                select new
                                                {
                                                    id = s.Id,
                                                    nombreUsuario = s.nombreUsuario
                                                }).Distinct(), "id", "nombreUsuario");

            if (!String.IsNullOrEmpty(usuarioId))
            {
                TempData["vUsuarioId"] = usuarioId;
                TempData["vTopico"] = topico;
                int id = int.Parse(usuarioId);

                List<Topico> topicos = new List<Topico>();
                List<Topico> topicosPorUsuario = new List<Topico>();
                TopicosPorUsuarioModel tpum = new TopicosPorUsuarioModel();
                tpum.topicos = new List<Topico>();
                tpum.topicosPorUsuario = new List<Topico>();

                var topicosAsignadosIds = (from x in db.TopicosUsuarios
                                           where x.usuarioId.Equals(id)
                                           && x.tipo.Equals(topico)
                                           select x.topicoId);

                List<int> tai = topicosAsignadosIds.ToList();

                switch (topico)
                {
                    //Plazas
                    case "P":
                        if (tai.Count() > 0)
                        {
                            var topicoTemp = (from t in db.Plazas
                                              where !tai.Contains(t.id) && t.indicador.Equals("U")
                                              orderby t.descripcion
                                              select new { t.id, t.descripcion }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.id, x.descripcion)));

                        }
                        else
                        {
                            var topicoTemp = (from t in db.Plazas
                                              where t.indicador.Equals("U")
                                              select new { t.id, t.descripcion }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.id, x.descripcion)));


                        }
                        var topicoTemp2 = (from t in db.Plazas
                                           where tai.Contains(t.id) && t.indicador.Equals("U")
                                           orderby t.descripcion
                                           select new { t.id, t.descripcion }).ToList();
                        topicoTemp2.ToList().ForEach(x => tpum.topicosPorUsuario.Add(new Topico(x.id, x.descripcion)));


                        break;
                    //Clientes
                    case "C":
                        if (tai.Count() > 0)
                        {
                            var topicoTemp = (from t in db.Clientes
                                              where !tai.Contains(t.Id)
                                              orderby t.descripcion
                                              select new { t.Id, t.descripcion }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.Id, x.descripcion)));


                        }
                        else
                        {
                            var topicoTemp = (from t in db.Clientes
                                              select new { t.Id, t.descripcion }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.Id, x.descripcion)));

                            //    var topicoTemp2 = (from t in db.Clientes
                            //                       select new { t.Id, t.descripcion }).ToList();
                            //    topicoTemp2.ToList().ForEach(x => tpum.topicosPorUsuario.Add(new Topico(x.Id, x.descripcion)));
                        }
                        var topicoTemp3 = (from t in db.Clientes
                                           where tai.Contains(t.Id)
                                           orderby t.descripcion
                                           select new { t.Id, t.descripcion }).ToList();
                        topicoTemp3.ToList().ForEach(x => tpum.topicosPorUsuario.Add(new Topico(x.Id, x.descripcion)));
                        break;
                    //Patrones
                    case "B":
                        if (tai.Count() > 0)
                        {
                            var topicoTemp = (from t in db.Patrones
                                              where !tai.Contains(t.Id)
                                              orderby t.nombre
                                              select new { t.Id, t.nombre, t.registro }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.Id, x.registro + " - " + x.nombre)));


                        }
                        else
                        {
                            var topicoTemp = (from t in db.Patrones
                                              select new { t.Id, t.nombre, t.registro }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.Id, x.registro + " - " + x.nombre)));

                        }
                        var topicoTemp4 = (from t in db.Patrones
                                           where tai.Contains(t.Id)
                                           orderby t.nombre
                                           select new { t.Id, t.nombre, t.registro }).ToList();
                        topicoTemp4.ToList().ForEach(x => tpum.topicosPorUsuario.Add(new Topico(x.Id, x.registro + " - " + x.nombre)));
                        break;
                    case "G":
                        if (tai.Count() > 0)
                        {
                            var topicoTemp = (from t in db.Grupos
                                              where !tai.Contains(t.Id)
                                              orderby t.nombre
                                              select new { t.Id, t.claveGrupo, t.nombre }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.Id, x.claveGrupo + " - " + x.nombre)));


                        }
                        else
                        {
                            var topicoTemp = (from t in db.Grupos
                                              select new { t.Id, t.claveGrupo, t.nombre }).ToList();
                            topicoTemp.ToList().ForEach(x => tpum.topicos.Add(new Topico(x.Id, x.claveGrupo + " - " + x.nombre)));

                        }
                        var topicoTemp5 = (from t in db.Grupos
                                           where tai.Contains(t.Id)
                                           orderby t.nombre
                                           select new { t.Id, t.claveGrupo, t.nombre }).ToList();
                        topicoTemp5.ToList().ForEach(x => tpum.topicosPorUsuario.Add(new Topico(x.Id, x.claveGrupo + " - " + x.nombre)));
                        break;
                    default:
                        break;

                }

                return View(tpum);


            }

            return View();
        }

        //Asignamos el topico al usuario
        public ActionResult asignarTopico(String usuarioId, String topico, String[] ids)
        {

            if (!String.IsNullOrEmpty(usuarioId))
            {
                TempData["vUsuarioId"] = usuarioId;
                TempData["vTopico"] = topico;
                Usuario usuario = db.Usuarios.Find(int.Parse(usuarioId));

                if (ids != null && ids.Length > 0)
                {
                    DateTime date = DateTime.Now;
                    Usuario user = db.Usuarios.Find(1);

                    foreach (String topicoId in ids)
                    {
                        TopicosUsuario topicoUsuario = new TopicosUsuario();
                        topicoUsuario.tipo = topico;
                        topicoUsuario.fechaCreacion = date;
                        topicoUsuario.Usuario1 = usuario;
                        topicoUsuario.Usuario = user;
                        topicoUsuario.usuarioCreacionId = user.Id;

                        int idTemp = int.Parse(topicoId);
                        topicoUsuario.topicoId = idTemp;
                        db.TopicosUsuarios.Add(topicoUsuario);
                        db.SaveChanges();

                    }
                }
            }
            return RedirectToAction("Index", new { usuarioId = usuarioId, topico = topico });
        }

        //Desasignamos el topico al usuario
        public ActionResult desasignarTopico(String usuarioId, String topico, String[] ids2)
        {

            if (!String.IsNullOrEmpty(usuarioId))
            {
                TempData["vUsuarioId"] = usuarioId;
                TempData["vTopico"] = topico;
                Usuario usuario = db.Usuarios.Find(int.Parse(usuarioId));

                if (ids2 != null && ids2.Length > 0)
                {
                    //Borramos los modulos refenciados a un rol
                    foreach (String topicoId in ids2)
                    {
                        int idUser = int.Parse(usuarioId);
                        int topicoInt = int.Parse(topicoId);
                        var topicoUsuario = (from x in db.TopicosUsuarios
                                             where x.topicoId.Equals(topicoInt)
                                            && x.usuarioId.Equals(idUser)
                                            && x.tipo.Equals(topico)
                                             select x).Distinct().FirstOrDefault();
                        if (topicoUsuario != null)
                        {
                            db.TopicosUsuarios.Remove(topicoUsuario);
                            db.SaveChanges();
                        }
                    }


                }
            }
            return RedirectToAction("Index", new { usuarioId = usuarioId, topico = topico });
        }
    }
}