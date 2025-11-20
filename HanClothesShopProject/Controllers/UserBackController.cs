using HanClothesShopProject.CommonUtil;
using HanClothesShopProject.Filter;
using HanClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HanClothesShopProject.Controllers
{
    [UserAuthen]//攔截器 登入驗證
    public class UserBackController : Controller
    {
        //自動注入資料庫上下文服務到這個類別
        private readonly dbContext _dbContext;
        //HttpContextAccessor的接口 添加依賴
        private readonly IHttpContextAccessor _httpContextAccessor;
        //注入
        public UserBackController(dbContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
        }
        //使用者首頁
        public IActionResult Index()
        {
            //從Session獲取當前用戶ID
            int uid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("uid");
            // 取10個最新訂單
            ViewBag.orderlist = _dbContext.Orders.Include(p => p.UidNavigation).Where(p => p.Uid == uid).OrderByDescending(p => p.Id).Take(10).ToList();
            return View();
        }

        //獲取我的優惠券列表
        public IActionResult MyCoupon(string keyword = "", int state = -1, int page = 1)
        {
            //從Session獲取當前用戶ID
            int uid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("uid");
            //獲取用戶(自己)的優惠券
            IEnumerable<GetCoupon> list = _dbContext.GetCoupons.Include(p => p.Coupon)
                                                               .Where(p => p.UserId == uid);

            //前端輸入框(查詢用)
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Coupon.Title.Contains(keyword));//找出優惠券關鍵字標題(名字)
            }
            if (state != -1)
            {
                list = list.Where(p => p.IsUse == state);//使用狀態              
            }
            ViewBag.keyword = keyword;
            ViewBag.state = state;
            //分頁條數
            int pageSize = 10;
            //總條數有多少
            var total = list.Count();
            //每頁10條的話，總共的可以分多少頁total/10
            // 21筆資料 每頁10條 問：可以分成幾頁？ 21/10 = 2.1 向上取整得到3 實際上可以分3頁
            ViewBag.pageNum = Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(pageSize));
            // 分頁演算法原理顯示第一頁：（1-1）*10 = 0，10 得到的是 0-10 條
            // 顯示第二頁：（2-1）*10 = 10，10 得到的是 10-20 條
            list = list.OrderByDescending(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(list);
        }

        //編輯密碼
        public async Task<IActionResult> Update()
        {
            int uid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("uid");
            if (uid == null || _dbContext.Users == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(uid);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //保存編輯
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
    }
}
