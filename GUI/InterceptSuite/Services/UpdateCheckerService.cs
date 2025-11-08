using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using InterceptSuite.Models;

namespace InterceptSuite.Services
{
    public class UpdateCheckerService
    {
        private const string GITHUB_API_URL = "https://api.github.com/repos/InterceptSuite/InterceptSuite/releases/latest";
        private const bool IS_PRO_VERSION = false; // Set to false for Standard edition
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
        };

        public UpdateCheckerService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "InterceptSuite");
        }
        public async Task<UpdateCheckResult> CheckForUpdatesAsync()
        {
            try
            {
                var currentVersion = GetCurrentVersion();
                var latestRelease = await FetchLatestReleaseAsync();

                if (latestRelease == null || !latestRelease.TryGetVersion(out var latestVersion))
                {
                    return new UpdateCheckResult
                    {
                        IsUpdateAvailable = false,
                        CurrentVersion = currentVersion?.ToString()
                    };
                }

                var isUpdateAvailable = latestVersion > currentVersion;

                return new UpdateCheckResult
                {
                    IsUpdateAvailable = isUpdateAvailable,
                    CurrentVersion = currentVersion?.ToString(),
                    LatestVersion = latestRelease.GetVersionString(),
                    ReleaseNotes = latestRelease.Body,
                    AvailableInstallers = ParseAvailableInstallers(latestRelease.Assets)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update check failed: {ex.Message}");
                return new UpdateCheckResult { IsUpdateAvailable = false };
            }
        }
        private Version? GetCurrentVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version;
        }
        private async Task<GitHubRelease?> FetchLatestReleaseAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(GITHUB_API_URL);
                var release = JsonSerializer.Deserialize<GitHubRelease>(response, JsonOptions);
                return release;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch release info: {ex.Message}");
                return null;
            }
        }
        private Dictionary<InstallerType, GitHubReleaseAsset> ParseAvailableInstallers(List<GitHubReleaseAsset> assets)
        {
            var installers = new Dictionary<InstallerType, GitHubReleaseAsset>();

            foreach (var asset in assets)
            {
                var nameLower = asset.Name.ToLowerInvariant();


                if (!nameLower.Contains("standard"))
                    continue;

                if (nameLower.Contains("setup.exe") || nameLower.EndsWith(".exe"))
                {
                    installers[InstallerType.WindowsSetup] = asset;
                }
                else if (nameLower.EndsWith(".pkg"))
                {
                    installers[InstallerType.MacOSPackage] = asset;
                }

                else if (nameLower.EndsWith(".deb"))
                {
                    installers[InstallerType.LinuxDeb] = asset;
                }

                else if (nameLower.EndsWith(".rpm"))
                {
                    installers[InstallerType.LinuxRpm] = asset;
                }

                else if (nameLower.EndsWith(".appimage"))
                {
                    installers[InstallerType.LinuxAppImage] = asset;
                }
            }

            return installers;
        }

        public async Task<string> DownloadInstallerAsync(
            GitHubReleaseAsset asset,
            IProgress<int> progress)
        {
            var downloadsFolder = GetDownloadsFolder();
            var filePath = GetUniqueFilePath(downloadsFolder, asset.Name);

            using var response = await _httpClient.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? 0;
            var buffer = new byte[8192];
            var bytesRead = 0L;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            int read;
            while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, read);
                bytesRead += read;

                if (totalBytes > 0)
                {
                    var percentComplete = (int)((bytesRead * 100) / totalBytes);
                    progress?.Report(percentComplete);
                }
            }

            return filePath;
        }

        private string GetDownloadsFolder()
        {
            if (OperatingSystem.IsWindows())
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
        }
        private string GetUniqueFilePath(string folder, string fileName)
        {
            var basePath = Path.Combine(folder, fileName);
            if (!File.Exists(basePath))
                return basePath;

            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var counter = 1;

            while (true)
            {
                var newPath = Path.Combine(folder, $"{nameWithoutExt} ({counter}){extension}");
                if (!File.Exists(newPath))
                    return newPath;
                counter++;
            }
        }
    }

    public class UpdateReminderPreference
    {
        public string? SkippedVersion { get; set; }

        private static readonly JsonSerializerOptions PreferenceJsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private static string GetPreferenceFilePath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (OperatingSystem.IsMacOS())
            {
                appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Library", "Application Support");
            }
            else if (OperatingSystem.IsLinux())
            {
                appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config");
            }

            var folder = Path.Combine(appDataFolder, "InterceptSuite");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "update_reminder.json");
        }

        public static UpdateReminderPreference Load()
        {
            try
            {
                var filePath = GetPreferenceFilePath();
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<UpdateReminderPreference>(json, PreferenceJsonOptions) ?? new UpdateReminderPreference();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load update preference: {ex.Message}");
            }

            return new UpdateReminderPreference();
        }

        public void Save()
        {
            try
            {
                var filePath = GetPreferenceFilePath();
                var json = JsonSerializer.Serialize(this, PreferenceJsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save update preference: {ex.Message}");
            }
        }

        public bool ShouldSkipVersion(string version)
        {
            return !string.IsNullOrEmpty(SkippedVersion) && SkippedVersion == version;
        }
    }
}
