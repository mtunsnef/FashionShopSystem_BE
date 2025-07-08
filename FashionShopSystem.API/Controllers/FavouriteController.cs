using System.Security.Claims;
using FashionShopSystem.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteController : ControllerBase
    {
        private readonly IFavouriteService _favouriteService;
        public FavouriteController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateFavourite([FromBody] AddFavouriteDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid favourite data.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            int parsedUserId;
            if (!int.TryParse(userId, out parsedUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            return Ok(await _favouriteService.AddToFavourites(dto, parsedUserId));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavourite(int id)
        {
            var result = await _favouriteService.deleteFavourite(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
