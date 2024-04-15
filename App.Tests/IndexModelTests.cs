using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace App.Tests;
using WebApp.Pages;

public class IndexModelTests
{
    private AppDbContext _context;
    private IndexModel _pageModel;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _pageModel = new IndexModel(_context);
    }

    [Test]
    public async Task OnGetAsync_FiltersUrgentInquiriesCorrectly()
    {
        var currentTime = DateTime.Now;
        var inquiries = new List<Inquiry>
        {
            new Inquiry { InquiryId = Guid.NewGuid(), IsResolved = false, ResolutionDeadline = currentTime.AddMinutes(30), Description = "Urgent Inquiry" },
            new Inquiry { InquiryId = Guid.NewGuid(), IsResolved = false, ResolutionDeadline = currentTime.AddDays(1), Description = "Non-Urgent Inquiry" },
            new Inquiry { InquiryId = Guid.NewGuid(), IsResolved = true, ResolutionDeadline = currentTime.AddMinutes(30), Description = "Resolved Inquiry" }
        };

        _context.Inquiries.AddRange(inquiries);
        await _context.SaveChangesAsync();

        await _pageModel.OnGetAsync();

        Assert.AreEqual(1, _pageModel.FilteredInquiries.Count);
        Assert.AreEqual("Urgent Inquiry", _pageModel.FilteredInquiries.First().Description);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}