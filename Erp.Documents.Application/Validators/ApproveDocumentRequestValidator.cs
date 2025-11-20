using FluentValidation;
using Erp.Documents.Application.DTOs;

namespace Erp.Documents.Application.Validators
{
    /// <summary>
    /// Validador para solicitudes de aprobación de documentos.
    /// </summary>
    public class ApproveDocumentRequestValidator : AbstractValidator<ApproveDocumentRequest>
    {
        public ApproveDocumentRequestValidator()
        {
            RuleFor(x => x.DocumentId)
                .NotEmpty()
                .WithMessage("DocumentId es requerido");

            RuleFor(x => x.ApproverUserId)
                .NotEmpty()
                .WithMessage("ApproverUserId es requerido")
                .MaximumLength(100)
                .WithMessage("ApproverUserId no puede exceder 100 caracteres");

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage("Reason no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Reason));
        }
    }
}
