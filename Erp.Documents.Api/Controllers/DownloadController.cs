using Microsoft.AspNetCore.Mvc;
using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Interfaces;

namespace Erp.Documents.Api.Controllers
{
    /// <summary>
    /// Controller para gestión de descarga de documentos.
    /// Proporciona endpoints para generar URLs presignadas de descarga.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadDocumentService _downloadService;
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(
            IDownloadDocumentService downloadService,
            ILogger<DownloadController> logger)
        {
            _downloadService = downloadService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene URL presignada para descargar un documento.
        /// La URL es válida por el tiempo especificado en configuración (típicamente 15 minutos).
        /// </summary>
        /// <param name="documentId">ID del documento a descargar</param>
        /// <returns>URL presignada y metadatos del documento</returns>
        /// <response code="200">URL generada exitosamente</response>
        /// <response code="404">Documento no encontrado</response>
        /// <response code="400">Archivo no encontrado en storage</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{documentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DownloadDocumentResponse>> GetDownloadUrl(
            [FromRoute] Guid documentId)
        {
            try
            {
                _logger.LogInformation("Generando URL de descarga para documento: {DocumentId}", documentId);

                var response = await _downloadService.GetDownloadUrlAsync(documentId);
                return Ok(response);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning("Documento no encontrado: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error generando URL de descarga: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en GetDownloadUrl");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}
