using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeEngine.UI.Auth_Data
{
    public class LocalCacheAttribute : OutputCacheAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
           
            filterContext.Result = new HttpNotFoundResult("Not Found");
        }
    }
}