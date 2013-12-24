namespace TeamViewer1S
{
    partial class TVServer
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
            this.Screen = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.paylasimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDosyaGonderim = new System.Windows.Forms.ToolStripMenuItem();
            this.alimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderChooser = new System.Windows.Forms.OpenFileDialog();
            this.proDosyaGonderimi = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.Screen)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Screen
            // 
            this.Screen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Screen.Location = new System.Drawing.Point(0, 24);
            this.Screen.Name = "Screen";
            this.Screen.Size = new System.Drawing.Size(533, 372);
            this.Screen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Screen.TabIndex = 1;
            this.Screen.TabStop = false;
            this.Screen.SizeChanged += new System.EventHandler(this.Screen_SizeChanged);
            this.Screen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Screen_MouseClick);
            this.Screen.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Screen_MouseDoubleClick);
            this.Screen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Screen_MouseMove);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.paylasimToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(533, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // paylasimToolStripMenuItem
            // 
            this.paylasimToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDosyaGonderim,
            this.alimToolStripMenuItem});
            this.paylasimToolStripMenuItem.Name = "paylasimToolStripMenuItem";
            this.paylasimToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.paylasimToolStripMenuItem.Text = "Paylasim";
            // 
            // mnuDosyaGonderim
            // 
            this.mnuDosyaGonderim.Name = "mnuDosyaGonderim";
            this.mnuDosyaGonderim.Size = new System.Drawing.Size(127, 22);
            this.mnuDosyaGonderim.Text = "Gonderim";
            this.mnuDosyaGonderim.Click += new System.EventHandler(this.mnuDosyaGonderim_Click);
            // 
            // alimToolStripMenuItem
            // 
            this.alimToolStripMenuItem.Name = "alimToolStripMenuItem";
            this.alimToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.alimToolStripMenuItem.Text = "Alim";
            // 
            // proDosyaGonderimi
            // 
            this.proDosyaGonderimi.Location = new System.Drawing.Point(409, 1);
            this.proDosyaGonderimi.Name = "proDosyaGonderimi";
            this.proDosyaGonderimi.Size = new System.Drawing.Size(124, 23);
            this.proDosyaGonderimi.TabIndex = 4;
            this.proDosyaGonderimi.Visible = false;
            // 
            // TVServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 396);
            this.Controls.Add(this.proDosyaGonderimi);
            this.Controls.Add(this.Screen);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TVServer";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TVServer_FormClosing);
            this.Load += new System.EventHandler(this.TVServer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TVServer_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TVServer_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.Screen)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.PictureBox Screen;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem paylasimToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuDosyaGonderim;
        private System.Windows.Forms.ToolStripMenuItem alimToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog folderChooser;
        internal System.Windows.Forms.ProgressBar proDosyaGonderimi;
    }
}

