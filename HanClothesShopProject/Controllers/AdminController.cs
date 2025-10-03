using HanClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HanClothesShopProject.Controllers
{
    public class AdminController : Controller
    {
        //自動注入資料庫上下文服務到這個類別
        private readonly dbContext _dbContext;
        //HttpContextAccessor的接口 添加依賴
        private readonly IHttpContextAccessor _httpContextAccessor;
        //注入
        public AdminController(dbContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
        }
        //後台管理首頁訊息展示
        public IActionResult Index()
        {
            //今日平台收入
            var todayStart=DateTime.Now;
            var endday=todayStart.AddDays(1);//明日
            ViewBag.todayMoney = _dbContext.Orders.Where(p => p.IsPay == 1 && p.Createtime >= todayStart && p.Createtime <= endday)
                                                  .Sum(p => (Decimal?)p.SumPrice) ?? 0;//加總 轉型為 Decimal? 避免因 null 值拋出例外 
            //本周
            DateTime dt=DateTime.Now;
            DateTime startWeek=dt.AddDays((int)dt.DayOfWeek+1).Date;//周一
            DateTime endWeek = startWeek.AddDays(7).AddMilliseconds(-1);//週日
            ViewBag.startWeek = startWeek;
            ViewBag.endWeek = endWeek;
             ViewBag.weekMoney= _dbContext.Orders.Where(p => p.IsPay == 1 && p.Createtime >= startWeek && p.Createtime <= endWeek)
                                                  .Sum(p => (Decimal?)p.SumPrice) ?? 0;
            //今年
            DateTime yearStart = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);//開始由 年,月,日,時,分,秒
            DateTime yearEnd   = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);//開始由 年,月,日,時,分,秒
            ViewBag.yearMoney= _dbContext.Orders.Where(p => p.IsPay == 1 && p.Createtime >= yearStart && p.Createtime <= yearEnd)
                                                  .Sum(p => (Decimal?)p.SumPrice) ?? 0;
            //今日訂單取回來 取10條
            ViewBag.orderList=_dbContext.Orders.Include(p=>p.UidNavigation)
                                               .Where(p=>p.Createtime>=todayStart&&p.Createtime<=endday)
                                               .OrderByDescending(p=>p.Id)
                                               .Take(10)
                                               .ToList();
            return View();
        }
    }
}
