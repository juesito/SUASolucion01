using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class AseguradoMovimientosModel
    {
        public Movimiento movimiento { get; set; }
        public List<Asegurado> allAsegurado { get; set; }
    }
}