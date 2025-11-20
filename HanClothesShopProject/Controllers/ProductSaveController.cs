using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;
using Microsoft.IdentityModel.Tokens;
using HanClothesShopProject.Filter;

namespace HanClothesShopProject.Controllers
{
    //商品收藏管理
    [UserAuthen]
    public class ProductSaveController : Controller
    {
        private readonly dbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductSaveController(dbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: ProductSave
        public async Task<IActionResult> Index(string keyword,int page=1)
        {
            //在Session中獲取當前用戶ID
            var uid =(int)_httpContextAccessor.HttpContext.Session.GetInt32("uid");

            //獲取用戶(自己)的10個最新訂單
            IEnumerable<ProductSave> list = _context.ProductSaves.Include(p => p.PidNavigation)
                                                                 .Include(p => p.UidNavigation)
                                                                 .Where(p=>p.Uid==uid);
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.PidNavigation.Title.Contains(keyword));//商品標題
            }
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

       
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ProductSaves == null)
            {
                return Problem("Entity set 'dbContext.ProductSaves'  is null.");
            }
            var productSave = await _context.ProductSaves.FindAsync(id);
            if (productSave != null)
            {
                _context.ProductSaves.Remove(productSave);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSaveExists(int id)
        {
          return (_context.ProductSaves?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
