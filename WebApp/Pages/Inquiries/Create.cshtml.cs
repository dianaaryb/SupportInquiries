using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain;

namespace WebApp.Pages.Inquiries
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        
        [BindProperty]
        public Inquiry Inquiry { get; set; }
        
        public IActionResult OnGet()
        {
            if (Inquiry == null)
            {
                Inquiry = new Inquiry();
            }
            Inquiry.ResolutionDeadline = new DateTime(DateTime.Now.Year, 1, 1);
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (Inquiry.Description == null)
            {
                ModelState.AddModelError("Inquiry.Description","Description must be entered.");
            }
            else if (Inquiry.ResolutionDeadline <= DateTime.Now)
            {
                ModelState.AddModelError("Inquiry.ResolutionDeadline", "The resolution deadline must be after the current time.");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            Inquiry.SubmissionTime = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Millisecond);


            _context.Inquiries.Add(Inquiry);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
