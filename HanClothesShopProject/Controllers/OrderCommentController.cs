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
    public class OrderCommentController : Controller
    {
        private readonly dbContext _context;

        public OrderCommentController(dbContext context)
        {
            _context = context;
        }

        // GET: OrderComment
        public async Task<IActionResult> Index(string nickname="", string titles="",int page=1)
        {
            IEnumerable<OrderComment> list = _context.OrderComments.Include(o => o.Order)
                                                                 .Include(o => o.PidNavigation)
                                                                 .Include(o => o.User);         
            if (!string.IsNullOrEmpty(nickname))
            {
                list = list.Where(p => p.Order.UidNavigation.Nickname.Contains(nickname));
            }
            if (!string.IsNullOrEmpty(titles))
            {
                list = list.Where(p => p.PidNavigation.Title.Contains(titles));
            }


            ViewBag.titles = titles;
            ViewBag.NickName = nickname;
         

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

        
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.OrderComments == null)
            {
                return Problem("Entity set 'dbContext.OrderComments'  is null.");
            }
            var orderComment = await _context.OrderComments.FindAsync(id);
            if (orderComment != null)
            {
                _context.OrderComments.Remove(orderComment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderCommentExists(int id)
        {
          return (_context.OrderComments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
