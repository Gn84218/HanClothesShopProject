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
    public class CategoryController : Controller
    {
        private readonly dbContext _context;

        public CategoryController(dbContext context)
        {
            _context = context;
        }

        // GET: Category 分類
        public async Task<IActionResult> Index(string keyword = "", int page = 1)
        {
            IEnumerable<Category> list = _context.Categories;
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Catename.Contains(keyword));
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

        

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Catename")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Catename")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        

        // POST: Category/Delete/5
       
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'dbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
