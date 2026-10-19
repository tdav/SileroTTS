namespace TtsTray;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        textBox = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        speakButton = new Button();
        pauseButton = new Button();
        stopButton = new Button();
        settingsButton = new Button();
        notifyIcon = new NotifyIcon(components);
        trayMenu = new ContextMenuStrip(components);
        openMenuItem = new ToolStripMenuItem();
        pauseMenuItem = new ToolStripMenuItem();
        stopMenuItem = new ToolStripMenuItem();
        settingsMenuItem = new ToolStripMenuItem();
        separatorMenuItem = new ToolStripSeparator();
        exitMenuItem = new ToolStripMenuItem();
        buttonPanel.SuspendLayout();
        trayMenu.SuspendLayout();
        SuspendLayout();
        //
        // textBox
        //
        textBox.AcceptsReturn = true;
        textBox.Dock = DockStyle.Fill;
        textBox.Font = new Font("Segoe UI", 11F);
        textBox.Multiline = true;
        textBox.Name = "textBox";
        textBox.ScrollBars = ScrollBars.Vertical;
        textBox.TabIndex = 0;
        textBox.KeyDown += textBox_KeyDown;
        //
        // buttonPanel
        //
        buttonPanel.AutoSize = true;
        buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        buttonPanel.Controls.Add(speakButton);
        buttonPanel.Controls.Add(pauseButton);
        buttonPanel.Controls.Add(stopButton);
        buttonPanel.Controls.Add(settingsButton);
        buttonPanel.Dock = DockStyle.Bottom;
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Padding = new Padding(6);
        buttonPanel.TabIndex = 1;
        //
        // speakButton
        //
        speakButton.AutoSize = true;
        speakButton.Name = "speakButton";
        speakButton.TabIndex = 0;
        speakButton.Text = "Озвучить";
        speakButton.Click += speakButton_Click;
        //
        // pauseButton
        //
        pauseButton.AutoSize = true;
        pauseButton.Enabled = false;
        pauseButton.Name = "pauseButton";
        pauseButton.TabIndex = 1;
        pauseButton.Text = "Пауза";
        pauseButton.Click += pauseButton_Click;
        //
        // stopButton
        //
        stopButton.AutoSize = true;
        stopButton.Enabled = false;
        stopButton.Name = "stopButton";
        stopButton.TabIndex = 2;
        stopButton.Text = "Стоп Ctrl+Alt+S";
        stopButton.Click += stopButton_Click;
        //
        // settingsButton
        //
        settingsButton.AutoSize = true;
        settingsButton.Name = "settingsButton";
        settingsButton.TabIndex = 3;
        settingsButton.Text = "Настройки…";
        settingsButton.Click += settingsButton_Click;
        //
        // notifyIcon
        //
        notifyIcon.ContextMenuStrip = trayMenu;
        notifyIcon.Text = "TtsTray";
        notifyIcon.Visible = true;
        notifyIcon.DoubleClick += openMenuItem_Click;
        //
        // trayMenu
        //
        trayMenu.Items.AddRange(new ToolStripItem[] { openMenuItem, pauseMenuItem, stopMenuItem, settingsMenuItem, separatorMenuItem, exitMenuItem });
        trayMenu.Name = "trayMenu";
        //
        // openMenuItem
        //
        openMenuItem.Font = new Font(openMenuItem.Font, FontStyle.Bold);
        openMenuItem.Name = "openMenuItem";
        openMenuItem.Text = "Открыть";
        openMenuItem.Click += openMenuItem_Click;
        //
        // pauseMenuItem
        //
        pauseMenuItem.Enabled = false;
        pauseMenuItem.Name = "pauseMenuItem";
        pauseMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+P";
        pauseMenuItem.Text = "Пауза";
        pauseMenuItem.Click += pauseButton_Click;
        //
        // stopMenuItem
        //
        stopMenuItem.Enabled = false;
        stopMenuItem.Name = "stopMenuItem";
        stopMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+S";
        stopMenuItem.Text = "Стоп";
        stopMenuItem.Click += stopButton_Click;
        //
        // settingsMenuItem
        //
        settingsMenuItem.Name = "settingsMenuItem";
        settingsMenuItem.Text = "Настройки…";
        settingsMenuItem.Click += settingsButton_Click;
        //
        // exitMenuItem
        //
        exitMenuItem.Name = "exitMenuItem";
        exitMenuItem.Text = "Выход";
        exitMenuItem.Click += exitMenuItem_Click;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(640, 400);
        Controls.Add(textBox);
        Controls.Add(buttonPanel);
        MinimumSize = new Size(400, 250);
        Name = "MainForm";
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TtsTray";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        trayMenu.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TextBox textBox;
    private FlowLayoutPanel buttonPanel;
    private Button speakButton;
    private Button pauseButton;
    private Button stopButton;
    private Button settingsButton;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip trayMenu;
    private ToolStripMenuItem openMenuItem;
    private ToolStripMenuItem pauseMenuItem;
    private ToolStripMenuItem stopMenuItem;
    private ToolStripMenuItem settingsMenuItem;
    private ToolStripSeparator separatorMenuItem;
    private ToolStripMenuItem exitMenuItem;
}
