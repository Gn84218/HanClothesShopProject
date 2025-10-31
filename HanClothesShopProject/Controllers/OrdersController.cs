using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;
using static NuGet.Packaging.PackagingConstants;

namespace HanClothesShopProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly dbContext _context;

        public OrdersController(dbContext context)
        {
            _context = context;
        }

        // GET: 訂單管理 isPay支付狀態 state發貨狀態
        public async Task<IActionResult> Index(string keyword = "", string nickname = "", int isPay = -1, int state = -1, int page = 1)
        {
            IEnumerable<Order> list = _context.Orders.Include(o => o.UidNavigation);
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.OrderNum.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(nickname))
            {
                list = list.Where(p => p.UidNavigation.Nickname.Contains(nickname));
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
            ViewBag.NickName = nickname;
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

        // GET: Orders/Details/5
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

            return View(order);
        }

        //訂單發貨處理 編號、快遞公司、快遞單號
        [HttpPost]
        public async Task<IActionResult> Send(int id,string ExpressName,string ExpressNumber)
        {
             
            Order o = _context.Orders.Where(p => p.Id == id).FirstOrDefault();
            o.ExpressName = ExpressName;
            o.ExpressNumber = ExpressNumber;
            //修改訂單狀態為已發貨 狀態2為已發貨
            o.State = 2;
            _context.Entry(o).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { code = 200, msg = "success" });
        }

        //刪除 訂單不能刪除方便移除數據 
        public async Task<IActionResult> Delete(int id)

        {
            if (_context.ProductAttributes == null)
            {
                return Problem("Entity set 'dbContext.ProductAttributes'  is null.");
            }
            var productAttribute = await _context.ProductAttributes.FindAsync(id);
            if (productAttribute != null)
            {
                _context.ProductAttributes.Remove(productAttribute);
            }

            await _context.SaveChangesAsync();
            //跳轉需要帶商品id參數
            return RedirectToAction(nameof(Index));
        }
    }
}
