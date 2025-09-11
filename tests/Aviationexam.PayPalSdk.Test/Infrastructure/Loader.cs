using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Aviationexam.PayPalSdk.Test.Infrastructure;

public static class Loader
{
    public static void LoadEnvFile(
        string envFileName, [CallerFilePath] string filePath = ""
    )
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        FileInfo fileInfo = new(filePath);
        var rootDirectory = fileInfo.Directory?.Parent?.Parent?.FullName;
        if (rootDirectory is null)
        {
            return;
        }

        var envFileFullPath = Path.Join(rootDirectory, envFileName);

        if (!File.Exists(envFileFullPath))
        {
            return;
        }

        foreach (var line in File.ReadAllLines(envFileFullPath))
        {
            var parts = line.Split(
                '=',
                2,
                StringSplitOptions.RemoveEmptyEntries
            );

            if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
            {
                Environment.SetEnvironmentVariable(parts[0], parts.Length > 1 ? parts[1] : null, EnvironmentVariableTarget.Process);
            }
        }
    }
}
