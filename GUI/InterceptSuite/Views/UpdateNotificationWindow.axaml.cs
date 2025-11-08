using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using InterceptSuite.Models;
using InterceptSuite.Services;

namespace InterceptSuite.Views
{
    public partial class UpdateNotificationWindow : Window
    {
        private readonly UpdateCheckResult _updateInfo;
        private readonly UpdateCheckerService _updateChecker;
        private RadioButton? _selectedInstallerRadio;

        public UpdateNotificationWindow(UpdateCheckResult updateInfo, UpdateCheckerService updateChecker)
        {
            InitializeComponent();
            _updateInfo = updateInfo;
            _updateChecker = updateChecker;

            LoadUpdateInfo();
        }

        private void LoadUpdateInfo()
        {
            if (VersionInfoText != null)
            {
                VersionInfoText.Text = $"Version {_updateInfo.LatestVersion} is now available (you have {_updateInfo.CurrentVersion})";
            }

            CreateInstallerOptions();
        }

        private void ReleaseNotesButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var url = $"https://github.com/InterceptSuite/InterceptSuite/releases/tag/v{_updateInfo.LatestVersion}";
                OpenUrl(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open release notes: {ex.Message}");
            }
        }

        private void CreateInstallerOptions()
        {
            if (InstallerOptionsPanel == null)
                return;

            InstallerOptionsPanel.Children.Clear();

            if (_updateInfo.AvailableInstallers.Count == 0)
            {
                var noInstallersText = new TextBlock
                {
                    Text = "No compatible installers found for this version.",
                    Foreground = new SolidColorBrush(Color.Parse("#FF6B6B"))
                };
                InstallerOptionsPanel.Children.Add(noInstallersText);
                if (DownloadButton != null)
                    DownloadButton.IsEnabled = false;
                return;
            }

            var recommendedType = GetRecommendedInstallerType();

            foreach (var installer in _updateInfo.AvailableInstallers)
            {
                var displayName = GetInstallerDisplayName(installer.Key);
                var isRecommended = installer.Key == recommendedType;

                var radioButton = new RadioButton
                {
                    Content = isRecommended ? $"{displayName} (Recommended)" : displayName,
                    Tag = installer.Key,
                    GroupName = "InstallerType"
                };

                if (isRecommended)
                {
                    radioButton.IsChecked = true;
                    _selectedInstallerRadio = radioButton;
                }

                radioButton.Checked += (s, e) =>
                {
                    _selectedInstallerRadio = radioButton;
                };

                InstallerOptionsPanel.Children.Add(radioButton);
            }
        }

        private InstallerType? GetRecommendedInstallerType()
        {
            if (OperatingSystem.IsWindows())
                return InstallerType.WindowsSetup;
            else if (OperatingSystem.IsMacOS())
                return InstallerType.MacOSPackage;
            else if (OperatingSystem.IsLinux())
                return InstallerType.LinuxDeb;

            return null;
        }
        private string GetInstallerDisplayName(InstallerType type)
        {
            return type switch
            {
                InstallerType.WindowsSetup => "Windows Installer (.exe)",
                InstallerType.MacOSPackage => "macOS Package (.pkg)",
                InstallerType.LinuxDeb => "Linux Debian Package (.deb)",
                InstallerType.LinuxRpm => "Linux RPM Package (.rpm)",
                InstallerType.LinuxAppImage => "Linux AppImage",
                _ => type.ToString()
            };
        }

