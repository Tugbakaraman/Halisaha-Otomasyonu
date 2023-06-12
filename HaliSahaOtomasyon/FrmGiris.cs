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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HaliSahaOtomasyon
{
    public partial class FrmGiris : Form
    {
        Context dbcon = new Context();
        public FrmGiris()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            txtsifre.UseSystemPasswordChar = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
        }
        private void btngiris_Click(object sender, EventArgs e)
        {
            String sqllogin = "SELECT * FROM Admin WHERE kullaniciadi='"+txtkullaniciad.Text+"' AND sifre='"+txtsifre.Text+"'";
            DataTable table =new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sqllogin,dbcon.GetCon());
            adapter.Fill(table);
            if (table.Rows.Count>0)
            {
                FrmAnaSayfa frma = new FrmAnaSayfa();
                frma.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı ve şifrenizi doğru girdiğinizden emin olun.","Hatalı Giriş",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            dbcon.CloseCon();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtsifre.UseSystemPasswordChar = !checkBox1.Checked;
        }
    }
}
