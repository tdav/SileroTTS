using System.Runtime.InteropServices;

namespace TtsTray;

/// <summary>
/// Главное окно и иконка в трее. Создаётся скрытым (см. Program), показывается по запросу.
/// Слушает изменения буфера обмена и читает длинный скопированный текст.
/// </summary>
public partial class MainForm : Form
{
    private const int WmClipboardUpdate = 0x031D;

    private readonly AppSettings settings;
    private readonly SapiSpeaker speaker;
    private bool exiting;

    /// <summary>Последний автоозвученный текст — чтобы не перезапускать чтение на повторных уведомлениях.</summary>
    private string? lastAutoText;

    /// <summary>Открытый диалог настроек — второй не создаётся.</summary>
    private SettingsForm? settingsForm;

    public MainForm(AppSettings settings)
    {
        this.InitializeComponent();
        this.Icon = SystemIcons.Application;
        this.notifyIcon.Icon = SystemIcons.Application;

        this.settings = settings;
        this.speaker = new SapiSpeaker();
        this.speaker.Rate = settings.Rate;
        this.speaker.Volume = settings.Volume;
        this.speaker.StateChanged += this.Speaker_StateChanged;

        if (this.speaker.Voices.Count == 0)
        {
            this.speakButton.Enabled = false;
            MessageBox.Show("Голоса SAPI не найдены. Озвучивание недоступно.", "TtsTray", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!this.speaker.SelectVoice(settings.VoiceId) && settings.VoiceId != null)
        {
            string name = this.speaker.Voices.First(v => v.Id == this.speaker.CurrentVoiceId).Name;
            this.notifyIcon.ShowBalloonTip(5000, "TtsTray", $"Сохранённый голос не найден, выбран «{name}».", ToolTipIcon.Warning);
        }

        this.speaker.WarmUp();
    }

    /// <summary>Показывает окно из трея.</summary>
    public void ShowFromTray()
    {
        this.Show();
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.WindowState = FormWindowState.Normal;
        }

        this.Activate();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        AddClipboardFormatListener(this.Handle);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        RemoveClipboardFormatListener(this.Handle);
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WmClipboardUpdate && this.settings.AutoReadClipboard && Clipboard.ContainsText())
        {
            string? text = null;
            try
            {
                text = Clipboard.GetText();
            }
            catch (ExternalException)
            {
                // Буфер занят другой программой — пропускаем это изменение.
            }

            int words = text?.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length ?? 0;

            // Повторное уведомление на то же копирование (браузеры, Office шлют несколько) не перезапускает чтение.
            // Окно не показываем и фокус не забираем.
            if (text != null && words > this.settings.WordThreshold && !(this.speaker.IsBusy && text == this.lastAutoText))
            {
                this.lastAutoText = text;
                this.speaker.Speak(text);
            }
        }

        base.WndProc(ref m);
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    private void Speaker_StateChanged(object? sender, EventArgs e)
    {
        bool busy = this.speaker.IsBusy;
        this.pauseButton.Enabled = busy;
        this.pauseMenuItem.Enabled = busy;
        this.stopButton.Enabled = busy;
        this.stopMenuItem.Enabled = busy;

        string pauseText = this.speaker.IsPaused ? "Продолжить" : "Пауза";
        this.pauseButton.Text = pauseText;
        this.pauseMenuItem.Text = pauseText;

        // Текст подсказки NotifyIcon ограничен 127 символами.
        this.notifyIcon.Text = this.speaker.IsWarmingUp ? "TtsTray — загрузка голоса…"
            : this.speaker.IsPaused ? "TtsTray — пауза"
            : busy ? "TtsTray — читаю"
            : "TtsTray";
        this.notifyIcon.Icon = busy ? SystemIcons.Information : SystemIcons.Application;
    }

    private void speakButton_Click(object? sender, EventArgs e)
    {
        this.speaker.Speak(this.textBox.Text);
    }

    private void pauseButton_Click(object? sender, EventArgs e)
    {
        if (this.speaker.IsPaused)
        {
            this.speaker.Resume();
        }
        else
        {
            this.speaker.Pause();
        }
    }

    private void stopButton_Click(object? sender, EventArgs e)
    {
        this.speaker.Stop();
    }

    private void settingsButton_Click(object? sender, EventArgs e)
    {
        if (this.settingsForm != null)
        {
            this.settingsForm.Activate();
            return;
        }

        string? previousVoiceId = this.speaker.CurrentVoiceId;
        DialogResult result;
        using (this.settingsForm = new SettingsForm(this.speaker, this.settings))
        {
            // Скрытое окно не может быть владельцем модального диалога.
            result = this.Visible ? this.settingsForm.ShowDialog(this) : this.settingsForm.ShowDialog();
        }

        this.settingsForm = null;
        if (result != DialogResult.OK)
        {
            return;
        }

        this.speaker.SelectVoice(this.settings.VoiceId);
        this.speaker.Rate = this.settings.Rate;
        this.speaker.Volume = this.settings.Volume;
        if (this.speaker.CurrentVoiceId != previousVoiceId && !this.speaker.IsBusy)
        {
            this.speaker.WarmUp();
        }
    }

    private void textBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            this.speakButton.PerformClick();
        }
    }

    private void openMenuItem_Click(object? sender, EventArgs e)
    {
        this.ShowFromTray();
    }

    private void exitMenuItem_Click(object? sender, EventArgs e)
    {
        this.exiting = true;
        this.Close();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Крестик прячет окно в трей; закрывает программу только «Выход» (или завершение Windows).
        if (e.CloseReason == CloseReason.UserClosing && !this.exiting)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        this.speaker.Dispose();
        this.notifyIcon.Visible = false;
        this.notifyIcon.Dispose();
        Application.ExitThread();
    }
}
