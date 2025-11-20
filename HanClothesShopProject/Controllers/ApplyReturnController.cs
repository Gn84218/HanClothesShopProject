using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;

namespace HanClothesShopProject.Controllers
{
    // 退貨申請管理
    public class ApplyReturnController : Controller
    {
        private readonly dbContext _context;

        public ApplyReturnController(dbContext context)
        {
            _context = context;
        }

        // GET: ApplyReturn
        public async Task<IActionResult> Index(string keyword="", string nickname="",int page=1)
        {
            IEnumerable<ApplyReturn> list = _context.ApplyReturns.Include(a => a.Order)
                                                                 .Include(a => a.PidNavigation)
                                                                 .Include(a => a.UidNavigation);
           
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Order.OrderNum.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(nickname))
            {
                list = list.Where(p => p.Order.UidNavigation.Nickname.Contains(nickname));
            }
            ViewBag.nickname = nickname;
            ViewBag.keyword = keyword;
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

       //修改訂單狀態 可能涉及退貨
        [HttpPost]     
        public async Task<IActionResult> Edit(int id,short state,string mark)
        {
            try
            {
                ApplyReturn ap = _context.ApplyReturns.Where(p => p.Id == id).FirstOrDefault();
                ap.Status = state;
                ap.BusinessMark = mark;
                _context.Entry(ap).State = EntityState.Modified;
                //如果商家同意退款 訂單狀態同步操作修改
                if (state == 1)
                {
                    Order o = _context.Orders.Where(p => p.Id == ap.OrderId).FirstOrDefault();
                    if (o != null)
                    {
                        o.State = 0;//修改訂單狀態為已取消 ,代表退貨成功
                        o.IsPay = 1;//修改支付狀態為已支付
                        _context.Entry(o).State = EntityState.Modified;
                    }
                }
                _context.SaveChanges();
                return Ok(new { code = 200, msg = "success!" });
                
            }
            catch (Exception ex)
            {
                return Ok(new { code = 201, msg = "退貨申請失敗" });
            }
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ApplyReturns == null)
            {
                return Problem("Entity set 'dbContext.ApplyReturns'  is null.");
            }
            var applyReturn = await _context.ApplyReturns.FindAsync(id);
            if (applyReturn != null)
            {
                _context.ApplyReturns.Remove(applyReturn);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplyReturnExists(int id)
        {
          return (_context.ApplyReturns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
