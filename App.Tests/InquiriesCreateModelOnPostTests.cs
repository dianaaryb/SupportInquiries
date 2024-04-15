using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Pages.Inquiries;

namespace App.Tests;

public class InquiriesCreateModelOnPostTests
{
    private DbContextOptions<AppDbContext> _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Test]
    public async Task OnPostAsync_WhenModelStateIsValid_RedirectsToIndex()
    {
        using var context = new AppDbContext(_options);
        var pageModel = new CreateModel(context)
        {
            Inquiry = new Inquiry
            {
                Description = "Test Inquiry",
                SubmissionTime = DateTime.UtcNow,
                ResolutionDeadline = DateTime.UtcNow.AddDays(1),
                IsResolved = false
            }
        };
        
        var result = await pageModel.OnPostAsync();
        
        Assert.IsInstanceOf<RedirectToPageResult>(result);
    }
    
    [Test]
    public async Task OnPostAsync_WithNullDescription_ReturnsPageWithModelError()
    {
        using var context = new AppDbContext(_options);
        var pageModel = new CreateModel(context)
        {
            Inquiry = new Inquiry
            {
                SubmissionTime = DateTime.UtcNow,
                ResolutionDeadline = DateTime.UtcNow.AddDays(1),
                IsResolved = false
            }
        };
        
        var result = await pageModel.OnPostAsync();
        
        Assert.IsInstanceOf<PageResult>(result);
        Assert.IsFalse(pageModel.ModelState.IsValid);
        Assert.IsTrue(pageModel.ModelState.ContainsKey("Inquiry.Description"));
    }
    
    [Test]
    public async Task OnPostAsync_WithPastResolutionDeadline_ReturnsPageWithModelError()
    {
        using var context = new AppDbContext(_options);
        var pageModel = new CreateModel(context)
        {
            Inquiry = new Inquiry
            {
                Description = "Inquiry with past deadline",
                SubmissionTime = DateTime.UtcNow,
                ResolutionDeadline = DateTime.UtcNow.AddMinutes(-1),
                IsResolved = false
            }
        };
        
        var result = await pageModel.OnPostAsync();
        
        Assert.IsInstanceOf<PageResult>(result);
        Assert.IsFalse(pageModel.ModelState.IsValid);
        Assert.IsTrue(pageModel.ModelState.ContainsKey("Inquiry.ResolutionDeadline"));
        var entryCount = context.Inquiries.Count();
        Assert.AreEqual(0, entryCount, "No inquiries should be saved when there's a model error.");
    }


    [TearDown]
    public void TearDown()
    {
        using var context = new AppDbContext(_options);
        context.Database.EnsureDeleted();
    }
}