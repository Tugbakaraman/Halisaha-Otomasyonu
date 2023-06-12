using HaliSahaOtomasyon.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HaliSahaOtomasyon
{
    public partial class FrmUyeEkle : Form
    {
        Context dbcon = new Context();
        public FrmUyeEkle()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void btnanasayfa_Click(object sender, EventArgs e)
        {
            FrmAnaSayfa frm = new FrmAnaSayfa();
            this.Hide();
            frm.ShowDialog();
        }
        private void btnekle_Click(object sender, EventArgs e)
        {
            try
            {
                String insertsql = "INSERT INTO UyeBilgileri(ad,soyad,yas,kimlikno,telefon,mail,uyeadi) VALUES('"+txtad.Text+ "','"+txtsoyad.Text+ "','"+txtyas.Text+"','"+txttc.Text+ "','"+txtnumara.Text+"','"+txtmail.Text+ "','"+txtuyead.Text+"')";
                SqlCommand command= new SqlCommand(insertsql,dbcon.GetCon());
                dbcon.OpenCon();
                command.ExecuteNonQuery();
                MessageBox.Show("Kayıt Oluşturuldu.");
                dbcon.CloseCon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            
        }

        private void btnresetle_Click(object sender, EventArgs e)
        {
            if (txtad.Text != string.Empty || txtsoyad.Text != string.Empty || txtyas.Text != string.Empty || txtnumara.Text != string.Empty || txtmail.Text != string.Empty || txttc.Text != string.Empty || txtuyead.Text != string.Empty)
            {
                txtad.Text = "";
                txtsoyad.Text = "";
                txtyas.Text = "";
                txtnumara.Text = "";
                txtmail.Text = "";
                txttc.Text = "";
                txtuyead.Text="";

            }
        }

        private void btnuyeekle_Click(object sender, EventArgs e)
        {
            FrmUyeEkle frme = new FrmUyeEkle();
            frme.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmRndIslemleri frmr = new FrmRndIslemleri();
            frmr.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmAnaSayfa frma = new FrmAnaSayfa();
            frma.Show();
            this.Hide();
        }

    }
}
