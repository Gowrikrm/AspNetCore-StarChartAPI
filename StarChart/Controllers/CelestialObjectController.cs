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
            if (celestialObjs.Count == 0) { return NotFound(); }
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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject co)
        {
            _context.CelestialObjects.Add(co);
            var result = _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = co.Id }, co);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CelestialObject co)
        {
            var dbCO = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (dbCO == null) { return NotFound(); }
            else
            {
                dbCO.Name = co.Name;
                dbCO.OrbitalPeriod = co.OrbitalPeriod;
                dbCO.OrbitedObjectId = co.OrbitedObjectId;
                _context.CelestialObjects.Update(dbCO);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var dbCO = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (dbCO == null) { return NotFound(); }
            else
            {
                dbCO.Name = name;
                _context.CelestialObjects.Update(dbCO);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dbCO = _context.CelestialObjects.Where(x => x.Id == id).ToList<CelestialObject>();
            if(dbCO.Count == 0) { return NotFound(); }
            else
            {
                _context.CelestialObjects.RemoveRange(dbCO);
                _context.SaveChanges();
                return NoContent();
            }
        }
    }
}
