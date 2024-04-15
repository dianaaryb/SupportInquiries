using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Pages.Inquiries;

namespace App.Tests;

public class InquiriesEditModelTests
{
    private AppDbContext _context;
    private EditModel _pageModel;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _pageModel = new EditModel(_context);
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
    public async Task OnGetAsync_WhenInquiryExists_ReturnsPageResultWithInquiry()
    {
        var expectedInquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Test Description",
            SubmissionTime = DateTime.Now,
            ResolutionDeadline = DateTime.Now.AddDays(1),
            IsResolved = false
        };
        await _context.Inquiries.AddAsync(expectedInquiry);
        await _context.SaveChangesAsync();
        
        var result = await _pageModel.OnGetAsync(expectedInquiry.InquiryId);

        Assert.IsInstanceOf<PageResult>(result);
        Assert.AreEqual(expectedInquiry.InquiryId, _pageModel.Inquiry.InquiryId);
    }

    [Test]
    public async Task OnPostAsync_WhenModelStateIsInvalid_ReturnsPageResult()
    {
        _pageModel.ModelState.AddModelError("Error", "Sample error description");
        
        var result = await _pageModel.OnPostAsync();

        Assert.IsInstanceOf<PageResult>(result);
    }

    [Test]
    public async Task OnPostAsync_WhenInquiryExistsAndValid_UpdatesInquiryAndRedirects()
    {
        var inquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Initial Description",
            IsResolved = false
        };
        await _context.Inquiries.AddAsync(inquiry);
        await _context.SaveChangesAsync();

        var inquiryToUpdate = await _context.Inquiries.FindAsync(inquiry.InquiryId);
        inquiryToUpdate.Description = "Updated Description";
        inquiryToUpdate.IsResolved = true;

        _pageModel.Inquiry = inquiryToUpdate;

        var result = await _pageModel.OnPostAsync();

        var updatedInquiry = await _context.Inquiries.FindAsync(inquiry.InquiryId);
        Assert.AreEqual("Updated Description", updatedInquiry.Description);
        Assert.IsTrue(updatedInquiry.IsResolved);
        Assert.IsInstanceOf<RedirectToPageResult>(result);
    }


    [Test]
    public async Task OnPostAsync_WhenInquiryDoesNotExist_ReturnsNotFoundResult()
    {
        _pageModel.Inquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Updated Description",
            IsResolved = true
        };

        var result = await _pageModel.OnPostAsync();

        Assert.IsInstanceOf<NotFoundResult>(result);
    }


    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}