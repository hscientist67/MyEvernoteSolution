using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp
{
    public class Exc : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            //Burada hatayı da aldık.
            filterContext.Controller.TempData["LastError"] = filterContext.Exception; 
            //Hatayı ben yöneteceğim.
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("/Home/HasError");
        }
    }
}