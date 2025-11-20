using Xunit;
using FluentAssertions;
using Erp.Documents.Domain.Entities;
using Erp.Documents.Domain.Enums;

namespace Erp.Documents.Tests.Integration
{
    public class DocumentEntityTests
    {
        [Fact]
        public void Document_Creation_ShouldInitializeProperties()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var documentId = Guid.NewGuid();

            // Act
            var document = new Document
            {
                Id = documentId,
                CompanyId = companyId,
                EntityType = "Invoice",
                EntityId = "INV-001",
                Name = "invoice.pdf",
                MimeType = "application/pdf",
                SizeBytes = 102400,
                BucketKey = "documents/company-123/invoice/INV-001-invoice.pdf",
                CreatedByUserId = "user-123",
                ValidationStatus = ValidationStatus.P,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            // Assert
            document.Id.Should().Be(documentId);
            document.CompanyId.Should().Be(companyId);
            document.EntityType.Should().Be("Invoice");
            document.ValidationStatus.Should().Be(ValidationStatus.P);
            document.MimeType.Should().Be("application/pdf");
        }

        [Fact]
        public void ValidationFlow_Creation_ShouldInitializeSteps()
        {
            // Arrange
            var flowId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var step1 = new ValidationStep
            {
                Id = Guid.NewGuid(),
                FlowId = flowId,
                Order = 1,
                Status = StepApprovalStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
            var step2 = new ValidationStep
            {
                Id = Guid.NewGuid(),
                FlowId = flowId,
                Order = 2,
                Status = StepApprovalStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            // Act
            var flow = new DocumentValidationFlow
            {
                Id = flowId,
                DocumentId = documentId,
                Steps = new List<ValidationStep> { step1, step2 },
                Actions = new List<ValidationAction>(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            // Assert
            flow.Steps.Should().HaveCount(2);
            flow.Steps.First().Order.Should().Be(1);
            flow.Steps.Last().Order.Should().Be(2);
            flow.Steps.All(s => s.Status == StepApprovalStatus.Pending).Should().BeTrue();
        }

        [Fact]
        public void ValidationStatus_Enum_ShouldHaveCorrectValues()
        {
            // Assert
            ValidationStatus.P.Should().Be(ValidationStatus.P);
            ValidationStatus.A.Should().Be(ValidationStatus.A);
            ValidationStatus.R.Should().Be(ValidationStatus.R);
        }

        [Fact]
        public void ValidationActionType_Enum_ShouldHaveCorrectValues()
        {
            // Assert
            ValidationActionType.Approve.Should().Be(ValidationActionType.Approve);
            ValidationActionType.Reject.Should().Be(ValidationActionType.Reject);
        }
    }
}
