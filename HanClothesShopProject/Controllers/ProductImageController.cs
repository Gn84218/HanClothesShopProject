using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;
using System.Net.NetworkInformation;

namespace HanClothesShopProject.Controllers
{
    //商品附加圖片管理
    public class ProductImageController : Controller
    {
        private readonly dbContext _context;

        public ProductImageController(dbContext context)
        {
            _context = context;
        }

        // GET: ProductImage sid在前端商品詳情頁面傳過來的商品id
        public async Task<IActionResult> Index(int sid)
        {
            var dbContext = _context.ProductImages.Include(p => p.PidNavigation);
            ViewBag.pid = sid;//儲存商品id 用於返回新增圖片頁面
            return View(await dbContext.Where(p=>p.Pid==sid).OrderByDescending(p=>p.Id).ToListAsync());

        }

        

        // GET: ProductImage/Create
        public IActionResult Create(int pid)
        {
           Product product =_context.Products.Where(p => p.Id == pid).FirstOrDefault();
            return View(product);
        }

        // POST: ProductImage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Pid,ImageUrl")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productImage);
                await _context.SaveChangesAsync();
                //返回圖片管理時 需要存在商品id 所以要儲存sid這個參數
                return RedirectToAction(nameof(Index), new {sid=productImage.Pid });
            }
            ViewData["Pid"] = new SelectList(_context.Products, "Id", "Id", productImage.Pid);
            return View(productImage);
        }

        // GET: ProductImage/Edit/5
        public async Task<IActionResult> Edit(int pid,int? id)
        {
            if (id == null || _context.ProductImages == null)
            {
                return NotFound();
            }
            //查詢當前的編輯商品附加圖操作
           ViewBag.pImg = await _context.ProductImages.FindAsync(id);
            Product product = _context.Products.Where(p => p.Id == pid).FirstOrDefault();
            return View(product);
        }

        // POST: ProductImage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Pid,ImageUrl")] ProductImage productImage)
        {
            if (id != productImage.Id)
            {
                return NotFound();
            }
       
                try
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //返回圖片管理時 需要存在商品id 所以要儲存sid這個參數
                return RedirectToAction(nameof(Index), new { sid = productImage.Pid });
           
            //ViewData["Pid"] = new SelectList(_context.Products, "Id", "Id", productImage.Pid);
           // return View(productImage);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ProductImages == null)
            {
                return Problem("Entity set 'dbContext.ProductImages'  is null.");
            }
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage != null)
            {
                _context.ProductImages.Remove(productImage);
            }
            
            await _context.SaveChangesAsync();
            //返回圖片管理時 需要存在商品id 所以要儲存sid這個參數
            return RedirectToAction(nameof(Index), new { sid = productImage.Pid });
        }

        private bool ProductImageExists(int id)
        {
          return (_context.ProductImages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
