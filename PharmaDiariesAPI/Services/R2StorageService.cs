using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PharmaDiariesAPI.Services
{
    public interface IR2StorageService
    {
        Task<string> UploadProductImageAsync(int compId, string productName, Stream imageStream, string contentType);
        Task<string> UploadLogoAsync(int compId, Stream imageStream, string contentType);
        Task<string> UploadIconAsync(int compId, Stream imageStream, string contentType);
        Task<string> UploadProfileImageAsync(int compId, int userId, Stream imageStream, string contentType);
        Task<bool> DeleteFileAsync(string key);
        string GetPublicUrl(string key);
    }

    public class R2StorageService : IR2StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _publicUrl;
        private readonly bool _isProduction;

        public R2StorageService(IConfiguration configuration)
        {
            var accountId = configuration["CloudflareR2:AccountId"];
            var accessKeyId = configuration["CloudflareR2:AccessKeyId"];
            var secretAccessKey = configuration["CloudflareR2:SecretAccessKey"];
            _bucketName = configuration["CloudflareR2:BucketName"] ?? "pdimages";
            _publicUrl = configuration["CloudflareR2:PublicUrl"] ?? "";

            // Determine environment based on connection string or other config
            var connectionString = configuration["ConnectionStrings:APIconnectionString"] ?? "";
            _isProduction = connectionString.Contains("mcmewa") && !connectionString.Contains("staging");

            var config = new AmazonS3Config
            {
                ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true
            };

            _s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, config);
        }

        private string GetEnvironmentPrefix()
        {
            return _isProduction ? "Prod" : "Staging";
        }

        public async Task<string> UploadProductImageAsync(int compId, string productName, Stream imageStream, string contentType)
        {
            // Sanitize product name - remove spaces and special characters
            var sanitizedName = SanitizeFileName(productName);
            var envPrefix = GetEnvironmentPrefix();

            // Process image to WebP format and create variants
            using var image = await Image.LoadAsync(imageStream);

            // Upload original (max 2500x2500)
            var originalKey = $"{envPrefix}/{compId}/products/original/{sanitizedName}.webp";
            await UploadImageVariant(image, originalKey, 2500, 2500, 85);

            // Upload large (1200x1200)
            var largeKey = $"{envPrefix}/{compId}/products/large/{sanitizedName}.webp";
            await UploadImageVariant(image, largeKey, 1200, 1200, 80);

            // Upload medium (800x800)
            var mediumKey = $"{envPrefix}/{compId}/products/medium/{sanitizedName}.webp";
            await UploadImageVariant(image, mediumKey, 800, 800, 75);

            // Upload small/thumbnail (400x400)
            var smallKey = $"{envPrefix}/{compId}/products/small/{sanitizedName}.webp";
            await UploadImageVariant(image, smallKey, 400, 400, 70);

            // Return the medium size URL as default
            return GetPublicUrl(mediumKey);
        }

        public async Task<string> UploadLogoAsync(int compId, Stream imageStream, string contentType)
        {
            var envPrefix = GetEnvironmentPrefix();

            using var image = await Image.LoadAsync(imageStream);

            // Upload logo directly under compId folder (replaces if exists)
            var logoKey = $"{envPrefix}/{compId}/logo.png";
            await UploadPngImage(image, logoKey, 1000, 1000);

            return GetPublicUrl(logoKey);
        }

        public async Task<string> UploadIconAsync(int compId, Stream imageStream, string contentType)
        {
            var envPrefix = GetEnvironmentPrefix();

            using var image = await Image.LoadAsync(imageStream);

            // Upload original icon
            var originalKey = $"{envPrefix}/{compId}/icon.png";
            await UploadPngImage(image, originalKey, 128, 128);

            // Upload xlarge (128x128)
            var xlargeKey = $"{envPrefix}/{compId}/icons/xlarge/icon.png";
            await UploadPngImage(image, xlargeKey, 128, 128);

            // Upload large (64x64)
            var largeKey = $"{envPrefix}/{compId}/icons/large/icon.png";
            await UploadPngImage(image, largeKey, 64, 64);

            // Upload medium (32x32)
            var mediumKey = $"{envPrefix}/{compId}/icons/medium/icon.png";
            await UploadPngImage(image, mediumKey, 32, 32);

            // Upload small (16x16)
            var smallKey = $"{envPrefix}/{compId}/icons/small/icon.png";
            await UploadPngImage(image, smallKey, 16, 16);

            return GetPublicUrl(originalKey);
        }

        public async Task<string> UploadProfileImageAsync(int compId, int userId, Stream imageStream, string contentType)
        {
            var envPrefix = GetEnvironmentPrefix();

            using var image = await Image.LoadAsync(imageStream);

            // Upload profile image under {compId}/{userId}/profile.jpeg (replaces if exists)
            var key = $"{envPrefix}/{compId}/{userId}/profile.jpeg";
            await UploadJpegImage(image, key, 500, 500, 85);

            return GetPublicUrl(key);
        }

        private async Task UploadImageVariant(Image image, string key, int maxWidth, int maxHeight, int quality)
        {
            using var clone = image.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(maxWidth, maxHeight),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Lanczos3
                });
            });

            using var ms = new MemoryStream();
            await clone.SaveAsWebpAsync(ms, new WebpEncoder { Quality = quality });
            ms.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = ms,
                ContentType = "image/webp",
                CannedACL = S3CannedACL.PublicRead,
                DisablePayloadSigning = true  // R2 doesn't support STREAMING-AWS4-HMAC-SHA256-PAYLOAD
            };

            await _s3Client.PutObjectAsync(request);
        }

        private async Task UploadPngImage(Image image, string key, int maxWidth, int maxHeight)
        {
            using var clone = image.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(maxWidth, maxHeight),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Lanczos3
                });
            });

            using var ms = new MemoryStream();
            await clone.SaveAsPngAsync(ms, new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression });
            ms.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = ms,
                ContentType = "image/png",
                CannedACL = S3CannedACL.PublicRead,
                DisablePayloadSigning = true  // R2 doesn't support STREAMING-AWS4-HMAC-SHA256-PAYLOAD
            };

            await _s3Client.PutObjectAsync(request);
        }

        private async Task UploadJpegImage(Image image, string key, int maxWidth, int maxHeight, int quality)
        {
            using var clone = image.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(maxWidth, maxHeight),
                    Mode = ResizeMode.Max,
                    Sampler = KnownResamplers.Lanczos3
                });
            });

            using var ms = new MemoryStream();
            await clone.SaveAsJpegAsync(ms, new JpegEncoder { Quality = quality });
            ms.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = ms,
                ContentType = "image/jpeg",
                CannedACL = S3CannedACL.PublicRead,
                DisablePayloadSigning = true  // R2 doesn't support STREAMING-AWS4-HMAC-SHA256-PAYLOAD
            };

            await _s3Client.PutObjectAsync(request);
        }

        public async Task<bool> DeleteFileAsync(string key)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetPublicUrl(string key)
        {
            if (string.IsNullOrEmpty(_publicUrl))
            {
                return $"https://{_bucketName}.r2.dev/{key}";
            }
            return $"{_publicUrl}/{key}";
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove spaces and special characters, convert to lowercase
            var sanitized = fileName.Trim()
                .Replace(" ", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("/", "-")
                .Replace("\\", "-")
                .ToLowerInvariant();

            // Remove any remaining invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                sanitized = sanitized.Replace(c.ToString(), "");
            }

            return sanitized;
        }
    }
}
