using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ParkyAPI.Repository.IRepository;
using AutoMapper;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    public class NationalParksController: ControllerBase
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;
        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetNationalPark()
        {
            var objList = _npRepo.GetNationalPark();

            var mapeo = _mapper.Map<List<NationalParkDTO>>(objList);

            return Ok(mapeo);
        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="nationalParkId">The Id of the national park</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Admin")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);

            if (obj == null)
            {
                return NotFound();
            }

            var objDTO = _mapper.Map<NationalParkDTO>(obj);

            return Ok(objDTO);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExist(nationalParkDTO.Name))
            {
                ModelState.AddModelError("","National Park exists!");

                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);

            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");

                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { version = HttpContext.GetRequestedApiVersion().ToString() , nationalParkId =  nationalParkObj.Id }, nationalParkObj);
        }


        [HttpPatch("{NationalParkId:int}",Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateNationalPark(int NationalParkId, [FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null || NationalParkId != nationalParkDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);

            if (!_npRepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{NationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteNationalPark(int NationalParkId)
        {
            if (!_npRepo.NationalParkExist(NationalParkId))
            {
                return NotFound();
            }

            var nationalParkObj = _npRepo.GetNationalPark(NationalParkId);

            if (!_npRepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
