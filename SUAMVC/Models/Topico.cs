using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class Topico
    {
        public Topico() { 
        }
        public Topico(int id, String descripcion) {
            this.id = id;
            this.topico = descripcion;
        }
        public int id { get; set; }
        public String topico { get; set; }
    }
}