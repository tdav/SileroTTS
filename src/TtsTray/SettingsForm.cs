using System.Diagnostics;
using Microsoft.Win32;

namespace TtsTray;

/// <summary>
/// Настройки: голос (по группам Silero / Microsoft / Другие), скорость, громкость, порог слов,
/// автоозвучка буфера и автозапуск. При «Сохранить» пишет в AppSettings и сохраняет файл.
/// </summary>
public partial class SettingsForm : Form
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    private readonly SapiSpeaker speaker;
    private readonly AppSettings settings;
    private readonly string sileroConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "SileroTTS", "bin", "silero_tts_config.exe");

    /// <summary>«Прослушать» менял голос, скорость или громкость — при отмене их надо вернуть.</summary>
    private bool previewed;

    public SettingsForm(SapiSpeaker speaker, AppSettings settings)
    {
        this.InitializeComponent();
        this.speaker = speaker;
        this.settings = settings;

        // Группы в порядке Voices (Silero, Microsoft, Другие); пустые не добавляются.
        foreach (string group in speaker.Voices.Select(v => v.Group).Distinct())
        {
            this.voicesList.Groups.Add(group, group);
        }

        foreach (VoiceItem voice in speaker.Voices)
        {
            var item = new ListViewItem(voice.Name, this.voicesList.Groups[voice.Group]) { Tag = voice.Id };
            this.voicesList.Items.Add(item);
            if (voice.Id == speaker.CurrentVoiceId)
            {
                item.Selected = true;
                item.Focused = true;
            }
        }

        this.previewButton.Enabled = speaker.Voices.Count > 0;
        this.rateTrackBar.Value = settings.Rate;
        this.volumeTrackBar.Value = settings.Volume;
        this.thresholdNumeric.Value = settings.WordThreshold;
        this.autoReadCheckBox.Checked = settings.AutoReadClipboard;
        this.startupCheckBox.Checked = settings.StartWithWindows;
        this.sileroConfigButton.Visible = File.Exists(this.sileroConfigPath);
    }

    private void SettingsForm_Shown(object? sender, EventArgs e)
    {
        if (this.voicesList.SelectedItems.Count > 0)
        {
            this.voicesList.SelectedItems[0].EnsureVisible();
        }

        this.voicesList.Focus();
    }

    private void rateTrackBar_ValueChanged(object? sender, EventArgs e)
    {
        this.rateLabel.Text = $"Скорость: {this.rateTrackBar.Value}";
    }

    private void volumeTrackBar_ValueChanged(object? sender, EventArgs e)
    {
        this.volumeLabel.Text = $"Громкость: {this.volumeTrackBar.Value}";
    }

    private void previewButton_Click(object? sender, EventArgs e)
    {
        string? id = this.voicesList.SelectedItems.Count > 0 ? (string)this.voicesList.SelectedItems[0].Tag! : this.speaker.CurrentVoiceId;
        this.previewed = true;
        this.speaker.SelectVoice(id);
        this.speaker.Rate = this.rateTrackBar.Value;
        this.speaker.Volume = this.volumeTrackBar.Value;
        this.speaker.Speak("Здравствуйте! Так звучит этот голос.");
    }

    private void sileroConfigButton_Click(object? sender, EventArgs e)
    {
        Process.Start(new ProcessStartInfo(this.sileroConfigPath) { UseShellExecute = true });
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        if (this.voicesList.SelectedItems.Count > 0)
        {
            this.settings.VoiceId = (string)this.voicesList.SelectedItems[0].Tag!;
        }

        this.settings.Rate = this.rateTrackBar.Value;
        this.settings.Volume = this.volumeTrackBar.Value;
        this.settings.WordThreshold = (int)this.thresholdNumeric.Value;
        this.settings.AutoReadClipboard = this.autoReadCheckBox.Checked;
        this.settings.StartWithWindows = this.startupCheckBox.Checked;

        try
        {
            this.settings.Save();

            using RegistryKey runKey = Registry.CurrentUser.CreateSubKey(RunKeyPath);
            if (this.settings.StartWithWindows)
            {
                runKey.SetValue("TtsTray", $"\"{Environment.ProcessPath}\"");
            }
            else
            {
                runKey.DeleteValue("TtsTray", throwOnMissingValue: false);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            // Настройки уже применятся к работающей программе, но после перезапуска могут потеряться.
            MessageBox.Show(this, $"Не удалось сохранить настройки: {ex.Message}", "TtsTray", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        this.DialogResult = DialogResult.OK;
    }

    private void SettingsForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (this.DialogResult == DialogResult.OK || !this.previewed)
        {
            return;
        }

        // Отмена после «Прослушать» — возвращаем сохранённые голос, скорость и громкость.
        this.speaker.Stop();
        this.speaker.SelectVoice(this.settings.VoiceId);
        this.speaker.Rate = this.settings.Rate;
        this.speaker.Volume = this.settings.Volume;
    }
}
