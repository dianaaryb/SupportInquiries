using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Pages.Inquiries;

namespace App.Tests;

public class InquiriesDeleteModelTests
{
    private AppDbContext _context;
    private DeleteModel _pageModel;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _pageModel = new DeleteModel(_context);
    }

    [Test]
    public async Task OnGetAsync_WhenIdIsNull_ReturnsNotFoundResult()
    {
        var result = await _pageModel.OnGetAsync(null);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task OnGetAsync_WhenInquiryDoesNotExist_ReturnsNotFoundResult()
    {
        var result = await _pageModel.OnGetAsync(Guid.NewGuid());
        
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task OnGetAsync_WhenInquiryExists_ReturnsPageResult()
    {
        var inquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Test",
        };
        await _context.Inquiries.AddAsync(inquiry);
        await _context.SaveChangesAsync();

        var result = await _pageModel.OnGetAsync(inquiry.InquiryId);

        Assert.IsInstanceOf<PageResult>(result);
        Assert.AreEqual(inquiry.InquiryId, _pageModel.Inquiry.InquiryId);
    }

    [Test]
    public async Task OnPostAsync_WhenIdIsNull_ReturnsNotFoundResult()
    {
        var result = await _pageModel.OnPostAsync(null);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task OnPostAsync_WhenInquiryExists_DeletesInquiryAndRedirects()
    {
        var inquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Test",
        };
        await _context.Inquiries.AddAsync(inquiry);
        await _context.SaveChangesAsync();

        var result = await _pageModel.OnPostAsync(inquiry.InquiryId);

        var deletedInquiry = await _context.Inquiries.FindAsync(inquiry.InquiryId);
        Assert.IsNull(deletedInquiry);
        Assert.IsInstanceOf<RedirectToPageResult>(result);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}