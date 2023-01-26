using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Disassemblers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

foreach (var summary in BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args))
{
    // WriteSummary(summary);
}

static void WriteSummary(Summary summary)
{
    var solutionDir = GetSolutionDirectory();
    if (solutionDir is null)
    {
        return;
    }

    var targetType = GetTargetType(summary);
    if (targetType is null)
    {
        return;
    }

    var title = targetType.Name;
    var pointIndex = targetType.Namespace.IndexOf('.');
    if (pointIndex >= 0)
        title = $"{EndSubstring(targetType.Namespace, pointIndex + 1)}.{targetType.Name}";

    var resultsPath = Path.Combine(solutionDir, "Results");
    _ = Directory.CreateDirectory(resultsPath);

    var filePath = Path.Combine(resultsPath, $"{title}.md");

    if (File.Exists(filePath))
        File.Delete(filePath);

    using var fileWriter = new StreamWriter(filePath, false, Encoding.UTF8);
    var logger = new StreamLogger(fileWriter);

    logger.WriteLine($"## {title}");
    logger.WriteLine();

    logger.WriteLine("### Source");
    var sourceLink = new StringBuilder("../LinqBenchmarks");
    foreach (var folder in targetType.Namespace.Split('.').AsEnumerable().Skip(1))
        _ = sourceLink.Append($"/{folder}");
    _ = sourceLink.Append($"/{targetType.Name}.cs");
    logger.WriteLine($"[{targetType.Name}.cs]({sourceLink})");
    logger.WriteLine();

    logger.WriteLine("### Results:");

    MarkdownExporter.GitHub.ExportToLog(summary, logger);
}

static string EndSubstring(string str, int index)
    => str.Substring(index, str.Length - index);

static Type GetTargetType(Summary summary)
{
    var targetTypes = summary.BenchmarksCases.Select(i => i.Descriptor.Type).Distinct().ToList();
    return targetTypes.Count == 1 ? targetTypes[0] : null;
}

static string GetSolutionDirectory()
{
    var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

    while (!string.IsNullOrEmpty(dir))
    {
        if (Directory.EnumerateFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).Any())
            return dir;

        dir = Path.GetDirectoryName(dir);
    }

    return null;
}