namespace Schedura.Infra.Configuration;

public static class DotEnvLoader
{
    public static void LoadFromSolutionRoot(string? environmentName = null, bool overwriteExisting = false)
    {
        var root = FindSolutionRoot();
        if (root is null)
        {
            return;
        }

        var envFiles = GetEnvFileCandidates(root, environmentName);
        foreach (var envPath in envFiles)
        {
            if (!File.Exists(envPath))
            {
                continue;
            }

            foreach (var rawLine in File.ReadAllLines(envPath))
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                if (line.StartsWith("export ", StringComparison.Ordinal))
                {
                    line = line["export ".Length..].TrimStart();
                }

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line[..separatorIndex].Trim();
                var value = NormalizeValue(line[(separatorIndex + 1)..].Trim());
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                if (!overwriteExisting && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                {
                    continue;
                }

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    public static string? ResolveEnvironmentName()
    {
        return Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
               ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    private static IReadOnlyList<string> GetEnvFileCandidates(string root, string? environmentName)
    {
        var files = new List<string>();

        AddEnvCandidates(files, Path.Combine(root, "Schedura.Api"), environmentName);
        AddEnvCandidates(files, root, environmentName);

        return files;
    }

    private static void AddEnvCandidates(ICollection<string> files, string basePath, string? environmentName)
    {
        files.Add(Path.Combine(basePath, ".env"));
        if (!string.IsNullOrWhiteSpace(environmentName))
        {
            files.Add(Path.Combine(basePath, $".env.{environmentName}"));
        }
    }

    private static string NormalizeValue(string rawValue)
    {
        if (rawValue.Length >= 2 && rawValue.StartsWith('"') && rawValue.EndsWith('"'))
        {
            return rawValue[1..^1]
                .Replace("\\n", "\n", StringComparison.Ordinal)
                .Replace("\\r", "\r", StringComparison.Ordinal)
                .Replace("\\t", "\t", StringComparison.Ordinal);
        }

        if (rawValue.Length >= 2 && rawValue.StartsWith('\'') && rawValue.EndsWith('\''))
        {
            return rawValue[1..^1];
        }

        return rawValue;
    }

    private static string? FindSolutionRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (current is not null)
        {
            if (current.GetFiles("*.sln").Length > 0)
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return null;
    }
}
