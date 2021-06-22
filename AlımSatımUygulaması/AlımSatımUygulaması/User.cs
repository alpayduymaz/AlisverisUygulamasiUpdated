using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Xml;

namespace AlımSatımUygulaması
{

    public class User
    {
        SqlConnection baglanti;
        SqlCommand komut;
        SqlDataReader reader;
        public User() { }
        public string kullaniciAdi { get; set; }
        public string sifre { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public long tcKimlikNo { get; set; }
        public long telefon { get; set; }
        public string email { get; set; }
        public string adres { get; set; }
        public double bakiye { get; set; }
        public bool adminMi { get; set; }
        public int eklenecekMiktar { get; set; }
        public void kayıtOl(string KulAdi, string Sifre, string Ad, string Soyad, long Tc, long Telefon, string Email, string Adres)
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            
            string sorgu = "INSERT INTO Users(kullaniciAdi,sifre,ad,soyad,tcKimlikNo,telefon,email,adres,bakiye,adminMi,eklenecekMiktar) VALUES (@kullaniciAdi,@sifre,@ad,@soyad,@tcKimlikNo,@telefon,@email,@adres,@bakiye,@adminmi,@eklenecekMiktar)";
            komut = new SqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@kullaniciAdi", KulAdi);
            komut.Parameters.AddWithValue("@sifre", Sifre);
            komut.Parameters.AddWithValue("@ad", Ad);
            komut.Parameters.AddWithValue("@soyad", Soyad);
            komut.Parameters.AddWithValue("@tcKimlikNo", Tc);
            komut.Parameters.AddWithValue("@telefon", Telefon);
            komut.Parameters.AddWithValue("@email", Email);
            komut.Parameters.AddWithValue("@adres", Adres);
            komut.Parameters.AddWithValue("@bakiye", 0);
            komut.Parameters.AddWithValue("@adminmi", false);
            komut.Parameters.AddWithValue("@eklenecekMiktar", 0);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }
        public void adminHesabinaParaEkle(int eklenecekParaMiktari, string paraBirimi)
        {
            XmlTextReader rdr = new XmlTextReader("http://www.tcmb.gov.tr/kurlar/today.xml");
            XmlDocument myxml = new XmlDocument();
            myxml.Load(rdr);
            XmlNodeList doviz_satis = myxml.SelectNodes("/Tarih_Date/Currency/ForexSelling");
            if (paraBirimi == "Dolar(USD)")
            {
                eklenecekParaMiktari = eklenecekParaMiktari * Convert.ToInt32(Convert.ToDouble(doviz_satis.Item(0).InnerText.ToString())) / 10000;
            }
            else if(paraBirimi == "Euro")
            {
                eklenecekParaMiktari = eklenecekParaMiktari * Convert.ToInt32(Convert.ToDouble(doviz_satis.Item(3).InnerText.ToString())) / 10000;
            }
            else if(paraBirimi == "İngiliz Sterlini")
            {
                eklenecekParaMiktari = eklenecekParaMiktari * Convert.ToInt32(Convert.ToDouble(doviz_satis.Item(4).InnerText.ToString())) / 10000;
            }

            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            komut = new SqlCommand("Select *From Users Where kullaniciAdi='" + Singleton.Instance.user.kullaniciAdi + "'", baglanti);
            baglanti.Open();
            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                komut.Dispose();
                komut = new SqlCommand("Update Users set bakiye=bakiye+'" + eklenecekParaMiktari + "' where kullaniciAdi='" + Singleton.Instance.user.kullaniciAdi + "'", baglanti);
                reader.Close();
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
            Singleton.Instance.user.bakiye += eklenecekParaMiktari;
        }
        public void paraEkle(int eklenecekPara, string paraBirimi)
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            komut = new SqlCommand("Select *From Users Where kullaniciAdi='" + Singleton.Instance.user.kullaniciAdi + "'", baglanti);
            baglanti.Open();
            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                komut.Dispose();
                komut = new SqlCommand("Update Users set eklenecekMiktar='" + eklenecekPara + "',paraBirimi='" + paraBirimi + "' where kullaniciAdi='" + Singleton.Instance.user.kullaniciAdi + "'", baglanti);
                reader.Close();
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
        }
        public void paraOnay(string kulAdi,int eklenecekParaMiktari)
        {
            XmlTextReader rdr = new XmlTextReader("http://www.tcmb.gov.tr/kurlar/today.xml");
            XmlDocument myxml = new XmlDocument();
            myxml.Load(rdr);
            XmlNodeList doviz_satis = myxml.SelectNodes("/Tarih_Date/Currency/ForexSelling");

            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            komut = new SqlCommand("Select *From Users Where kullaniciAdi='" + kulAdi + "'", baglanti);
            baglanti.Open();
            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                if(reader["paraBirimi"].ToString() == "Dolar(USD)")
                {
                    eklenecekParaMiktari = eklenecekParaMiktari * Convert.ToInt32(Convert.ToDouble(doviz_satis.Item(0).InnerText.ToString())) / 10000;
                }
                else if(reader["paraBirimi"].ToString() == "Euro")
                {
                    eklenecekParaMiktari = eklenecekParaMiktari * Convert.ToInt32(Convert.ToDouble(doviz_satis.Item(3).InnerText.ToString())) / 10000;
                }
                else if(reader["paraBirimi"].ToString() == "İngiliz Sterlini")
                {
                    eklenecekParaMiktari = eklenecekParaMiktari * Convert.ToInt32(Convert.ToDouble(doviz_satis.Item(4).InnerText.ToString())) / 10000;
                }

                komut.Dispose();
                komut = new SqlCommand("Update Users set bakiye=bakiye+'" + eklenecekParaMiktari + "', eklenecekMiktar='"+0+"' where kullaniciAdi='" + kulAdi + "'", baglanti);                
                reader.Close();
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
            
        }
        public void satisOnay(int urunid, int eksilecekmiktar)
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            komut = new SqlCommand("Select *From Uruns Where urunId='" + urunid + "'", baglanti);
            baglanti.Open();
            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                komut.Dispose();
                komut = new SqlCommand("Update Uruns set miktar=miktar-'" + eksilecekmiktar + "',satisbeklemeDurumu='"+false+"',eksilecekMiktar='"+0+"' where urunId = '" + urunid + "'", baglanti);
                reader.Close();
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
        }
        public void satisUcretiAl(string kullaniciAdi,int satisUcreti)
        {
            satisUcreti += satisUcreti / 100;
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            komut = new SqlCommand("Select *From Users Where kullaniciAdi='" + kullaniciAdi + "'", baglanti);
            baglanti.Open();
            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                komut.Dispose();
                komut = new SqlCommand("Update Users set bakiye=bakiye-'" + satisUcreti + "' where kullaniciAdi='" + kullaniciAdi + "'", baglanti);
                reader.Close();
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
        }
        public void satisUcretiVer(string kullaniciAdi, int satisUcreti)
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            komut = new SqlCommand("Select *From Users Where kullaniciAdi='" + kullaniciAdi + "'", baglanti);
            baglanti.Open();
            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                komut.Dispose();
                komut = new SqlCommand("Update Users set bakiye=bakiye+'" + satisUcreti + "' where kullaniciAdi='" + kullaniciAdi + "'", baglanti);
                reader.Close();
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
        }
    }
}
