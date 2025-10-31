using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;
using System.Text;
using System.Collections;

namespace HanClothesShopProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly dbContext _context;

        public ProductController(dbContext context)
        {
            _context = context;
        }

        // GET: Product 模糊名稱搜索 分類篩選 狀態篩選 分頁
        public async Task<IActionResult> Index(string keyword="",int cid=-1,int state=-1,int page=-1)
        {
            IEnumerable<Product> list = _context.Products.Include(p => p.CidNavigation);
                     
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Title.Contains(keyword));
            }
            if (cid!=-1)
            {
                list = list.Where(p => p.Cid== cid);
            }
            if (state!=-1)
            {
                list = list.Where(p => p.State== state);
            }
            ViewBag.Keyword = keyword;
            ViewBag.Cid = cid;
            ViewBag.State = state;

            //獲取商業分類列表
            ViewBag.clist = _context.Categories.ToList();
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

        

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["Cid"] = new SelectList(_context.Categories, "Id", "Catename");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Cid,Price,SalePrice,Number,Detail,Img,State,Createtime,Score,Postage")] Product product)
        {
           
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
          
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Cid"] = new SelectList(_context.Categories, "Id", "Catename", product.Cid);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Cid,Price,SalePrice,Number,Detail,Img,State,Createtime,Score,Postage")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));      
        }

        //刪除 關聯表無法刪除報錯
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (_context.Products == null)
                {
                    return Problem("Entity set 'dbContext.Products'  is null.");
                }
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content(
                   "<script>" +
                   "alert('當前商品跟其他表關聯無法刪除');" +
                   "window.history.back(-1);" +
                   "</script>",
                   "text/html"
                   , Encoding.UTF8);
            }
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
