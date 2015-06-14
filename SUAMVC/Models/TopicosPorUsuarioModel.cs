using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class TopicosPorUsuarioModel
    {

        public Usuario usuario{get; set;}
        public String ltopico { get; set; }
        public List<Topico> topicos { get; set; }
        public List<Topico> topicosPorUsuario { get; set; }

    }
}