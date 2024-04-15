using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace App.Tests;

public class AppDbContextTests
{
    private AppDbContext _context;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        
        _context.Inquiries.Add(new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Test description",
            SubmissionTime = DateTime.Now,
            ResolutionDeadline = DateTime.Now.AddDays(1),
            IsResolved = false
        });
        _context.SaveChanges();
    }

    [Test]
    public void CreateInquiry_SavesToDatabase()
    {
        var newInquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "New Inquiry",
            SubmissionTime = DateTime.UtcNow,
            ResolutionDeadline = DateTime.UtcNow.AddDays(1),
            IsResolved = false
        };

        _context.Inquiries.Add(newInquiry);
        _context.SaveChanges();

        var inquiryExists = _context.Inquiries.Any(i => i.InquiryId == newInquiry.InquiryId);
        Assert.IsTrue(inquiryExists);
    }
    
    [Test]
    public void GetInquiry_ByInquiryId_ReturnsInquiry()
    {
        var inquiryId = Guid.NewGuid();
        var inquiry = new Inquiry
        {
            InquiryId = inquiryId,
            Description = "Get Inquiry",
            SubmissionTime = DateTime.UtcNow,
            ResolutionDeadline = DateTime.UtcNow.AddDays(1),
            IsResolved = false
        };

        _context.Inquiries.Add(inquiry);
        _context.SaveChanges();
        
        var foundInquiry = _context.Inquiries.Find(inquiryId);
        
        Assert.IsNotNull(foundInquiry);
        Assert.AreEqual("Get Inquiry", foundInquiry.Description);
    }
    
    [Test]
    public void DeleteInquiry_ByInquiryId_RemovesFromDatabase()
    {
        var inquiryId = Guid.NewGuid();
        var inquiry = new Inquiry
        {
            InquiryId = inquiryId,
            Description = "Delete Inquiry",
            SubmissionTime = DateTime.UtcNow,
            ResolutionDeadline = DateTime.UtcNow.AddDays(1),
            IsResolved = false
        };

        _context.Inquiries.Add(inquiry);
        _context.SaveChanges();
        
        var foundInquiry = _context.Inquiries.Find(inquiryId);
        _context.Inquiries.Remove(foundInquiry);
        _context.SaveChanges();

        var inquiryAfterDelete = _context.Inquiries.Find(inquiryId);
        
        Assert.IsNull(inquiryAfterDelete);
    }
    
    [Test]
    public void MarkInquiryIsResolved_UpdatesIsResolved()
    {
        var inquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "Resolve this inquiry",
            SubmissionTime = DateTime.UtcNow,
            ResolutionDeadline = DateTime.UtcNow.AddDays(1),
            IsResolved = false
        };

        _context.Inquiries.Add(inquiry);
        _context.SaveChanges();
        
        inquiry.IsResolved = true;
        _context.Inquiries.Update(inquiry);
        _context.SaveChanges();
        
        var updatedInquiry = _context.Inquiries.Find(inquiry.InquiryId);
        Assert.IsNotNull(updatedInquiry);
        Assert.IsTrue(updatedInquiry.IsResolved);
    }

    [Test]
    public void CreateInquiry_DescriptionNull_ShouldFail()
    {
        var newInquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = null,
            SubmissionTime = DateTime.UtcNow,
            ResolutionDeadline = DateTime.UtcNow.AddDays(1),
            IsResolved = false
        };

        _context.Inquiries.Add(newInquiry);
        
        var ex = Assert.Throws<DbUpdateException>(() => _context.SaveChanges());
        Assert.IsNotNull(ex);
    }

    [Test]
    public void CreateInquiry_WithPastResolutionDeadline_ShouldFail()
    {
        var newInquiry = new Inquiry
        {
            InquiryId = Guid.NewGuid(),
            Description = "This should fail",
            SubmissionTime = DateTime.UtcNow,
            ResolutionDeadline = DateTime.UtcNow.AddDays(-1),
            IsResolved = false
        };

        _context.Inquiries.Add(newInquiry);
        
        var ex = Assert.Throws<DbUpdateException>(() => _context.SaveChanges());
        Assert.IsNotNull(ex);
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
