using Microsoft.AspNet.SignalR; // Đảm bảo rằng bạn đã thêm đúng namespace này để sử dụng SignalR Hub

namespace accgame.Hubs  // Đảm bảo namespace của bạn đúng với thư mục chứa Hub
{
    public class NapGameHub : Hub
    {
        // Phương thức này sẽ được gọi từ client để gửi thông báo cho tất cả client đang kết nối
        public void NotifyClients()
        {
            // Gửi yêu cầu reload trang tới tất cả client đang kết nối
            Clients.All.reloadPage();  // "reloadPage" là tên phương thức mà client sẽ lắng nghe
        }
    }
}
