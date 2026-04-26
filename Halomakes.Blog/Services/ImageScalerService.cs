using Halomakes.Blog.Models;
using PhotoSauce.MagicScaler;

namespace Halomakes.Blog.Services;

public class ImageScalerService(IWebHostEnvironment environment)
{
    private readonly List<int> _standardSizes =
        [144, 196, 240, 320, 480, 512, 640, 720, 820, 960, 1024, 1200, 1440, 1600, 1920, 2560, 3180, 4200];

    public ScaledImage ScaleImage(string originalPath) =>
        new(FormatForClient(originalPath), GenerateSteps(originalPath).ToList());

    private IEnumerable<ScaleStep> GenerateSteps(string originalPath)
    {
        var directoryName = Path.GetDirectoryName(originalPath);
        var originalFileName = Path.GetFileNameWithoutExtension(originalPath);
        var originalFsPath = Path.Combine(environment.WebRootPath, originalPath);
        using var pipeline = MagicImageProcessor.BuildPipeline(originalFsPath, new ProcessImageSettings());
        foreach (var size in _standardSizes.Order())
        {
            if (pipeline.PixelSource.Width < size)
                yield break;
            var newFileName = $"{originalFileName}_{size}px.webp";
            var newFilePath = Path.Combine(directoryName!, newFileName);
            var newFsPath = Path.Combine(environment.WebRootPath, newFilePath);
            if (File.Exists(newFsPath))
            {
                yield return new(FormatForClient(newFilePath), size);
                continue;
            }

            MagicImageProcessor.ProcessImage(originalFsPath, newFsPath, new ProcessImageSettings()
            {
                Width = size
            });
            yield return new(FormatForClient(newFilePath), size);
        }
    }

    private string FormatForClient(string ioPath) => $"/{ioPath.Replace('\\', '/')}";
}