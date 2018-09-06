using System.Windows.Forms;

namespace ZedTester
{
    partial class ZedUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZedUI));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.serverMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.thresholdTrack = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.erodeTrack = new System.Windows.Forms.TrackBar();
            this.erodeLabel = new System.Windows.Forms.Label();
            this.cleanupLabel = new System.Windows.Forms.Label();
            this.dilateTrack = new System.Windows.Forms.TrackBar();
            this.cleanupTrackbar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.contrastTrack = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.depthTrack = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.previewCheck = new System.Windows.Forms.CheckBox();
            this.depthQuality = new System.Windows.Forms.ComboBox();
            this.sideBySideCheck = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.useCuda = new System.Windows.Forms.CheckBox();
            this.depthMode = new System.Windows.Forms.ComboBox();
            this.resolution = new System.Windows.Forms.ComboBox();
            this.resolutionLabel = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
            this.pictureBoxRight = new System.Windows.Forms.PictureBox();
            this.toolTipThreshold = new System.Windows.Forms.ToolTip(this.components);
            this.depthToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.erodeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.dilateToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.contrastToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cleanupToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.camera = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.toolStrip2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.erodeTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dilateTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cleanupTrackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthTrack)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1390, 39);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(207, 36);
            this.toolStripButton1.Text = "Clear Background";
            this.toolStripButton1.Click += new System.EventHandler(this.clearBackground_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverMessage,
            this.fpsLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 742);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1390, 37);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip2";
            // 
            // serverMessage
            // 
            this.serverMessage.Name = "serverMessage";
            this.serverMessage.Size = new System.Drawing.Size(153, 32);
            this.serverMessage.Text = "Server Ready";
            // 
            // fpsLabel
            // 
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(53, 32);
            this.fpsLabel.Text = "FPS";
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(1136, 39);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(254, 703);
            this.panel2.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(254, 703);
            this.tabControl1.TabIndex = 21;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.thresholdTrack);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.erodeTrack);
            this.tabPage1.Controls.Add(this.erodeLabel);
            this.tabPage1.Controls.Add(this.cleanupLabel);
            this.tabPage1.Controls.Add(this.dilateTrack);
            this.tabPage1.Controls.Add(this.cleanupTrackbar);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.contrastTrack);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.depthTrack);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(238, 656);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Modifiers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // thresholdTrack
            // 
            this.thresholdTrack.Location = new System.Drawing.Point(6, 25);
            this.thresholdTrack.Maximum = 255;
            this.thresholdTrack.Name = "thresholdTrack";
            this.thresholdTrack.Size = new System.Drawing.Size(247, 90);
            this.thresholdTrack.TabIndex = 0;
            this.thresholdTrack.Value = 10;
            this.thresholdTrack.ValueChanged += new System.EventHandler(this.thresholdValueBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Threshold";
            // 
            // erodeTrack
            // 
            this.erodeTrack.Location = new System.Drawing.Point(6, 85);
            this.erodeTrack.Maximum = 30;
            this.erodeTrack.Name = "erodeTrack";
            this.erodeTrack.Size = new System.Drawing.Size(247, 90);
            this.erodeTrack.TabIndex = 2;
            this.erodeTrack.ValueChanged += new System.EventHandler(this.erodeBox_TextChanged);
            // 
            // erodeLabel
            // 
            this.erodeLabel.AutoSize = true;
            this.erodeLabel.Location = new System.Drawing.Point(6, 71);
            this.erodeLabel.Name = "erodeLabel";
            this.erodeLabel.Size = new System.Drawing.Size(69, 25);
            this.erodeLabel.TabIndex = 3;
            this.erodeLabel.Text = "Erode";
            // 
            // cleanupLabel
            // 
            this.cleanupLabel.AutoSize = true;
            this.cleanupLabel.Location = new System.Drawing.Point(6, 311);
            this.cleanupLabel.Name = "cleanupLabel";
            this.cleanupLabel.Size = new System.Drawing.Size(92, 25);
            this.cleanupLabel.TabIndex = 17;
            this.cleanupLabel.Text = "Cleanup";
            // 
            // dilateTrack
            // 
            this.dilateTrack.Location = new System.Drawing.Point(6, 145);
            this.dilateTrack.Maximum = 30;
            this.dilateTrack.Name = "dilateTrack";
            this.dilateTrack.Size = new System.Drawing.Size(247, 90);
            this.dilateTrack.TabIndex = 4;
            this.dilateTrack.ValueChanged += new System.EventHandler(this.dilateBox_TextChanged);
            // 
            // cleanupTrackbar
            // 
            this.cleanupTrackbar.Location = new System.Drawing.Point(6, 325);
            this.cleanupTrackbar.Name = "cleanupTrackbar";
            this.cleanupTrackbar.Size = new System.Drawing.Size(247, 90);
            this.cleanupTrackbar.TabIndex = 16;
            this.cleanupTrackbar.ValueChanged += new System.EventHandler(this.cleanupTrackbar_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Dilate";
            // 
            // contrastTrack
            // 
            this.contrastTrack.Location = new System.Drawing.Point(6, 205);
            this.contrastTrack.Maximum = 255;
            this.contrastTrack.Name = "contrastTrack";
            this.contrastTrack.Size = new System.Drawing.Size(247, 90);
            this.contrastTrack.TabIndex = 6;
            this.contrastTrack.ValueChanged += new System.EventHandler(this.contrast_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "Contrast";
            // 
            // depthTrack
            // 
            this.depthTrack.Location = new System.Drawing.Point(6, 265);
            this.depthTrack.Maximum = 255;
            this.depthTrack.Name = "depthTrack";
            this.depthTrack.Size = new System.Drawing.Size(247, 90);
            this.depthTrack.TabIndex = 8;
            this.depthTrack.ValueChanged += new System.EventHandler(this.depthValueBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 251);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 25);
            this.label4.TabIndex = 9;
            this.label4.Text = "Depth";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.camera);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.previewCheck);
            this.tabPage2.Controls.Add(this.depthQuality);
            this.tabPage2.Controls.Add(this.sideBySideCheck);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.useCuda);
            this.tabPage2.Controls.Add(this.depthMode);
            this.tabPage2.Controls.Add(this.resolution);
            this.tabPage2.Controls.Add(this.resolutionLabel);
            this.tabPage2.Location = new System.Drawing.Point(8, 39);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(238, 656);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Setup";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 25);
            this.label5.TabIndex = 11;
            this.label5.Text = "Depth Quality";
            // 
            // previewCheck
            // 
            this.previewCheck.AutoSize = true;
            this.previewCheck.Checked = true;
            this.previewCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.previewCheck.Location = new System.Drawing.Point(6, 209);
            this.previewCheck.Name = "previewCheck";
            this.previewCheck.Size = new System.Drawing.Size(120, 29);
            this.previewCheck.TabIndex = 20;
            this.previewCheck.Text = "Preview";
            this.previewCheck.UseVisualStyleBackColor = true;
            this.previewCheck.CheckedChanged += new System.EventHandler(this.previewCheck_CheckedChanged);
            // 
            // depthQuality
            // 
            this.depthQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.depthQuality.FormattingEnabled = true;
            this.depthQuality.Items.AddRange(new object[] {
            "No Depth",
            "Performance",
            "Medium",
            "Quality",
            "Ultra"});
            this.depthQuality.Location = new System.Drawing.Point(6, 31);
            this.depthQuality.Name = "depthQuality";
            this.depthQuality.Size = new System.Drawing.Size(221, 33);
            this.depthQuality.TabIndex = 10;
            this.depthQuality.SelectedIndexChanged += new System.EventHandler(this.depthQuality_SelectedIndexChanged);
            // 
            // sideBySideCheck
            // 
            this.sideBySideCheck.AutoSize = true;
            this.sideBySideCheck.Location = new System.Drawing.Point(5, 179);
            this.sideBySideCheck.Name = "sideBySideCheck";
            this.sideBySideCheck.Size = new System.Drawing.Size(167, 29);
            this.sideBySideCheck.TabIndex = 19;
            this.sideBySideCheck.Text = "Side By Side";
            this.sideBySideCheck.UseVisualStyleBackColor = true;
            this.sideBySideCheck.CheckedChanged += new System.EventHandler(this.sideBySideCheck_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(129, 25);
            this.label6.TabIndex = 12;
            this.label6.Text = "Depth Mode";
            // 
            // useCuda
            // 
            this.useCuda.AutoSize = true;
            this.useCuda.Location = new System.Drawing.Point(6, 149);
            this.useCuda.Name = "useCuda";
            this.useCuda.Size = new System.Drawing.Size(139, 29);
            this.useCuda.TabIndex = 18;
            this.useCuda.Text = "Use Cuda";
            this.useCuda.UseVisualStyleBackColor = true;
            this.useCuda.CheckedChanged += new System.EventHandler(this.useCuda_CheckedChanged);
            // 
            // depthMode
            // 
            this.depthMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.depthMode.FormattingEnabled = true;
            this.depthMode.Items.AddRange(new object[] {
            "Standard",
            "Fill"});
            this.depthMode.Location = new System.Drawing.Point(6, 75);
            this.depthMode.Name = "depthMode";
            this.depthMode.Size = new System.Drawing.Size(221, 33);
            this.depthMode.TabIndex = 13;
            this.depthMode.SelectedIndexChanged += new System.EventHandler(this.depthMode_SelectedIndexChanged);
            // 
            // resolution
            // 
            this.resolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolution.FormattingEnabled = true;
            this.resolution.Items.AddRange(new object[] {
            "2K",
            "HD 1980",
            "HD 720",
            "VGA"});
            this.resolution.Location = new System.Drawing.Point(6, 120);
            this.resolution.Name = "resolution";
            this.resolution.Size = new System.Drawing.Size(221, 33);
            this.resolution.TabIndex = 15;
            this.resolution.SelectedIndexChanged += new System.EventHandler(this.resolution_SelectedIndexChanged);
            // 
            // resolutionLabel
            // 
            this.resolutionLabel.AutoSize = true;
            this.resolutionLabel.Location = new System.Drawing.Point(6, 104);
            this.resolutionLabel.Name = "resolutionLabel";
            this.resolutionLabel.Size = new System.Drawing.Size(114, 25);
            this.resolutionLabel.TabIndex = 14;
            this.resolutionLabel.Text = "Resolution";
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.Black;
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 39);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pictureBoxLeft);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBoxRight);
            this.splitContainer2.Size = new System.Drawing.Size(1136, 703);
            this.splitContainer2.SplitterDistance = 828;
            this.splitContainer2.TabIndex = 3;
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxLeft.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.Size = new System.Drawing.Size(826, 701);
            this.pictureBoxLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLeft.TabIndex = 3;
            this.pictureBoxLeft.TabStop = false;
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxRight.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(302, 701);
            this.pictureBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxRight.TabIndex = 0;
            this.pictureBoxRight.TabStop = false;
            // 
            // camera
            // 
            this.camera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.camera.FormattingEnabled = true;
            this.camera.Items.AddRange(new object[] {
            "ZED Camera",
            "Web Camera"});
            this.camera.Location = new System.Drawing.Point(6, 298);
            this.camera.Name = "camera";
            this.camera.Size = new System.Drawing.Size(221, 33);
            this.camera.TabIndex = 21;
            this.camera.SelectedIndexChanged += new System.EventHandler(this.camera_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 267);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 25);
            this.label7.TabIndex = 22;
            this.label7.Text = "Camera";
            // 
            // ZedUI
            // 
            this.ClientSize = new System.Drawing.Size(1390, 779);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip2);
            this.Name = "ZedUI";
            this.Text = "Holo Server 0.0.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveSettings);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.erodeTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dilateTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cleanupTrackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthTrack)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip2;
        private SplitContainer splitContainer2;
        private Panel panel2;
        private StatusStrip statusStrip1;
        private TrackBar thresholdTrack;
        private Label label1;
        private Label label3;
        private TrackBar contrastTrack;
        private Label label2;
        private TrackBar dilateTrack;
        private Label erodeLabel;
        private TrackBar erodeTrack;
        private ToolStripStatusLabel serverMessage;
        private ToolStripStatusLabel fpsLabel;
        private Label label4;
        private TrackBar depthTrack;
        private ToolTip toolTipThreshold;
        private ToolTip depthToolTip;
        private ToolTip erodeToolTip;
        private ToolTip dilateToolTip;
        private ToolTip contrastToolTip;
        private ToolStripButton toolStripButton1;
        private ComboBox depthMode;
        private Label label6;
        private Label label5;
        private ComboBox depthQuality;
        private ComboBox resolution;
        private Label resolutionLabel;
        private Label cleanupLabel;
        private TrackBar cleanupTrackbar;
        private ToolTip cleanupToolTip;
        private CheckBox useCuda;
        private CheckBox sideBySideCheck;
        private PictureBox pictureBoxLeft;
        private PictureBox pictureBoxRight;
        private CheckBox previewCheck;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label7;
        private ComboBox camera;
    }
}