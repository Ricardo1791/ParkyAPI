using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;

        public TrailController(INationalParkRepository npRepo, ITrailRepository trailRepo)
        {
            _npRepo = npRepo;
            _trailRepo = trailRepo;
        }

        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        public async Task<IActionResult> GetAllTrail()
        {
            return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailAPIPath) });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Trails = new Trail()
            };

            if (id == null)
            {
                return View(objVM);
            }

            objVM.Trails = await _trailRepo.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault(), token);

            if (objVM.Trails == null)
            {
                return NotFound();
            }

            return View(objVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (ModelState.IsValid)
            {
                if (obj.Trails.Id == 0)
                {
                    await _trailRepo.CreateAsync(SD.TrailAPIPath, obj.Trails, token);
                }
                else
                {
                    await _trailRepo.UpdateAsync(SD.TrailAPIPath+obj.Trails.Id.ToString(), obj.Trails, token);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    Trails = obj.Trails
                };

                return View(objVM);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            var status = await _trailRepo.DeleteAsync(SD.TrailAPIPath, id, token);
            if (status)
            {
                return Json(new { success = true, message = "Delete successful" });
            }

            return Json(new { success = false, message = "Delete not successful" });
        }
    }
}