using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InterceptSuite.Models
{
    public class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("body")]
        public string Body { get; set; } = "";

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("assets")]
        public List<GitHubReleaseAsset> Assets { get; set; } = new();

        public string GetVersionString()
        {
            return TagName.TrimStart('v');
        }

        public bool TryGetVersion(out Version? version)
        {
            var versionString = GetVersionString();
            return Version.TryParse(versionString, out version);
        }
    }

    public class GitHubReleaseAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = "";

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = "";

        public string GetFormattedSize()
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = Size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    public enum InstallerType
    {
        WindowsSetup,
        MacOSPackage,
        LinuxDeb,
        LinuxRpm,
        LinuxAppImage
    }

    public class UpdateCheckResult
    {
        public bool IsUpdateAvailable { get; set; }
        public string? CurrentVersion { get; set; }
        public string? LatestVersion { get; set; }
        public string? ReleaseNotes { get; set; }
        public Dictionary<InstallerType, GitHubReleaseAsset> AvailableInstallers { get; set; } = new();
    }
}
