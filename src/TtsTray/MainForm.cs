namespace TtsTray;

/// <summary>
/// Главное окно и иконка в трее. Создаётся скрытым (см. Program), показывается по запросу.
/// </summary>
public partial class MainForm : Form
{
    private readonly AppSettings settings;
    private readonly SapiSpeaker speaker;
    private bool exiting;

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
