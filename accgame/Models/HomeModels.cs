using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using accgame.Context;
using PagedList;

namespace accgame.Models
{
    public class HomeModels
    {
        public List<NhanVat> listNhanVat { get; set; }
        public List<VuKhi> listVuKhi { get; set; }
        public Acc acc { get; set;}
        public List<Anh_Acc> listAnh_Acc { get; set; }
        public List<Acc> listAccGS { get; set; }
        public List<Acc> listAccHK { get; set; }
        public List<Acc> listAcc { get; set; }
        public List<DanhMuc> listDanhMuc { get; set; }
        public List<AccRandom> listAccRandom { get; set; }
        public List<AccRandomDamua> listAccRandomDamua { get; set; }
        public List<LoaiCode> listLoaiCode { get; set; }
        public List<NguoiDung> listNguoiDung { get; set; }
        public List<LoaiGame> listLoaiGame { get; set; }
        public List<LoaiAcc> listLoaiAcc { get; set; }

        public IPagedList<Code> listCode { get; set; }
        public IPagedList<Acc> listPageAcc { get; set; }
        public string[] nhanvat { get; set; }
        public string[] vukhi { get; set; }
        public string CapKhaiPha { get; set; }
        public int? timtheogia { get; set; }
        public string timtheoma { get; set; }
        public int? sapxep { get; set; }
    }
}