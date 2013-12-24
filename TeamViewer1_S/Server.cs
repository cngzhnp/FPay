using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace TeamViewer1S.Classes
{
    class Server
    {
        //Serverin clientleri dinleyecegi main sockets
        Socket socketSerEkran;
        Socket socketSerFareKlavye;
        Socket socketSerDosya;
        Thread progresBar;
        byte[] buffer = new byte[20004];
        byte[] buffer2 = new byte[100];
        byte[] buffer3 = new byte[20004];
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
        TVServer anaForm;
        Rectangle clientEkran = new Rectangle(0,0,0,0);
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
                socketSerFareKlavye = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketSerDosya = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress[] ipAdresleri = Dns.GetHostAddresses(Dns.GetHostName());
                IPAddress benimIpV4 = ipAdresleri[5];// Xpde 0,vista ve 7de 1. hatta bazen wirelessde 2; 
                
                IPEndPoint ipEnd = new IPEndPoint(benimIpV4, 6600);
                socketSerEkran.Bind(ipEnd); // 1. Socketi bagladim;
                ipEnd = new IPEndPoint(benimIpV4, 6601);
                socketSerFareKlavye.Bind(ipEnd); // 2. Socketi bagladim;
                ipEnd = new IPEndPoint(benimIpV4, 6602);
                socketSerDosya.Bind(ipEnd); // 3. Socketi bagladim;
                
                socketSerEkran.Listen(1);
                socketSerEkran.BeginAccept(new AsyncCallback(ekranBaglantiGeldi), null);
                socketSerFareKlavye.Listen(1);
                socketSerFareKlavye.BeginAccept(new AsyncCallback(fareBaglantiGeldi), null);
                socketSerDosya.Listen(1);
                socketSerDosya.BeginAccept(new AsyncCallback(dosyaBaglantiGeldi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server'a Baglanamadi-Kapatiliyor..", "Hata");
                Environment.Exit(0);
            }
        }
              
        void ekranBaglantiGeldi(IAsyncResult rst)
        {
            try
            {
                socketSerEkran = socketSerEkran.EndAccept(rst);
                socketSerEkran.BeginReceive(buffer, 0, 20004, 0, new AsyncCallback(ekranMesajGeldi), null);
                alindi = byteArrayYap("-al-");
                socketSerEkran.Send(alindi,0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "  Ekran Goruntusu Baglantisi ");
            }
        }

        void fareBaglantiGeldi(IAsyncResult rst)
        {
            try
            {
                socketSerFareKlavye = socketSerFareKlavye.EndAccept(rst);
                socketSerFareKlavye.BeginReceive(buffer2, 0, 100, 0, new AsyncCallback(fareMesajGeldi), null);
                socketSerEkran.Send(alindi, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Fare-Klavye Baglantisi ");
            }
        }

        void dosyaBaglantiGeldi(IAsyncResult rst)
        {
            try
            {
                socketSerDosya = socketSerDosya.EndAccept(rst);
                socketSerDosya.BeginReceive(buffer3, 0, 20004, 0, new AsyncCallback(dosyaMesajGeldi), null);
                socketSerDosya.Send(byteArrayYap("-al-"), 4, 0);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Dosya Paylasimi Baglantisi ");
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
                        resimBoyutu = Convert.ToInt32(stringYap(buffer, gelenDataBoyutu).Substring(4));
                        gelenResim = new byte[resimBoyutu];
                        alinanResim = 0;
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show("dosya Boyutu " + "  Server MesajGeldi..");
                    }
                }
                else if (string.Compare(stringYap(buffer, 4), "-sg-") == 0 && resimBoyutu!=0)
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
                socketSerEkran.Send(alindi, 4, 0);
                socketSerEkran.BeginReceive(buffer, 0, 20004, 0, new AsyncCallback(ekranMesajGeldi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " 3 ");
            }


        }

        void fareMesajGeldi(IAsyncResult rst)
        {
            try
            {
                int gelenDataBoyutu = socketSerFareKlavye.EndReceive(rst);
                if (string.Compare(stringYap(buffer2, 4), "-al-") == 0)
                    teyitGeldi = true;
                socketSerFareKlavye.BeginReceive(buffer2, 0, 100, 0, new AsyncCallback(fareMesajGeldi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fare-Klavye'den Mesaj Geldi" + ex.Message);
            }
        }

        void dosyaMesajGeldi(IAsyncResult rst)
        {
            try
            {
                int gelenDataBoyutu = socketSerDosya.EndReceive(rst);
                if (string.Compare(stringYap(buffer3, 4), "-al-") == 0)
                    teyitGeldiD = true;
                if (string.Compare(stringYap(buffer3, 4), "-ds-") == 0)
                    MessageBox.Show("Dosya Alimi Tamamlandi..");
                if (string.Compare(stringYap(buffer3, 4), "-dh-") == 0)
                    MessageBox.Show("Dosya Aliminda Hata Olustu..");
                else if (string.Compare(stringYap(buffer3, 4), "-db-") == 0) // Dosyanin boyutunu verir;
                {
                    dosyaBoyutu = Convert.ToInt32(stringYap(buffer3, gelenDataBoyutu).Substring(4));
                    socketSerDosya.Send(byteArrayYap("-al-"), 4, 0);
                    gelenDosya = new byte[dosyaBoyutu];
                    alinanDosya = 0;
                }
                else if (string.Compare(stringYap(buffer3, 4), "-di-") == 0) // Dosyanin ismini verir;
                {
                    dosyaAdi = stringYap(buffer3, gelenDataBoyutu).Substring(4);
                    socketSerDosya.Send(byteArrayYap("-al-"), 4, 0);
                }
                else if (string.Compare(stringYap(buffer3, 4), "-dv-") == 0 && dosyaBoyutu != 0) // Dosya verisini yazar;
                {
                    for (int i = alinanDosya; i < alinanDosya + (gelenDataBoyutu - 4); i++)
                    {
                        gelenDosya[i] = buffer3[4 + i - alinanDosya];
                    }
                    socketSerDosya.Send(byteArrayYap("-al-"), 4, 0);
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
                socketSerDosya.BeginReceive(buffer3, 0, 20004, 0, new AsyncCallback(dosyaMesajGeldi), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dosya Gonderiminde-Aliminda Hata");
                socketSerDosya.BeginReceive(buffer3, 0, 20004, 0, new AsyncCallback(dosyaMesajGeldi), null);
                dosyaBoyutu = 0;
                alinanDosya = 0;
                gelenDosya = new byte[1];
            }
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

                try
                {
                    FileInfo Info = new FileInfo(dosyaYolu);
                    byte[] dosyaVerisi = File.ReadAllBytes(dosyaYolu);
                    byte[] dosyaVeriBolumu = new byte[20004];
                    int mesajUzunlugu = 20000, toplamUzunluk = 0;
                    socketSerDosya.Send(byteArrayYap("-db-" + Info.Length.ToString()), Info.Length.ToString().Length + 4, 0);
                    teyitBekleD();
                    // ProgresBas
                    anaForm.proDosyaGonderimi.Visible = true;
                    anaForm.proDosyaGonderimi.Minimum = 0;
                    anaForm.proDosyaGonderimi.Maximum = 100;
                    anaForm.proDosyaGonderimi.Value = 0;
                    socketSerDosya.Send(byteArrayYap("-di-" + Info.Name), Info.Name.Length + 4, 0);
                    teyitBekleD();
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
                        dolanProgres = Convert.ToInt32((double)toplamUzunluk/Info.Length)*100;
                        socketSerDosya.Send(dosyaVeriBolumu, mesajUzunlugu + 4, 0);
                        teyitBekleD();
                    
                    }
                    anaForm.proDosyaGonderimi.Visible = false;
                    MessageBox.Show("Dosya Gonderimi Tamamlandi..");
                }
                catch (Exception)
                {
                    MessageBox.Show("Dosya Gonderim Hatasi");
                }
            }
        public void KlavyeTusuAl(char tus,int olayi)
            {
                if (olayi == 1)
                    basilan.Enqueue("-kl-" + tus);
                else
                    basilan.Enqueue("+kl-" + tus);


                if (basilan.Count == 1)
                    klavyeTusuGonder();
            }
        void klavyeTusuGonder()
            {
             while (true)
                {
                    if (basilan.Count == 0)
                        break;
                    byte[] gonder = byteArrayYap(basilan.Dequeue());
                    if (socketSerFareKlavye.Connected == true)
                    {
                        socketSerFareKlavye.Send(gonder, gonder.Length, 0);
                        teyitBekle();
                    }
                }
            }

        public bool MouseKordinat(Point mouseKordinat,int gondermeTipi)
        {
            if (socketSerFareKlavye.Connected)
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
                        socketSerFareKlavye.Send(gonderilecek, gonderilecek.Length, 0);
                        //       teyitBekle();
                    }
                }
                else if (gondermeTipi == 2)
                {
                    gonderilecek = byteArrayYap("-mt-" + digerEkranKordinatiX + "," + digerEkranKordinatiY + ".");
                    socketSerFareKlavye.Send(gonderilecek, gonderilecek.Length, 0);
                    //      teyitBekle();
                }
                else if (gondermeTipi == 3)
                {
                    gonderilecek = byteArrayYap("-md-" + digerEkranKordinatiX + "," + digerEkranKordinatiY + ".");
                    socketSerFareKlavye.Send(gonderilecek, gonderilecek.Length, 0);
                    //       teyitBekle();
                }
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
        void teyitBekle()
        {
            Exception hata = new Exception("Baglanti Hatasi-Program kapaniyor..");
            DateTime gelmeZamani = DateTime.Now;

            try
            {
                while (true)
                    if (teyitGeldi)
                    {
                        if (DateTime.Now.Subtract(gelmeZamani).Seconds > 10)
                            throw hata;
                        teyitGeldi = false;
                        break;
                    }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        void teyitBekleD()
        {
            Exception hata = new Exception("Baglanti Hatasi-Program kapaniyor..");
            DateTime gelmeZamani = DateTime.Now;

            try
            {
                while (true)
                    if (teyitGeldiD)
                    {
                        if (DateTime.Now.Subtract(gelmeZamani).Seconds > 10)
                            throw hata;
                        teyitGeldiD = false;
                        break;
                    }

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
