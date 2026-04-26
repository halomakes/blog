using System.Web;
using Halomakes.Blog.Extensions;
using Halomakes.Blog.Models;
using PhotoSauce.MagicScaler;
using PhotoSauce.NativeCodecs.Libwebp;

namespace Halomakes.Blog.Services;

public class ImageScalerService(IWebHostEnvironment environment, ILogger<ImageScalerService> logger)
{
    private static readonly int[] StandardSizes =
        [144, 196, 240, 320, 480, 512, 640, 720, 820, 960, 1024, 1200, 1440, 1600, 1920, 2560, 3180, 4200];

    private const int ThumbnailSize = 1200;

    private const int Quality = 50;

    private const string MimeType = "image/webp";

    public ScaledImage ScaleImage(string originalPath)
    {
        var workingPath = originalPath.TrimStart('/');
        return new(FormatForClient(workingPath), GenerateImage(workingPath, out var aspect), aspect);
    }

    private IList<ScaleStep> GenerateImage(string originalPath, out double aspect)
    {
        try
        {
            logger.LogInformation("Getting image set for {Source}", originalPath);
            var directoryName = Path.GetDirectoryName(originalPath);
            var originalFileName = Path.GetFileNameWithoutExtension(originalPath);
            var originalFsPath = Path.Combine(environment.WebRootPath, originalPath);
            using var pipeline = MagicImageProcessor.BuildPipeline(originalFsPath, new ProcessImageSettings());
            aspect = (double)pipeline.PixelSource.Width / pipeline.PixelSource.Height;
            return GenerateSteps(pipeline, originalFileName, directoryName, originalFsPath).ToList();
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning(ex, "Failed to generated images for {Source}", originalPath);
            aspect = 1;
            return [];
        }
    }

    private IEnumerable<ScaleStep> GenerateSteps(
        ProcessingPipeline pipeline,
        string originalFileName,
        string? directoryName,
        string originalFsPath)
    {
        foreach (var width in StandardSizes.Order())
        {
            logger.LogInformation("Generating scaled image for {Source}: {Resolution}", originalFileName, width);
            if (pipeline.PixelSource.Width < width)
                yield break;
            var newFileName = $"{originalFileName}_{width}px.webp";
            var newFilePath = Path.Combine(directoryName!, newFileName);
            var newFsPath = Path.Combine(environment.WebRootPath, newFilePath);
            if (File.Exists(newFsPath))
            {
                yield return new(FormatForClient(newFilePath), width);
                continue;
            }

            MagicImageProcessor.ProcessImage(originalFsPath, newFsPath, new ProcessImageSettings
            {
                Width = width,
                EncoderOptions = new WebpLossyEncoderOptions(Quality)
            });
            yield return new(FormatForClient(newFilePath), width);
        }
    }

    public Thumbnail GenerateThumbnail(string clientPath)
    {
        logger.LogInformation("Generating thumbnail image for {Source}", clientPath);
        var originalFsPath = Path.Combine(environment.WebRootPath, clientPath);
        using var pipeline = MagicImageProcessor.BuildPipeline(clientPath, new ProcessImageSettings());
        var ulid = Path.GetFileName(clientPath).ToUlid();
        var thumbnailFsDir = $"{environment.WebRootFileProvider}/thumbnails";
        if (!Path.Exists(thumbnailFsDir))
            Directory.CreateDirectory(thumbnailFsDir);
        var newFileName = $"{ulid}.webp";
        var newClientPath = Path.Combine("thumbnails", newFileName);
        var newFsPath = Path.Combine(thumbnailFsDir, newClientPath);
        if (File.Exists(newFsPath))
        {
            using var existingPipeline = MagicImageProcessor.BuildPipeline(newFsPath, new ProcessImageSettings());
            var source = existingPipeline.PixelSource;
            return new(FormatForClient(newClientPath), MimeType, source.Width, source.Height);
        }

        var result = MagicImageProcessor.ProcessImage(originalFsPath, newFsPath, new ProcessImageSettings
        {
            Width = ThumbnailSize,
            EncoderOptions = new WebpLossyEncoderOptions(Quality)
        });
        return new(FormatForClient(newClientPath), MimeType, result.Settings.Width, result.Settings.Height);
    }

    private static string FormatForClient(string ioPath) =>
        $"/{HttpUtility.UrlPathEncode(ioPath.Replace('\\', '/'))}";
}