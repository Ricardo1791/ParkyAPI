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
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/trails")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    public class TrailsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITrailRepository _Trailrepo;

        public TrailsController(IMapper mapper, ITrailRepository trailrepo)
        {
            _mapper = mapper;
            _Trailrepo = trailrepo;
        }

        /// <summary>
        /// Get list of trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetTrail()
        {
            var objList = _Trailrepo.GetTrail().ToList();
            var mapeo = _mapper.Map<List<TrailDTO>>(objList);

            return Ok(mapeo);
        }

        [HttpGet("{trailId:int}", Name = "GetTrails")]
        [ProducesResponseType(200, Type = typeof(TrailDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "test")]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _Trailrepo.GetTrail(trailId);

            if (obj == null)
            {
                return NotFound();
            }

            var mapeo = _mapper.Map<TrailDTO>(obj);

            return Ok(mapeo);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "test")]
        public IActionResult CreateTrail([FromBody] TrailCreateDTO trailDTO)
        {
            if (trailDTO == null)
            {
                return BadRequest();
            }

            if (_Trailrepo.TrailExist(trailDTO.Name))
            {
                ModelState.AddModelError("", $"Trail exists!");

                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDTO);

            if (!_Trailrepo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");

                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrails", new { trailId = trailObj.Id }, trailObj);
        }


        [HttpPatch("{TrailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "test")]
        public IActionResult UpdateTrail(int TrailId, [FromBody] TrailUpdateDTO trailDTO)
        {
            if (trailDTO == null || TrailId != trailDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDTO);

            if (!_Trailrepo.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{trailId:int}", Name ="DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "test")]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_Trailrepo.TrailExist(trailId))
            {
                return NotFound();
            }

            var trailObj = _Trailrepo.GetTrail(trailId);

            if (!_Trailrepo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}