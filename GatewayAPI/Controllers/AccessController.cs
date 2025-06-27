using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("api/access")]
    public class AccessController : ControllerBase
    {
        private readonly AccessServiceClient _accessServiceClient;
        private readonly ILogger<AccessController> _logger;

        public AccessController(AccessServiceClient accessServiceClient, ILogger<AccessController> logger)
        {
            _accessServiceClient = accessServiceClient ?? throw new ArgumentNullException(nameof(accessServiceClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new access record
        /// </summary>
        /// <param name="identityId">The identity ID</param>
        /// <param name="resourceId">The resource ID</param>
        /// <param name="accessData">The access data in JSON format</param>
        /// <returns>The created access record</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAccess([FromQuery] string identityId, [FromQuery] string resourceId, [FromBody] string accessData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(resourceId))
                    return BadRequest("Resource ID is required");

                if (string.IsNullOrWhiteSpace(accessData))
                    return BadRequest("Access data is required");

                var response = await _accessServiceClient.CreateAccessAsync(identityId, resourceId, accessData);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating access");
                return StatusCode(500, "An error occurred while creating access");
            }
        }

        /// <summary>
        /// Updates an existing access record
        /// </summary>
        /// <param name="identityId">The identity ID</param>
        /// <param name="resourceId">The resource ID</param>
        /// <param name="accessData">The updated access data in JSON format</param>
        /// <returns>The updated access record</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAccess([FromQuery] string identityId, [FromQuery] string resourceId, [FromBody] string accessData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(resourceId))
                    return BadRequest("Resource ID is required");

                if (string.IsNullOrWhiteSpace(accessData))
                    return BadRequest("Access data is required");

                var response = await _accessServiceClient.UpdateAccessAsync(identityId, resourceId, accessData);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating access");
                return StatusCode(500, "An error occurred while updating access");
            }
        }

        /// <summary>
        /// Gets an access record by identity and resource IDs
        /// </summary>
        /// <param name="identityId">The identity ID</param>
        /// <param name="resourceId">The resource ID</param>
        /// <returns>The access record</returns>
        [HttpGet]
        public async Task<IActionResult> GetAccess([FromQuery] string identityId, [FromQuery] string resourceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(resourceId))
                    return BadRequest("Resource ID is required");

                var response = await _accessServiceClient.GetAccessAsync(identityId, resourceId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting access");
                return StatusCode(500, "An error occurred while getting access");
            }
        }

        /// <summary>
        /// Deletes an access record by identity and resource IDs
        /// </summary>
        /// <param name="identityId">The identity ID</param>
        /// <param name="resourceId">The resource ID</param>
        /// <returns>Empty response if successful</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAccess([FromQuery] string identityId, [FromQuery] string resourceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(resourceId))
                    return BadRequest("Resource ID is required");

                var response = await _accessServiceClient.DeleteAccessAsync(identityId, resourceId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting access");
                return StatusCode(500, "An error occurred while deleting access");
            }
        }
    }
}
