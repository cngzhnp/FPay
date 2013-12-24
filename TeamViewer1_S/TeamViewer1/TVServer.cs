using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamViewer1S.Classes;
using System.Threading;
using gma.System.Windows;

namespace TeamViewer1S
{
    public partial class TVServer : Form
    {
        UserActivityHook actHook; // Klavyeden basilan tuslari anlamak icin class
        static Server2 ser = new Server2();
        bool mouseGonderildi = true;
        IAsyncResult result;
        AsyncMethodCaller caller;
        public TVServer()
        {
            InitializeComponent();
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;
            caller = new AsyncMethodCaller(dosyaGonderimi);
          /*  actHook = new UserActivityHook(false, true);
            actHook.KeyDown += new KeyEventHandler(MyKeyDown);//klavyeye basılma esnasındaki yakalama
            actHook.KeyUp += new KeyEventHandler(MyKeyUp);//klavyeye basmayı kaldırırken ki yakalamalara*/
        }

        private void TVServer_Load(object sender, EventArgs e)
        {
            Screen.ImageLocation = Application.StartupPath + "\\baslangic.jpg";
            ser.Logger("Baslangic image'i yuklendi");
            ser.BaglantiBaslat(this);
            ser.Logger("Baglanti Tamamlandi");
            ser.EkranBoyutu(Screen.Size);
            ser.Logger("Pencere Boyutu Kaydedildi");
        }

        private void Screen_SizeChanged(object sender, EventArgs e)
        {
            ser.EkranBoyutu(Screen.Size);
        }

        #region Mouse
        private void Screen_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseGonderildi)
            {
                mouseGonderildi = false;
                Point mouseKordinat = new Point(e.X, e.Y);
       //         ser.Logger("Mouse Kordinati Gonderildi = (" + e.X + "," + e.Y + ")" );
                mouseGonderildi = ser.MouseKordinat(mouseKordinat, 1); // Sadece Mouse'nin Kordinatlarini cliente gonderir.     
            }
        }

        private void Screen_MouseClick(object sender, MouseEventArgs e)
        {
            if (mouseGonderildi)
            {
                mouseGonderildi = false;
                Point mouseKordinat = new Point(e.X, e.Y);
                ser.Logger("Mouse Tiklamasi Gonderildi = (" + e.X + "," + e.Y + ")");
                mouseGonderildi = ser.MouseKordinat(mouseKordinat, 2); // Sadece Mouse'nin Kordinarlarini ve Sol tek klik oldugunu soyler.
            }

        }

        private void Screen_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (mouseGonderildi)
            {
                mouseGonderildi = false;
                Point mouseKordinat = new Point(e.X, e.Y);
                ser.Logger("Mouse Cift Tiklamasi Gonderildi = (" + e.X + "," + e.Y + ")");
                mouseGonderildi = ser.MouseKordinat(mouseKordinat, 3); // Sadece Mouse'nin Kordinarlarini ve Sol cift klik oldugunu soyler.
            }

        }

        /*      private void Screen_MouseDown(object sender, MouseEventArgs e) 
              {
                  if (mouseGonderildi)
                  {
                      mouseGonderildi = false;
                      Point mouseKordinat = new Point(e.X, e.Y);
                      mouseGonderildi = ser.MouseKordinat(mouseKordinat, 4); // Sadece Mouse'nin Kordinarlarini ve Sol cift klik oldugunu soyler.
                  }
              }*/

        #endregion
        #region Klavye
        //klavyeye basılma esnasındaki ilk hamlede alınan tuş bilgisi
        public void MyKeyDown(object sender, KeyEventArgs e)
        {
        //    ser.KlavyeTusuAl(Convert.ToChar(e.KeyValue), 1); // basma icin 1
        }

        //klavyedeki basıli halden kaldırılan tuş bilgisi
        public void myKeyPressed(object sender, KeyPressEventArgs e)
        {
            ;
        }

        //klavyedeki basıli halden kaldırılan tuş bilgisi
        public void MyKeyUp(object sender, KeyEventArgs e)
        {
         //   ser.KlavyeTusuAl(Convert.ToChar(e.KeyValue), 2); // cekme icin 2
        }
        #endregion
        
        private delegate void AsyncMethodCaller();

        private void dosyaGonderimi()
        {
            string dosyaYolu = folderChooser.FileName;
            if (dosyaYolu != "")
                ser.DosyaGonder(dosyaYolu);
            mnuDosyaGonderim.Enabled = true;
        }

        private void mnuDosyaGonderim_Click(object sender, EventArgs e)
        {
            mnuDosyaGonderim.Enabled = false;
            folderChooser.ShowDialog();
            result = caller.BeginInvoke(null, null);
        }

        private void TVServer_KeyDown(object sender, KeyEventArgs e)
        {
            ser.KlavyeTusuAl(Convert.ToChar(e.KeyValue), 1); // basma icin 1
        }

        private void TVServer_KeyUp(object sender, KeyEventArgs e)
        {
            ser.KlavyeTusuAl(Convert.ToChar(e.KeyValue), 2); // cekme icin 2
        }

        private void TVServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string message = "Programi Kapatmak istediginize emin misiniz ?";
            const string caption = "Kapat";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.No)
                e.Cancel = true;
            else
                ser.Kapat();
        }
    }
}
