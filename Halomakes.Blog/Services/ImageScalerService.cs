using System.Web;
using Halomakes.Blog.Models;
using PhotoSauce.MagicScaler;

namespace Halomakes.Blog.Services;

public class ImageScalerService(IWebHostEnvironment environment, ILogger<ImageScalerService> logger)
{
    private readonly List<int> _standardSizes =
        [144, 196, 240, 320, 480, 512, 640, 720, 820, 960, 1024, 1200, 1440, 1600, 1920, 2560, 3180, 4200];

    public ScaledImage ScaleImage(string originalPath)
    {
        var workingPath = originalPath.TrimStart('/');
        return new(FormatForClient(workingPath), GenerateImage(workingPath, out var aspect), aspect);
    }

    private IList<ScaleStep> GenerateImage(string originalPath, out double aspect)
    {
        var directoryName = Path.GetDirectoryName(originalPath);
        var originalFileName = Path.GetFileNameWithoutExtension(originalPath);
        var originalFsPath = Path.Combine(environment.WebRootPath, originalPath);
        using var pipeline = MagicImageProcessor.BuildPipeline(originalFsPath, new ProcessImageSettings());
        aspect = (double)pipeline.PixelSource.Width / pipeline.PixelSource.Height;
        try
        {
            return GenerateSteps(pipeline, originalFileName, directoryName, originalFsPath).ToList();
        }
        catch (InvalidDataException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(originalPath);
            throw;
        }
    }

    private IEnumerable<ScaleStep> GenerateSteps(ProcessingPipeline pipeline, string originalFileName,
        string? directoryName,
        string originalFsPath)
    {
        foreach (var width in _standardSizes.Order())
        {
            logger.LogWarning("Generating scaled image for {Source}: {Resolution}", originalFileName, width);
            if (pipeline.PixelSource.Width < width)
                yield break;
            var newFileName = $"{originalFileName}_{width}px.webp";
            var newFilePath = Path.Combine(directoryName!, newFileName);
            var newFsPath = Path.Combine(environment.WebRootPath, newFilePath);
            var expectedHeight = width / pipeline.PixelSource.Width * pipeline.PixelSource.Height;
            if (File.Exists(newFsPath))
            {
                yield return new(FormatForClient(newFilePath), width);
                continue;
            }

            MagicImageProcessor.ProcessImage(originalFsPath, newFsPath, new ProcessImageSettings
            {
                Width = width
            });
            yield return new(FormatForClient(newFilePath), width);
        }
    }

    private static string FormatForClient(string ioPath) =>
        $"/{HttpUtility.UrlPathEncode(ioPath.Replace('\\', '/'))}";
}