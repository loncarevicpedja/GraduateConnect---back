using AngularAuthAPI.Context;
using AngularAythAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAythAPI.Context
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public ThemeController (AppDbContext appDbContext)
        {
           _authContext = appDbContext;
        }

        [HttpPost("add-theme")]
        public async Task<IActionResult> AddTeme(Theme themeObj)
        {
            if (themeObj == null)
                return BadRequest();
            if(await CheckThemeExist(themeObj.Name, themeObj.Field))
                return BadRequest(new { Message = "Uneta tema u toj oblasti vec postoji"});

            await _authContext.Themes.AddAsync(themeObj);
            await _authContext.SaveChangesAsync();

            return Ok(new {Message = "Tema je uspesno dodata!"});
        }

        [HttpPut("reserve-theme/{themeId}/{studentId}")]
        public async Task<IActionResult> ReserveTheme(int themeId, int studentId)
        {
            var theme = await _authContext.Themes.FindAsync(themeId);
            if (theme == null)
                return BadRequest(new { Message = "Greska u pronalazenju teme!" });
            theme.Status = false;
            theme.StudentId = studentId;
            await _authContext.SaveChangesAsync();

            return Ok(new {Message = "Tema je uspesno rezrervisana!!"});
        }

        [HttpPut("return-theme/{studentId}")]
        public async Task<IActionResult> ReturnTheme(int themeId, int studentId)
        {
            var theme = await _authContext.Themes.FindAsync(studentId);
            if (theme == null)
                return BadRequest(new { Message = "Greska u pronalazenju teme!" });
            theme.Status = true;
            theme.StudentId = null;
            await _authContext.SaveChangesAsync();

            return Ok(new { Message = "Tema je uspesno rezrervisana!!" });
        }

        [HttpGet("get-theme/{userId}")]
        public async Task<ActionResult<Theme>> GetUser(int userId)
        {
            var theme = await _authContext.Themes.FirstOrDefaultAsync(user => user.StudentId == userId);

            if (theme != null)
            {
                return Ok(theme);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("user-exist/{userId}")]
        public async Task<bool> UserExist (int userId){
            var userExists = await _authContext.Themes.AnyAsync(user => user.StudentId == userId);

            if (userExists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet("free-themes")]
        public async Task<IActionResult> FreeThemes()
        {
            return Ok(_authContext.Themes.Where( theme => theme.Status == true).ToList());
        }

        private Task<bool> CheckThemeExist(string name, string field)
            => _authContext.Themes.AnyAsync(x => x.Name == name && x.Field == field);

        private Task<bool> CheckUserExist(int userId)
            => _authContext.Themes.AllAsync(x => x.StudentId == userId);
        
    }
}
