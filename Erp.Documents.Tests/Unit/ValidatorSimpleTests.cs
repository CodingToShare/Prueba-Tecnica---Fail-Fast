using Xunit;
using FluentAssertions;
using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Validators;

namespace Erp.Documents.Tests.Unit
{
    public class ValidatorSimpleTests
    {
        [Fact]
        public void UploadValidator_Valid_ShouldPass()
        {
            var validator = new UploadDocumentRequestValidator();
            var dto = new UploadDocumentRequest
            {
                CompanyId = Guid.NewGuid(),
                EntityType = "Invoice",
                EntityId = "INV-001",
                FileName = "test.pdf",
                MimeType = "application/pdf",
                FileSizeBytes = 1000
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ApproveValidator_Valid_ShouldPass()
        {
            var validator = new ApproveDocumentRequestValidator();
            var dto = new ApproveDocumentRequest
            {
                DocumentId = Guid.NewGuid(),
                ApproverUserId = "user-123",
                Reason = "OK"
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RejectValidator_Valid_ShouldPass()
        {
            var validator = new RejectDocumentRequestValidator();
            var dto = new RejectDocumentRequest
            {
                DocumentId = Guid.NewGuid(),
                RejecterUserId = "user-456",
                Reason = "Invalid"
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeTrue();
        }

        // Error cases

        [Fact]
        public void UploadValidator_WithEmptyFileName_ShouldFail()
        {
            var validator = new UploadDocumentRequestValidator();
            var dto = new UploadDocumentRequest
            {
                CompanyId = Guid.NewGuid(),
                EntityType = "Invoice",
                EntityId = "INV-001",
                FileName = "",
                MimeType = "application/pdf",
                FileSizeBytes = 1000
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void UploadValidator_WithZeroFileSize_ShouldFail()
        {
            var validator = new UploadDocumentRequestValidator();
            var dto = new UploadDocumentRequest
            {
                CompanyId = Guid.NewGuid(),
                EntityType = "Invoice",
                EntityId = "INV-001",
                FileName = "test.pdf",
                MimeType = "application/pdf",
                FileSizeBytes = 0
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ApproveValidator_WithEmptyDocumentId_ShouldFail()
        {
            var validator = new ApproveDocumentRequestValidator();
            var dto = new ApproveDocumentRequest
            {
                DocumentId = Guid.Empty,
                ApproverUserId = "user-123",
                Reason = "OK"
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ApproveValidator_WithEmptyApproverUserId_ShouldFail()
        {
            var validator = new ApproveDocumentRequestValidator();
            var dto = new ApproveDocumentRequest
            {
                DocumentId = Guid.NewGuid(),
                ApproverUserId = "",
                Reason = "OK"
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void RejectValidator_WithEmptyReason_ShouldFail()
        {
            var validator = new RejectDocumentRequestValidator();
            var dto = new RejectDocumentRequest
            {
                DocumentId = Guid.NewGuid(),
                RejecterUserId = "user-456",
                Reason = ""
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void RejectValidator_WithEmptyRejecterUserId_ShouldFail()
        {
            var validator = new RejectDocumentRequestValidator();
            var dto = new RejectDocumentRequest
            {
                DocumentId = Guid.NewGuid(),
                RejecterUserId = "",
                Reason = "Invalid document"
            };
            
            var result = validator.Validate(dto);
            result.IsValid.Should().BeFalse();
        }
    }
}
