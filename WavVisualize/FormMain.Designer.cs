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
            this.pictureBoxPlot = new System.Windows.Forms.PictureBox();
            this.timerUpdater = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxSpectrum = new System.Windows.Forms.PictureBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEasing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPow2Spectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrimFrequency)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxPlot
            // 
            this.pictureBoxPlot.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBoxPlot.Location = new System.Drawing.Point(64, 0);
            this.pictureBoxPlot.Name = "pictureBoxPlot";
            this.pictureBoxPlot.Size = new System.Drawing.Size(728, 128);
            this.pictureBoxPlot.TabIndex = 0;
            this.pictureBoxPlot.TabStop = false;
            this.pictureBoxPlot.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxPlot_Paint);
            this.pictureBoxPlot.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPlot_MouseDown);
            this.pictureBoxPlot.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPlot_MouseMove);
            this.pictureBoxPlot.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPlot_MouseUp);
            // 
            // timerUpdater
            // 
            this.timerUpdater.Interval = 16;
            this.timerUpdater.Tick += new System.EventHandler(this.timerUpdater_Tick);
            // 
            // pictureBoxSpectrum
            // 
            this.pictureBoxSpectrum.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBoxSpectrum.Location = new System.Drawing.Point(0, 152);
            this.pictureBoxSpectrum.Name = "pictureBoxSpectrum";
            this.pictureBoxSpectrum.Size = new System.Drawing.Size(792, 280);
            this.pictureBoxSpectrum.TabIndex = 1;
            this.pictureBoxSpectrum.TabStop = false;
            this.pictureBoxSpectrum.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxSpectrum_Paint);
            // 
            // numericUpDownEasing
            // 
            this.numericUpDownEasing.Location = new System.Drawing.Point(968, 8);
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
            this.numericUpDownPow2Spectrum.Location = new System.Drawing.Point(968, 48);
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
            this.label1.Location = new System.Drawing.Point(800, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Easing";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(800, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Pow 2 Spectrum Samples";
            // 
            // labelElapsed
            // 
            this.labelElapsed.AutoSize = true;
            this.labelElapsed.Location = new System.Drawing.Point(800, 80);
            this.labelElapsed.Name = "labelElapsed";
            this.labelElapsed.Size = new System.Drawing.Size(59, 17);
            this.labelElapsed.TabIndex = 6;
            this.labelElapsed.Text = "Elapsed";
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(800, 160);
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
            this.labelStatus.Location = new System.Drawing.Point(800, 200);
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
            this.labelFPS.Location = new System.Drawing.Point(800, 224);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(34, 17);
            this.labelFPS.TabIndex = 11;
            this.labelFPS.Text = "FPS";
            // 
            // trackBarTrimFrequency
            // 
            this.trackBarTrimFrequency.Location = new System.Drawing.Point(800, 248);
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
            this.buttonPlayPause.Location = new System.Drawing.Point(800, 104);
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
            this.checkBoxApplyTimeThinning.Location = new System.Drawing.Point(808, 352);
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
            this.labelMaxFrequency.Location = new System.Drawing.Point(808, 312);
            this.labelMaxFrequency.Name = "labelMaxFrequency";
            this.labelMaxFrequency.Size = new System.Drawing.Size(112, 17);
            this.labelMaxFrequency.TabIndex = 15;
            this.labelMaxFrequency.Text = "Max Frequency: ";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 450);
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
            this.Controls.Add(this.pictureBoxSpectrum);
            this.Controls.Add(this.pictureBoxPlot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Awesome Player";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEasing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPow2Spectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTrimFrequency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxPlot;
        private System.Windows.Forms.Timer timerUpdater;
        private System.Windows.Forms.PictureBox pictureBoxSpectrum;
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
    }
}

