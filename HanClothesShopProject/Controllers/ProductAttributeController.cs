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
    public class ProductAttributeController : Controller
    {
        private readonly dbContext _context;

        public ProductAttributeController(dbContext context)
        {
            _context = context;
        }

        // GET: ProductAttribute
        public async Task<IActionResult> Index(int sid)
        {
            var dbContext = _context.ProductAttributes.Include(p => p.PidNavigation);
            ViewBag.pid = sid;//儲存商品id 用於返回新增圖片頁面
            return View(await dbContext.Where(p => p.Pid == sid).OrderByDescending(p => p.Id).ToListAsync());

        }


        // GET: ProductAttribute/Create
        public IActionResult Create(int pid)
        {
            Product product = _context.Products.Where(p => p.Id == pid).FirstOrDefault();
            return View(product);
        }

        // POST: ProductAttribute/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Price,Pid")] ProductAttribute productAttribute)
        {

            if (ModelState.IsValid)
            {
                _context.Add(productAttribute);
                await _context.SaveChangesAsync();
                //返回圖片管理時 需要存在商品id 所以要儲存sid這個參數
                return RedirectToAction(nameof(Index), new { sid = productAttribute.Pid });
            }
            ViewData["Pid"] = new SelectList(_context.Products, "Id", "Id", productAttribute.Pid);
            return View(productAttribute);

        }

        // GET: ProductAttribute/Edit/5
        public async Task<IActionResult> Edit(int pid , int? id)
        {
            if (id == null || _context.ProductAttributes == null)
            {
                return NotFound();
            }
            //獲取當前屬性資訊
            ViewBag.ainfo  = await _context.ProductAttributes.FindAsync(id);
            Product product= _context.Products.Where(p=>p.Id==pid).FirstOrDefault();            
            return View(product);
        }

        // POST: ProductAttribute/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Price,Pid")] ProductAttribute productAttribute)
        {
            if (id != productAttribute.Id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(productAttribute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductAttributeExists(productAttribute.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //跳轉需要帶商品id參數
                return RedirectToAction(nameof(Index), new { sid = productAttribute.Pid });
            
        }

        
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
            return RedirectToAction(nameof(Index), new { sid = productAttribute.Pid });
        }

        private bool ProductAttributeExists(int id)
        {
          return (_context.ProductAttributes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
