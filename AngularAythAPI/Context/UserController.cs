using AngularAythAPI.Helpers;
using AngularAythAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using MimeKit;
using System;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using AngularAuthAPI.Context;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AngularAythAPI.Context
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public UserController(AppDbContext authDbContext)
        {

            _authContext = authDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }
            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.UserName == userObj.UserName);
            if (user == null)
                return NotFound(new { Message = "Ne postoji korisnik sa tim korisničkim imenom!" });

            if (!PasswordHasher.VarifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Netačna lozinka!"});
            }
                
            user.Token = CreateJwt(user);
            await _authContext.SaveChangesAsync();
            return Ok(new {
                Token = user.Token,
                Message = "Uspešno logovanje!" }); ;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            if (await CheckUserNameExistAsync(userObj.UserName))
                return BadRequest(new { Message = "Postoji korisnik sa tim korisničkim imenom" });
            if (await CheckUEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Postoji korisnik sa tom email adresom" });
            var pass = CheckPasswordStregth(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });
            string unHashed = userObj.Password;
            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            SendRegistrationEmail(userObj, unHashed);

            return Ok(new { Message = "Uspešno ste se registrovali!" });
        }

        [HttpGet("all-profesors")]
        public async Task<IActionResult> getFreeMentors()
        {
            try
            {
                int limit = 10;

                var mentorsUnderLimit = await _authContext.Users
                    .Where(user => user.Role == "Profesor")
                    .Where(professor => _authContext.Labors.Count(labor => labor.ProfesorId == professor.Id) < limit)
                    .ToListAsync();

                return Ok(mentorsUnderLimit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dohvatanja profesora ispod limita.", Error = ex.Message });
            }
        }

        [HttpGet("all-mentorships/{userId}")]
        public async Task<IActionResult> AllMentorships(int userId)
        {
            if (userId == null)
                return BadRequest(new { Message = "Nemate nijedno mentorstvo!" });

            return Ok(_authContext.Users.Where(user => user.ProfesorId == userId && user.Role == "Student").ToList());
        }

        [HttpPut("remove-mentor/{userId}")]
        public async Task<IActionResult> RemoveMentor(int userId)
        {
            var user = await _authContext.Users.FindAsync(userId);
            if (user == null)
                return BadRequest(new { Message = "Greska u pronalazenju korisnika!" });
            user.StudentsLaborId = null;
            user.ProfesorId = null;
            await _authContext.SaveChangesAsync();

            return Ok(new { Message = "Tema za uspesno vracena!" });
        }

        private Task<bool> CheckUserNameExistAsync(string username)
           => _authContext.Users.AnyAsync(x => x.UserName == username);

        private Task<bool> CheckUEmailExistAsync(string email)
           => _authContext.Users.AnyAsync(x => x.Email == email);

        private string CheckPasswordStregth(string password)
        {
            StringBuilder sb = new StringBuilder();

            if (password.Length < 8)
                sb.Append("Lozinka mora sadržati minimum 8 karaktera" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Lozinka mora biti alfanumerička" + Environment.NewLine);
            //if (!(Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))) ;
            //sb.Append("Password shoul contian special chars" + Environment.NewLine);

            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
            }) ;

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        [HttpGet("get-user/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _authContext.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Message = "Korisnik nije pronađen!" });
                }

                return Ok(new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dohvatanja korisnika.", Error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChagePasswordRequest changePasswordRequest)
        {
            try
            {
                var user = await _authContext.Users.FindAsync(changePasswordRequest.Id);

                if (user == null)
                {
                    return NotFound(new { Message = "Korisnik nije pronađen!" });
                }

                if (!PasswordHasher.VarifyPassword(changePasswordRequest.Password, user.Password))
                {
                    return BadRequest(new { Message = "Stara lozinka nije ispravna!" });
                }

                user.Password = PasswordHasher.HashPassword(changePasswordRequest.NewPassword);
                await _authContext.SaveChangesAsync();

                return Ok(new { Message = "Lozinka je uspešno promenjena!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom promene lozinke.", Error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }

        private void SendRegistrationEmail(User userObj, string password)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("GraduateConnect", "graduateconnect7@gmail.com"));
                message.To.Add(new MailboxAddress("", userObj.Email));
                message.Subject = "Uspešna registracija";

                message.Body = new TextPart("plain")
                {
                    Text = userObj.Role == "Student" ? $"Poštovani/a {userObj.FirstName} {userObj.LastName},\n\nHvala što ste se registrovali na GraduateConnect platformu. Vaš nalog je uspešno kreiran.\n\nSrdačan pozdrav,\nGraduateConnect" : $"Poštovani/a {userObj.FirstName} {userObj.LastName},\\n\\nKreiran Vam je nalog na platformi GraduateConnect, sa ulogom {userObj.Role}.\n\nVaše korisničko ime i lozinka:\n\nKorisničko ime: {userObj.UserName}\nLozinka: {password}\n\n OBAVEŠTENJE: \nZa prijavu na sistem koristite korisničko ime. \nKreiranu lozinku možete promeniti kada se prijavite na sistem. \n\nSrdačan pozdrav,\nGraduateConnect"
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true; 
                    client.Connect("smtp.gmail.com", 587, false);

                    client.Authenticate(new NetworkCredential("graduateconnect7@gmail.com", "tsan uwbr xhcu rmbf"));

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom slanja email-a: {ex.Message}");
            }
        }
    }
}
