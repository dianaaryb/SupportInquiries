using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Pages.Inquiries;

namespace App.Tests;

public class InquiriesDetailsModelTests
{
    private AppDbContext _context;
    private DetailsModel _pageModel;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _pageModel = new DetailsModel(_context);
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
    public async Task OnGetAsync_WhenInquiryExists_ReturnsPageResultAndSetsInquiry()
    {
        var expectedInquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Test Description",
            SubmissionTime = DateTime.Now,
            ResolutionDeadline = DateTime.Now.AddDays(2),
            IsResolved = false
        };

        await _context.Inquiries.AddAsync(expectedInquiry);
        await _context.SaveChangesAsync();

        var result = await _pageModel.OnGetAsync(expectedInquiry.InquiryId);

        Assert.IsInstanceOf<PageResult>(result);
        Assert.AreEqual(expectedInquiry.InquiryId, _pageModel.Inquiry.InquiryId);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}