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
using System.IO;

namespace AlımSatımUygulaması
{
    public partial class Profil : Form
    {
        SqlDataAdapter da;
        DataSet ds;
        SqlConnection baglanti;
        SqlCommand komut;
        SqlDataReader reader;
        public Profil()
        {
            InitializeComponent();
        }
        void satislarimiDoldur()
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            da = new SqlDataAdapter("Select *From Uruns Where kullaniciAdi='" + Singleton.Instance.user.kullaniciAdi + "'", baglanti);
            ds = new DataSet();
            baglanti.Open();
            da.Fill(ds, "Uruns");
            dataGridView1.DataSource = ds.Tables["Uruns"];
            baglanti.Close();
        }
        void alisverislerimidoldur()
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            da = new SqlDataAdapter("Select *From Satis Where (saticiKullanici='" + Singleton.Instance.user.kullaniciAdi + "' or aliciKullanici='" + Singleton.Instance.user.kullaniciAdi + "') AND (satisTarihi BETWEEN '" + dateTimePicker1.Value.Date.Year + "-" + dateTimePicker1.Value.Date.Month + "-" + dateTimePicker1.Value.Date.Day + "' AND '" + dateTimePicker2.Value.Date.Year + "-" + dateTimePicker2.Value.Date.Month + "-" + dateTimePicker2.Value.Date.Day + "')", baglanti);
            ds = new DataSet();
            baglanti.Open();
            da.Fill(ds, "Satis");
            dataGridView2.DataSource = ds.Tables["Satis"];
            baglanti.Close();
        }
        private void profilBackBtn_Click(object sender, EventArgs e)
        {
            BilgiGiris blgGiris = new BilgiGiris();
            blgGiris.Show();
            this.Hide();
        }
        void profilBilgiDoldur()
        {
            baglanti = new SqlConnection("Data Source=DESKTOP-H0M8STK\\MSSQL;Initial Catalog=YazilimYapimiDatabase;Integrated Security=True");
            baglanti.Open();
            komut = new SqlCommand("SELECT * FROM Users WHERE kullaniciAdi='" + Singleton.Instance.user.kullaniciAdi + "'", baglanti);

            reader = komut.ExecuteReader();
            if (reader.Read())
            {
                lblKulAd.Text = reader["kullaniciAdi"].ToString();
                lblSifre.Text = reader["sifre"].ToString();
                lblAd.Text = reader["ad"].ToString();
                lblSoyad.Text = reader["soyad"].ToString();
                lblTc.Text = reader["tcKimlikNo"].ToString();
                lblTelefon.Text = reader["telefon"].ToString();
                lblEmail.Text = reader["email"].ToString();
                lblAdres.Text = reader["adres"].ToString();
                lblBakiye.Text = reader["bakiye"].ToString();
            }
        }
        private void Profil_Load(object sender, EventArgs e)
        {
            satislarimiDoldur();
            profilBilgiDoldur();

        }

        private void satisIptalBtn_Click(object sender, EventArgs e)
        {
            Product prdctt = new Product();
            prdctt.satistanKaldır(Convert.ToInt32(satisiKaldır_Label.Text));
            satislarimiDoldur();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;
            satisiKaldır_Label.Text = dataGridView1.Rows[e.RowIndex].Cells["urunId"].FormattedValue.ToString();
        }

        private void btnRaporAl_Click(object sender, EventArgs e)
        {
            alisverislerimidoldur();
            if (dataGridView2.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application xcelApp = new Microsoft.Office.Interop.Excel.Application();
                xcelApp.Application.Workbooks.Add(Type.Missing);
                for(int i = 1; i < dataGridView2.Columns.Count+1; i++)
                {
                    xcelApp.Cells[1, i] = dataGridView2.Columns[i - 1].HeaderText;
                }
                for(int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    for(int j = 0; j < dataGridView2.Columns.Count; j++)
                    {
                        if(dataGridView2.Rows[i].Cells[j].Value != null)
                        {
                            xcelApp.Cells[i + 2, j + 1] = dataGridView2.Rows[i].Cells[j].Value.ToString();
                        }
                    }
                }
                xcelApp.Columns.AutoFit();
                xcelApp.Visible = true;
            }
            
        }
    }
}
