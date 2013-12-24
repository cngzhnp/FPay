using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SV_Client.Classes;
using System.Windows.Forms;
using System.Net;

namespace SV_Client
{
    public partial class SV_Client : Form
    {
        Client client = new Client();
        private string dosyaYolu = Application.StartupPath + "/screen.jpeg";
        private bool YeniDosyaGonder = true;
        string ipAdresi = "";

        private delegate void AsyncMethodCaller();
        IAsyncResult result;
        AsyncMethodCaller cagiran;

        public SV_Client()
        {
            InitializeComponent();
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;
            cagiran = new AsyncMethodCaller(DosyaGonder);
        }

        // Form Yuklenirken IPler Listesine tum IP Adresleri'ni ekler 
        private void SV_Client_Load(object sender, EventArgs e)
        {
            IPAddress[] IpAdresleri = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in IpAdresleri)
            {
                lstIPler.Items.Add(ip.ToString());
            }
        }

        // Ekran Paylasimi Butonuna basildiginda calisan fonksiyon
        private void BtnEkranPaylasimi_Click(object sender, EventArgs e)
        {
            // ekran cozunurlugunu al ve karsi tarafa gonder
            client.ekranCozunurlugu();
            // Timer calismaya baslar ve belirli araliklarla Timer_Tick methodu calisir.
            Timer.Start();
        }

        // Timer'in Her Tick'inde calisan method
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Eger daha timer'in onceki vurusundan kalan bir gonderim yoksa YeniDosyaGonder = true olur.
            if (YeniDosyaGonder == true)
            {
                try
                {
                    client.ScreenShotAl(dosyaYolu);
                    YeniScreenShotGonder();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ekranGonder Fonksiyonunda Sikinti Var..");
                }
            }
        }

        // Yeni bir Screenshot alindiginda ve gonderilmek istendiginde calisan fonksiyon
        private void YeniScreenShotGonder()
        {
            // gonderim yapilirken yeni bir ekran goruntusu alinmasini engellemek icin
            YeniDosyaGonder = false;
            // ekran goruntusunu gondermeye basla
            YeniDosyaGonder = client.EkranGonder(dosyaYolu);
        }

        // IP uzerine cift tiklandiginda calisan fonksiyon
        private void lstIPler_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                // hangi IP'ye tiklandigini bul
                ipAdresi = lstIPler.Items[lstIPler.SelectedIndex].ToString();
                // arada baglantiyi kur
                client.BaglantiKur(txtBaglanilacakIP.Text, ipAdresi, this);
           //     MessageBox.Show("Server'a Baglanti Tamamlandi..");
            }
            catch (Exception ex)
            {
                ;
            }
        }

        // DosyaGonderimi butonuna tiklandiginda calisir.
        private void btnDosyaGonderimi_Click(object sender, EventArgs e)
        {
            // DosyaGonderimi butonunu gizle
            btnDosyaGonderimi.Visible = false;
            // Dosya secme ekranini getir
            DosyaSecici.ShowDialog();
            // cagiran asenkron fonksiyonu calistir.
            result = cagiran.BeginInvoke(null, null);
        }

        // Dosya gonderimi icin olusturulmus asenkron fonksiyon
        private void DosyaGonder()
        {
            // dosyanin path'ini al
            string dosyaYolu = DosyaSecici.FileName;
            // gonder
            if (dosyaYolu != "")
                client.DosyaGonder(dosyaYolu);
            // gonderim sonunda butonu tekrar gorunur yap
            btnDosyaGonderimi.Visible = true;
        }

        // Form kapatilmaya calisildiginda cagirilacak method
        private void SV_Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string message = "Programi Kapatmak istediginize emin misiniz ?";
            const string caption = "Kapat";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.No)
                e.Cancel = true;
            else
            {
                client.Kapat();
            }
        }

    }
}
