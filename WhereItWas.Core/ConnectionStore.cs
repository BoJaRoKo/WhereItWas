using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WhereItWas.Core;

public sealed class ConnectionStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string FilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WhereItWas",
        "login.json");

    public SqlConnectionProfile? ReadConnection()
    {
        if (!File.Exists(FilePath))
        {
            return null;
        }

        var encryptedDocumentJson = File.ReadAllText(FilePath, Encoding.UTF8);
        var encryptedDocument = JsonSerializer.Deserialize<EncryptedConnectionDocument>(encryptedDocumentJson, JsonOptions);

        if (encryptedDocument is null || string.IsNullOrWhiteSpace(encryptedDocument.Payload))
        {
            return null;
        }

        var protectedBytes = Convert.FromBase64String(encryptedDocument.Payload);
        var plainBytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
        var plainJson = Encoding.UTF8.GetString(plainBytes);

        return JsonSerializer.Deserialize<SqlConnectionProfile>(plainJson, JsonOptions);
    }

    public void WriteConnection(SqlConnectionProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        var plainJson = JsonSerializer.Serialize(profile, JsonOptions);
        var plainBytes = Encoding.UTF8.GetBytes(plainJson);
        var protectedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);

        var encryptedDocument = new EncryptedConnectionDocument
        {
            Payload = Convert.ToBase64String(protectedBytes)
        };

        var encryptedDocumentJson = JsonSerializer.Serialize(encryptedDocument, JsonOptions);
        File.WriteAllText(FilePath, encryptedDocumentJson, Encoding.UTF8);
    }

    private sealed class EncryptedConnectionDocument
    {
        public string Payload { get; set; } = string.Empty;
    }
}
