using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public static class SecurityUserModel
    {
        private static suaEntities db;
        private static List<RoleFuncion> roleFunciones;

        //Recogemos los permisos del perfil
        public static void llenarPermisos(int roleId)
        {
            db = new suaEntities();
            roleFunciones = db.RoleFuncions.Where(x => x.roleId.Equals(roleId)
                && x.Funcion.tipo.Trim().Equals("A")).ToList();
        }

        //Verficamos si se tiene permiso al modulo función
        public static Boolean verificarPermiso(String modulo, String funcion)
        {
            Boolean perfilConPermiso = false;

            if (roleFunciones.Count() > 0)
            {
                RoleFuncion roleFuncion = roleFunciones
                    .Where(x => x.Funcion.descripcionCorta.Trim().Equals(modulo)
                     && x.Funcion.descripcionLarga.Trim().Equals(funcion)).FirstOrDefault();

                if (roleFuncion != null) {
                    perfilConPermiso = true;
                }
            }

            return perfilConPermiso;

        }

        public static void limpiarListaDePermisos(){
            if (roleFunciones.Count() > 0)
            {
                roleFunciones.Clear();
            }
        }
    }
}