using MagicVilla_Api.Data;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {

            return Ok(await _db.Villas.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {

                return BadRequest();

            }


            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa is null)
                return NotFound();

            return Ok(villa);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto villaDto)
        {
            if (villaDto is null)
                return BadRequest(villaDto);

            if (await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDto.Name) != null)
            {
                ModelState.AddModelError("Error", "Villa already exists");
                return BadRequest(ModelState);
            }


            /* if (villaDto.Id > 0)
                 return StatusCode(StatusCodes.Status500InternalServerError);*/

            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };




            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
                return BadRequest();

            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);


            if (villa is null)
                return NotFound();

            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            return Ok();

        }


        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
        {
            if (villaDto == null || id != villaDto.Id)
                return BadRequest();

            /*           var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

                        villa.Name = villaDto.Name;
                                   villa.Sqft = villaDto.Sqft;
                                   villa.Occupancy = villaDto.Occupancy;*/

            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return Ok();

        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
                return BadRequest();
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);




            if (villa == null)
                return BadRequest();

            VillaUpdateDto villaDto = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };


            patchDto.ApplyTo(villaDto, ModelState);


            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft,
            };

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();

        }





    }
}
