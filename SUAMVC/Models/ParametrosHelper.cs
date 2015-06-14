using SUADATOS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class ParametrosHelper
    {
        private suaEntities db = new suaEntities();

        public Parametro getParameterByKey(String key) {
            Parametro parameter = new Parametro();
            DateTime date = DateTime.Now;

            var smdfParameterTemp = (from b in db.Parametros
                                    where b.parametroId.Equals(key.Trim())
                                    orderby b.fechaCreacion
                                    select b).FirstOrDefault();

            if (smdfParameterTemp == null)
            {
                parameter = new Parametro();
                if (!key.Equals("FACINFOSAL"))
                {
                    parameter.descripcion = "No Value";
                    parameter.valorMoneda = Decimal.Parse("0.00");
                    parameter.fechaCreacion = date;
                }
                else
                {

                    parameter.parametroId = "FACINFOSAL";
                    parameter.descripcion = "Ultima fecha de actualizacion de datos infonavit";
                    parameter.fechaCreacion = date;
                    parameter.valorFecha = date.AddYears(-1);
                    db.Parametros.Add(parameter);
                    db.SaveChanges();
                }
            }
            else {
                parameter = (Parametro)smdfParameterTemp;
            }

            return parameter;

        }

        /**
         * Actualizamos el parametro de fecha de actualizacion infonavit. 
         * 
         */
        public void actualizaFechaActualizacionInfonavit() {
            DateTime date = DateTime.Now;
            // Consultamos el parametro FACINFOSAL
            var parameter =
                (from c in db.Parametros
                 where c.parametroId == "FACINFOSAL"
                 select c).First();

            if (parameter != null)
            {
                
                parameter.valorFecha = date;
                db.Entry(parameter).State = EntityState.Modified;
                db.SaveChanges();
            }
            else {
                parameter = new Parametro();
                parameter.parametroId = "FACINFOSAL";
                parameter.descripcion = "Ultima fecha de actualizacion de datos infonavit";
                parameter.fechaCreacion = date;
                parameter.valorFecha = date.AddYears(-1); ;
                db.Parametros.Add(parameter);
                db.SaveChanges();
            }
        }

    }
}