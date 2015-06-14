using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class RoleModulosModel
    {
        public Role role { get; set; }
        public List<Modulo> modulos {get; set;}
        public List<Modulo> modulosByRole { get; set; }
    }
}