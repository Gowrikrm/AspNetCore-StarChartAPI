using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObj = _context.CelestialObjects.Where(cd => cd.Id == id).FirstOrDefault();
            if (celestialObj == null) { return NotFound(); }
            else
            {
                celestialObj.Satellites = _context.CelestialObjects.Where(cd => cd.OrbitedObjectId == celestialObj.Id).ToList<CelestialObject>();
                return Ok(celestialObj);
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjs = _context.CelestialObjects.Where(cd => cd.Name == name).ToList();
            if(celestialObjs.Count == 0) { return NotFound(); }
            else
            {
                celestialObjs.ForEach(x => x.Satellites = _context.CelestialObjects.Where(cd => cd.OrbitedObjectId == x.Id).ToList<CelestialObject>());
                return Ok(celestialObjs);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjs = _context.CelestialObjects.Select(x => x).ToList<CelestialObject>();
            celestialObjs.ForEach(x => x.Satellites = _context.CelestialObjects.Where(cd => cd.OrbitedObjectId == x.Id).ToList<CelestialObject>());
            return Ok(celestialObjs);
        }
    }
}
