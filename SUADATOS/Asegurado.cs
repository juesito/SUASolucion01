//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SUADATOS
{
    using System;
    using System.Collections.Generic;
    
    public partial class Asegurado
    {
        public Asegurado()
        {
            this.Incapacidades = new HashSet<Incapacidade>();
            this.Movimientos = new HashSet<Movimiento>();
            this.MovimientosAseguradoes = new HashSet<MovimientosAsegurado>();
            this.Pagos = new HashSet<Pago>();
        }
    
        public int id { get; set; }
        public string numeroAfiliacion { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string nombre { get; set; }
        public Nullable<decimal> salarioImss { get; set; }
        public Nullable<decimal> salarioInfo { get; set; }
        public System.DateTime fechaAlta { get; set; }
        public Nullable<System.DateTime> fechaBaja { get; set; }
        public string tipoTrabajo { get; set; }
        public string semanaJornada { get; set; }
        public string paginaInfo { get; set; }
        public string tipoDescuento { get; set; }
        public Nullable<decimal> valorDescuento { get; set; }
        public Nullable<int> ClienteId { get; set; }
        public string nombreTemporal { get; set; }
        public Nullable<System.DateTime> fechaDescuento { get; set; }
        public Nullable<System.DateTime> finDescuento { get; set; }
        public string articulo33 { get; set; }
        public Nullable<decimal> salarioArticulo33 { get; set; }
        public string trapeniv { get; set; }
        public string estado { get; set; }
        public string claveMunicipio { get; set; }
        public int PatroneId { get; set; }
        public string extranjero { get; set; }
        public string ocupacion { get; set; }
        public Nullable<System.DateTime> fechaCreacion { get; set; }
        public Nullable<System.DateTime> fechaModificacion { get; set; }
        public string alta { get; set; }
        public string baja { get; set; }
        public string modificacion { get; set; }
        public string permanente { get; set; }
        public int Plaza_id { get; set; }
        public Nullable<decimal> salarioDiario { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<Incapacidade> Incapacidades { get; set; }
        public virtual ICollection<Movimiento> Movimientos { get; set; }
        public virtual ICollection<MovimientosAsegurado> MovimientosAseguradoes { get; set; }
        public virtual Patrone Patrone { get; set; }
        public virtual Plaza Plaza { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }
}
