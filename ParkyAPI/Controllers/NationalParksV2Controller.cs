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

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiVersion("2.0")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    public class NationalParksV2Controller: ControllerBase
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;
        public NationalParksV2Controller(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetNationalPark()
        {
            var obj = _npRepo.GetNationalPark().FirstOrDefault();

            var mapeo = _mapper.Map<NationalParkDTO>(obj);

            return Ok(mapeo);
        }
    }
}
