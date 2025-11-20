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
    // 退貨設定管理
    public class SetInfoController : Controller
    {
        private readonly dbContext _context;

        public SetInfoController(dbContext context)
        {
            _context = context;
        }

        // GET: 退貨設定
        public async Task<IActionResult> Index()
        {
              return _context.ReturnSets != null ? 
                          View(await _context.ReturnSets.ToListAsync()) :
                          Problem("Entity set 'dbContext.ReturnSets'  is null.");
        }

        

        // GET: SetInfo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SetInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Province,City,Area,Detail,Name,Phone,Mark,Createtime")] ReturnSet returnSet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(returnSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(returnSet);
        }

        // GET: SetInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReturnSets == null)
            {
                return NotFound();
            }

            var returnSet = await _context.ReturnSets.FindAsync(id);
            if (returnSet == null)
            {
                return NotFound();
            }
            return View(returnSet);
        }

        // POST: SetInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Province,City,Area,Detail,Name,Phone,Mark,Createtime")] ReturnSet returnSet)
        {
            if (id != returnSet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(returnSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReturnSetExists(returnSet.Id))
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
            return View(returnSet);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ReturnSets == null)
            {
                return Problem("Entity set 'dbContext.ReturnSets'  is null.");
            }
            var returnSet = await _context.ReturnSets.FindAsync(id);
            if (returnSet != null)
            {
                _context.ReturnSets.Remove(returnSet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnSetExists(int id)
        {
          return (_context.ReturnSets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
