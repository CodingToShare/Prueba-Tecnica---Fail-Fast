using FluentValidation;
using Erp.Documents.Application.DTOs;

namespace Erp.Documents.Application.Validators
{
    /// <summary>
    /// Validador para solicitudes de rechazo de documentos.
    /// </summary>
    public class RejectDocumentRequestValidator : AbstractValidator<RejectDocumentRequest>
    {
        public RejectDocumentRequestValidator()
        {
            RuleFor(x => x.DocumentId)
                .NotEmpty()
                .WithMessage("DocumentId es requerido");

            RuleFor(x => x.RejecterUserId)
                .NotEmpty()
                .WithMessage("RejecterUserId es requerido")
                .MaximumLength(100)
                .WithMessage("RejecterUserId no puede exceder 100 caracteres");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason es requerido para rechazar un documento")
                .MaximumLength(500)
                .WithMessage("Reason no puede exceder 500 caracteres");
        }
    }
}
