using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeEngine.UI.Auth_Data
{
    public class AuthAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.Controller.ViewBag.Content == null)
            {
              //  filterContext.Result = new RedirectResult("/Home/Extract");
            }
        }
    }
}