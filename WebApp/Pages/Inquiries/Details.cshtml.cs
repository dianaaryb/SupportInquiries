using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages.Inquiries
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public Inquiry Inquiry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries.FirstOrDefaultAsync(m => m.InquiryId == id);
            if (inquiry == null)
            {
                return NotFound();
            }
            else
            {
                Inquiry = inquiry;
            }
            return Page();
        }
    }
}
