using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDiariesAPI.Services;
using System;
using System.Threading.Tasks;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ImageController : ControllerBase
    {
        private readonly IR2StorageService _r2StorageService;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB max

        public ImageController(IR2StorageService r2StorageService)
        {
            _r2StorageService = r2StorageService;
        }

        /// <summary>
        /// Upload a product image
        /// </summary>
        [HttpPost("UploadProductImage")]
        public async Task<IActionResult> UploadProductImage(
            [FromQuery] int compId,
            [FromQuery] string productName,
            IFormFile file)
        {
            try
            {
                // Validate compId - must be greater than 0
                if (compId <= 0)
                {
                    return BadRequest(new { success = false, message = $"Invalid Company ID: {compId}. Company ID must be greater than 0." });
                }

                if (string.IsNullOrWhiteSpace(productName))
                {
                    return BadRequest(new { success = false, message = "Product name is required" });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                if (file.Length > _maxFileSize)
                {
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });
                }

                if (!IsValidImageType(file.ContentType))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Only JPEG, PNG, and WebP are allowed" });
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await _r2StorageService.UploadProductImageAsync(compId, productName, stream, file.ContentType);

                return Ok(new
                {
                    success = true,
                    message = "Image uploaded successfully",
                    imageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error uploading image: {ex.Message}" });
            }
        }

        /// <summary>
        /// Upload company logo
        /// </summary>
        [HttpPost("UploadLogo")]
        public async Task<IActionResult> UploadLogo(
            [FromQuery] int compId,
            IFormFile file)
        {
            try
            {
                // Validate compId - must be greater than 0
                if (compId <= 0)
                {
                    return BadRequest(new { success = false, message = $"Invalid Company ID: {compId}. Company ID must be greater than 0." });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                if (file.Length > _maxFileSize)
                {
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });
                }

                if (!IsValidImageType(file.ContentType))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Only JPEG, PNG, and WebP are allowed" });
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await _r2StorageService.UploadLogoAsync(compId, stream, file.ContentType);

                return Ok(new
                {
                    success = true,
                    message = "Logo uploaded successfully",
                    imageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error uploading logo: {ex.Message}" });
            }
        }

        /// <summary>
        /// Upload company icon
        /// </summary>
        [HttpPost("UploadIcon")]
        public async Task<IActionResult> UploadIcon(
            [FromQuery] int compId,
            IFormFile file)
        {
            try
            {
                // Validate compId - must be greater than 0
                if (compId <= 0)
                {
                    return BadRequest(new { success = false, message = $"Invalid Company ID: {compId}. Company ID must be greater than 0." });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                if (file.Length > _maxFileSize)
                {
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });
                }

                if (!IsValidImageType(file.ContentType))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Only JPEG, PNG, and WebP are allowed" });
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await _r2StorageService.UploadIconAsync(compId, stream, file.ContentType);

                return Ok(new
                {
                    success = true,
                    message = "Icon uploaded successfully",
                    imageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error uploading icon: {ex.Message}" });
            }
        }

        /// <summary>
        /// Upload user profile image
        /// </summary>
        [HttpPost("UploadProfileImage")]
        public async Task<IActionResult> UploadProfileImage(
            [FromQuery] int compId,
            [FromQuery] int userId,
            IFormFile file)
        {
            try
            {
                // Validate compId - must be greater than 0
                if (compId <= 0)
                {
                    return BadRequest(new { success = false, message = $"Invalid Company ID: {compId}. Company ID must be greater than 0." });
                }

                // Validate userId - must be greater than 0
                if (userId <= 0)
                {
                    return BadRequest(new { success = false, message = $"Invalid User ID: {userId}. User ID must be greater than 0." });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                if (file.Length > _maxFileSize)
                {
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });
                }

                if (!IsValidImageType(file.ContentType))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Only JPEG, PNG, and WebP are allowed" });
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await _r2StorageService.UploadProfileImageAsync(compId, userId, stream, file.ContentType);

                return Ok(new
                {
                    success = true,
                    message = "Profile image uploaded successfully",
                    imageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error uploading profile image: {ex.Message}" });
            }
        }

        /// <summary>
        /// Delete an image by its key/path
        /// </summary>
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteImage([FromQuery] string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return BadRequest(new { success = false, message = "Key is required" });
                }

                var result = await _r2StorageService.DeleteFileAsync(key);

                if (result)
                {
                    return Ok(new { success = true, message = "Image deleted successfully" });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Failed to delete image" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error deleting image: {ex.Message}" });
            }
        }

        private bool IsValidImageType(string contentType)
        {
            var validTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            return Array.Exists(validTypes, t => t.Equals(contentType, StringComparison.OrdinalIgnoreCase));
        }
    }
}
