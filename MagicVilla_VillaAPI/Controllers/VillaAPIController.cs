//using MagicVilla_VillaAPI.Data;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDto>>(villaList));

        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDto>(villa));

        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto createDto)
        {

            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Name Already Exists");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa model = _mapper.Map<Villa>(createDto);

           
            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);

        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            //return Ok("villa Removed Successfully");
            return NoContent();
        }

        [HttpPut("{id:int}", Name ="Updatevilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Updatevilla(int id,[FromBody]VillaUpdateDto updateDto)
        {
            if(updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDto);

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return Ok("Villa Updated Successfully");
        }

        [HttpPatch("{id:int}", Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if(patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            if (villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villaDto, ModelState);

            Villa model = _mapper.Map<Villa>(villaDto);

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
