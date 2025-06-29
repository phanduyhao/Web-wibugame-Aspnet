using accgame.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class nguoidungApiController : Controller
    {
        // GET: quantri/nguoidungApi
        public ActionResult Index()
        {
            ViewBag.urlApi = HttpContext.Application["url_api"]?.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult load()
        {
            using (var db = new accgameEntities())
            {
                var apiUrl = HttpContext.Application["url_api"]?.ToString();

                string content = "";
                // api version 2
                string url = apiUrl + "api/game.php";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                    return Json(content);
                }
               
            }
        }
    }
}