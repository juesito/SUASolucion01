using SUADATOS;
using System;
using System.Collections.Generic;

namespace SUAMVC.Models
{
    public class RoleFuncionesModel
    {
        public Role role { get; set; }
        public List<Funcion> funciones { get; set; }
        public List<Funcion> funcionesByRole { get; set; }


        
    }
}