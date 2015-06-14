using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace SUAMVC.Helpers
{
    public static class StatusDrownList
    {
        public static MvcHtmlString SearchStatusList(this HtmlHelper htmlHelper)
        {
            return htmlHelper.DropDownList("statusId", new List<SelectListItem>() { new SelectListItem { Text = "Todos", Value = "" },  new SelectListItem { Text = "Activo", Value = "A" }, new SelectListItem { Text = "Inactivo", Value = "B" } }, new { onchange = "form.submit();" });
        }

        public static MvcHtmlString FieldsAvailablesToFilter(this HtmlHelper htmlHelper)
        {

            List<SelectListItem> listFields = new List<SelectListItem> {
                              new SelectListItem {Value = "", Text = "Seleccione"},
                              new SelectListItem {Value = "1", Text = "Reg. Patronal"},
                              new SelectListItem {Value = "2", Text = "Num. Afiliación"},
                              new SelectListItem {Value = "3", Text = "CURP"},
                              new SelectListItem {Value = "4", Text = "RFC"},
                              new SelectListItem {Value = "5", Text = "Nombre"},
                              new SelectListItem {Value = "6", Text = "Fecha Alta"},
                              new SelectListItem {Value = "7", Text = "Fecha Baja"},
                              new SelectListItem {Value = "8", Text = "Salario IMMS"},
                              new SelectListItem {Value = "9", Text = "Ubicación"},
                              new SelectListItem {Value = "10", Text = "ID Grupo"},
                              new SelectListItem {Value = "11", Text = "Ocupación"},
                              new SelectListItem {Value = "12", Text = "Plaza"},
                              new SelectListItem {Value = "13", Text = "Extranjero?"}
            };
             
            return htmlHelper.DropDownList("opcion", listFields);
        }

        public static MvcHtmlString FieldsAvailablesToFilterAcreditados(this HtmlHelper htmlHelper)
        {

            List<SelectListItem> listFields = new List<SelectListItem> {
                              new SelectListItem {Value = "", Text = "Seleccione"},
                              new SelectListItem {Value = "1", Text = "Reg. Patronal"},
                              new SelectListItem {Value = "2", Text = "Num. Afiliación"},
                              new SelectListItem {Value = "3", Text = "CURP"},
                              new SelectListItem {Value = "4", Text = "RFC"},
                              new SelectListItem {Value = "5", Text = "Nombre"},
                              new SelectListItem {Value = "6", Text = "Fecha Alta"},
                              new SelectListItem {Value = "7", Text = "Fecha Baja"},
                              new SelectListItem {Value = "8", Text = "Salario IMMS"},
                              new SelectListItem {Value = "9", Text = "Ubicación"},
                              new SelectListItem {Value = "10", Text = "ID Grupo"},
                              new SelectListItem {Value = "11", Text = "Ocupación"},
                              new SelectListItem {Value = "12", Text = "Plaza"}
            };
             
            return htmlHelper.DropDownList("opcion", listFields);
        }

        public static MvcHtmlString topicosList(this HtmlHelper htmlHelper)
        {

            return htmlHelper.DropDownList("topico", new List<SelectListItem>() { 
                new SelectListItem { Text = "Seleccione", Value = "" }, 
                new SelectListItem { Text = "Cliente", Value = "C" }, 
                new SelectListItem { Text = "Grupos", Value = "G" }, 
                new SelectListItem { Text = "Patrones", Value = "B" },
                new SelectListItem { Text = "Plaza", Value = "P" }}, new { onchange = "form.submit();" });
        }

        public static MvcHtmlString periodosList(this HtmlHelper htmlHelper)
        {

            return htmlHelper.DropDownList("periodoId", new List<SelectListItem>() { 
                new SelectListItem { Text = "Seleccione", Value = "" }, 
                new SelectListItem { Text = "Enero", Value = "01" }, 
                new SelectListItem { Text = "Febrero", Value = "02" }, 
                new SelectListItem { Text = "Marzo", Value = "03" },
                new SelectListItem { Text = "Abril", Value = "04" },
                new SelectListItem { Text = "Mayo", Value = "05" },
                new SelectListItem { Text = "Junio", Value = "06" },
                new SelectListItem { Text = "Julio", Value = "07" },
                new SelectListItem { Text = "Agosto", Value = "08" },
                new SelectListItem { Text = "Septiembre", Value = "09" },
                new SelectListItem { Text = "Octubre", Value = "10" },
                new SelectListItem { Text = "Noviembre", Value = "11" },
                new SelectListItem { Text = "Diciembre", Value = "12" }
            });
        }

        public static MvcHtmlString ejercicioList(this HtmlHelper htmlHelper)
        {

            return htmlHelper.DropDownList("ejercicioId", new List<SelectListItem>() { 
                new SelectListItem { Text = "Seleccione", Value = "" }, 
                new SelectListItem { Text = "2010", Value = "2010" }, 
                new SelectListItem { Text = "2011", Value = "2011" }, 
                new SelectListItem { Text = "2012", Value = "2012" },
                new SelectListItem { Text = "2013", Value = "2013" },
                new SelectListItem { Text = "2014", Value = "2014" },
                new SelectListItem { Text = "2015", Value = "2015" },
                new SelectListItem { Text = "2016", Value = "2016" },
                new SelectListItem { Text = "2017", Value = "2017" },
                new SelectListItem { Text = "2018", Value = "2018" },
                new SelectListItem { Text = "2019", Value = "2019" },
                new SelectListItem { Text = "2020", Value = "2020" },
                new SelectListItem { Text = "2021", Value = "2021" }
            });
        }
    }
}