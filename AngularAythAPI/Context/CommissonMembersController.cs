using AngularAuthAPI.Context;
using AngularAythAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAythAPI.Context
{
    public class CommissonMembersController
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

            [HttpPost("add-member/{commisionId}/{memberId}")]
            public async Task<IActionResult> AddMember(int commissionId, int memberId)
            {
                try
                {
                    var commission = _appContext.Commissions.Find(commissionId);
                    if (commission == null)
                    {
                        return NotFound(new { Message = "Nije pronadjena komisija!"});
                    }

                    var user = await _appContext.Users.FindAsync(commissionId);
                    if (user == null)
                    {
                        return NotFound(new { Message = "Korisnik nije pronadjen!" });
                    }
                    commission.CommissionMembers.Add(user);

                    _appContext.SaveChanges();

                    return Ok(new { Message = "Član komisije je uspešno dodat" });

                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "Greška prilikom dodavanja člana komisije", Error = ex.Message });
                }
            }

        }
    }
}
