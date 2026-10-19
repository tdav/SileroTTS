namespace TtsTray;

partial class SettingsForm
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
        voiceLabel = new Label();
        voicesList = new ListView();
        voiceColumn = new ColumnHeader();
        previewButton = new Button();
        sileroConfigButton = new Button();
        rateLabel = new Label();
        rateTrackBar = new TrackBar();
        volumeLabel = new Label();
        volumeTrackBar = new TrackBar();
        autoReadCheckBox = new CheckBox();
        thresholdLabel = new Label();
        thresholdNumeric = new NumericUpDown();
        startupCheckBox = new CheckBox();
        cancelButton = new Button();
        saveButton = new Button();
        ((System.ComponentModel.ISupportInitialize)rateTrackBar).BeginInit();
        ((System.ComponentModel.ISupportInitialize)volumeTrackBar).BeginInit();
        ((System.ComponentModel.ISupportInitialize)thresholdNumeric).BeginInit();
        SuspendLayout();
        //
        // voiceLabel
        //
        voiceLabel.AutoSize = true;
        voiceLabel.Location = new Point(12, 12);
        voiceLabel.Name = "voiceLabel";
        voiceLabel.Text = "Голос:";
        //
        // voicesList
        //
        voicesList.Columns.AddRange(new ColumnHeader[] { voiceColumn });
        voicesList.FullRowSelect = true;
        voicesList.HeaderStyle = ColumnHeaderStyle.None;
        voicesList.HideSelection = false;
        voicesList.Location = new Point(12, 32);
        voicesList.MultiSelect = false;
        voicesList.Name = "voicesList";
        voicesList.Size = new Size(496, 220);
        voicesList.TabIndex = 0;
        voicesList.View = View.Details;
        //
        // voiceColumn
        //
        voiceColumn.Text = "Голос";
        voiceColumn.Width = 470;
        //
        // previewButton
        //
        previewButton.AutoSize = true;
        previewButton.Location = new Point(12, 260);
        previewButton.Name = "previewButton";
        previewButton.TabIndex = 1;
        previewButton.Text = "Прослушать";
        previewButton.Click += previewButton_Click;
        //
        // sileroConfigButton
        //
        sileroConfigButton.AutoSize = true;
        sileroConfigButton.Location = new Point(368, 260);
        sileroConfigButton.Name = "sileroConfigButton";
        sileroConfigButton.Size = new Size(140, 27);
        sileroConfigButton.TabIndex = 2;
        sileroConfigButton.Text = "Настройки Silero…";
        sileroConfigButton.Click += sileroConfigButton_Click;
        //
        // rateLabel
        //
        rateLabel.AutoSize = true;
        rateLabel.Location = new Point(12, 302);
        rateLabel.Name = "rateLabel";
        rateLabel.Text = "Скорость: 0";
        //
        // rateTrackBar
        //
        rateTrackBar.Location = new Point(12, 320);
        rateTrackBar.Maximum = 10;
        rateTrackBar.Minimum = -10;
        rateTrackBar.Name = "rateTrackBar";
        rateTrackBar.Size = new Size(496, 45);
        rateTrackBar.TabIndex = 3;
        rateTrackBar.ValueChanged += rateTrackBar_ValueChanged;
        //
        // volumeLabel
        //
        volumeLabel.AutoSize = true;
        volumeLabel.Location = new Point(12, 368);
        volumeLabel.Name = "volumeLabel";
        volumeLabel.Text = "Громкость: 100";
        //
        // volumeTrackBar
        //
        volumeTrackBar.LargeChange = 10;
        volumeTrackBar.Location = new Point(12, 386);
        volumeTrackBar.Maximum = 100;
        volumeTrackBar.Name = "volumeTrackBar";
        volumeTrackBar.Size = new Size(496, 45);
        volumeTrackBar.TabIndex = 4;
        volumeTrackBar.TickFrequency = 10;
        volumeTrackBar.Value = 100;
        volumeTrackBar.ValueChanged += volumeTrackBar_ValueChanged;
        //
        // autoReadCheckBox
        //
        autoReadCheckBox.AutoSize = true;
        autoReadCheckBox.Location = new Point(12, 440);
        autoReadCheckBox.Name = "autoReadCheckBox";
        autoReadCheckBox.TabIndex = 5;
        autoReadCheckBox.Text = "Читать длинный текст из буфера обмена";
        //
        // thresholdLabel
        //
        thresholdLabel.AutoSize = true;
        thresholdLabel.Location = new Point(32, 470);
        thresholdLabel.Name = "thresholdLabel";
        thresholdLabel.Text = "если в нём слов больше:";
        //
        // thresholdNumeric
        //
        thresholdNumeric.Location = new Point(220, 467);
        thresholdNumeric.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        thresholdNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        thresholdNumeric.Name = "thresholdNumeric";
        thresholdNumeric.Size = new Size(90, 23);
        thresholdNumeric.TabIndex = 6;
        thresholdNumeric.Value = new decimal(new int[] { 100, 0, 0, 0 });
        //
        // startupCheckBox
        //
        startupCheckBox.AutoSize = true;
        startupCheckBox.Location = new Point(12, 504);
        startupCheckBox.Name = "startupCheckBox";
        startupCheckBox.TabIndex = 7;
        startupCheckBox.Text = "Запускать вместе с Windows";
        //
        // cancelButton
        //
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(332, 546);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(85, 27);
        cancelButton.TabIndex = 9;
        cancelButton.Text = "Отмена";
        //
        // saveButton
        //
        saveButton.Location = new Point(423, 546);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(85, 27);
        saveButton.TabIndex = 8;
        saveButton.Text = "Сохранить";
        saveButton.Click += saveButton_Click;
        //
        // SettingsForm
        //
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(520, 585);
        Controls.Add(voiceLabel);
        Controls.Add(voicesList);
        Controls.Add(previewButton);
        Controls.Add(sileroConfigButton);
        Controls.Add(rateLabel);
        Controls.Add(rateTrackBar);
        Controls.Add(volumeLabel);
        Controls.Add(volumeTrackBar);
        Controls.Add(autoReadCheckBox);
        Controls.Add(thresholdLabel);
        Controls.Add(thresholdNumeric);
        Controls.Add(startupCheckBox);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Настройки TtsTray";
        FormClosed += SettingsForm_FormClosed;
        Shown += SettingsForm_Shown;
        ((System.ComponentModel.ISupportInitialize)rateTrackBar).EndInit();
        ((System.ComponentModel.ISupportInitialize)volumeTrackBar).EndInit();
        ((System.ComponentModel.ISupportInitialize)thresholdNumeric).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label voiceLabel;
    private ListView voicesList;
    private ColumnHeader voiceColumn;
    private Button previewButton;
    private Button sileroConfigButton;
    private Label rateLabel;
    private TrackBar rateTrackBar;
    private Label volumeLabel;
    private TrackBar volumeTrackBar;
    private CheckBox autoReadCheckBox;
    private Label thresholdLabel;
    private NumericUpDown thresholdNumeric;
    private CheckBox startupCheckBox;
    private Button cancelButton;
    private Button saveButton;
}
