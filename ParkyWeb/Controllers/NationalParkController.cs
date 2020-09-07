using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _npRepo;

        public NationalParkController(INationalParkRepository npRepo)
        {
            _npRepo = npRepo;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _npRepo.GetAllAsync(SD.NationalParkAPIPath) });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            NationalPark obj = new NationalPark();

            if (id == null)
            {
                return View(obj);
            }

            obj = await _npRepo.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault(), token);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    obj.Picture = p1;
                }
                else
                {
                    var objFromDb = await _npRepo.GetAsync(SD.NationalParkAPIPath, obj.Id, token);
                    obj.Picture = objFromDb.Picture;
                }

                if (obj.Id == 0)
                {
                    await _npRepo.CreateAsync(SD.NationalParkAPIPath, obj, token);
                }
                else
                {
                    await _npRepo.UpdateAsync(SD.NationalParkAPIPath+obj.Id.ToString(), obj, token);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            var status = await _npRepo.DeleteAsync(SD.NationalParkAPIPath, id, token);
            if (status)
            {
                return Json(new { success = true, message = "Delete successful" });
            }

            return Json(new { success = false, message = "Delete not successful" });
        }
    }
}