using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QT.Models
{
    public class ViewBagContainer
    {
        public string Info { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }

        public ViewBagContainer()
        {
            Info = "";
            Error = "";
            Message = "";
        }
    }
}