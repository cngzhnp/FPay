using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace SV_Client.Classes
{
    class Client
    {

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void keybd_event(Byte bVk, Byte bScan, int dwFlags, int dwExtraInfo);

        SV_Client anaForm;
        Socket socketClient;
        const int MAX = 20004;

        byte[] gelenDosya;              // Dosya Geldiginde tum dosyayi tutar.
        int alinanVeri;
        int ekranBoyutu = 0;            // ekran icin gonderilecek toplam byte miktari
        int dosyaBoyutu = 0;            // dosya icin Gelecek-Gonderilecek toplam byte miktari
        string dosyaAdi = "";           // Gelecek-Gidecek dosyanin adi
        bool call = true;

        byte[] buffer = new byte[MAX];  // verilerin konuldugu buffer 
        
        public void BaglantiKur(string ServerIP,string ClientIP, SV_Client SV_Client)
        {
            try
            {
                anaForm = SV_Client;
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress ipClient = IPAddress.Parse(ClientIP);
                IPAddress ipServer = IPAddress.Parse(ServerIP);

                IPEndPoint ipEnd = new IPEndPoint(ipClient, 0);
                socketClient.Bind(ipEnd);

                IPEndPoint ipEndServer = new IPEndPoint(ipServer, 6060); // Ekran Goruntusu icin port;
                socketClient.BeginConnect(ipEndServer, new AsyncCallback(SunucuyaBaglanildi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server'a Baglanamadi-Kapatiliyor..", "Hata");
                Environment.Exit(0);
            }
        }

        void SunucuyaBaglanildi(IAsyncResult rst)
        {
            
            socketClient.BeginReceive(buffer, 0, MAX, 0, new AsyncCallback(VeriGeldi), null);
        }
        IAsyncResult a;
        void VeriGeldi(IAsyncResult rst)
        {
            a = rst;
            try 
            {
                int gelenVeriBoyutu = socketClient.EndReceive(rst);

                if (string.Compare(stringYap(buffer, 4), "-cl-") == 0) // Server Kapatildi
                {
                    call = false;
                    anaForm.Close();
                }

                else if (string.Compare(stringYap(buffer, 4), "-db-") == 0) // Dosyanin boyutunu verir;
                {
                    dosyaBoyutu = Convert.ToInt32(stringYap(buffer, gelenVeriBoyutu).Substring(4));
                    gelenDosya = new byte[dosyaBoyutu];
                    alinanVeri = 0;
                }
                else if (string.Compare(stringYap(buffer, 4), "-di-") == 0) // Dosyanin ismini verir;
                {
                    dosyaAdi = stringYap(buffer, gelenVeriBoyutu).Substring(4);
                }
                else if (string.Compare(stringYap(buffer, 4), "-dv-") == 0 && dosyaBoyutu != 0) // Dosya verisini yazar;
                {
                    for (int i = alinanVeri; i < alinanVeri + (gelenVeriBoyutu - 4); i++)
                    {
                        gelenDosya[i] = buffer[4 + i - alinanVeri];
                    }
                    alinanVeri += (gelenVeriBoyutu - 4);
                    if (alinanVeri == dosyaBoyutu)  // Gelen dosya tamamlandiginda calisir.
                    {
                        string startupPath = Environment.CurrentDirectory;
                        File.WriteAllBytes(startupPath + "\\" + dosyaAdi, gelenDosya);
                        dosyaBoyutu = 0;
                        alinanVeri = 0;
                    }
                }

                else if (string.Compare(stringYap(buffer, 4), "-mo-") == 0)
                {
                    mouseGotur(gelenVeriBoyutu, buffer, 1); // Sadece Mouse'u hareket ettirir..
                }

                else if (string.Compare(stringYap(buffer, 4), "-mt-") == 0)
                {
                    mouseGotur(gelenVeriBoyutu, buffer, 2);// Uzerinde olunan noktaya sol tek tiklar..
                }
                else if (string.Compare(stringYap(buffer, 4), "-md-") == 0)
                {
                    mouseGotur(gelenVeriBoyutu, buffer, 3);// uzerinde olunan noktaya cift tiklar..
                }
                else if (string.Compare(stringYap(buffer, 4), "-kl-") == 0)
                {
                    tusaBasildi(Convert.ToChar(buffer[4]), 1);
                }
                else if (string.Compare(stringYap(buffer, 4), "+kl-") == 0)
                {
                    tusaBasildi(Convert.ToChar(buffer[4]), 2);
                }

                // Alma islemleri tamamlandi..
                temizle();
                if(call)
                    socketClient.BeginReceive(buffer, 0, MAX, 0, new AsyncCallback(VeriGeldi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekran Girisi Hatasi... \n" + ex.Message);
            }
        }


        public bool EkranGonder(string fileName)
        {
            try
            {
                FileInfo Info = new FileInfo(fileName);
                byte[] fileData = File.ReadAllBytes(fileName);
                byte[] fileDataPart;
                int mesajUzunlugu = 20000, toplamUzunluk = 0;
                
                byte[] fileSize = byteArrayYap("-sb-" + Info.Length + "END");
                ekranBoyutu = Convert.ToInt32(Info.Length);
                socketClient.Send(fileSize, fileSize.Length, 0);

                for (int i = 0; i < (ekranBoyutu / 20000) + (ekranBoyutu % 20000 == 0 ? 0 : 1); i++)
                {
                    if (Convert.ToInt32(Info.Length) - toplamUzunluk < 20000)
                    {
                        mesajUzunlugu = Convert.ToInt32(Info.Length) - toplamUzunluk;
                    }
                    fileDataPart = new byte[mesajUzunlugu + 4];
                    Array.Copy(byteArrayYap("-sg-"), 0, fileDataPart, 0, 4);
                    Array.Copy(fileData, toplamUzunluk, fileDataPart, 4, mesajUzunlugu);
                    toplamUzunluk += mesajUzunlugu;
                    socketClient.Send(fileDataPart, fileDataPart.Length,0);
                }
                return true;
            }
            catch (SocketException Ex)
            {
                MessageBox.Show("Baglanti Problemi");
                return false;
            }
        }

        public void DosyaGonder(string dosyaYolu)
        {

            try
            {
                FileInfo Info = new FileInfo(dosyaYolu);
                byte[] dosyaVerisi = File.ReadAllBytes(dosyaYolu);
                byte[] dosyaVeriBolumu = new byte[20004];
                int mesajUzunlugu = 20000, toplamUzunluk = 0;
        //        socketCliDosya.Send(byteArrayYap("-db-" + Info.Length.ToString()), Info.Length.ToString().Length + 4, 0);
        //        teyitBekleD();
                // ProgresBas
       //         socketCliDosya.Send(byteArrayYap("-di-" + Info.Name), Info.Name.Length + 4, 0);
       //         teyitBekleD();
                Array.Copy(byteArrayYap("-dv-"), dosyaVeriBolumu, 4);
                while (true)
                {
                    if (toplamUzunluk >= Info.Length)
                        break;
                    if (Convert.ToInt32(Info.Length) - toplamUzunluk < 20000)
                    {
                        mesajUzunlugu = Convert.ToInt32(Info.Length) - toplamUzunluk;
                    }
                    for (int i = toplamUzunluk; i < toplamUzunluk + mesajUzunlugu; i++)
                        dosyaVeriBolumu[4 + i - toplamUzunluk] = dosyaVerisi[i];
                    toplamUzunluk += mesajUzunlugu;
       //             socketCliDosya.Send(dosyaVeriBolumu, mesajUzunlugu + 4, 0);
       //             teyitBekleD();

                }
      //          socketCliDosya.Send(byteArrayYap("-ds-"), 4, 0);
            }
            catch (Exception)
            {
     //           socketCliDosya.Send(byteArrayYap("-dh-"), 4, 0);
            }
        }





        void mouseGotur(int bufferUzunlugu, byte[] tutBuffer, int tiklamaTipi)
        {
            Point yeniPozisyon = new Point();
            try
            {
                string gelenString = stringYap(tutBuffer, bufferUzunlugu);
                yeniPozisyon.X = Convert.ToInt32(gelenString.Substring(4, gelenString.IndexOf(',') - 4));
                yeniPozisyon.Y = Convert.ToInt32(gelenString.Substring(gelenString.IndexOf(',') + 1, (gelenString.IndexOf('.') - gelenString.IndexOf(',')) - 1));
                Cursor.Position = yeniPozisyon;
                if (tiklamaTipi == 2) // sol tek tik
                {
                    mouse_event(2, yeniPozisyon.X, yeniPozisyon.Y, 0, 0); //LeftDown
                    mouse_event(4, yeniPozisyon.X, yeniPozisyon.Y, 0, 0); //LeftUp
                }
                else if (tiklamaTipi == 3) // sol cift tik
                {
                    mouse_event(2, yeniPozisyon.X, yeniPozisyon.Y, 0, 0); //LeftDown
                    mouse_event(4, yeniPozisyon.X, yeniPozisyon.Y, 0, 0); //LeftUp
                    mouse_event(2, yeniPozisyon.X, yeniPozisyon.Y, 0, 0); //LeftDown
                    mouse_event(4, yeniPozisyon.X, yeniPozisyon.Y, 0, 0); //LeftUp
                }
            }
            catch (Exception eX)
            {

                MessageBox.Show(eX.Message + " mouseGotur");
            }
        }

        void tusaBasildi(char tutBuffer, int olay)
        {
            byte kul = Convert.ToByte(tutBuffer);
            if (olay == 1)
                keybd_event(kul, 0, 0, 0);
            else
                keybd_event(kul, 0, 2, 0);
        }

        public void ScreenShotAl(string dosyaYolu)
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bitmap.Save(dosyaYolu, ImageFormat.Jpeg);
            }
        }

        public void ekranCozunurlugu()
        {
            Exception Ex = new Exception("Baglanti yok");
            try
            {
                Rectangle ClientCozunurlugu = new Rectangle();
                ClientCozunurlugu = Screen.GetBounds(ClientCozunurlugu);
                byte[] gonderilecek = new byte[20];
                gonderilecek = byteArrayYap("-sc-" + ClientCozunurlugu.Height + "," + ClientCozunurlugu.Width);
                socketClient.Send(gonderilecek, gonderilecek.Length, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Kapat()
        {
            try
            {
                socketClient.Send(byteArrayYap("-cl-"));
                socketClient.EndConnect(a);
            }
            catch (Exception)
            {
                ;
            }
        }

        byte[] byteArrayYap(string veri)
        {
            return Encoding.GetEncoding(1254).GetBytes(veri);
        }
        string stringYap(byte[] dizi, int miktar)
        {
            return Encoding.GetEncoding(1254).GetString(dizi, 0, miktar);
        }
        void temizle()
        {
            Array.Clear(buffer, 0, buffer.Length);
        }
    }
}
