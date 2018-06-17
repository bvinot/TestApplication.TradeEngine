using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace TradeEngine.UI.Controllers
{
    public class SecurityController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var prd = new List<object> { new { Thell = "sdf", Bell = "sss" }, new { Thell = "ddd", Cell = "asd" } };

            return Request.CreateResponse(HttpStatusCode.NotFound, JsonConvert.SerializeObject(prd));
        }
    }
}
