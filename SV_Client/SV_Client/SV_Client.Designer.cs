namespace SV_Client
{
    partial class SV_Client
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
            this.lstIPler = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnEkranPaylasimi = new System.Windows.Forms.Button();
            this.btnDosyaGonderimi = new System.Windows.Forms.Button();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.DosyaSecici = new System.Windows.Forms.OpenFileDialog();
            this.txtBaglanilacakIP = new System.Windows.Forms.TextBox();
            this.lblBaglanilacakIP = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstIPler
            // 
            this.lstIPler.FormattingEnabled = true;
            this.lstIPler.Location = new System.Drawing.Point(12, 42);
            this.lstIPler.Name = "lstIPler";
            this.lstIPler.Size = new System.Drawing.Size(249, 134);
            this.lstIPler.TabIndex = 3;
            this.lstIPler.DoubleClick += new System.EventHandler(this.lstIPler_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Baglanti baslatmak icin IP secip uzerine cift tiklayiniz";
            // 
            // BtnEkranPaylasimi
            // 
            this.BtnEkranPaylasimi.Location = new System.Drawing.Point(12, 211);
            this.BtnEkranPaylasimi.Name = "BtnEkranPaylasimi";
            this.BtnEkranPaylasimi.Size = new System.Drawing.Size(103, 23);
            this.BtnEkranPaylasimi.TabIndex = 5;
            this.BtnEkranPaylasimi.Text = "Ekran Paylasimi";
            this.BtnEkranPaylasimi.UseVisualStyleBackColor = true;
            this.BtnEkranPaylasimi.Click += new System.EventHandler(this.BtnEkranPaylasimi_Click);
            // 
            // btnDosyaGonderimi
            // 
            this.btnDosyaGonderimi.Location = new System.Drawing.Point(158, 211);
            this.btnDosyaGonderimi.Name = "btnDosyaGonderimi";
            this.btnDosyaGonderimi.Size = new System.Drawing.Size(103, 23);
            this.btnDosyaGonderimi.TabIndex = 6;
            this.btnDosyaGonderimi.Text = "Dosya Gonderimi";
            this.btnDosyaGonderimi.UseVisualStyleBackColor = true;
            this.btnDosyaGonderimi.Click += new System.EventHandler(this.btnDosyaGonderimi_Click);
            // 
            // Timer
            // 
            this.Timer.Interval = 10;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // txtBaglanilacakIP
            // 
            this.txtBaglanilacakIP.Location = new System.Drawing.Point(108, 15);
            this.txtBaglanilacakIP.Name = "txtBaglanilacakIP";
            this.txtBaglanilacakIP.Size = new System.Drawing.Size(122, 20);
            this.txtBaglanilacakIP.TabIndex = 7;
            // 
            // lblBaglanilacakIP
            // 
            this.lblBaglanilacakIP.AutoSize = true;
            this.lblBaglanilacakIP.Location = new System.Drawing.Point(12, 18);
            this.lblBaglanilacakIP.Name = "lblBaglanilacakIP";
            this.lblBaglanilacakIP.Size = new System.Drawing.Size(90, 13);
            this.lblBaglanilacakIP.TabIndex = 8;
            this.lblBaglanilacakIP.Text = "Baglanilacak IP =";
            // 
            // SV_Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 262);
            this.Controls.Add(this.lblBaglanilacakIP);
            this.Controls.Add(this.txtBaglanilacakIP);
            this.Controls.Add(this.btnDosyaGonderimi);
            this.Controls.Add(this.BtnEkranPaylasimi);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstIPler);
            this.Name = "SV_Client";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "SV_Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SV_Client_FormClosing);
            this.Load += new System.EventHandler(this.SV_Client_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstIPler;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnEkranPaylasimi;
        private System.Windows.Forms.Button btnDosyaGonderimi;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.OpenFileDialog DosyaSecici;
        private System.Windows.Forms.TextBox txtBaglanilacakIP;
        private System.Windows.Forms.Label lblBaglanilacakIP;
    }
}

