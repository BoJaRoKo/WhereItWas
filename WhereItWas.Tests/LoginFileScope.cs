using WhereItWas.Core;

namespace WhereItWas.Tests;

internal sealed class LoginFileScope : IDisposable
{
    private const int RetryCount = 10;
    private readonly string _filePath;
    private readonly byte[]? _backup;

    public LoginFileScope()
    {
        _filePath = new ConnectionStore().FilePath;
        _backup = File.Exists(_filePath) ? ReadAllBytesWithRetry(_filePath) : null;
    }

    public string FilePath => _filePath;

    public void DeleteLoginFile()
    {
        ExecuteWithRetry(() =>
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        });
    }

    public void Dispose()
    {
        if (_backup is null)
        {
            DeleteLoginFile();
            return;
        }

        ExecuteWithRetry(() =>
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllBytes(_filePath, _backup);
        });
    }

    private static byte[] ReadAllBytesWithRetry(string path)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var memory = new MemoryStream();
                stream.CopyTo(memory);
                return memory.ToArray();
            }
            catch (IOException) when (attempt < RetryCount)
            {
                Thread.Sleep(100);
            }
        }
    }

    private static void ExecuteWithRetry(Action action)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                action();
                return;
            }
            catch (IOException) when (attempt < RetryCount)
            {
                Thread.Sleep(100);
            }
        }
    }
}
