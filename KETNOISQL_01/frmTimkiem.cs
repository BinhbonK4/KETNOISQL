using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KETNOISQL_01.Model;

namespace KETNOISQL_01
{
    public partial class frmTimkiem : Form
    {
        public frmTimkiem()
        {
            InitializeComponent();
        }

        private void frmTimkiem_Load(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1())
                {
                    // Tải danh sách khoa cho ComboBox
                    var faculties = context.Faculties.ToList();
                    faculties.Insert(0, new Faculty { FacultyID = 0, FacultyName = "Empty" });
                    cmbKhoa.DataSource = faculties;
                    cmbKhoa.DisplayMember = "FacultyName";
                    cmbKhoa.ValueMember = "FacultyID";

                    // Tải danh sách sinh viên hiển thị lên DataGridView
                    var students = context.Students.ToList();
                    BindGrid(students);
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1())
                {
                    var query = context.Students.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(txtMSSV.Text))
                    {
                        query = query.Where(s => s.StudentID.Contains(txtMSSV.Text));
                    }
                    if (!string.IsNullOrWhiteSpace(txtHoTen.Text))
                    {
                        query = query.Where(s => s.FullName.Contains(txtHoTen.Text));
                    }
                    if (cmbKhoa.SelectedValue != null && (int)cmbKhoa.SelectedValue != 0)
                    {
                        int selectedFacultyID = (int)cmbKhoa.SelectedValue;
                        query = query.Where(s => s.FacultyID == selectedFacultyID);
                    }

                    var result = query.ToList();

                    if (result.Any())
                    {
                        BindGrid(result); // Hiển thị kết quả lên DataGridView
                        TinhTongSoLuongSinhVien(result); // Cập nhật tổng số lượng sinh viên
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy kết quả nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvTiemKiem.Rows.Clear();
                        txtKetQua.Text = "0"; // Đặt về 0 khi không có kết quả
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvTiemKiem.Rows.Clear();
            foreach (var student in listStudents)
            {
                int index = dgvTiemKiem.Rows.Add();
                dgvTiemKiem.Rows[index].Cells[0].Value = student.StudentID;
                dgvTiemKiem.Rows[index].Cells[1].Value = student.FullName;
                dgvTiemKiem.Rows[index].Cells[2].Value = student.Faculty.FacultyName;
                dgvTiemKiem.Rows[index].Cells[3].Value = student.AverageScore;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            txtMSSV.Clear();
            txtHoTen.Clear();
            cmbKhoa.SelectedIndex = cmbKhoa.FindStringExact("Công Nghệ Thông Tin");
            dgvTiemKiem.Rows.Clear();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void TinhTongSoLuongSinhVien(List<Student> listStudents)
        {
            // Tính tổng số lượng sinh viên trong danh sách
            int totalStudents = listStudents.Count;

            // Hiển thị kết quả vào TextBox hoặc Label (vô hiệu hóa TextBox nếu cần)
            txtKetQua.Text = totalStudents.ToString();
        }
    }
}