        private async void DownloadButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_selectedInstallerRadio?.Tag is not InstallerType selectedType)
            {
                await ShowErrorMessage("Please select an installer type.");
                return;
            }

            if (!_updateInfo.AvailableInstallers.TryGetValue(selectedType, out var asset))
            {
                await ShowErrorMessage("Selected installer not found.");
                return;
            }

            if (DownloadProgressPanel != null)
                DownloadProgressPanel.IsVisible = true;

            if (DownloadButton != null)
                DownloadButton.IsEnabled = false;

            try
            {
                var progress = new Progress<int>(percent =>
                {
                    if (DownloadProgressBar != null)
                        DownloadProgressBar.Value = percent;

                    if (DownloadStatusText != null)
                        DownloadStatusText.Text = $"Downloading... {percent}%";
                });

                var filePath = await _updateChecker.DownloadInstallerAsync(asset, progress);

                if (DownloadStatusText != null)
                    DownloadStatusText.Text = "Download complete!";

                await ShowDownloadComplete(filePath);
            }
            catch (Exception ex)
            {
                await ShowErrorMessage($"Download failed: {ex.Message}");

                if (DownloadProgressPanel != null)
                    DownloadProgressPanel.IsVisible = false;

                if (DownloadButton != null)
                    DownloadButton.IsEnabled = true;
            }
        }

        private async Task ShowDownloadComplete(string filePath)
        {
            var dialog = new Window
            {
                Title = "Download Complete",
                Width = 450,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var panel = new StackPanel { Margin = new Avalonia.Thickness(20) };

            panel.Children.Add(new TextBlock
            {
                Text = "Update downloaded successfully!",
                FontSize = 14,
                FontWeight = FontWeight.SemiBold,
                Margin = new Avalonia.Thickness(0, 0, 0, 10)
            });

            panel.Children.Add(new TextBlock
            {
                Text = $"File: {Path.GetFileName(filePath)}",
                FontSize = 12,
                Margin = new Avalonia.Thickness(0, 0, 0, 20)
            });

            var buttonPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Spacing = 10
            };

            var runButton = new Button
            {
                Content = "Run Installer",
                Padding = new Avalonia.Thickness(15, 8)
            };
            runButton.Click += async (s, e) =>
            {
                dialog.Close();
                await LaunchInstaller(filePath);
            };

            var openFolderButton = new Button
            {
                Content = "Open Folder",
                Padding = new Avalonia.Thickness(15, 8)
            };
            openFolderButton.Click += (s, e) =>
            {
                OpenFolder(Path.GetDirectoryName(filePath)!);
                dialog.Close();
                Close();
            };

            var laterButton = new Button
            {
                Content = "Later",
                Padding = new Avalonia.Thickness(15, 8)
            };
            laterButton.Click += (s, e) =>
            {
                dialog.Close();
                Close();
            };

            buttonPanel.Children.Add(runButton);
            buttonPanel.Children.Add(openFolderButton);
            buttonPanel.Children.Add(laterButton);

            panel.Children.Add(buttonPanel);
            dialog.Content = panel;

            await dialog.ShowDialog(this);
        }
        private async Task LaunchInstaller(string filePath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };

                if (OperatingSystem.IsMacOS() && filePath.EndsWith(".pkg"))
                {
                    startInfo.FileName = "open";
                    startInfo.Arguments = filePath;
                }

                Process.Start(startInfo);


                await Task.Delay(500);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                await ShowErrorMessage($"Failed to launch installer: {ex.Message}");
            }
        }

        private void OpenFolder(string folderPath)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    Process.Start("explorer.exe", folderPath);
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", folderPath);
                }
                else if (OperatingSystem.IsLinux())
                {
                    Process.Start("xdg-open", folderPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open folder: {ex.Message}");
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", url);
                }
                else if (OperatingSystem.IsLinux())
                {
                    Process.Start("xdg-open", url);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open URL: {ex.Message}");
            }
        }

        private async Task ShowErrorMessage(string message)
        {
            var dialog = new Window
            {
                Title = "Error",
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var panel = new StackPanel { Margin = new Avalonia.Thickness(20) };

            panel.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Avalonia.Thickness(0, 0, 0, 20)
            });

            var okButton = new Button
            {
                Content = "OK",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Padding = new Avalonia.Thickness(20, 8)
            };
            okButton.Click += (s, e) => dialog.Close();

            panel.Children.Add(okButton);
            dialog.Content = panel;

            await dialog.ShowDialog(this);
        }

        private void NotNowButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DontRemindButton_Click(object? sender, RoutedEventArgs e)
        {
            var preference = new UpdateReminderPreference
            {
                SkippedVersion = _updateInfo.LatestVersion
            };
            preference.Save();

            Close();
        }
    }
}
