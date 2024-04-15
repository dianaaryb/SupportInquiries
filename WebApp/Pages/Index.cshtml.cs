using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly DAL.AppDbContext _context;
    public IList<Inquiry> FilteredInquiries { get; set; } = default!;

    public IndexModel(DAL.AppDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        var currentTime = DateTime.Now;
        if (_context.Inquiries != null)
        {
             FilteredInquiries = await _context.Inquiries
                 .Where(i => !i.IsResolved)
                .Where(i => i.ResolutionDeadline <= currentTime.AddHours(1))
                .ToListAsync();
        }
    }
}