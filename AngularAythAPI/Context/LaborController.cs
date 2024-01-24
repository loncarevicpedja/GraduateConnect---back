using AngularAuthAPI.Context;
using AngularAythAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace AngularAythAPI.Context
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaborController : ControllerBase
    {
        private readonly AppDbContext _appContext;
        private readonly IWebHostEnvironment _environment;
        private object _authContext;

        public LaborController(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _appContext = appDbContext;
            _environment = environment;
        }

        [HttpPost("create-labor")]
        public async Task<IActionResult> CreateLabor(Theme themeObj)
        {
            if (themeObj == null)
                return BadRequest();

            try
            {
                var labor = new Labor
                {
                    StudentId = themeObj.StudentId,
                    Name = themeObj.Name,
                    Description = themeObj.Description,
                    Status = "Nezapocet"
                };

                await _appContext.Labors.AddAsync(labor);
                await _appContext.SaveChangesAsync();

                return Ok(new { Message = themeObj });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom kreiranja Labor-a.", Error = ex.Message });
            }
        }

        [HttpPut("change-status/{laborId}/{status}")]
        public async Task<IActionResult> ChangeStatus(int laborId, string status)
        {
            var labor = await _appContext.Labors.FindAsync(laborId);
            if (labor == null)
                return BadRequest(new { Message = "Greska u pronalazenju zavrsnog rada!" });
            if(status == "Zavrsen")
            {
                labor.DateOfSubmission = DateTime.Now;
            }
            labor.Status = status;
            await _appContext.SaveChangesAsync();

            return Ok(new { Message = "Uspesno je promenjen status završnog rada!" });
        }

        [HttpGet("get-labor/{userId}")]
        public async Task<ActionResult<Labor>> GetLaborFromUser(int userId)
        {
            var labor = await _appContext.Labors.FirstOrDefaultAsync(labor => labor.StudentId == userId);

            if (labor != null)
            {
                return Ok(labor);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("get-finished-labors")]
        public async Task<ActionResult<Labor>> GetFinishedLabors()
        {
            try
            {
                var completedLabors = await _appContext.Labors
                    .Where(labor => labor.Status == "Zavrsen" && labor.DateOfSubmission != null && labor.DateOfDefense == null)
                    .ToListAsync();

                return Ok(completedLabors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dobijanja završenih radova.", Error = ex.Message });
            }
        }

        [HttpGet("get-defendend-labors")]
        public async Task<ActionResult<Labor>> GetDefendedLabors()
        {
            try
            {
                DateTime today = DateTime.Now.Date;

                var completedLabors = await _appContext.Labors
                    .Where(labor => labor.Status == "Zavrsen" && labor.DateOfSubmission != null && labor.DateOfDefense != null && labor.DateOfDefense < today)
                    .ToListAsync();

                return Ok(completedLabors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dobijanja završenih radova.", Error = ex.Message });
            }
        }

        [HttpPost("labors-by-commissions")]
        public IActionResult GetLaborsByCommissions([FromBody] List<Commission> commissionRequests)
        {
            try
            {
                var commissionIds = commissionRequests.Select(cr => cr.Id).ToList();

                // Danasšnji datum
                DateTime today = DateTime.Now.Date;

                var labors = _appContext.Labors
                    .Where(l => commissionIds.Contains((int)l.CommissionId) && l.DateOfDefense >= today)
                    .ToList();

                return Ok(labors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dohvatanja labore-a po komisijama.", Error = ex.Message });
            }
        }

        [HttpDelete("delete-labor/{laborId}")]
        public async Task<IActionResult> DeleteLabor(int laborId)
        {
            var labor = await _appContext.Labors.FirstOrDefaultAsync(labor => labor.Id == laborId);

            if (labor == null)
            {
                return NotFound();
            }
            _appContext.Labors.Remove(labor);
            await _appContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("set-mentor/{laborId}/{profesorId}")]
        public async Task<IActionResult> SetMentor(int laborId, int profesorId)
        {
            var labor = await _appContext.Labors.FindAsync(laborId);
            if (labor == null)
                return BadRequest(new { Message = "Greska u pronalazenju zavrsnog rada!" });

            var student = await _appContext.Users.FindAsync(labor.StudentId);
            if (student == null || student.Role != "Student")
                return BadRequest(new { Message = "Greska u pronalazenju studenta!" });

            labor.Status = "Zapocet";
            labor.ProfesorId = profesorId;

            student.StudentsLaborId = laborId;
            student.ProfesorId = profesorId;
            await _appContext.SaveChangesAsync();

            return Ok(new { Message = "Tema je uspesno rezervisana!" });
        }

        [HttpPut("set-rate/{laborId}/{rate}")]
        public async Task<IActionResult> SetRate(int laborId, int rate)
        {
            var labor = await _appContext.Labors.FindAsync(laborId);
            if (labor == null)
                return BadRequest(new { Message = "Greska u pronalazenju zavrsnog rada!" });

            labor.Status = "Zapocet";
            labor.Rate = rate;

            await _appContext.SaveChangesAsync();

            return Ok(new { Message = "Ocena je uspesno evidentirana!" });
        }

        [HttpPost("upload-file/{laborId}")]
        public async Task<IActionResult> UploadFile(int laborId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Greska prilikom dodavanja fajla!");

            try
            {
                if (Path.GetExtension(file.FileName).ToLower() != ".docx")
                {
                    return BadRequest("Mozete dodavati samo fajlove koji imaju .docx ekstenziju");
                }

                var uploadsFolder = Path.Combine("C:\\Users\\Predrag\\Desktop\\ang\\AngularAythAPI\\AngularAythAPI\\Uploads\\", laborId.ToString());

                if (Directory.Exists(uploadsFolder))
                {
                    Directory.Delete(uploadsFolder, true);
                }

                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return Ok(new { Message = "Fajl je uspesno dodat!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greska prilikom dodavanja fajla", Error = ex.Message });
            }
        }

        [HttpGet("check-file-existence/{laborId}")]
        public IActionResult CheckFileExistence(int laborId)
        {
            try
            {
                var uploadsFolder = Path.Combine("C:\\Users\\Predrag\\Desktop\\ang\\AngularAythAPI\\AngularAythAPI\\Uploads", laborId.ToString());
                var status = false;
                if (!Directory.Exists(uploadsFolder))
                {
                    status = false;
                }

                var files = Directory.GetFiles(uploadsFolder);

                if (files.Length > 0)
                {
                    var fileName = Path.GetFileName(files[0]);
                    return Ok(new { Exists = true, FileName = fileName });
                }

                return Ok(new { Exists = status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }



        [HttpGet("download-file/{laborId}")]
        public IActionResult DownloadFile(int laborId)
        {
            try
            {
                var uploadsFolder = Path.Combine("C:\\Users\\Predrag\\Desktop\\ang\\AngularAythAPI\\AngularAythAPI\\Uploads\\", laborId.ToString());
                var filePath = Directory.GetFiles(uploadsFolder).FirstOrDefault();

                if (filePath == null)
                    return NotFound(new { Message = "Fajl nije pronađen." });

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);

                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom preuzimanja fajla: {ex}");
                return StatusCode(500, new { Message = "Greška prilikom preuzimanja fajla", Error = ex.Message });
            }
        }

        [HttpPost("add-commision/{laborId}/{commissionId}")]
        public async Task<IActionResult> AddCommission(int laborId, int commissionId)
        {
            try
            {
                var labor = await _appContext.Labors.FindAsync(laborId);
                if (labor == null)
                    return BadRequest(new { Message = "Nema labora" });

                var commission = await _appContext.Commissions.FindAsync(commissionId);
                if (commission == null)
                    return BadRequest(new { Message = "Nema komisije" });

                labor.CommissionId = commissionId + 1;

                var user = await _appContext.Users.FirstOrDefaultAsync(u => u.StudentsLaborId == laborId);

                if (user == null)
                    return BadRequest(new { Message = "Nema korisnika" });

                user.CommissionId = commissionId + 1;

                await _appContext.SaveChangesAsync();
                return Ok(new { Message = "Uspesno evidentiranje" });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("set-defense-date/{laborId}/{defenseDate}")]
        public async Task<IActionResult> SetDefenseDate(int laborId,string defenseDate)
        {
            try
            {
                var labor = await _appContext.Labors.FindAsync(laborId);
                if (labor == null)
                    return BadRequest(new { Message = "Nema labora sa datim ID-om" });

                labor.DateOfDefense = DateTime.Parse(defenseDate);

                await _appContext.SaveChangesAsync();

                return Ok(new { Message = "Datum odbrane uspešno postavljen" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom postavljanja datuma odbrane", Error = ex.Message });
            }
        }
    }
}
