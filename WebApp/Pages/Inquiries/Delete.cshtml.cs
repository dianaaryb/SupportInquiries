using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages.Inquiries
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DeleteModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry != null)
            {
                Inquiry = inquiry;
                _context.Inquiries.Remove(Inquiry);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
