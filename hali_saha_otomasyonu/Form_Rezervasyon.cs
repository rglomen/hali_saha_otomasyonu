using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using System.IO;
using System.Data.OleDb;

namespace hali_saha_otomasyonu
{
    public partial class Form_Rezervasyon : Form
    {
        public Form_Rezervasyon()
        {
            InitializeComponent();
        }
        string bagcum = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=c:halisaha.mdb"; // halisaha.mdb veri tabanımızı tanımladık

        // global bağlantı değişkenlerimiz
        OleDbConnection bag;
        OleDbConnection con;
        OleDbDataAdapter da;
        OleDbCommand cmd;
        DataSet ds;
        OleDbDataReader dr;
        
        PictureBox p; // p değişkenine ekrandaki pictureboxları atadık yani seçtiğimiz picturebox p değişkeni olacak
        string secilen = ""; // seçilen pictureboxın namesi secilen değişkenince olacak

        private void Form_Rezervasyon_Load(object sender, EventArgs e)
        {

            bag = new OleDbConnection(bagcum);  // burada bağlantıyı oluşturduk ve açtık
            bag.Open();
            da = new OleDbDataAdapter("SELECT * FROM sahalar", bag); // sahalar tablosundaki tüm bilgileri çektik
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataRow dr = dt.NewRow(); // comboboxa saha seçinizi ekledik
            dr["saha_id"] = 0;
            dr["saha_adi"] = "Saha Seçiniz...";
            dt.Rows.InsertAt(dr, 0);

            comboBox_Saha.DataSource = dt; // comboBox_Saha ya sahanın bilgilerini attık saha_id si gizli saha_adi görünür oldu
            comboBox_Saha.ValueMember = "saha_id";   
            comboBox_Saha.DisplayMember = "saha_adi"; 
            bag.Close();
            

        }


        private void comboBox_Saha_SelectedIndexChanged(object sender, EventArgs e) // saha seçtiğimizde
        {
            foreach (Control item in panel2.Controls) // öncelikle bütün p lerin arkaplanını yeşil yaptık
            {
                if (item is PictureBox)
                {
                    item.BackColor = Color.Green;
                }
            }
            temizle(); // textboxları temizledik
            if (comboBox_Saha.SelectedIndex != 0) // comboboxta saha seçiniz seçili değilse
            {
                con = new OleDbConnection(bagcum); // veri tabanındaki rezervasyon_id si p lerden herhangi birinin adına eşitse o p nin arkaplanını kırmızı yapıyoruz
                con.Open();
                cmd = new OleDbCommand("select rezervasyon_id from rezervasyon", con);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    foreach (Control item in panel2.Controls)
                    {
                        if (item is PictureBox)
                        {
                            if (dr["rezervasyon_id"].ToString() == comboBox_Saha.SelectedValue.ToString() + item.Name.ToString())
                            {
                                item.BackColor = Color.Red;
                            }
                        }
                    }
                }
                con.Close();
            }
        }

        private void pbox_Click(object sender, EventArgs e)
        {
            temizle();
            p = sender as PictureBox;
            secilen = p.Name.ToString();
            button_rezervasyon.Text = secilen + " REZERVASYONU YAP";
            if (comboBox_Saha.SelectedIndex != 0)
            {
                label_hangigun.Text = p.Name.ToString().ToUpper();
                if (p.BackColor == Color.Red)
                {
                    button_rezervasyonguncelle.Enabled = true;
                    button_rezervasyonguncelle.BackColor = Color.LimeGreen;
                    button_rezervasyon.BackColor = Color.SeaGreen;
                    button_rezervasyon.Enabled = false;
                    con = new OleDbConnection(bagcum);
                    con.Open();
                    cmd = new OleDbCommand("select * from rezervasyon where rezervasyon_id='" + comboBox_Saha.SelectedValue.ToString() + p.Name.ToString() + "'", con);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        textBox_ucret.Text = dr["ucret"].ToString();
                        textBox_adsoyad.Text = dr["adsoyad"].ToString();
                        textBox_telefon.Text = dr["telefon"].ToString();
                    }
                    con.Close();
                }
                else
                {
                    button_rezervasyon.Enabled = true;
                    button_rezervasyon.BackColor = Color.LimeGreen;
                    button_rezervasyonguncelle.Enabled = false;
                    button_rezervasyonguncelle.BackColor = Color.SeaGreen;
                }
            }
        }

        private void pictureBox105_MouseHover(object sender, EventArgs e)
        {
            PictureBox p1 = sender as PictureBox;
            p1.BorderStyle = BorderStyle.Fixed3D;
        }

        private void pictureBox105_MouseLeave(object sender, EventArgs e)
        {
            PictureBox p1 = sender as PictureBox;
            p1.BorderStyle = BorderStyle.None;
        }

        void temizle()
        {
            textBox_adsoyad.Text = "";
            textBox_telefon.Text = "";
            textBox_ucret.Text = "";
        }

        private void button_rezervasyonguncelle_Click(object sender, EventArgs e)
        {
            if (textBox_adsoyad.Text != "" || textBox_telefon.Text != "" || textBox_ucret.Text != "")
            {
                con = new OleDbConnection(bagcum); con.Open();
                cmd = new OleDbCommand("update rezervasyon set adsoyad='" + textBox_adsoyad.Text + "',telefon='" + textBox_telefon.Text + "',ucret='" + textBox_ucret.Text + "' where rezervasyon_id='" + comboBox_Saha.SelectedValue.ToString() + p.Name.ToString() + "'", con);
                cmd.ExecuteNonQuery();
                p.BackColor = Color.Red;
                MessageBox.Show(secilen + " Rezervasyon Güncellendi");
                button_rezervasyonguncelle.Enabled = false;
                button_rezervasyonguncelle.BackColor = Color.SeaGreen;
                con.Close();
            }
            else
            {
                MessageBox.Show("Boş alan bırakmayınız. ");
            }
            temizle();
        }

        private void Form_Rezervasyon_Click(object sender, EventArgs e)
        {
            if (textBox_adsoyad.Text != "" || textBox_telefon.Text != "" || textBox_ucret.Text != "")
            {

                con = new OleDbConnection(bagcum); con.Open();
                cmd = new OleDbCommand("insert into rezervasyon(rezervasyon_id, adsoyad, telefon, ucret)values('" + comboBox_Saha.SelectedValue.ToString() + p.Name.ToString() + "','" + textBox_adsoyad.Text + "','" + textBox_telefon.Text + "','" + textBox_ucret.Text + "')", con);
                cmd.ExecuteNonQuery();
                p.BackColor = Color.Red;
                MessageBox.Show(secilen + " Rezervasyon Yapıldı");
                button_rezervasyon.Enabled = false;
                button_rezervasyon.BackColor = Color.SeaGreen;
                con.Close();
                
            }
            else
            {
                MessageBox.Show("Boş alan bırakmayınız. ");
            }
            temizle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Haftayı bitirip tüm kayıtları silmek iştiyor musunuz ? ", "Emin misiniz ? ", MessageBoxButtons.YesNo);
            if (cevap == DialogResult.Yes)
            {
                con = new OleDbConnection(bagcum);
                con.Open();
                cmd = new OleDbCommand("delete from rezervasyon", con);
                cmd.ExecuteNonQuery();
                con.Close();
                comboBox_Saha.SelectedIndex = 0;
            }
        }
        

        private void Form_Rezervasyon_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
