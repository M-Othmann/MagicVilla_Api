using AutoMapper;
using MagicVilla_Api.Data;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using MagicVilla_Api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        private readonly APIResponse _response;

        public VillaAPIController(IMapper mapper, IVillaRepository dbVilla)
        {

            _mapper = mapper;
            _dbVilla = dbVilla;
            this._response = new();
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDto>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            if (id == 0)
            {

                return BadRequest();

            }

            try
            {


                var villa = await _dbVilla.GetAsync(v => v.Id == id);

                if (villa is null)
                    return NotFound();
                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;

        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                if (createDto is null)
                    return BadRequest(createDto);

                if (await _dbVilla.GetAsync(v => v.Name.ToLower() == createDto.Name) != null)
                {
                    ModelState.AddModelError("Error", "Villa already exists");
                    return BadRequest(ModelState);
                }


                /* if (villaDto.Id > 0)
                     return StatusCode(StatusCodes.Status500InternalServerError);*/

                Villa villa = _mapper.Map<Villa>(createDto);


                /*Villa model = new()
                {
                    Amenity = createDto.Amenity,
                    Details = createDto.Details,
                    ImageUrl = createDto.ImageUrl,
                    Name = createDto.Name,
                    Occupancy = createDto.Occupancy,
                    Rate = createDto.Rate,
                    Sqft = createDto.Sqft,
                };*/




                await _dbVilla.CreateAsync(villa);
                await _dbVilla.SaveAsync();


                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.Created;


                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;

        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "CUSTOM")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {


                if (id == 0)
                    return BadRequest();

                var villa = await _dbVilla.GetAsync(v => v.Id == id);


                if (villa is null)
                    return NotFound();

                await _dbVilla.RemoveAsync(villa);
                await _dbVilla.SaveAsync();


                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;


        }


        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            try
            {


                if (updateDto == null || id != updateDto.Id)
                    return BadRequest();

                /*           var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

                            villa.Name = villaDto.Name;
                                       villa.Sqft = villaDto.Sqft;
                                       villa.Occupancy = villaDto.Occupancy;*/

                Villa model = _mapper.Map<Villa>(updateDto);

                /*Villa model = new()
                {
                    Amenity = villaDto.Amenity,
                    Details = villaDto.Details,
                    Id = villaDto.Id,
                    ImageUrl = villaDto.ImageUrl,
                    Name = villaDto.Name,
                    Occupancy = villaDto.Occupancy,
                    Rate = villaDto.Rate,
                    Sqft = villaDto.Sqft,
                };*/
                await _dbVilla.UpdateAsync(model);
                await _dbVilla.SaveAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;


        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
                return BadRequest();
            var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);


            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);


            if (villa == null)
                return BadRequest();



            /*VillaUpdateDto villaDto = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };*/


            patchDto.ApplyTo(villaDto, ModelState);



            Villa model = _mapper.Map<Villa>(villaDto);

            /* Villa model = new()
             {
                 Amenity = villaDto.Amenity,
                 Details = villaDto.Details,
                 Id = villaDto.Id,
                 ImageUrl = villaDto.ImageUrl,
                 Name = villaDto.Name,
                 Occupancy = villaDto.Occupancy,
                 Rate = villaDto.Rate,
                 Sqft = villaDto.Sqft,
             };*/

            await _dbVilla.UpdateAsync(model);
            await _dbVilla.SaveAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();

        }





    }
}
