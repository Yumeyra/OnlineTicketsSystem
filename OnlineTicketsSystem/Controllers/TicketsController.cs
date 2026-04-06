
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using OnlineTicketsSystem.Services.Interfaces;
//using System.Security.Claims;

//namespace OnlineTicketsSystem.Controllers
//{
//    [Authorize]
//    public class TicketsController : Controller
//    {
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly ITicketService _ticketService;

//        public TicketsController(UserManager<IdentityUser> userManager, ITicketService ticketService)
//        {
//            _userManager = userManager;
//            _ticketService = ticketService;
//        }

//        // /Tickets/My
//        public async Task<IActionResult> My()
//        {
//            var userId = _userManager.GetUserId(User);
//            if (userId == null) return Challenge();

//            var tickets = await _ticketService.GetUserPaidTicketsAsync(userId);
//            return View(tickets);
//        }

//        [Authorize]
//        public async Task<IActionResult> DownloadPdf(int id)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var pdfBytes = await _ticketService.GenerateTicketPdfAsync(id, userId!);

//            if (pdfBytes == null)
//                return BadRequest("PDF може да се сваля само за платени билети или билетът не съществува.");

//            var fileName = $"ticket_{id}.pdf";
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
            if (userId == null)
            {
                TempData["Warning"] = "Трябва да влезете в профила си.";
                return Challenge();
            }

            var tickets = await _ticketService.GetUserPaidTicketsAsync(userId);
            return View(tickets);
        }

        [Authorize]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                TempData["Warning"] = "Трябва да влезете в профила си.";
                return Challenge();
            }

            var pdfBytes = await _ticketService.GenerateTicketPdfAsync(id, userId);

            if (pdfBytes == null)
            {
                TempData["Error"] = "PDF може да се сваля само за платени билети или билетът не съществува.";
                return RedirectToAction("My");
            }

            TempData["Success"] = "PDF билетът беше изтеглен успешно.";

            var fileName = $"ticket_{id}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
