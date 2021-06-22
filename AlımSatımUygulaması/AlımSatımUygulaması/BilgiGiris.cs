using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace AlımSatımUygulaması
{
    public partial class BilgiGiris : Form
    {

        SqlDataAdapter da;
        DataSet ds;
        SqlConnection baglanti;
        SqlCommand komut;
        SqlDataReader reader;

        public BilgiGiris()
        {
            InitializeComponent();
        }
        public void griddoldur()
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            da = new SqlDataAdapter("Select *From Uruns Where kullaniciAdi!='" + Singleton.Instance.user.kullaniciAdi + "' AND OnayDurumu='"+true+"'", baglanti);
            ds = new DataSet();
            baglanti.Open();
            da.Fill(ds, "Uruns");
            dataGridView1.DataSource = ds.Tables["Uruns"];
            baglanti.Close();
        }
        private void BilgiGiris_Load(object sender, EventArgs e)
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            baglanti.Open();
            komut = new SqlCommand("Select * From Users Where kullaniciAdi='" + Singleton.Instance.user.kullaniciAdi + "'", baglanti);

            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                bakiye_Label.Text = reader["bakiye"].ToString();
            }
            baglanti.Close();
            if (Singleton.Instance.user.adminMi == false)
            {
                System.Windows.Forms.Button button, btn, btnn;
                button = paraOnayGorntBtn;
                btn = satisOnayGorntBtn;
                btnn = UrunOnayGrntBtn;
                button.Visible = false;
                btn.Visible = false;
                btnn.Visible = false;
            }

            griddoldur();
        }
        private void btnBelirlenenFiyattanUrunAl_Click(object sender, EventArgs e)
        {
            Product product = new Product();
            product = product.belirliKosullardaUrunAl(txtUrunAd.Text, Convert.ToInt32(txtMiktar.Text), Convert.ToInt32(txtBirimFiyat.Text));
            if (product.satinAlinacakUrunMu == true)
            {
                User user = new User();
                user.satisUcretiAl(Singleton.Instance.user.kullaniciAdi, Convert.ToInt32(txtMiktar.Text) * Convert.ToInt32(txtBirimFiyat.Text));
                user.satisUcretiVer(product.kullaniciAdi, Convert.ToInt32(txtMiktar.Text) * Convert.ToInt32(txtBirimFiyat.Text));
                Satis satis = new Satis();
                satis.satisIstekOlustur(product.urunId, product.kullaniciAdi, Singleton.Instance.user.kullaniciAdi, txtUrunAd.Text, Convert.ToInt32(txtMiktar.Text), Convert.ToInt32(txtBirimFiyat.Text), DateTime.Now, true);
                MessageBox.Show("Belirlenen Fiyattan Urun Satısta Oldugu Icin Direkt ALım Yapıldı");
            }
            else
            {
                product.urunEkle(Singleton.Instance.user.kullaniciAdi, txtUrunAd.Text,Convert.ToInt32(txtMiktar.Text), Convert.ToInt32(txtBirimFiyat.Text),true);
                MessageBox.Show("Belirlenen Fiyattan Urun Satısta Olmadıgı Icin Alisveris Beklemeye Alindi");
            }
        }
        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            Product product = new Product();
            product = product.EklemedenOnceAlanVarMi(txtUrunAd.Text, Convert.ToInt32(txtMiktar.Text), Convert.ToInt32(txtBirimFiyat.Text));
            if(product.satinAlinacakUrunMu == false)
            {
                product.urunEkle(Singleton.Instance.user.kullaniciAdi, txtUrunAd.Text, Convert.ToInt32(txtMiktar.Text), Convert.ToInt32(txtBirimFiyat.Text),false);
                MessageBox.Show("Ürün satis istegi oluşturuldu");
            }
            else
            {
                User user = new User();
                user.satisUcretiAl(product.kullaniciAdi, Convert.ToInt32(txtMiktar.Text) * Convert.ToInt32(txtBirimFiyat.Text));
                user.satisUcretiVer(Singleton.Instance.user.kullaniciAdi, Convert.ToInt32(txtMiktar.Text) * Convert.ToInt32(txtBirimFiyat.Text));
                Satis satis = new Satis();
                satis.satisIstekOlustur(product.urunId, Singleton.Instance.user.kullaniciAdi, product.kullaniciAdi, product.urunAdi, Convert.ToInt32(txtMiktar.Text), Convert.ToInt32(txtBirimFiyat.Text), DateTime.Now, true);
                MessageBox.Show("Urunun Alicisi Olduğu Icin Diretk Satıldı");
            }
            griddoldur();
        }

        private void btnParaEkle_Click(object sender, EventArgs e)
        {

            User usrr = new User();
            if (Singleton.Instance.user.adminMi == false)
            {
                usrr.paraEkle(Convert.ToInt32(txtPara.Text),cmbParaBirimi.Text);
                MessageBox.Show(txtPara.Text + " " + cmbParaBirimi.Text + " ekleme isteğiniz gönderilmiştir\nIstek onaylandığında hesabınıza TL karşılığı ile eklenecektir");
            }
            else
            {
                usrr.adminHesabinaParaEkle(Convert.ToInt32(txtPara.Text),cmbParaBirimi.Text);
                MessageBox.Show("Hesabınıza " + txtPara.Text + " " + cmbParaBirimi.Text + " TL karşılığı ile eklendi.");
                bakiye_Label.Text = Singleton.Instance.user.bakiye.ToString();
            }
        }

        private void btnSatinAl_Click(object sender, EventArgs e)
        {
            if (Singleton.Instance.user.adminMi == true)
            {
                User u = new User();
                Product p = new Product();
                Satis s = new Satis();
                if (Singleton.Instance.user.bakiye >= Convert.ToInt32(birimFiyatLabel.Text) * Convert.ToInt32(miktarLabel.Text) && Convert.ToInt32(stoktakiMiktar.Text) >= Convert.ToInt32(miktarLabel.Text))
                {

                    p.urunGuncelle(Convert.ToInt32(SatisurunIdLabel.Text), Convert.ToInt32(miktarLabel.Text));
                    p.urunKontrol(Convert.ToInt32(SatisurunIdLabel.Text));
                    u.satisUcretiAl(Singleton.Instance.user.kullaniciAdi, Convert.ToInt32(birimFiyatLabel.Text) * Convert.ToInt32(miktarLabel.Text));
                    u.satisUcretiVer(saticiKullaniciAdi.Text, Convert.ToInt32(birimFiyatLabel.Text) * Convert.ToInt32(miktarLabel.Text));
                    s.satisIstekOlustur(Convert.ToInt32(SatisurunIdLabel.Text), saticiKullaniciAdi.Text, Singleton.Instance.user.kullaniciAdi, urunAdiLabel.Text, Convert.ToInt32(miktarLabel.Text), Convert.ToInt32(birimFiyatLabel.Text), DateTime.Now, true);
                    bakiye_Label.Text = (Convert.ToInt32(bakiye_Label.Text) - (Convert.ToInt32(birimFiyatLabel.Text) * Convert.ToInt32(miktarLabel.Text))).ToString();
                    MessageBox.Show("Satın Alma Isleminiz Basariyla Gerceklesmistir");
                }              
                else MessageBox.Show("Bakiye Yetersiz veya Satistan Cok Alim Yapmaya Calistiniz");
            }
            else
            {
                if (Singleton.Instance.user.bakiye >= Convert.ToInt32(birimFiyatLabel.Text) * Convert.ToInt32(miktarLabel.Text) && Convert.ToInt32(stoktakiMiktar.Text) >= Convert.ToInt32(miktarLabel.Text))
                {

                    Satis st = new Satis();
                    Product pp = new Product();
                    st.satisIstekOlustur(Convert.ToInt32(SatisurunIdLabel.Text), saticiKullaniciAdi.Text, Singleton.Instance.user.kullaniciAdi, urunAdiLabel.Text, Convert.ToInt32(miktarLabel.Text), Convert.ToInt32(birimFiyatLabel.Text), DateTime.Now,false);
                    pp.urunAl(Convert.ToInt32(SatisurunIdLabel.Text), Convert.ToInt32(miktarLabel.Text));
                    MessageBox.Show("Satın Alma Isteginiz Basariyla Iletilmistir\nIleti Onaylandiginda Satis Gerceklesecektir");
                }              
                else MessageBox.Show("Bakiye Yetersiz veya Satistan Cok Alim Yapmaya Calistiniz");
            }
            griddoldur();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;
            SatisurunIdLabel.Text = dataGridView1.Rows[e.RowIndex].Cells["urunId"].FormattedValue.ToString();
            saticiKullaniciAdi.Text = dataGridView1.Rows[e.RowIndex].Cells["kullaniciAdi"].FormattedValue.ToString();
            urunAdiLabel.Text = dataGridView1.Rows[e.RowIndex].Cells["urunAdi"].FormattedValue.ToString();
            birimFiyatLabel.Text = dataGridView1.Rows[e.RowIndex].Cells["birimFiyat"].FormattedValue.ToString();
            stoktakiMiktar.Text = dataGridView1.Rows[e.RowIndex].Cells["miktar"].FormattedValue.ToString();
        }

        private void paraOnayGorntBtn_Click(object sender, EventArgs e)
        {
            ParaOnay paraOnay = new ParaOnay();
            paraOnay.Show();
            this.Hide();
        }

        private void satisOnayGorntBtn_Click(object sender, EventArgs e)
        {
            SatisOnay satisOnay = new SatisOnay();
            satisOnay.Show();
            this.Hide();
        }

        private void profilBtn_Click(object sender, EventArgs e)
        {
            Profil profil = new Profil();
            profil.Show();
            this.Hide();
        }

        private void btnMiktarArttir_Click(object sender, EventArgs e)
        {
            int mktr = Convert.ToInt32(miktarLabel.Text);
            mktr++;
            miktarLabel.Text = mktr.ToString();
        }

        private void btnMiktarAzalt_Click(object sender, EventArgs e)
        {
            int mkTr = Convert.ToInt32(miktarLabel.Text);
            if (mkTr > 1)
            {
                mkTr--;
                miktarLabel.Text = mkTr.ToString();
            }
            else MessageBox.Show("En az 1 adet bir şey alabilirsiniz");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Giris blgg = new Giris();
            blgg.Show();
            this.Hide();
        }       

        private void UrunOnayGrntBtn_Click(object sender, EventArgs e)
        {
            UrunOnay urunOnay = new UrunOnay();
            urunOnay.Show();
            this.Hide();
        }

        private void btnRaporAl_Click(object sender, EventArgs e)
        {

        }
    }
}

