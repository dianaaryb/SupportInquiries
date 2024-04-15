using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using WebApp.Pages.Inquiries;

namespace App.Tests;

public class InquiriesIndexModelTests
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
    public async Task OnGetAsync_PopulatesUnresolvedInquiriesOrderedByDeadline()
    {
        var inquiries = new List<Inquiry>
        {
            new Inquiry { InquiryId = Guid.NewGuid(), IsResolved = false, ResolutionDeadline = DateTime.Now.AddDays(2) },
            new Inquiry { InquiryId = Guid.NewGuid(), IsResolved = false, ResolutionDeadline = DateTime.Now.AddDays(1) },
            new Inquiry { InquiryId = Guid.NewGuid(), IsResolved = true, ResolutionDeadline = DateTime.Now.AddDays(3) }
        };

        foreach (var inquiry in inquiries)
        {
            await _context.Inquiries.AddAsync(inquiry);
        }
        await _context.SaveChangesAsync();
        
        await _pageModel.OnGetAsync();
        
        Assert.AreEqual(2, _pageModel.Inquiries.Count);
        Assert.AreEqual(inquiries[1].InquiryId, _pageModel.Inquiries[0].InquiryId);
        Assert.IsFalse(_pageModel.Inquiries[0].IsResolved);
    }
    
    [Test]
    public async Task OnPostAsync_ResolvesInquiry_WhenInquiryExists()
    {
        var inquiry = new Inquiry { 
            InquiryId = Guid.NewGuid(), 
            IsResolved = false,
        };

        await _context.Inquiries.AddAsync(inquiry);
        await _context.SaveChangesAsync();

        await _pageModel.OnPostAsync(inquiry.InquiryId);

        var resolvedInquiry = await _context.Inquiries.FindAsync(inquiry.InquiryId);
        Assert.IsNotNull(resolvedInquiry);
        Assert.IsTrue(resolvedInquiry?.IsResolved);
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}