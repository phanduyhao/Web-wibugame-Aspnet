using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(accgame.Startup))]

namespace accgame
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình CORS để cho phép tất cả các yêu cầu
            app.UseCors(CorsOptions.AllowAll);

            // Cấu hình SignalR và sử dụng fallback transport nếu WebSocket không khả dụng
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true, // Cho phép thông báo lỗi chi tiết
                // Chọn transport fallback khi WebSocket không khả dụng
                EnableJavaScriptProxies = true
            };

            // Đảm bảo sử dụng đúng URL cho SignalR
            app.MapSignalR("/signalr", hubConfiguration);
        }
    }
}
