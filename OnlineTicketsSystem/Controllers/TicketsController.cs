//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineTicketsSystem.Data;
//using OnlineTicketsSystem.Helpers;
//using System.Security.Claims;
//namespace OnlineTicketsSystem.Controllers
//{
//    public class TicketsController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<IdentityUser> _userManager;

//        public TicketsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        // /Tickets/My
//        public async Task<IActionResult> My()
//        {
//            var userId = _userManager.GetUserId(User);
//            if (userId == null) return Challenge();

//            var tickets = await _context.Tickets
//    .Where(t => t.UserId == userId && t.IsPaid)
//                .Include(t => t.Event)
//                .ThenInclude(e => e.Category)
//                .Where(t => t.UserId == userId)
//                .OrderByDescending(t => t.PaidAt ?? t.PurchaseDate)
//                .ToListAsync();

//            return View(tickets);
//        }
//        [Authorize]
//        public async Task<IActionResult> DownloadPdf(int id)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


//            var ticket = await _context.Tickets
//                .Include(t => t.Event)
//                    .ThenInclude(e => e.Category)
//                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

//            if (ticket == null)
//                return NotFound();

//            if (!ticket.IsPaid)
//                return BadRequest("PDF може да се сваля само за платени билети.");

//            var pdfBytes = TicketPdfDocument.Generate(
//                ticket.Event.Title,
//                ticket.Event.Category?.Name ?? "Без категория",
//                ticket.Event.City,
//                ticket.Event.Venue,
//                ticket.Event.Date,
//                ticket.Quantity,
//                ticket.UnitPrice,
//                ticket.UnitPrice * ticket.Quantity,
//                ticket.PaidAt,
//                $"TICKET-{ticket.Id}"
//            );

//            var fileName = $"ticket_{ticket.Id}.pdf";
//            return File(pdfBytes, "application/pdf", fileName);
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Services.Interfaces;
using System.Security.Claims;

namespace OnlineTicketsSystem.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITicketService _ticketService;

        public TicketsController(UserManager<IdentityUser> userManager, ITicketService ticketService)
        {
            _userManager = userManager;
            _ticketService = ticketService;
        }

        // /Tickets/My
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var tickets = await _ticketService.GetUserPaidTicketsAsync(userId);
            return View(tickets);
        }

        [Authorize]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pdfBytes = await _ticketService.GenerateTicketPdfAsync(id, userId!);

            if (pdfBytes == null)
                return BadRequest("PDF може да се сваля само за платени билети или билетът не съществува.");

            var fileName = $"ticket_{id}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}