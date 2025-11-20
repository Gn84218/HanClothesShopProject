using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;
using static NuGet.Packaging.PackagingConstants;
using HanClothesShopProject.Filter;

namespace HanClothesShopProject.Controllers
{
    //訂單管理
    [UserAuthen]
    public class UserOrderController : Controller
    {
        private readonly dbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserOrderController(dbContext context , IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: 訂單管理 isPay支付狀態 state發貨狀態
        public async Task<IActionResult> Index(string keyword = "", int isPay = -1, int state = -1, int page = 1)
        {
            int uid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("uid");
            IEnumerable<Order> list = _context.Orders.Include(o => o.UidNavigation).Where(o=>o.Uid==uid);
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.OrderNum.Contains(keyword));
            }           
            if (isPay != -1)
            {
                list = list.Where(p => p.IsPay == isPay);
            }
            if (state != -1)
            {
                list = list.Where(p => p.State == state);
            }
            ViewBag.Keyword = keyword;      
            ViewBag.IsPay = isPay;
            ViewBag.State = state;

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

        // GET: 訂單詳情
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            //訂單詳情 包含用戶資訊
            var order = await _context.Orders
                .Include(o => o.UidNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            //
            ViewBag.detaiList = await _context.OrdersDetails
                .Include(p => p.PidNavigation)
                .Where(p => p.OrderId ==order.Id).ToListAsync();
            if (order == null)
            {
                return NotFound();
            }

            //獲取當前訂單售後情形
            ViewBag.turnList = await _context.ApplyReturns.Include(p => p.PidNavigation)
                                                          .Where(p => p.OrderId == order.Id).ToListAsync();
                                        
            return View(order);
        }

        //訂單發貨處理 編號、快遞公司、快遞單號
        [HttpPost]
        public async Task<IActionResult> Send(int id,short state)
        {
            
            Order o = _context.Orders.Where(p => p.Id == id).FirstOrDefault();
            //修改訂單狀態
            o.State = state;                  
            _context.Entry(o).State = EntityState.Modified;
            //如果:訂單未支付 也使用優惠券 取消時反還優惠券
            if (o.CouponId != 0 && state ==0)
            {
                GetCoupon gc = _context.GetCoupons.Where(p => p.Id == o.CouponId).FirstOrDefault();
                if (gc != null)
                {
                    gc.IsUse = 0; // 修改為未使用
                    _context.Entry(gc).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
            return Ok(new { code = 200, msg = "success" });
        }
        //申請退貨操作
        public async Task<IActionResult> ApplyReturn(int id, string orderNum)
        {
            //貨去申請退貨商品詳情 讓用戶操作退貨
            IEnumerable<OrdersDetail> olist=_context.OrdersDetails.Include(p => p.PidNavigation)
                                                                 .Where(p => p.OrderId == id).ToList();
            //獲取商家退貨展示
            ViewBag.setInfo = _context.ReturnSets.OrderByDescending(p => p.Id).ToList();
            ViewBag.orderNum = orderNum;
            return View(olist);
        }
        //處理申請退貨提交的請求
        [HttpPost]
        public async Task<IActionResult> Apply(ApplyReturn apply)
        {
            //檢查當前商品是否已經申請過退貨
            ApplyReturn ar=_context.ApplyReturns.Where(p => p.OrderId == apply.OrderId && p.Pid == apply.Pid).FirstOrDefault();
            if (ar != null)
            {
                return Ok(new { code = 201, msg = "不好意思,該商品您以操作過售後申請,不需要重複提交" });
            }
            //使用事務保證程式操作完整性
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //1.新增退貨申請記錄                 
                    apply.Createtime = DateTime.Now;
                    apply.Uid = (int)_httpContextAccessor.HttpContext.Session.GetInt32("uid");
                    apply.Status = 0; //等待商家審核
                    _context.ApplyReturns.Add(apply);

                    //2.修改訂單為退貨中
                    Order o = _context.Orders.Where(p => p.Id == apply.OrderId).FirstOrDefault();
                    o.State=5; //退貨中
                    _context.Entry(o).State = EntityState.Modified;
                    //更新數據,異步方法儲存
                    _context.SaveChangesAsync();
                    //提交事務,永久修改
                    transaction.CommitAsync();
                    return Ok(new { code = 200, msg = "退貨申請提交成功,請等待商家審核!" });
                }
                catch(Exception ex) { 
                 return Ok(new { code = 202, msg = "退貨申請提交失敗,請稍後重試!" });
                }
            }


        }


        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
