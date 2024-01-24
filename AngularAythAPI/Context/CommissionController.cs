using AngularAuthAPI.Context;
using AngularAythAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAythAPI.Context
{

    [Route("api/[controller]")]
    [ApiController]
    public class CommissionController : ControllerBase
    {
        private readonly AppDbContext _appContext;
        public CommissionController(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        [HttpPost("create-commission")]
        public IActionResult KreirajVrednost()
        {
            try
            {

                var noviRed = new Commission();
                _appContext.Commissions.Add(noviRed);
                _appContext.SaveChanges();

                return Ok(new { Message = "Vrednost uspešno kreirana", Id = noviRed.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom kreiranja vrednosti", Error = ex.Message });
            }
        }

        [HttpGet("last-created")]
        public IActionResult DohvatiPoslednjiRed()
        {
            try
            {
                var poslednjiRed = _appContext.Commissions
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                if (poslednjiRed == null)
                {
                    return NotFound(new { Message = "Tabela je prazna" });
                }

                return Ok(poslednjiRed);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dohvatanja poslednjeg reda", Error = ex.Message });
            }
        }

        [HttpPost("add-commission-member")]
        public IActionResult AddCommission([FromBody] Commission commissionRequest)
        {
            try
            {
            var commission = new Commission();
            var members = _appContext.Users.Where(u => commissionRequest.CommissionMembers.Select(m => m.Id).Contains(u.Id)).ToList();

            commission.CommissionMembers = members;

            _appContext.Commissions.Add(commission);
            _appContext.SaveChanges();

            return Ok(new { Message = "Komisija je uspešno kreirana."});
            }
            catch
            {
                return StatusCode(500, new { Message = "Greska prilikom kreiranja komisije!" });
            }
        }

        [HttpGet("all-commissions")]
        public IActionResult GetAllCommissions()
        {
            var commissions = _appContext.Commissions.ToList();
            return Ok(commissions);
        }

        [HttpGet("user-commissions/{commissionMembersId}")]
        public IActionResult GetUserCommissions(int commissionMembersId)
        {
            try
            {
                var userCommissions = _appContext.Commissions
                    .Where(c => c.CommissionMembers.Any())
                    .ToList();

                return Ok(userCommissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dohvatanja komisija korisnika", Error = ex.Message });
            }
        }

        [HttpGet("commission-members/{commissionId}")]
        public IActionResult GetCommissionMembersIds(int commissionId)
        {
            try
            {
                var commissionMembersIds = _appContext.Commissions
                    .Include(c => c.CommissionMembers) 
                    .Where(c => c.Id == commissionId)
                    .SelectMany(c => c.CommissionMembers)
                    .ToList();

                return Ok(commissionMembersIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Greška prilikom dohvatanja commissionMembersId", Error = ex.Message });
            }
        }

    }
}
