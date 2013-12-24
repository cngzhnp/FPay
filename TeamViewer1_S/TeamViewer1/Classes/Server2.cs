using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace TeamViewer1S.Classes
{
    class Server2
    {
        //Serverin clientleri dinleyecegi main sockets
        Socket socketSerEkran;
        byte[] buffer = new byte[20004];
        byte[] gelenResim;
        byte[] gelenDosya;
        byte[] alindi = new byte[4];
        int resimBoyutu = 0;
        int dosyaBoyutu = 0;
        int alinanResim;
        int alinanDosya;
        string dosyaAdi = "";           // Gelecek-Gidecek dosyanin adi
        bool teyitGeldi = false;
        bool teyitGeldiD = false;
        bool TeyitGonder = false;
        TVServer anaForm;
        Rectangle clientEkran = new Rectangle(0, 0, 0, 0);
        Rectangle serverEkran; // client ekraninin ve serverdaki acik pencerenin boyutlari (mouse kordinatini gondermek icin kullaniliyor)
        Point eskiKordinatlar = new Point(); // mouse imlecinin son kordinatlari
        Queue<string> basilan = new Queue<string>(); // basilan tuslari sirasiyla gondermek icin kullanilan queue

        public void BaglantiBaslat(TVServer tvServer)
        {
            try
            {
                anaForm = tvServer;
                //     progresBar = new Thread(new ThreadStart(ProgressBarDoldur));
                socketSerEkran = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress[] ipAdresleri = Dns.GetHostAddresses(Dns.GetHostName());
                IPAddress benimIpV4 = ipAdresleri[5];// Xpde 0,vista ve 7de 1. hatta bazen wirelessde 2; 
                Logger("Kullanilan IP4 Adresi= " + benimIpV4.ToString());
                IPEndPoint ipEnd = new IPEndPoint(benimIpV4, 6060);
                socketSerEkran.Bind(ipEnd); // 1. Socketi bagladim;
                Logger("IP Bind edildi.");
                socketSerEkran.Listen(1);
                socketSerEkran.BeginAccept(new AsyncCallback(ekranBaglantiGeldi), null);
            }
            catch (Exception ex)
            {
                Logger("Server'in kurulumunda hata olustu, kapatiliyor..");
                Logger("Olusan hata = " + ex.Message);
                Environment.Exit(0);
            }
        }

        void ekranBaglantiGeldi(IAsyncResult rst)
        {
            try
            {
                socketSerEkran = socketSerEkran.EndAccept(rst);
                MessageBox.Show("Baglandi");
                socketSerEkran.BeginReceive(buffer, 0, 20004, 0, new AsyncCallback(ekranMesajGeldi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "  Ekran Goruntusu Baglantisi ");
            }
        }

        void ekranMesajGeldi(IAsyncResult rst)
        {
            try
            {
                int gelenDataBoyutu = socketSerEkran.EndReceive(rst);

                if (string.Compare(stringYap(buffer, 4), "-cl-") == 0)
                {
                    anaForm.Close();
                }
                else if (string.Compare(stringYap(buffer, 4), "-sc-") == 0) // Client'ten ekran boyutu bilgileri server'a ulasti..
                {
                    clientEkranBoyutu(gelenDataBoyutu);
                }
                else if (string.Compare(stringYap(buffer, 4), "-sb-") == 0) // Client'ten gonderilen ekran goruntusunun boyutu alindi.
                {
                    try
                    {
                        resimBoyutu = Convert.ToInt32(stringYap(buffer, gelenDataBoyutu).Substring(4,stringYap(buffer, gelenDataBoyutu).IndexOf("END")-4));
                        gelenResim = new byte[resimBoyutu];
                        alinanResim = 0;
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show("Resim Boyutu Alma Hatasi.. ");
                        MessageBox.Show(stringYap(buffer, gelenDataBoyutu));
                    }
                }
                else if (string.Compare(stringYap(buffer, 4), "-sg-") == 0 && resimBoyutu != 0)
                {
                    try
                    {
                        for (int i = alinanResim; i < alinanResim + gelenDataBoyutu - 4; i++)
                        {
                            gelenResim[i] = buffer[(i - alinanResim) + 4];
                        }
                        alinanResim += gelenDataBoyutu - 4;
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message + "  veri al");
                        anaForm.Close();
                    }
                }

             //   else if (string.Compare(stringYap(buffer, 4), "-al-") == 0)
             //       teyitGeldi = true;

                else if (string.Compare(stringYap(buffer, 4), "-ds-") == 0)
                    MessageBox.Show("Dosya Alimi Tamamlandi..");
                else if (string.Compare(stringYap(buffer, 4), "-dh-") == 0)
                    MessageBox.Show("Dosya Aliminda Hata Olustu..");
                else if (string.Compare(stringYap(buffer, 4), "-db-") == 0) // Dosyanin boyutunu verir;
                {
                    dosyaBoyutu = Convert.ToInt32(stringYap(buffer, gelenDataBoyutu).Substring(4));
                    gelenDosya = new byte[dosyaBoyutu];
                    alinanDosya = 0;
                }
                else if (string.Compare(stringYap(buffer, 4), "-di-") == 0) // Dosyanin ismini verir;
                {
                    dosyaAdi = stringYap(buffer, gelenDataBoyutu).Substring(4);
                }
                else if (string.Compare(stringYap(buffer, 4), "-dv-") == 0 && dosyaBoyutu != 0) // Dosya verisini yazar;
                {
                    for (int i = alinanDosya; i < alinanDosya + (gelenDataBoyutu - 4); i++)
                    {
                        gelenDosya[i] = buffer[4 + i - alinanDosya];
                    }
                    alinanDosya += (gelenDataBoyutu - 4);
                    if (alinanDosya == dosyaBoyutu)  // Gelen dosya tamamlandiginda calisir.
                    {
                        string startupPath = Environment.CurrentDirectory;
                        File.WriteAllBytes(startupPath + "\\" + dosyaAdi, gelenDosya);
                        dosyaBoyutu = 0;
                        alinanDosya = 0;
                        gelenDosya = new byte[1];
                    }
                }

                if (alinanResim == resimBoyutu && alinanResim != 0)
                {
                    try
                    {
                        string startupPath = Application.StartupPath;
                        File.WriteAllBytes(startupPath + "\\screen.jpg", gelenResim);
                        anaForm.Screen.ImageLocation = startupPath + "\\screen.jpg";
                        resimBoyutu = 0;
                        alinanResim = 0;
                    }
                    catch (Exception Ex)
                    {
                        // MessageBox.Show("Alinan veri == Dosya boyutu"  + "  Server MesajGeldi..");
                        resimBoyutu = 0;
                        alinanResim = 0;
                    }
                }

                temizle();
                socketSerEkran.BeginReceive(buffer, 0, 20004, 0, new AsyncCallback(ekranMesajGeldi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " 3 ");
            }


        }

        void dosyaMesajGeldi(IAsyncResult rst)
        {
            //try
            //{
            //    int gelenDataBoyutu = socketSerDosya.EndReceive(rst);
            //    if (string.Compare(stringYap(buffer3, 4), "-al-") == 0)
            //        teyitGeldiD = true;
            //    if (string.Compare(stringYap(buffer3, 4), "-ds-") == 0)
            //        MessageBox.Show("Dosya Alimi Tamamlandi..");
            //    if (string.Compare(stringYap(buffer3, 4), "-dh-") == 0)
            //        MessageBox.Show("Dosya Aliminda Hata Olustu..");
            //    else if (string.Compare(stringYap(buffer3, 4), "-db-") == 0) // Dosyanin boyutunu verir;
            //    {
            //        dosyaBoyutu = Convert.ToInt32(stringYap(buffer3, gelenDataBoyutu).Substring(4));
            //        socketSerDosya.Send(byteArrayYap("-al-"), 4, 0);
            //        gelenDosya = new byte[dosyaBoyutu];
            //        alinanDosya = 0;
            //    }
            //    else if (string.Compare(stringYap(buffer3, 4), "-di-") == 0) // Dosyanin ismini verir;
            //    {
            //        dosyaAdi = stringYap(buffer3, gelenDataBoyutu).Substring(4);
            //        socketSerDosya.Send(byteArrayYap("-al-"), 4, 0);
            //    }
            //    else if (string.Compare(stringYap(buffer3, 4), "-dv-") == 0 && dosyaBoyutu != 0) // Dosya verisini yazar;
            //    {
            //        for (int i = alinanDosya; i < alinanDosya + (gelenDataBoyutu - 4); i++)
            //        {
            //            gelenDosya[i] = buffer3[4 + i - alinanDosya];
            //        }
            //        socketSerDosya.Send(byteArrayYap("-al-"), 4, 0);
            //        alinanDosya += (gelenDataBoyutu - 4);
            //        if (alinanDosya == dosyaBoyutu)  // Gelen dosya tamamlandiginda calisir.
            //        {
            //            string startupPath = Environment.CurrentDirectory;
            //            File.WriteAllBytes(startupPath + "\\" + dosyaAdi, gelenDosya);
            //            dosyaBoyutu = 0;
            //            alinanDosya = 0;
            //            gelenDosya = new byte[1];
            //        }
            //    }
            //    socketSerDosya.BeginReceive(buffer3, 0, 20004, 0, new AsyncCallback(dosyaMesajGeldi), null);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Dosya Gonderiminde-Aliminda Hata");
            //    socketSerDosya.BeginReceive(buffer3, 0, 20004, 0, new AsyncCallback(dosyaMesajGeldi), null);
            //    dosyaBoyutu = 0;
            //    alinanDosya = 0;
            //    gelenDosya = new byte[1];
            //}
        }


        // ProgressBar doldurmak sikintili
        int dolanProgres;
        /*      public void ProgressBarDoldur()
              {
                  anaForm.proDosyaGonderimi.Value = dolanProgres;
                  progresBar.Abort();
              } */

        public void DosyaGonder(string dosyaYolu)
        {

            //try
            //{
            //    FileInfo Info = new FileInfo(dosyaYolu);
            //    byte[] dosyaVerisi = File.ReadAllBytes(dosyaYolu);
            //    byte[] dosyaVeriBolumu = new byte[20004];
            //    int mesajUzunlugu = 20000, toplamUzunluk = 0;
            //    socketSerDosya.Send(byteArrayYap("-db-" + Info.Length.ToString()), Info.Length.ToString().Length + 4, 0);
            //    teyitBekleD();
            //    // ProgresBas
            //    anaForm.proDosyaGonderimi.Visible = true;
            //    anaForm.proDosyaGonderimi.Minimum = 0;
            //    anaForm.proDosyaGonderimi.Maximum = 100;
            //    anaForm.proDosyaGonderimi.Value = 0;
            //    socketSerDosya.Send(byteArrayYap("-di-" + Info.Name), Info.Name.Length + 4, 0);
            //    teyitBekleD();
            //    Array.Copy(byteArrayYap("-dv-"), dosyaVeriBolumu, 4);
            //    while (true)
            //    {
            //        if (toplamUzunluk >= Info.Length)
            //            break;
            //        if (Convert.ToInt32(Info.Length) - toplamUzunluk < 20000)
            //        {
            //            mesajUzunlugu = Convert.ToInt32(Info.Length) - toplamUzunluk;
            //        }
            //        for (int i = toplamUzunluk; i < toplamUzunluk + mesajUzunlugu; i++)
            //            dosyaVeriBolumu[4 + i - toplamUzunluk] = dosyaVerisi[i];
            //        toplamUzunluk += mesajUzunlugu;
            //        dolanProgres = Convert.ToInt32((double)toplamUzunluk / Info.Length) * 100;
            //        socketSerDosya.Send(dosyaVeriBolumu, mesajUzunlugu + 4, 0);
            //        teyitBekleD();

            //    }
            //    anaForm.proDosyaGonderimi.Visible = false;
            //    MessageBox.Show("Dosya Gonderimi Tamamlandi..");
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Dosya Gonderim Hatasi");
            //}
        }
        public void KlavyeTusuAl(char tus, int olayi)
        {
            if (olayi == 1)
                basilan.Enqueue("-kl-" + tus);
            else
                basilan.Enqueue("+kl-" + tus);


            if (basilan.Count >= 1)
                klavyeTusuGonder();
        }
        void klavyeTusuGonder()
        {
            byte[] gonder = byteArrayYap(basilan.Dequeue());
            if (gonder.Length != socketSerEkran.Send(gonder, gonder.Length, 0))
                {
                MessageBox.Show("Failed to Send Keyboard push in klavyeTusuGonder");
                }
        }

        public bool MouseKordinat(Point mouseKordinat, int gondermeTipi)
        {
                int digerEkranKordinatiY = Convert.ToInt32((Convert.ToDouble(clientEkran.Height) / Convert.ToDouble(serverEkran.Height)) * mouseKordinat.Y);
                int digerEkranKordinatiX = Convert.ToInt32((Convert.ToDouble(clientEkran.Width) / Convert.ToDouble(serverEkran.Width)) * mouseKordinat.X);
                byte[] gonderilecek = new byte[100];
                if (gondermeTipi == 1)
                {
                    if ((eskiKordinatlar.X != mouseKordinat.X || eskiKordinatlar.Y != mouseKordinat.Y) && clientEkran.Width != 0)
                    {
                        eskiKordinatlar.X = mouseKordinat.X;
                        eskiKordinatlar.Y = mouseKordinat.Y;
                        gonderilecek = byteArrayYap("-mo-" + digerEkranKordinatiX + "," + digerEkranKordinatiY + ".");
                        if(gonderilecek.Length != socketSerEkran.Send(gonderilecek, gonderilecek.Length, 0))
                        {
                            MessageBox.Show("Failed to Send Mouse Coord in MouseKordinat");
                        }
                    }
                }
                else if (gondermeTipi == 2)
                {
                    gonderilecek = byteArrayYap("-mt-" + digerEkranKordinatiX + "," + digerEkranKordinatiY + ".");
                    socketSerEkran.Send(gonderilecek, gonderilecek.Length, 0);
                }
                else if (gondermeTipi == 3)
                {
                    gonderilecek = byteArrayYap("-md-" + digerEkranKordinatiX + "," + digerEkranKordinatiY + ".");
                    socketSerEkran.Send(gonderilecek, gonderilecek.Length, 0);
                }
            return true;
        }

        public void EkranBoyutu(Size size)
        {
            serverEkran = new Rectangle(new Point(0, 0), size);
        }
        void clientEkranBoyutu(int gelenDataBoyutu)
        {
            int virgulIndex = stringYap(buffer, gelenDataBoyutu).IndexOf(',');
            int height, width;

            height = Convert.ToInt32(stringYap(buffer, gelenDataBoyutu).Substring(4, virgulIndex - 4));

            width = Convert.ToInt32(stringYap(buffer, gelenDataBoyutu).Substring(virgulIndex + 1, (gelenDataBoyutu - virgulIndex) - 1));
            clientEkran = new Rectangle(0, 0, width, height);

        }


        public void Kapat()
        {
            try
            {
                socketSerEkran.Send(byteArrayYap("-cl-"));
                socketSerEkran.Close();
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

        string source = "ScreenViewer-Server";
        string log = "ServerApplication";

        public void Logger(string loginfo)
        {
        if (!EventLog.SourceExists(source))
            EventLog.CreateEventSource(source, log);
        EventLog.WriteEntry(source, loginfo);
        }
    }
}
