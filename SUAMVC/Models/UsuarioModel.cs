using SUADATOS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class UsuarioModel
    {

        private suaEntities db = new suaEntities();

        [Required]
        [Display(Name = "Usuario:")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordar en esta computadora")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public bool IsValid(string _username, string _password)
        {

            Boolean isExist = false;

            String sSQL = "SELECT * FROM Usuarios WHERE claveUsuario = @username " +
                " AND contrasena = @pass ";

            SqlParameter pNick = new SqlParameter("username", _username);
            SqlParameter pPass = new SqlParameter("pass", _password);

            object[] parameters = new object[]{pNick, pPass};


            Usuario user = db.Usuarios.SqlQuery(sSQL,parameters).FirstOrDefault();
            
            if (user != null)
            {
                isExist = true;
            }

            return isExist;
        }

        public Usuario extraeUsuario(string _username, string _password)
        { 
            String sSQL = "SELECT * FROM Usuarios WHERE claveUsuario = @username " +
                " AND contrasena = @pass ";

            SqlParameter pNick = new SqlParameter("username", _username);
            SqlParameter pPass = new SqlParameter("pass", _password);

            object[] parameters = new object[]{pNick, pPass};


            Usuario user = db.Usuarios.SqlQuery(sSQL,parameters).FirstOrDefault();

            return user;
        }

    }


    
}