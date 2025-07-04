USE [master]
GO
/****** Object:  Database [wibugame]    Script Date: 6/29/2025 11:40:31 PM ******/
CREATE DATABASE [wibugame]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'wibugame', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\wibugame.mdf' , SIZE = 204800KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'wibugame_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\wibugame_log.ldf' , SIZE = 204800KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [wibugame] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [wibugame].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [wibugame] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [wibugame] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [wibugame] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [wibugame] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [wibugame] SET ARITHABORT OFF 
GO
ALTER DATABASE [wibugame] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [wibugame] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [wibugame] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [wibugame] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [wibugame] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [wibugame] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [wibugame] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [wibugame] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [wibugame] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [wibugame] SET  DISABLE_BROKER 
GO
ALTER DATABASE [wibugame] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [wibugame] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [wibugame] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [wibugame] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [wibugame] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [wibugame] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [wibugame] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [wibugame] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [wibugame] SET  MULTI_USER 
GO
ALTER DATABASE [wibugame] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [wibugame] SET DB_CHAINING OFF 
GO
ALTER DATABASE [wibugame] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [wibugame] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [wibugame] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [wibugame] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [wibugame] SET QUERY_STORE = ON
GO
ALTER DATABASE [wibugame] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [wibugame]
GO
/****** Object:  Table [dbo].[Acc]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Acc](
	[IDAcc] [int] IDENTITY(1,1) NOT NULL,
	[TenAcc] [nvarchar](500) NULL,
	[Server] [nvarchar](100) NULL,
	[MaTaiKhoan] [nvarchar](50) NULL,
	[ChiTiet] [nvarchar](max) NULL,
	[Gia] [int] NULL,
	[IDNguoiDung] [int] NULL,
	[IDNguoiMua] [int] NULL,
	[ThoiGianDang] [datetime] NULL,
	[ThoiGianBan] [datetime] NULL,
	[DaBan] [bit] NULL,
	[Xoa] [bit] NULL,
	[AnhDaiDien] [nvarchar](350) NULL,
	[NhanVat] [nvarchar](1000) NULL,
	[VuKhi] [nvarchar](1000) NULL,
	[LoaiGame] [int] NULL,
	[TaiKhoan] [nvarchar](500) NULL,
	[MatKhau] [nvarchar](500) NULL,
	[ThongTinKhac] [nvarchar](500) NULL,
	[CapKhaiPha] [nvarchar](50) NULL,
	[TaiKhoanNoiBo] [bit] NULL,
	[AccVip] [bit] NULL,
	[AccKhoiDau] [bit] NULL,
	[RanDom] [bit] NULL,
	[PhanTram] [int] NULL,
	[MaGiamGia] [nvarchar](150) NULL,
	[SoTienGiam] [int] NULL,
	[AnhNhanVat] [nvarchar](max) NULL,
	[TenNhanVat] [nvarchar](max) NULL,
	[AnhVuKhi] [nvarchar](max) NULL,
	[TenVuKhi] [nvarchar](max) NULL,
	[IDLoaiAcc] [int] NULL,
	[IDOldAcc] [int] NULL,
	[GiaCtvCollab] [int] NULL,
	[GiaDaMua] [int] NULL,
	[GiaGoc] [int] NULL,
	[An] [bit] NULL,
 CONSTRAINT [PK_Acc] PRIMARY KEY CLUSTERED 
(
	[IDAcc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccRandom]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccRandom](
	[IDAccRandom] [int] IDENTITY(1,1) NOT NULL,
	[IDDanhMuc] [int] NULL,
	[TaiKhoan] [nvarchar](500) NULL,
	[MatKhau] [nvarchar](500) NULL,
	[ThongTinKhac] [nvarchar](500) NULL,
	[IDNguoiDung] [int] NULL,
	[Xoa] [bit] NULL,
	[DaBan] [bit] NULL,
	[IDNguoiMua] [int] NULL,
	[ThoiGianBan] [datetime] NULL,
	[IDOldAcc] [int] NULL,
	[GiaDaMua] [int] NULL,
	[ThoiGianDang] [datetime] NULL,
	[MatKhauOutlook] [nvarchar](500) NULL,
	[MailKhoiPhuc] [nvarchar](500) NULL,
	[MatKhauMailKhoiPhuc] [nvarchar](500) NULL,
	[MatKhauMail] [nvarchar](500) NULL,
 CONSTRAINT [PK_AccRandom] PRIMARY KEY CLUSTERED 
(
	[IDAccRandom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccRandomDamua]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccRandomDamua](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Danhmuc] [nvarchar](500) NULL,
	[IdAccRandom] [int] NULL,
	[Server] [nvarchar](50) NULL,
	[Ghichu] [nvarchar](max) NULL,
 CONSTRAINT [PK_AccRandomDamua] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminCongTien]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminCongTien](
	[IdAdminCongTien] [int] IDENTITY(1,1) NOT NULL,
	[SoTien] [bigint] NULL,
	[IDNguoiDung] [int] NULL,
	[IDNguoiCong] [int] NULL,
	[Lydo] [nvarchar](550) NULL,
	[ThoiGian] [datetime] NULL,
	[CongTien] [bit] NULL,
	[SoDuTruocCong] [bigint] NULL,
	[SoDuSauCong] [bigint] NULL,
	[Status] [bit] NULL,
	[HuyDon] [bit] NULL,
 CONSTRAINT [PK_AdminCongTien] PRIMARY KEY CLUSTERED 
(
	[IdAdminCongTien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminNotifyRead]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminNotifyRead](
	[IdStatusAdminNotify] [int] IDENTITY(1,1) NOT NULL,
	[IdNotifyUser] [int] NULL,
	[IdNguoidung] [int] NULL,
 CONSTRAINT [PK_AdminNotifyRead] PRIMARY KEY CLUSTERED 
(
	[IdStatusAdminNotify] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnAcc]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnAcc](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AnAccVip] [bit] NULL,
	[AnAccRR] [bit] NULL,
	[IdUser] [int] NULL,
	[IsAdminAn] [bit] NULL,
 CONSTRAINT [PK_AnAcc] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anh_Acc]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anh_Acc](
	[IDAnh_Acc] [int] IDENTITY(1,1) NOT NULL,
	[DuongDanAnh] [nvarchar](500) NULL,
	[IDAcc] [int] NULL,
	[IDNguoiDung] [int] NULL,
	[AnhDaiDien] [bit] NULL,
	[ThoiGian] [datetime] NULL,
 CONSTRAINT [PK_Anh_Acc] PRIMARY KEY CLUSTERED 
(
	[IDAnh_Acc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApiAcc]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiAcc](
	[IdApi] [int] IDENTITY(1,1) NOT NULL,
	[Website] [nvarchar](150) NULL,
	[IDAcc] [int] NULL,
	[Name] [nvarchar](250) NULL,
	[Price] [int] NULL,
	[Time] [datetime] NULL,
 CONSTRAINT [PK_ApiAcc] PRIMARY KEY CLUSTERED 
(
	[IdApi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Banner]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banner](
	[IDBanner] [int] IDENTITY(1,1) NOT NULL,
	[DuongDan] [nvarchar](150) NULL,
	[STT] [int] NULL,
 CONSTRAINT [PK_Banner] PRIMARY KEY CLUSTERED 
(
	[IDBanner] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BienDongSoDu]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BienDongSoDu](
	[IDBienDongSoDu] [int] IDENTITY(1,1) NOT NULL,
	[SoTien] [bigint] NULL,
	[TruTien] [bit] NULL,
	[IDNguoiDung] [int] NULL,
	[LoiNhan] [nvarchar](150) NULL,
	[ThoiGian] [datetime] NULL,
	[TienTruoc] [bigint] NULL,
	[TienSau] [bigint] NULL,
 CONSTRAINT [PK_BienDongSoDu] PRIMARY KEY CLUSTERED 
(
	[IDBienDongSoDu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CaiDat]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaiDat](
	[IDCaiDat] [int] IDENTITY(1,1) NOT NULL,
	[NoiDung] [nvarchar](4000) NULL,
	[MaCaiDat] [nvarchar](50) NULL,
	[ThoiGianHet] [datetime] NULL,
	[ThoiGianCapNhat] [datetime] NULL,
 CONSTRAINT [PK_CaiDat] PRIMARY KEY CLUSTERED 
(
	[IDCaiDat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CaiDatVongQuay]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CaiDatVongQuay](
	[IDCaiDatVongQuay] [int] IDENTITY(1,1) NOT NULL,
	[TyLe1] [float] NULL,
	[TyLe2] [float] NULL,
	[TyLe3] [float] NULL,
	[TyLe4] [float] NULL,
	[TyLe5] [float] NULL,
	[TyLe6] [float] NULL,
	[Gia] [int] NULL,
	[LoaiVong] [int] NULL,
 CONSTRAINT [PK_CaiDatVongQuay] PRIMARY KEY CLUSTERED 
(
	[IDCaiDatVongQuay] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CayThue]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CayThue](
	[IDCayThue] [int] IDENTITY(1,1) NOT NULL,
	[IDNguoiDung] [int] NULL,
	[IDGoiNhiemVu] [int] NULL,
	[SoTien] [int] NULL,
	[UID] [nvarchar](150) NULL,
	[TenDangNhap] [nvarchar](450) NULL,
	[MatKhau] [nvarchar](450) NULL,
	[Server] [nvarchar](250) NULL,
	[TenNhanVat] [nvarchar](250) NULL,
	[SoDienThoai] [nvarchar](50) NULL,
	[GhiChu] [nvarchar](1500) NULL,
	[HoanThanh] [bit] NULL,
	[ThoiGianHoanThanh] [datetime] NULL,
	[ThoiGianGui] [datetime] NULL,
	[LoaiGame] [int] NULL,
	[IDNguoiNhan] [int] NULL,
 CONSTRAINT [PK_CayThue] PRIMARY KEY CLUSTERED 
(
	[IDCayThue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Code]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Code](
	[IDCode] [int] IDENTITY(1,1) NOT NULL,
	[GiaTri] [int] NULL,
	[ThoiGianMua] [datetime] NULL,
	[MaCode] [nvarchar](150) NULL,
	[IDLoaiCode] [int] NULL,
	[IDNguoiDung] [int] NULL,
	[DaBan] [bit] NULL,
	[Xoa] [bit] NULL,
 CONSTRAINT [PK_Code] PRIMARY KEY CLUSTERED 
(
	[IDCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DanhMuc]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DanhMuc](
	[IDDanhMuc] [int] IDENTITY(1,1) NOT NULL,
	[LoaiGame] [int] NULL,
	[TenDanhMuc] [nvarchar](500) NULL,
	[DanhMucLevel] [nvarchar](50) NULL,
	[LuotRoll] [nvarchar](50) NULL,
	[HienCo] [int] NULL,
	[DaBan] [int] NULL,
	[Gia] [int] NULL,
	[Xoa] [bit] NULL,
	[AnhDanhMuc] [nvarchar](500) NULL,
	[TaiKhoanNoiBo] [bit] NULL,
	[MoTa] [nvarchar](500) NULL,
	[STT] [int] NULL,
	[IDNguoiDung] [int] NULL,
	[random] [bit] NULL,
	[IDLoaiAcc] [int] NULL,
	[Server] [nvarchar](50) NULL,
	[slug] [nvarchar](150) NULL,
	[IDOldDanhMuc] [int] NULL,
	[Ar] [nvarchar](50) NULL,
	[GiaCtvCollab] [int] NULL,
	[ThongBaoMuaAcc] [nvarchar](500) NULL,
	[GhiChuAcc] [nvarchar](max) NULL,
	[CauHinhDangNhieuAcc] [nvarchar](1000) NULL,
	[Hide] [bit] NULL,
 CONSTRAINT [PK_DanhMuc] PRIMARY KEY CLUSTERED 
(
	[IDDanhMuc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GiaiThuongVongQuay]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GiaiThuongVongQuay](
	[IDGiaiThuongVongQuay] [int] IDENTITY(1,1) NOT NULL,
	[DaBan] [bit] NULL,
	[ThongTin] [nvarchar](350) NULL,
	[ThoiGianBan] [datetime] NULL,
	[LoaiGiai] [int] NULL,
 CONSTRAINT [PK_GiaiThuongVongQuay] PRIMARY KEY CLUSTERED 
(
	[IDGiaiThuongVongQuay] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiNap]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiNap](
	[IDGoiNap] [int] IDENTITY(1,1) NOT NULL,
	[Gia] [int] NULL,
	[TenGoi] [nvarchar](300) NULL,
	[Xoa] [bit] NULL,
	[SoThuTu] [int] NULL,
	[LoaiGame] [int] NULL,
	[Anh] [nvarchar](255) NULL,
	[LoaiNap] [int] NULL,
	[LinkHdUID] [nvarchar](255) NULL,
	[X2LanDau] [bit] NULL,
	[An] [bit] NULL,
	[ThoigianAn] [datetime] NULL,
 CONSTRAINT [PK_GoiNap] PRIMARY KEY CLUSTERED 
(
	[IDGoiNap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiNhiemVu]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiNhiemVu](
	[IDGoiNhiemVu] [int] IDENTITY(1,1) NOT NULL,
	[TenGoiNhiemVu] [nvarchar](500) NULL,
	[GiaTien] [int] NULL,
	[LoaiGame] [int] NULL,
	[Xoa] [bit] NULL,
 CONSTRAINT [PK_GoiNhiemVu] PRIMARY KEY CLUSTERED 
(
	[IDGoiNhiemVu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Image]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[IDImage] [int] IDENTITY(1,1) NOT NULL,
	[IDNguoiDung] [int] NULL,
	[DuongDan] [nvarchar](100) NULL,
	[ThoiGian] [datetime] NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[IDImage] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiAcc]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiAcc](
	[IDLoaiAcc] [int] IDENTITY(1,1) NOT NULL,
	[TenLoaiAcc] [nvarchar](150) NULL,
	[DangBan] [int] NULL,
	[DaBan] [int] NULL,
	[Image] [nvarchar](500) NULL,
	[slug] [nvarchar](150) NULL,
	[DacBiet] [bit] NULL,
	[IDLoaiGame] [int] NULL,
	[STT] [int] NULL,
	[Hide] [bit] NULL,
	[MoTa] [nvarchar](500) NULL,
	[Server] [nvarchar](50) NULL,
	[IDLoaiAccCha] [int] NULL,
 CONSTRAINT [PK_LoaiAcc] PRIMARY KEY CLUSTERED 
(
	[IDLoaiAcc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiCode]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiCode](
	[IDLoaiCode] [int] IDENTITY(1,1) NOT NULL,
	[TenLoaiCode] [nvarchar](500) NULL,
	[Gia] [int] NULL,
	[Xoa] [bit] NULL,
	[CodeNoiBo] [bit] NULL,
	[LoaiGame] [int] NULL,
 CONSTRAINT [PK_LoaiCode] PRIMARY KEY CLUSTERED 
(
	[IDLoaiCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiGame]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiGame](
	[IDLoaiGame] [int] IDENTITY(1,1) NOT NULL,
	[TenLoaiGame] [nvarchar](150) NULL,
	[Slug] [nvarchar](150) NULL,
	[Image] [nvarchar](500) NULL,
	[Hide] [bit] NULL,
	[STT] [int] NULL,
	[AnhNapGame] [nvarchar](150) NULL,
	[AnhCayThue] [nvarchar](150) NULL,
	[DichVuCayThue] [bit] NULL,
	[DichVuNapGame] [bit] NULL,
	[DichVuCode] [bit] NULL,
	[AnhCode] [nvarchar](150) NULL,
 CONSTRAINT [PK_LoaiGame] PRIMARY KEY CLUSTERED 
(
	[IDLoaiGame] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NapGame]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NapGame](
	[IDNapGame] [int] IDENTITY(1,1) NOT NULL,
	[IDNguoiDung] [int] NULL,
	[SoTien] [int] NULL,
	[UID] [nvarchar](150) NULL,
	[TenDangNhap] [nvarchar](450) NULL,
	[MatKhau] [nvarchar](450) NULL,
	[Server] [nvarchar](250) NULL,
	[TenNhanVat] [nvarchar](250) NULL,
	[SoDienThoai] [nvarchar](50) NULL,
	[GhiChu] [nvarchar](1500) NULL,
	[HoanThanh] [bit] NULL,
	[ThoiGianHoanThanh] [datetime] NULL,
	[ThoiGianGui] [datetime] NULL,
	[LoaiGame] [int] NULL,
	[IDGoiNap] [int] NULL,
	[IDNguoiDuyet] [int] NULL,
	[SoLuong] [int] NULL,
	[AdminEdit] [bit] NULL,
	[IDNguoiPick] [int] NULL,
 CONSTRAINT [PK_NapGame] PRIMARY KEY CLUSTERED 
(
	[IDNapGame] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NapTien]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NapTien](
	[IdNapTien] [int] IDENTITY(1,1) NOT NULL,
	[IDNguoiDung] [int] NULL,
	[Magd] [nvarchar](250) NULL,
	[Noidung] [nvarchar](250) NULL,
	[SoTien] [bigint] NULL,
	[thoigian] [datetime] NULL,
	[request_id] [nvarchar](50) NULL,
	[code] [nvarchar](50) NULL,
	[serial] [nvarchar](50) NULL,
	[amount] [int] NULL,
	[telco] [nvarchar](50) NULL,
	[trangthai] [bit] NULL,
	[atm] [bit] NULL,
	[TruocNap] [bigint] NULL,
	[SauNap] [bigint] NULL,
	[IDNguoiDuyet] [int] NULL,
 CONSTRAINT [PK_NapTien] PRIMARY KEY CLUSTERED 
(
	[IdNapTien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NguoiDung]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NguoiDung](
	[IDNguoiDung] [int] IDENTITY(1,1) NOT NULL,
	[TenNguoiDung] [nvarchar](500) NULL,
	[MatKhau] [nvarchar](500) NULL,
	[Email] [nvarchar](500) NULL,
	[Tien] [bigint] NULL,
	[NgayThamGia] [datetime] NULL,
	[LeverAdmin] [int] NULL,
	[MatMa] [int] NULL,
	[ThoiVang] [int] NULL,
	[TienNap] [int] NULL,
	[HoaHong] [int] NULL,
	[TienNapThang] [int] NULL,
	[Phantramhoahong] [float] NULL,
	[AnhDaiDien] [nvarchar](50) NULL,
	[OldPass] [nvarchar](150) NULL,
	[GoogleAccount] [bit] NULL,
	[OldAccount] [bit] NULL,
	[CtvCollab] [bit] NULL,
	[OnOffCtvCollab] [bit] NULL,
	[SessionID] [nvarchar](100) NULL,
	[MatKhau2] [nvarchar](500) NULL,
	[FailedLoginAttempts] [int] NULL,
	[LockoutEndTime] [datetime] NULL,
	[Block] [bit] NULL,
	[BlockContent] [nvarchar](max) NULL,
	[ResetPasswordToken] [nvarchar](150) NULL,
	[ResetTokenExpiry] [datetime] NULL,
	[SessionID_Desktop] [nvarchar](50) NULL,
	[SessionID_Mobile] [nvarchar](50) NULL,
	[FailedPasswordRequests] [int] NULL,
	[LastPasswordRequestTime] [datetime] NULL,
	[LockedUntil] [datetime] NULL,
 CONSTRAINT [PK_NguoiDung] PRIMARY KEY CLUSTERED 
(
	[IDNguoiDung] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhanVat]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhanVat](
	[IDNhanVat] [int] IDENTITY(1,1) NOT NULL,
	[TenNhanVat] [nvarchar](500) NULL,
	[LoaiGame] [int] NULL,
	[Avatar] [nvarchar](250) NULL,
	[Sao] [int] NULL,
 CONSTRAINT [PK_NhanVat] PRIMARY KEY CLUSTERED 
(
	[IDNhanVat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotifyUser]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotifyUser](
	[IdNotify] [int] IDENTITY(1,1) NOT NULL,
	[Contents] [nvarchar](1000) NULL,
	[IsRead] [bit] NULL,
	[IdNguoidung] [int] NULL,
	[IsAdminPost] [bit] NULL,
	[IdAdmin] [int] NULL,
	[Thoigian] [datetime] NULL,
 CONSTRAINT [PK_NotifyUser] PRIMARY KEY CLUSTERED 
(
	[IdNotify] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OldPass]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OldPass](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[oldPassword] [nvarchar](500) NULL,
	[IDNguoiDung] [int] NULL,
 CONSTRAINT [PK_OldPass] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RutTien]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RutTien](
	[IDRutTien] [int] IDENTITY(1,1) NOT NULL,
	[IDNguoiDung] [int] NULL,
	[SoTienRut] [bigint] NULL,
	[TrangThai] [bit] NULL,
	[ThoiGian] [datetime] NULL,
	[SoTienNhan] [bigint] NULL,
	[NganHang] [nvarchar](500) NULL,
	[NguoiNhan] [nvarchar](500) NULL,
	[SoTaiKhoan] [nvarchar](150) NULL,
 CONSTRAINT [PK_RutTien] PRIMARY KEY CLUSTERED 
(
	[IDRutTien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SettingApi]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SettingApi](
	[IDSettingApi] [int] IDENTITY(1,1) NOT NULL,
	[DomainWebsite] [nvarchar](150) NULL,
	[ApiKey] [nvarchar](50) NULL,
 CONSTRAINT [PK_SettingApi] PRIMARY KEY CLUSTERED 
(
	[IDSettingApi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Video]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Video](
	[IDVideo] [int] IDENTITY(1,1) NOT NULL,
	[DuongDan] [nvarchar](150) NULL,
 CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED 
(
	[IDVideo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VongQuay]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VongQuay](
	[IDVongQuay] [int] IDENTITY(1,1) NOT NULL,
	[IDNguoiDung] [int] NULL,
	[LoaiGiai] [int] NULL,
	[GiaTien] [int] NULL,
	[NoiDung] [nvarchar](350) NULL,
	[ThoiGian] [datetime] NULL,
	[IDCaiDatVongQuay] [int] NULL,
 CONSTRAINT [PK_VongQuay] PRIMARY KEY CLUSTERED 
(
	[IDVongQuay] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VuKhi]    Script Date: 6/29/2025 11:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VuKhi](
	[IDVuKhi] [int] IDENTITY(1,1) NOT NULL,
	[TenVuKhi] [nvarchar](500) NULL,
	[LoaiGame] [int] NULL,
	[Avatar] [nvarchar](250) NULL,
	[Sao] [int] NULL,
 CONSTRAINT [PK_VuKhi] PRIMARY KEY CLUSTERED 
(
	[IDVuKhi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [wibugame] SET  READ_WRITE 
GO
