using HanClothesShopProject.CommonUtil;
using HanClothesShopProject.Filter;
using HanClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HanClothesShopProject.Controllers
{
    [AdminAuthen]
    public class AdminController : Controller
    {
        // 自動注入資料庫上下文服務到這個類別
        private readonly dbContext _dbContext;
        // HttpContextAccessor 的介面，添加依賴
        private readonly IHttpContextAccessor _httpContextAccessor;

        // 注入
        public AdminController(dbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        // 後台管理首頁訊息展示
        public IActionResult Index()
        {
            // 今日平台收入
            var todayStart = DateTime.Now.Date;
            var endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59); // 明日
            ViewBag.todayMoney = _dbContext.Orders
                .Where(p => p.IsPay == 1 && p.Createtime >= todayStart && p.Createtime <= endday)
                .Sum(p => (decimal?)p.SumPrice) ?? 0; // 加總，轉型為 decimal? 避免因 null 值拋出例外 

            // 本週
            DateTime dt = DateTime.Now;
            DateTime startWeek = dt.AddDays((int)dt.DayOfWeek + 1).Date; // 周一
            DateTime endWeek = startWeek.AddDays(7).AddMilliseconds(-1); // 週日
            ViewBag.startWeek = startWeek;
            ViewBag.endWeek = endWeek;
            ViewBag.weekMoney = _dbContext.Orders
                .Where(p => p.IsPay == 1 && p.Createtime >= startWeek && p.Createtime <= endWeek)
                .Sum(p => (decimal?)p.SumPrice) ?? 0;

            // 今年
            DateTime yearStart = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0); // 開始由 年,月,日,時,分,秒
            DateTime yearEnd = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59); // 結束由 年,月,日,時,分,秒
            ViewBag.yearMoney = _dbContext.Orders
                .Where(p => p.IsPay == 1 && p.Createtime >= yearStart && p.Createtime <= yearEnd)
                .Sum(p => (decimal?)p.SumPrice) ?? 0;

            // 今日訂單取回來，取 10 條
            ViewBag.orderList = _dbContext.Orders.Include(p => p.UidNavigation)
                .Where(p => p.Createtime >= todayStart && p.Createtime <= endday)
                .OrderByDescending(p => p.Id)
                .Take(10)
                .ToList();

            return View();
        }

        // 商品統計
        public async Task<IActionResult> ShopCount()
        {
            return View();
        }

        // 獲取商品統計的結果返回
        public async Task<IActionResult> GetShopList(DateTime? start, DateTime? end, string keyword = "")
        {
            // 筛選出在時間段內的訂單，再去匹配該時間段內的商品有哪些
            IEnumerable<Order> olist = _dbContext.Orders;
            if (start.HasValue)
            {
                olist = olist.Where(p => p.Createtime >= start);
            }
            if (end.HasValue)
            {
                olist = olist.Where(p => p.Createtime <= end);
            }
            List<int> ids = olist.Select(p => p.Id).Distinct().ToList();

            // 筛選出符合條件的商品列表
            IEnumerable<ShopCount> list = _dbContext.OrdersDetails
                .Where(p => ids.Contains((int)p.OrderId))
                .GroupBy(p => p.Title)
                .Select(g => new ShopCount()
                {
                    Title = g.Key,
                    Count = g.Sum(p => (int)p.Count)
                }).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Title.Contains(keyword));
            }

            return Json(new { code = 200, msg = "success", data = list });
        }

        // 訂單統計
        public async Task<IActionResult> OrderCount()
        {
            return View();
        }

        // 獲取訂單統計的結果返回
        public async Task<IActionResult> GetOrderList(DateTime? start, DateTime? end, int isPay = -1)
        {
            // 筛選出在時間段內的訂單，再去匹配該時間段內的商品有哪些
            IEnumerable<Order> olist = _dbContext.Orders;
            if (start.HasValue)
            {
                olist = olist.Where(p => p.Createtime >= start);
            }
            if (end.HasValue)
            {
                olist = olist.Where(p => p.Createtime <= end);
            }
            if (isPay != -1)
            {
                olist = olist.Where(p => p.IsPay == isPay);
            }

            IEnumerable<OrderCount> list = olist.GroupBy(p => p.Createtime.Date)
                .Select(p => new Models.OrderCount()
                {
                    Date = p.Key.Date,
                    Price = p.Sum(g => g.SumPrice)
                }).ToList();

            return Json(new { code = 200, msg = "success", data = list });
        }

        //修改密碼
        //編輯
        
        public async Task<IActionResult> Update()
        {
            int id=(int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Phone,Pwd,Nickname,Sex,Introduce,Age,Img,Mibao,Role")] User user, string oldPwd)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //更改窗密碼不等於 原有密碼 重新加密保存˙
                    if (oldPwd != user.Pwd)
                    {
                        user.Pwd = PasswordHelper.HashPasswordWithMD5(user.Pwd, PasswordHelper.GenerateSalt());
                    }
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                return RedirectToAction(nameof(Update));
            }
            
            return View(user);
        }


        //管理員發送消息頁面
        //打開頁面獲取接收人ID
        [AdminAuthen]
        public IActionResult send(int rid = 0)
        {
            ViewBag.rid = rid;
            //获取我的消息列表用户显示
            int uid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            //获取发信息给我和我发的用户
            IEnumerable<Message> list = _dbContext.ChatMessages.Where(p => p.ToUserid == uid || p.FromUserid == uid)
                                                               .GroupBy(p => p.FromUserid)
                                                               .Select(p => new Message()
                                                               {
                                                                  FromUserId = p.Key,
                                                                  ToUserId = uid
                                                               })
                                                               .ToList();
            ViewBag.uid = uid;
            //获取当前聊天的用户的信息返回
            if (rid != 0)
            {
                ViewBag.uinfo = _dbContext.Users.Where(p => p.Id == rid).FirstOrDefault();
            }

            return View(list);
        }

        // 获取历史聊天记录
        [AdminAuthen]
        [HttpGet]
        [Route("api/chat/history2/{receiverId}")]
        public async Task<IActionResult> GetChatHistory(int receiverId)
        {
            // 查询双方的聊天记录
            int senderId = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            var messages = await _dbContext.ChatMessages
                .Where(m => (m.FromUserid == senderId && m.ToUserid == receiverId) ||
                            (m.FromUserid == receiverId && m.ToUserid == senderId))
                .OrderBy(m => m.SendTime) // 按时间顺序排列
                .ToListAsync();
            //批量修改聊天记录为已读
            string sql = $"update chat_message set is_read = 1 where ((from_userid = {senderId} and to_userid = {receiverId}) or (from_userid = {receiverId} and to_userid = {senderId})) and is_read = 0";
            //执行sql修改状态为已读
            _dbContext.Database.ExecuteSqlRaw(sql);

            return Ok(messages);
        }

    }
}
