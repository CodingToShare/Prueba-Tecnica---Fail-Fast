using Microsoft.AspNetCore.Mvc;
using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Interfaces;

namespace Erp.Documents.Api.Controllers
{
    /// <summary>
    /// Controller para gestión de validación y aprobación de documentos.
    /// Proporciona endpoints para aprobar documentos en flujos jerárquicos multi-paso.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ValidationController : ControllerBase
    {
        private readonly IApproveDocumentService _approveService;
        private readonly IRejectDocumentService _rejectService;
        private readonly ILogger<ValidationController> _logger;

        public ValidationController(
            IApproveDocumentService approveService,
            IRejectDocumentService rejectService,
            ILogger<ValidationController> logger)
        {
            _approveService = approveService;
            _rejectService = rejectService;
            _logger = logger;
        }

        /// <summary>
        /// Aprueba un documento en el flujo de validación.
        /// Avanza al siguiente paso o marca como completamente aprobado si es el último paso.
        /// </summary>
        /// <param name="request">Datos de la solicitud de aprobación</param>
        /// <returns>Estado actualizado del documento</returns>
        /// <response code="200">Documento aprobado exitosamente</response>
        /// <response code="400">Solicitud inválida o documento no puede ser aprobado en su estado actual</response>
        /// <response code="404">Documento o flujo de validación no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocumentOperationResponse>> ApproveDocument(
            [FromBody] ApproveDocumentRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Aprobando documento: {DocumentId}, ApproverUserId: {ApproverUserId}",
                    request.DocumentId,
                    request.ApproverUserId);

                var response = await _approveService.ApproveAsync(request);
                return Ok(response);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning("Documento no encontrado: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error aprobando documento: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en ApproveDocument");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Rechaza un documento en el flujo de validación.
        /// Marca el documento con estado terminal "Rejected" (R).
        /// </summary>
        /// <param name="request">Datos de la solicitud de rechazo</param>
        /// <returns>Estado actualizado del documento</returns>
        /// <response code="200">Documento rechazado exitosamente</response>
        /// <response code="400">Solicitud inválida o documento no puede ser rechazado en su estado actual</response>
        /// <response code="404">Documento no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocumentOperationResponse>> RejectDocument(
            [FromBody] RejectDocumentRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Rechazando documento: {DocumentId}, RejecterUserId: {RejecterUserId}",
                    request.DocumentId,
                    request.RejecterUserId);

                var response = await _rejectService.RejectAsync(request);
                return Ok(response);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning("Documento no encontrado: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error rechazando documento: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en RejectDocument");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el estado actual del flujo de validación de un documento.
        /// Incluye información sobre todos los pasos y acciones registradas.
        /// </summary>
        /// <param name="documentId">ID del documento</param>
        /// <returns>Estado completo del flujo de validación</returns>
        /// <response code="200">Estado obtenido exitosamente</response>
        /// <response code="404">Documento no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{documentId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ValidationFlowStatusDto>> GetValidationStatus(
            [FromRoute] Guid documentId)
        {
            try
            {
                _logger.LogInformation("Obteniendo estado de validación para documento: {DocumentId}", documentId);

                var response = await _approveService.GetValidationStatusAsync(documentId);
                return Ok(response);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning("Documento no encontrado: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en GetValidationStatus");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}
