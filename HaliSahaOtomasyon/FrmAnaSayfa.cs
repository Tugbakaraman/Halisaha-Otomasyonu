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

namespace HaliSahaOtomasyon
{
    public partial class FrmAnaSayfa : Form
    {
        Context dbcon = new Context();
        public FrmAnaSayfa()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnuyeekle_Click(object sender, EventArgs e)
        {
            FrmUyeEkle frmu = new FrmUyeEkle();
            frmu.Show();
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
        private void GetRandevuTable()
        {
            try
            {
                string query = "SELECT randevuid ,uyead, randevutarih, randevubslsaat, randevubtsaat, sahadurum, ucret FROM Randevu";
                SqlCommand command = new SqlCommand(query, dbcon.GetCon());
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }
        private void TarihAratmaList(DateTime selectedDate)
        {
            try
            {
                string query = "SELECT randevuid, uyead, randevutarih, randevubslsaat, randevubtsaat, sahadurum, ucret FROM Randevu WHERE randevutarih = @randevutarih";
                SqlCommand command = new SqlCommand(query, dbcon.GetCon());
                command.Parameters.AddWithValue("@randevutarih", selectedDate.Date);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string id = selectedRow.Cells[0].Value.ToString();
                string uyead = selectedRow.Cells[1].Value.ToString();
                string randevutarih = selectedRow.Cells[2].Value.ToString();
                string randevusaat = selectedRow.Cells[3].Value.ToString();
                string sahadurum = selectedRow.Cells[4].Value.ToString();
                string ucret = selectedRow.Cells[5].Value.ToString();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value.Date;
            TarihAratmaList(selectedDate);
            //dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            //DateTime selectedDate = dateTimePicker1.Value;
            //dataGridView1.Rows.Clear();
            //dataGridView1.Rows.Add(selectedDate.ToShortDateString());
        }

        private void FrmAnaSayfa_Load(object sender, EventArgs e)
        {
            GetRandevuTable();
        }
    }
}
