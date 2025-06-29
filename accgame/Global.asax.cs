using accgame.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace accgame
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["url_api"] = "http://wibugame.devshop.vn/";
        }

        protected void Session_Start()
        {
            Session["IDNguoiDung"] = 0;
            Session["levelAdmin"] = 0;
            Session["url"] = "/";
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            using (var db = new accgameEntities())
            {
                // Lấy cài đặt thời gian kết thúc Sale Tết
                var endSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "time_sale_end");
                if (endSetting != null && !string.IsNullOrEmpty(endSetting.NoiDung))
                {
                    // Chuyển đổi thời gian kết thúc từ chuỗi sang DateTime
                    DateTime endTime = DateTime.Parse(endSetting.NoiDung);

                    // Kiểm tra nếu thời gian hiện tại >= thời gian kết thúc
                    if (DateTime.Now >= endTime)
                    {
                        var isSaleTet = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "is_sale_tet");
                        if (isSaleTet != null && isSaleTet.NoiDung == "1") // Nếu Sale Tết đang bật
                        {
                            isSaleTet.NoiDung = "0"; // Tắt Sale Tết
                            db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                        }
                    }
                }
            }
        }


    }
}
