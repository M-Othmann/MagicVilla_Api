using AutoMapper;
using MagicVilla_Api.Data;
using MagicVilla_Api.Migrations;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using MagicVilla_Api.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        private readonly APIResponse _response;

        public VillaNumberAPIController(IMapper mapper, IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla)
        {

            _mapper = mapper;
            _dbVillaNumber = dbVillaNumber;
            this._response = new();
            _dbVilla = dbVilla;
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetVillasNumber()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumberList);
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

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            if (id == 0)
            {

                return BadRequest();

            }

            try
            {


                var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);

                if (villaNumber is null)
                    return NotFound();
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
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
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDto createDto)
        {
            try
            {
                if (createDto is null)
                    return BadRequest(createDto);

                if (await _dbVillaNumber.GetAsync(v => v.VillaNo == createDto.VillaNo) != null)
                {
                    ModelState.AddModelError("Error", "Villa Number already exists");
                    return BadRequest(ModelState);
                }


                if (await _dbVilla.GetAsync(v => v.Id == createDto.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa ID is invalid");
                    return BadRequest(ModelState);
                }



                /* if (villaDto.Id > 0)
                     return StatusCode(StatusCodes.Status500InternalServerError);*/

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDto);


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




                await _dbVillaNumber.CreateAsync(villaNumber);
                await _dbVillaNumber.SaveAsync();


                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;


                return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;

        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {


                if (id == 0)
                    return BadRequest();

                var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);


                if (villaNumber is null)
                    return NotFound();

                await _dbVillaNumber.RemoveAsync(villaNumber);
                await _dbVillaNumber.SaveAsync();


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


        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDto updateDto)
        {
            try
            {


                if (updateDto == null || id != updateDto.VillaNo)
                    return BadRequest();

                if (await _dbVilla.GetAsync(v => v.Id == updateDto.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa ID is invalid");
                    return BadRequest(ModelState);
                }


                /*           var villa = _db.Villas.FirstOrDefault(v => v.Id == id);

                            villa.Name = villaDto.Name;
                                       villa.Sqft = villaDto.Sqft;
                                       villa.Occupancy = villaDto.Occupancy;*/

                VillaNumber model = _mapper.Map<VillaNumber>(updateDto);

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
                await _dbVillaNumber.UpdateAsync(model);
                await _dbVillaNumber.SaveAsync();

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
        public async Task<IActionResult> UpdatePartialVillaNumber(int id, JsonPatchDocument<VillaNumberUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
                return BadRequest();
            var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id, tracked: false);


            VillaNumberUpdateDto villaNumberDto = _mapper.Map<VillaNumberUpdateDto>(villaNumber);


            if (villaNumber == null)
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


            patchDto.ApplyTo(villaNumberDto, ModelState);



            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDto);

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

            await _dbVillaNumber.UpdateAsync(model);
            await _dbVillaNumber.SaveAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();

        }





    }
}
