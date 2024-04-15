using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApp.Pages.Inquiries;

namespace App.Tests;

public class InquiriesCreateModelOnGetTests
{
    private Mock<AppDbContext> _mockContext;
    private CreateModel _createPageModel;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AppDbContext>(options);
        _createPageModel = new CreateModel(_mockContext.Object);
    }

    [Test]
    public void OnGet_WhenCalled_InitializesInquiryWithDefaultValues()
    {
        var result = _createPageModel.OnGet();
        
        Assert.IsInstanceOf<PageResult>(result);
        Assert.NotNull(_createPageModel.Inquiry);
        Assert.That(_createPageModel.Inquiry.SubmissionTime, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(60)));
        Assert.AreEqual(new DateTime(DateTime.Now.Year, 1, 1), _createPageModel.Inquiry.ResolutionDeadline);
    }
    
    [TearDown]
    public void TearDown()
    {
        _mockContext.Object?.Dispose();
    }
}