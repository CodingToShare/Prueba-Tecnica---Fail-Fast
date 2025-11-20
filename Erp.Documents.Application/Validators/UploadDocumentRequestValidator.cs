using FluentValidation;
using Erp.Documents.Application.DTOs;

namespace Erp.Documents.Application.Validators
{
    /// <summary>
    /// Validador para solicitudes de carga de documentos.
    /// </summary>
    public class UploadDocumentRequestValidator : AbstractValidator<UploadDocumentRequest>
    {
        public UploadDocumentRequestValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("CompanyId es requerido");

            RuleFor(x => x.EntityType)
                .NotEmpty()
                .WithMessage("EntityType es requerido")
                .MaximumLength(100)
                .WithMessage("EntityType no puede exceder 100 caracteres");

            RuleFor(x => x.EntityId)
                .NotEmpty()
                .WithMessage("EntityId es requerido")
                .MaximumLength(100)
                .WithMessage("EntityId no puede exceder 100 caracteres");

            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("FileName es requerido")
                .MaximumLength(255)
                .WithMessage("FileName no puede exceder 255 caracteres")
                .Matches(@"^[a-zA-Z0-9._\-\s]+$")
                .WithMessage("FileName contiene caracteres inválidos");

            RuleFor(x => x.MimeType)
                .NotEmpty()
                .WithMessage("MimeType es requerido")
                .Matches(@"^[a-zA-Z]+\/[a-zA-Z0-9.\-\+]+$")
                .WithMessage("MimeType inválido (ej: application/pdf)");

            RuleFor(x => x.FileSizeBytes)
                .GreaterThan(0)
                .WithMessage("FileSizeBytes debe ser mayor a 0");

            RuleFor(x => x.UploadedByUserId)
                .MaximumLength(100)
                .WithMessage("UploadedByUserId no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.UploadedByUserId));
        }
    }
}
