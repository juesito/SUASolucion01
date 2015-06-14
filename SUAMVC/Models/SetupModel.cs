using SUADATOS;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{

    public class SetupModel
    {
        private suaEntities db = new suaEntities();

        public SetupModel() { }

        /**
         * Creamos la configuración inicial para empezar a trabajar con 
         * el sistema.
         * */
        public void setUpSystem()
        {
            int count = db.Usuarios.Count();
            if (count == 0)
            {
                Plaza plaza = crearPlazas();

                crearTablas();
                Role role = db.Roles.Where(x => x.descripcion.Trim().Equals("Administrador")).FirstOrDefault();
                int usuarioId = usuarioRoot(plaza, role);
                crearFunciones(usuarioId);
                asignarPermisosBasicos();
            }//existen usuarios dados de alta?
        }


        /**
         * Creamos los roles
         * */

        /**
         * creamos los modulos
         * */
        private void asignarPermisosBasicos()
        {
            int count = db.Modulos.Count();

            if (count > 0)
            {
                //Obtenemos el modulo de seguridad para darle permisos
                Modulo modulo = db.Modulos.Where(x => x.descripcionCorta.Trim().Equals("Seguridad")).FirstOrDefault();

                DateTime date = DateTime.Now;

                //Grabamos las funciones de seguridad
                RoleModulo rm = new RoleModulo();
                rm.moduloId = modulo.id;

                //Buscamos el rol de administrador
                Role role = new Role();
                role = (from x in db.Roles
                        where x.descripcion.Equals("Administrador")
                        select x).FirstOrDefault();

                rm.roleId = role.id;
                rm.usuarioCreacionId = 1;
                rm.fechaCreacion = date;

                db.RoleModulos.Add(rm);
                db.SaveChanges();

                //Damos permisos a funciones de seguridad
                permisosFuncionesSeguridad(role.id, modulo.id);

            }
        }
        /**
         * Creamos las plazas
         * */
        private Plaza crearPlazas()
        {
            int count = db.Plazas.Count();
            Plaza plaza = new Plaza();
            if (count == 0)
            {
                plaza.descripcion = "Local";

                db.Plazas.Add(plaza);
                db.SaveChanges();
            }
            else
            {
                plaza = db.Plazas.Find(1);
            }
            return plaza;
        }
        /**
         * Damos de alta al usuario principal
         * */
        private int usuarioRoot(Plaza plaza, Role role)
        {

            var usuarioRoot = db.Usuarios.Where(x => x.claveUsuario.Trim().Equals("root")).FirstOrDefault();

            Usuario usuario = new Usuario();

            if (usuarioRoot == null)
            {
                DateTime date = DateTime.Now;

                usuario.claveUsuario = "root";
                usuario.apellidoPaterno = "SIAP";
                usuario.apellidoMaterno = "Admon";
                usuario.contrasena = "123";
                usuario.email = "user@siap.com.mx";
                usuario.estatus = "A";
                usuario.nombreUsuario = "El Administrador";
                usuario.fechaIngreso = date;
                usuario.Role = role;
                usuario.roleId = role.id;
                usuario.plazaId = plaza.id;
                usuario.Plaza = plaza;

                db.Usuarios.Add(usuario);
                db.SaveChanges();
            }
            else {
                usuario = usuarioRoot;
            }

            return usuario.Id;
        }

        //Otorgamos permisos a las funciones
        private void permisosFuncionesSeguridad(int roleId, int moduloId)
        {

            var funciones = from x in db.Funcions
                            where x.moduloId.Equals(moduloId)
                            select x;

            if (funciones.Count() > 0)
            {
                DateTime date = DateTime.Now;
                foreach (Funcion fun in funciones)
                {
                    RoleFuncion rf = new RoleFuncion();
                    rf.funcionId = fun.id;
                    rf.roleId = roleId;
                    rf.Funcion = fun;
                    rf.usuarioCreacionId = 1;
                    rf.fechaCreacion = date;

                    db.RoleFuncions.Add(rf);

                }
                db.SaveChanges();
            }



        }

        private void crearTablas()
        {

            int result = 0;
            int count = db.Roles.Count();
            if (count == 0)
            {
                result = db.Database.ExecuteSqlCommand("sp_createRoles");
                
                result = db.Database.ExecuteSqlCommand("sp_createModules");
                result = db.Database.ExecuteSqlCommand("sp_createCatalogoMovimientos");
                result = db.Database.ExecuteSqlCommand("sp_createParameters");
                
            }


        }

        private void crearFunciones(int usuarioId)
        {
            int count = db.Funcions.Count();
            if (count == 0)
            {
                int result = db.Database.ExecuteSqlCommand("sp_createFunctions @usuarioId", new SqlParameter("@usuarioId", usuarioId));
                result = db.Database.ExecuteSqlCommand("spCreateActionFunctions @usuarioId", new SqlParameter("@usuarioId", usuarioId));
            }
        }


    }



}