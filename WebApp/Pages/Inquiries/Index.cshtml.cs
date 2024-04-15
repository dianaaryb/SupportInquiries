using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages.Inquiries
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        [BindProperty] public IList<Inquiry> Inquiries { get;set; } = default!;

        public async Task<IActionResult> OnPostAsync(Guid InquiryId)
        {
            var inquiry = await _context.Inquiries.FindAsync(InquiryId);
            if (inquiry != null)
            {
                inquiry.IsResolved = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task OnGetAsync()
        {
            Inquiries = await _context.Inquiries
                .Where(i => !i.IsResolved)
                .OrderBy(i => i.ResolutionDeadline)
                .ToListAsync();
        }
    }
}
