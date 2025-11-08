using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InterceptSuite.ViewModels;
using InterceptSuite.Services;
using InterceptSuite.Models;

namespace InterceptSuite.Views;

public partial class ProjectStartupWindow : Window
{
    private readonly UpdateCheckerService _updateChecker;

    public ProjectStartupWindow()
    {
        InitializeComponent();
        DataContext = new ProjectStartupViewModel();
        _updateChecker = new UpdateCheckerService();

        if (DataContext is ProjectStartupViewModel viewModel)
        {
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ProjectStartupViewModel.DialogResult) && viewModel.DialogResult)
                {
                    Close();
                }
            };
        }

        // check for updates when window opens
        Opened += async (sender, e) =>
        {
            await CheckForUpdatesAsync();
        };
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            var preference = UpdateReminderPreference.Load();

            var updateInfo = await _updateChecker.CheckForUpdatesAsync();

            if (updateInfo.IsUpdateAvailable &&
                !preference.ShouldSkipVersion(updateInfo.LatestVersion ?? ""))
            {
                var updateWindow = new UpdateNotificationWindow(updateInfo, _updateChecker);
                await updateWindow.ShowDialog(this);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update check failed: {ex.Message}");
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
    }
}
