using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public String UserName { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Url { get; set; }
        public Menu ParentMenu { get; set; }
    }
}