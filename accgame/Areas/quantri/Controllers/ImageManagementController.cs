using accgame;
using accgame.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace accgame.Areas.quantri.Controllers
{
    [RouteArea("quantri")]
    [RoutePrefix("delete-images-than-more-month")]
    public class ImageManagementController : Controller
    {
        // GET: quantri/Image
        [Route("")]
        public ActionResult Index()
        {
            using (var context = new accgameEntities())
            {
                var oneMonthAgo = DateTime.Now.AddMonths(-1);

                var imagesToDelete = context.Anh_Acc
                    .Where(a => a.Acc.DaBan == true && a.Acc.ThoiGianBan <= oneMonthAgo)
                    .ToList();

                string folderPath = Server.MapPath("~/Content/images/");
                List<string> paths = new List<string>();

                foreach (var image in imagesToDelete)
                {
                        // Lấy chỉ tên file
                        string fileName = Path.GetFileName(image.DuongDanAnh);
                        string imagePath = Path.Combine(folderPath, fileName);

                        paths.Add(imagePath); // lưu lại đường dẫn để hiển thị thử

                }

                ViewBag.ImagePaths = paths; // gửi list đường dẫn về View để kiểm tra
                return View();
            }
        }




        [HttpPost]
        [Route("delete-old-sold-images")]
        public JsonResult DeleteOldSoldAccImages()
        {
            try
            {
                using (var context = new accgameEntities())
                {
                    var oneMonthAgo = DateTime.Now.AddMonths(-1);

                    // Lấy các tài khoản đã bán hơn 1 tháng
                    var soldAccs = context.Accs
                        .Where(a => a.DaBan == true && a.ThoiGianBan.HasValue && a.ThoiGianBan <= oneMonthAgo)
                        .ToList();

                    if (!soldAccs.Any())
                    {
                        return Json(new { success = false, message = "Không có tài khoản nào đã bán hơn 1 tháng." });
                    }

                    // Lấy danh sách ảnh
                    var imageIdsToDelete = soldAccs
                        .SelectMany(a => a.Anh_Acc)
                        .Select(a => a.IDAnh_Acc)
                        .ToList();

                    var imagesToDelete = context.Anh_Acc
                        .Where(a => imageIdsToDelete.Contains(a.IDAnh_Acc))
                        .ToList();

                    if (!imagesToDelete.Any())
                    {
                        return Json(new { success = false, message = "Không có ảnh nào cần xóa." });
                    }

                    string folderPath = Server.MapPath("~/Content/images/");

                    foreach (var image in imagesToDelete)
                    {
                        try
                        {
                            string fileName = Path.GetFileName(image.DuongDanAnh);
                            string imagePath = Path.Combine(folderPath, fileName);
                                System.IO.File.Delete(imagePath);
                        }
                        catch (Exception fileEx)
                        {
                            // Ghi log nếu xóa file lỗi, nhưng không ngừng tiến trình
                            System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa ảnh {image.DuongDanAnh}: {fileEx.Message}");
                        }
                    }
                    context.SaveChanges();

                    return Json(new { success = true, message = $"Đã xóa {imagesToDelete.Count} ảnh thành công." });
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                System.Diagnostics.Debug.WriteLine("Lỗi toàn cục khi xóa ảnh: " + ex.Message);

                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

    }
}