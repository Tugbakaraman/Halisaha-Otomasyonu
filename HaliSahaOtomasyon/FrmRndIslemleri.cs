using HaliSahaOtomasyon.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HaliSahaOtomasyon
{
    public partial class FrmRndIslemleri : Form
    {
        Context dbcon = new Context();
        //private DataGridViewRow selectedRow;
        public FrmRndIslemleri()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void GetRandevuTable()
        {
            try
            {
                string query = "SELECT randevuid ,uyead, randevutarih, randevubslsaat, randevubtsaat, sahadurum, ucret FROM Randevu WHERE randevutarih >= @randevutarih";
                SqlCommand command = new SqlCommand(query, dbcon.GetCon());
                command.Parameters.AddWithValue("@randevutarih", DateTime.Now.Date);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
                AddButtonColumn();
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }
        private void UyelerComboBox()
        {
            string query = "SELECT uyeadi FROM UyeBilgileri";
            dbcon.OpenCon();
            SqlCommand command = new SqlCommand(query, dbcon.GetCon());
            SqlDataReader reader = command.ExecuteReader();

            List<string> uyeadi = new List<string>();
            uyeadi.Add(" ");

            while (reader.Read())
            {
                string memberName = reader["uyeadi"].ToString();
                uyeadi.Add(memberName);
            }
            combouye.DataSource = uyeadi;
            reader.Close();
            dbcon.CloseCon();
        }
        private bool RandevuControl(string tarih, string baslangicSaat, string bitisSaat)
        {
            try
            {
                string sql = "SELECT COUNT(*) FROM Randevu WHERE randevutarih = @randevutarih AND ((randevubslsaat <= @randevubslsaat AND randevubtsaat > @randevubslsaat) OR (randevubslsaat < @randevubtsaat AND randevubtsaat > @randevubtsaat))";
                SqlCommand command = new SqlCommand(sql, dbcon.GetCon());
                command.Parameters.AddWithValue("@randevutarih", tarih);
                command.Parameters.AddWithValue("@randevubslsaat", baslangicSaat);
                command.Parameters.AddWithValue("@randevubtsaat", bitisSaat);

                dbcon.OpenCon();
                command.ExecuteNonQuery();

                int count = (int)command.ExecuteScalar();

                dbcon.CloseCon();

                return count == 0;

            }catch(Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
                return false;
            }
        }
        private void UpdateSahaDurum()
        {
            try
            {
                string updateSql = "UPDATE Randevu SET sahadurum = @sahadurum WHERE randevutarih < @randevutarih OR (randevutarih = @randevutarih AND randevubslsaat < @randevubslsaat)";
                SqlCommand command = new SqlCommand(updateSql, dbcon.GetCon());
                command.Parameters.AddWithValue("@sahadurum", false);
                command.Parameters.AddWithValue("@randevutarih", DateTime.Now.Date);
                command.Parameters.AddWithValue("@randevubslsaat", DateTime.Now.TimeOfDay);

                dbcon.OpenCon();
                command.ExecuteNonQuery();
                dbcon.CloseCon();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }
        private void AddButtonColumn()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns.Cast<DataGridViewColumn>().ToArray())
            {
                if (column.Name == "Actions")
                    dataGridView1.Columns.Remove(column);
            }

            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.Name = "Actions";
            imageColumn.HeaderText = "Actions";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom; 

            string imagePath = @"C:\Repo\to-do-list.png";
            Image icon = Image.FromFile(imagePath);

            dataGridView1.Columns.Add(imageColumn);

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Actions"].Value = icon;
            }

        }
        private void HideEmptyRow()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.AllowUserToAddRows = false;
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;
                dataGridView1.Rows.Add();
                dataGridView1.Rows[0].Visible = false;
            }
        }
        public void reset()
        {
            combouye.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Today;
            txtbaslangic.Text = string.Empty;
            txtbitis.Text = string.Empty;
            txtucret.Text = string.Empty;
        }
        private void btnrandevuadd_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime selectedDateTime = dateTimePicker1.Value.Date + TimeSpan.Parse(txtbaslangic.Text);
                DateTime selectedEndDateTime = dateTimePicker1.Value.Date + TimeSpan.Parse(txtbitis.Text);

                if (selectedDateTime < DateTime.Now)
                {
                    MessageBox.Show("Seçilen tarih ve saatte randevu oluşturulamaz.");
                    return;
                }

                if (selectedEndDateTime <= selectedDateTime)
                {
                    MessageBox.Show("Randevu bitiş saati başlangıç saatinden önce olamaz.");
                    return;
                } 

                string selectedTarih = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string selectedSaat = txtbaslangic.Text;
                string selectedBSaat = txtbitis.Text;

                if (RandevuControl(selectedTarih, selectedSaat, selectedBSaat))
                {
                    string insertsql = "INSERT INTO Randevu(uyead, randevutarih, randevubslsaat, randevubtsaat, sahadurum, ucret) VALUES(@uyead, @randevutarih, @randevubslsaat, @randevubtsaat, @sahadurum,@ucret)";
                    SqlCommand command = new SqlCommand(insertsql, dbcon.GetCon());
                    command.Parameters.AddWithValue("@uyead", combouye.Text);
                    command.Parameters.AddWithValue("@randevutarih", dateTimePicker1.Value);
                    command.Parameters.AddWithValue("@randevubslsaat", txtbaslangic.Text);
                    command.Parameters.AddWithValue("@randevubtsaat", txtbitis.Text);
                    command.Parameters.AddWithValue("@sahadurum", true);
                    command.Parameters.AddWithValue("@ucret", txtucret.Text);
                    dbcon.OpenCon();
                    command.ExecuteNonQuery();
                    dbcon.CloseCon();
                    GetRandevuTable();
                    reset();
                    MessageBox.Show("Randevu Oluşturuldu.", "Kayıt Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Seçilen tarih ve saat aralığı dolu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                string randevuid = selectedRow.Cells["randevuid"].Value.ToString();
                combouye.Text = selectedRow.Cells["uyead"].Value.ToString();
                DateTime selectedDate = DateTime.Parse(selectedRow.Cells["randevutarih"].Value.ToString());
                dateTimePicker1.Value = selectedDate;
                txtbaslangic.Text = selectedRow.Cells["randevubslsaat"].Value.ToString();
                txtbitis.Text = selectedRow.Cells["randevubtsaat"].Value.ToString();
                string sahadurum = selectedRow.Cells["sahadurum"].Value.ToString();
                txtucret.Text = selectedRow.Cells["ucret"].Value.ToString();
            }
        }

        private void FrmRndİslemleri_Load(object sender, EventArgs e)
        {
            lbltarih.Text = DateTime.Today.ToShortDateString();
            UyelerComboBox();
            GetRandevuTable();
            UpdateSahaDurum();
            HideEmptyRow();
        }

        private void btnuyeekle_Click(object sender, EventArgs e)
        {
            FrmUyeEkle frmu = new FrmUyeEkle();
            frmu.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FrmRndIslemleri frmr = new FrmRndIslemleri();
            frmr.Show();
            this.Hide();
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void btnguncelle_Click(object sender, EventArgs e)
        {
            DateTime selectedDateTime = dateTimePicker1.Value.Date + TimeSpan.Parse(txtbaslangic.Text);

            if (selectedDateTime < DateTime.Now)
            {
                MessageBox.Show("Seçilen tarih ve saatte randevu oluşturulamaz.");
                return;
            }
            string tarih = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string baslangicSaat = txtbaslangic.Text;
            string bitisSaat = txtbitis.Text;

            if (RandevuControl(tarih, baslangicSaat, bitisSaat))
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    string randevuid = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                    if (string.IsNullOrEmpty(combouye.Text) || string.IsNullOrEmpty(txtbaslangic.Text) || string.IsNullOrEmpty(txtbitis.Text) || string.IsNullOrEmpty(txtucret.Text))
                    {
                        MessageBox.Show("Güncellemeden önce lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    try
                    {
                        string updateSql = "UPDATE Randevu SET uyead = @uyead, randevutarih =@randevutarih, randevubslsaat = @randevubslsaat, randevubtsaat = @randevubtsaat, sahadurum = @sahadurum, ucret = @ucret WHERE randevuid = @randevuid";
                        SqlCommand command = new SqlCommand(updateSql, dbcon.GetCon());
                        command.Parameters.AddWithValue("@uyead", combouye.Text);
                        command.Parameters.AddWithValue("@randevutarih", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@randevubslsaat", txtbaslangic.Text);
                        command.Parameters.AddWithValue("@randevubtsaat", txtbitis.Text);
                        command.Parameters.AddWithValue("@sahadurum", true);
                        command.Parameters.AddWithValue("@ucret", txtucret.Text);
                        command.Parameters.AddWithValue("@randevuid", randevuid);

                        dbcon.OpenCon();
                        command.ExecuteNonQuery();
                        dbcon.CloseCon();
                        GetRandevuTable();
                        reset();
                        MessageBox.Show("Randevu Güncellendi");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Randevu güncellenirken bir hata oluştu: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncelleme yapmak için bir randevu seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btniptal_Click(object sender, EventArgs e)
        {
            string randevuid = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            String sql = "DELETE FROM Randevu WHERE randevuid=" + randevuid + "";
            SqlCommand command = new SqlCommand(sql, dbcon.GetCon());
            dbcon.OpenCon();
            command.ExecuteNonQuery();
            MessageBox.Show("Randevu İptal edildi.", "Güncellenen Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dbcon.CloseCon();
            GetRandevuTable();
            reset();

        }
        private void button5_Click(object sender, EventArgs e)
        {
            FrmAnaSayfa frma = new FrmAnaSayfa();
            frma.Show();
            this.Hide();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Actions"].Index && e.RowIndex >= 0)
            {
                string randevuid = dataGridView1.Rows[e.RowIndex].Cells["randevuid"].Value.ToString();
                string memberName = dataGridView1.Rows[e.RowIndex].Cells["uyead"].Value.ToString();
                string query = "SELECT * FROM UyeBilgileri WHERE uyeadi = @uyeadi";
                SqlCommand command = new SqlCommand(query, dbcon.GetCon());
                command.Parameters.AddWithValue("@uyeadi", memberName);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    string memberInfo = $"Üye Id: {dt.Rows[0]["uyeid"]}\n" +
                                        $"Ad: {dt.Rows[0]["ad"]}\n" +
                                        $"Soyad: {dt.Rows[0]["soyad"]}\n" +
                                        $"Yaş: {dt.Rows[0]["yas"]}\n" +
                                        $"Kimlik No: {dt.Rows[0]["kimlikno"]}\n" +
                                        $"Telefon Numarası: {dt.Rows[0]["telefon"]}\n" +
                                        $"Mail: {dt.Rows[0]["mail"]}\n" +
                                        $"Üye Adı: {dt.Rows[0]["uyeadi"]}";

                    MessageBox.Show(memberInfo, "Üye Bilgileri", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Üye Bulunamadı.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.RowIndex >= 0)
            {
                // Clear any selection if the user clicked on a different row
                dataGridView1.ClearSelection();
            }
        }
    }
}
