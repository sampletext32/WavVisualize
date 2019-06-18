namespace WavVisualize
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBoxWaveform = new System.Windows.Forms.PictureBox();
            this.timerUpdater = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxRealtimeSpectrum = new System.Windows.Forms.PictureBox();
            this.numericUpDownEasing = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPow2Spectrum = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelElapsed = new System.Windows.Forms.Label();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelFPS = new System.Windows.Forms.Label();
            this.trackBarTrimFrequency = new System.Windows.Forms.TrackBar();
            this.buttonPlayPause = new System.Windows.Forms.Button();
            this.checkBoxApplyTimeThinning = new System.Windows.Forms.CheckBox();
            this.labelMaxFrequency = new System.Windows.Forms.Label();
            this.pictureBoxSpectrumDiagram = new System.Windows.Forms.PictureBox();
            this.pictureBoxVolume = new System.Windows.Forms.PictureBox();
            this.hScrollBarScale = new System.Windows.Forms.HScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWaveform)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRealtimeSpectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEasing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPow2Spectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrimFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrumDiagram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxWaveform
            // 
            this.pictureBoxWaveform.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBoxWaveform.Location = new System.Drawing.Point(0, 8);
            this.pictureBoxWaveform.Name = "pictureBoxWaveform";
            this.pictureBoxWaveform.Size = new System.Drawing.Size(1248, 128);
            this.pictureBoxWaveform.TabIndex = 0;
            this.pictureBoxWaveform.TabStop = false;
            this.pictureBoxWaveform.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxWaveform_Paint);
            this.pictureBoxWaveform.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPlot_MouseDown);
            this.pictureBoxWaveform.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPlot_MouseMove);
            this.pictureBoxWaveform.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPlot_MouseUp);
            // 
            // timerUpdater
            // 
            this.timerUpdater.Interval = 16;
            this.timerUpdater.Tick += new System.EventHandler(this.timerUpdater_Tick);
            // 
            // pictureBoxRealtimeSpectrum
            // 
            this.pictureBoxRealtimeSpectrum.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBoxRealtimeSpectrum.Location = new System.Drawing.Point(0, 400);
            this.pictureBoxRealtimeSpectrum.Name = "pictureBoxRealtimeSpectrum";
            this.pictureBoxRealtimeSpectrum.Size = new System.Drawing.Size(1248, 248);
            this.pictureBoxRealtimeSpectrum.TabIndex = 1;
            this.pictureBoxRealtimeSpectrum.TabStop = false;
            this.pictureBoxRealtimeSpectrum.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxSpectrum_Paint);
            // 
            // numericUpDownEasing
            // 
            this.numericUpDownEasing.Location = new System.Drawing.Point(1480, 8);
            this.numericUpDownEasing.Maximum = new decimal(new int[] {
            19,
            0,
            0,
            0});
            this.numericUpDownEasing.Name = "numericUpDownEasing";
            this.numericUpDownEasing.Size = new System.Drawing.Size(96, 22);
            this.numericUpDownEasing.TabIndex = 2;
            this.numericUpDownEasing.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownEasing.ValueChanged += new System.EventHandler(this.numericUpDownEasing_ValueChanged);
            // 
            // numericUpDownPow2Spectrum
            // 
            this.numericUpDownPow2Spectrum.Location = new System.Drawing.Point(1480, 48);
            this.numericUpDownPow2Spectrum.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDownPow2Spectrum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPow2Spectrum.Name = "numericUpDownPow2Spectrum";
            this.numericUpDownPow2Spectrum.Size = new System.Drawing.Size(96, 22);
            this.numericUpDownPow2Spectrum.TabIndex = 3;
            this.numericUpDownPow2Spectrum.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPow2Spectrum.ValueChanged += new System.EventHandler(this.numericUpDownPow2Spectrum_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1312, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Easing";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1312, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Pow 2 Spectrum Samples";
            // 
            // labelElapsed
            // 
            this.labelElapsed.AutoSize = true;
            this.labelElapsed.Location = new System.Drawing.Point(1312, 80);
            this.labelElapsed.Name = "labelElapsed";
            this.labelElapsed.Size = new System.Drawing.Size(59, 17);
            this.labelElapsed.TabIndex = 6;
            this.labelElapsed.Text = "Elapsed";
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(1312, 160);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(264, 32);
            this.buttonOpenFile.TabIndex = 7;
            this.buttonOpenFile.Text = "Open File";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(1312, 200);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(48, 17);
            this.labelStatus.TabIndex = 9;
            this.labelStatus.Text = "Status";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelFPS
            // 
            this.labelFPS.AutoSize = true;
            this.labelFPS.Location = new System.Drawing.Point(1312, 224);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(34, 17);
            this.labelFPS.TabIndex = 11;
            this.labelFPS.Text = "FPS";
            // 
            // trackBarTrimFrequency
            // 
            this.trackBarTrimFrequency.Location = new System.Drawing.Point(1312, 248);
            this.trackBarTrimFrequency.Maximum = 20000;
            this.trackBarTrimFrequency.Minimum = 1000;
            this.trackBarTrimFrequency.Name = "trackBarTrimFrequency";
            this.trackBarTrimFrequency.Size = new System.Drawing.Size(264, 56);
            this.trackBarTrimFrequency.TabIndex = 12;
            this.trackBarTrimFrequency.Value = 1000;
            this.trackBarTrimFrequency.Scroll += new System.EventHandler(this.trackBarTrimFrequency_Scroll);
            // 
            // buttonPlayPause
            // 
            this.buttonPlayPause.Location = new System.Drawing.Point(1312, 104);
            this.buttonPlayPause.Name = "buttonPlayPause";
            this.buttonPlayPause.Size = new System.Drawing.Size(264, 48);
            this.buttonPlayPause.TabIndex = 13;
            this.buttonPlayPause.Text = "Play / Pause";
            this.buttonPlayPause.UseVisualStyleBackColor = true;
            this.buttonPlayPause.Click += new System.EventHandler(this.buttonPlayPause_Click);
            // 
            // checkBoxApplyTimeThinning
            // 
            this.checkBoxApplyTimeThinning.AutoSize = true;
            this.checkBoxApplyTimeThinning.Location = new System.Drawing.Point(1320, 352);
            this.checkBoxApplyTimeThinning.Name = "checkBoxApplyTimeThinning";
            this.checkBoxApplyTimeThinning.Size = new System.Drawing.Size(159, 21);
            this.checkBoxApplyTimeThinning.TabIndex = 14;
            this.checkBoxApplyTimeThinning.Text = "Apply Time Thinning";
            this.checkBoxApplyTimeThinning.UseVisualStyleBackColor = true;
            this.checkBoxApplyTimeThinning.CheckedChanged += new System.EventHandler(this.checkBoxApplyTimeThinning_CheckedChanged);
            // 
            // labelMaxFrequency
            // 
            this.labelMaxFrequency.AutoSize = true;
            this.labelMaxFrequency.Location = new System.Drawing.Point(1320, 312);
            this.labelMaxFrequency.Name = "labelMaxFrequency";
            this.labelMaxFrequency.Size = new System.Drawing.Size(112, 17);
            this.labelMaxFrequency.TabIndex = 15;
            this.labelMaxFrequency.Text = "Max Frequency: ";
            // 
            // pictureBoxSpectrumDiagram
            // 
            this.pictureBoxSpectrumDiagram.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBoxSpectrumDiagram.Location = new System.Drawing.Point(0, 144);
            this.pictureBoxSpectrumDiagram.Name = "pictureBoxSpectrumDiagram";
            this.pictureBoxSpectrumDiagram.Size = new System.Drawing.Size(1248, 248);
            this.pictureBoxSpectrumDiagram.TabIndex = 16;
            this.pictureBoxSpectrumDiagram.TabStop = false;
            this.pictureBoxSpectrumDiagram.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxSpectrumDiagram_Paint);
            // 
            // pictureBoxVolume
            // 
            this.pictureBoxVolume.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBoxVolume.Location = new System.Drawing.Point(1256, 144);
            this.pictureBoxVolume.Name = "pictureBoxVolume";
            this.pictureBoxVolume.Size = new System.Drawing.Size(48, 248);
            this.pictureBoxVolume.TabIndex = 17;
            this.pictureBoxVolume.TabStop = false;
            this.pictureBoxVolume.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxVolume_Paint);
            // 
            // hScrollBarScale
            // 
            this.hScrollBarScale.Location = new System.Drawing.Point(1320, 408);
            this.hScrollBarScale.Maximum = 10000;
            this.hScrollBarScale.Minimum = 100;
            this.hScrollBarScale.Name = "hScrollBarScale";
            this.hScrollBarScale.Size = new System.Drawing.Size(256, 21);
            this.hScrollBarScale.TabIndex = 18;
            this.hScrollBarScale.Value = 100;
            this.hScrollBarScale.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBarScale_Scroll);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1585, 657);
            this.Controls.Add(this.hScrollBarScale);
            this.Controls.Add(this.pictureBoxVolume);
            this.Controls.Add(this.pictureBoxSpectrumDiagram);
            this.Controls.Add(this.labelMaxFrequency);
            this.Controls.Add(this.checkBoxApplyTimeThinning);
            this.Controls.Add(this.buttonPlayPause);
            this.Controls.Add(this.trackBarTrimFrequency);
            this.Controls.Add(this.labelFPS);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.labelElapsed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownPow2Spectrum);
            this.Controls.Add(this.numericUpDownEasing);
            this.Controls.Add(this.pictureBoxRealtimeSpectrum);
            this.Controls.Add(this.pictureBoxWaveform);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Awesome Player";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWaveform)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRealtimeSpectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEasing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPow2Spectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrimFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrumDiagram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxWaveform;
        private System.Windows.Forms.Timer timerUpdater;
        private System.Windows.Forms.PictureBox pictureBoxRealtimeSpectrum;
        private System.Windows.Forms.NumericUpDown numericUpDownEasing;
        private System.Windows.Forms.NumericUpDown numericUpDownPow2Spectrum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelElapsed;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelFPS;
        private System.Windows.Forms.TrackBar trackBarTrimFrequency;
        private System.Windows.Forms.Button buttonPlayPause;
        private System.Windows.Forms.CheckBox checkBoxApplyTimeThinning;
        private System.Windows.Forms.Label labelMaxFrequency;
        private System.Windows.Forms.PictureBox pictureBoxSpectrumDiagram;
        private System.Windows.Forms.PictureBox pictureBoxVolume;
        private System.Windows.Forms.HScrollBar hScrollBarScale;
    }
}

