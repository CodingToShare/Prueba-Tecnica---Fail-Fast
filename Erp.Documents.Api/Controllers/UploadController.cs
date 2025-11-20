using Microsoft.AspNetCore.Mvc;
using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Interfaces;

namespace Erp.Documents.Api.Controllers
{
    /// <summary>
    /// Controller para gestión de carga de documentos.
    /// Proporciona endpoints para iniciar y completar cargas de documentos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadDocumentService _uploadService;
        private readonly ILogger<UploadController> _logger;

        public UploadController(
            IUploadDocumentService uploadService,
            ILogger<UploadController> logger)
        {
            _uploadService = uploadService;
            _logger = logger;
        }

        /// <summary>
        /// Inicia la carga de un documento.
        /// Retorna una URL presignada para subir el archivo a storage.
        /// </summary>
        /// <param name="request">Datos de la solicitud de carga</param>
        /// <returns>URL presignada y metadatos del documento</returns>
        /// <response code="200">Carga iniciada exitosamente</response>
        /// <response code="400">Solicitud inválida (archivo muy grande, MIME type no permitido, etc.)</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("initiate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UploadDocumentResponse>> InitiateUpload(
            [FromBody] UploadDocumentRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando carga de documento: {FileName}, Tamaño: {FileSizeBytes} bytes",
                    request.FileName,
                    request.FileSizeBytes);

                var response = await _uploadService.InitiateUploadAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Error de validación en solicitud de carga: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Operación inválida en carga: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en InitiateUpload");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Completa la carga de un documento.
        /// Verifica que el archivo fue subido a storage correctamente.
        /// </summary>
        /// <param name="documentId">ID del documento</param>
        /// <returns>URL de descarga y metadatos finales</returns>
        /// <response code="200">Carga completada exitosamente</response>
        /// <response code="404">Documento no encontrado</response>
        /// <response code="400">Archivo no encontrado en storage</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("{documentId}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UploadDocumentResponse>> CompleteUpload(
            [FromRoute] Guid documentId)
        {
            try
            {
                _logger.LogInformation("Completando carga del documento: {DocumentId}", documentId);

                var response = await _uploadService.CompleteUploadAsync(documentId);
                return Ok(response);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning("Documento no encontrado: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error completando carga: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en CompleteUpload");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}
