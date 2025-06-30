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
        /// Создает новую запись доступа
        /// </summary>
        /// <param name="request">Данные для создания доступа</param>
        /// <returns>Созданная запись доступа</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAccess([FromBody] CreateAccessRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request body is required");

                if (string.IsNullOrWhiteSpace(request.IdentityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(request.ResourceId))
                    return BadRequest("Resource ID is required");

                if (string.IsNullOrWhiteSpace(request.AccessData))
                    return BadRequest("Access data is required");

                var response = await _accessServiceClient.CreateAccessAsync(
                    request.IdentityId,
                    request.ResourceId,
                    request.AccessData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании доступа");
                return StatusCode(500, "Произошла ошибка при создании доступа");
            }
        }

        /// <summary>
        /// Обновляет существующую запись доступа
        /// </summary>
        /// <param name="request">Данные для обновления доступа</param>
        /// <returns>Обновленная запись доступа</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAccess([FromBody] UpdateAccessRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request body is required");

                if (string.IsNullOrWhiteSpace(request.IdentityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(request.ResourceId))
                    return BadRequest("Resource ID is required");

                if (string.IsNullOrWhiteSpace(request.AccessData))
                    return BadRequest("Access data is required");

                var response = await _accessServiceClient.UpdateAccessAsync(
                    request.IdentityId,
                    request.ResourceId,
                    request.AccessData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении доступа");
                return StatusCode(500, "Произошла ошибка при обновлении доступа");
            }
        }

        /// <summary>
        /// Получает запись доступа по ID идентификатора и ресурса
        /// </summary>
        /// <param name="request">Данные для поиска доступа</param>
        /// <returns>Запись доступа</returns>
        [HttpGet("get")]
        public async Task<IActionResult> GetAccess([FromBody] GetAccessRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request body is required");

                if (string.IsNullOrWhiteSpace(request.IdentityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(request.ResourceId))
                    return BadRequest("Resource ID is required");

                var response = await _accessServiceClient.GetAccessAsync(
                    request.IdentityId,
                    request.ResourceId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении доступа");
                return StatusCode(500, "Произошла ошибка при получении доступа");
            }
        }

        /// <summary>
        /// Удаляет запись доступа по ID идентификатора и ресурса
        /// </summary>
        /// <param name="request">Данные для удаления доступа</param>
        /// <returns>Пустой ответ при успешном удалении</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAccess([FromBody] DeleteAccessRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request body is required");

                if (string.IsNullOrWhiteSpace(request.IdentityId))
                    return BadRequest("Identity ID is required");

                if (string.IsNullOrWhiteSpace(request.ResourceId))
                    return BadRequest("Resource ID is required");

                var response = await _accessServiceClient.DeleteAccessAsync(
                    request.IdentityId,
                    request.ResourceId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении доступа");
                return StatusCode(500, "Произошла ошибка при удалении доступа");
            }
        }

        public class CreateAccessRequest
        {
            public string IdentityId { get; set; }
            public string ResourceId { get; set; }
            public string AccessData { get; set; }
        }

        public class UpdateAccessRequest
        {
            public string IdentityId { get; set; }
            public string ResourceId { get; set; }
            public string AccessData { get; set; }
        }

        public class GetAccessRequest
        {
            public string IdentityId { get; set; }
            public string ResourceId { get; set; }
        }

        public class DeleteAccessRequest
        {
            public string IdentityId { get; set; }
            public string ResourceId { get; set; }
        }
    }
}
