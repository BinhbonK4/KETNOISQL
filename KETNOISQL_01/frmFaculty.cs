using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;
using KETNOISQL_01.Model;

namespace KETNOISQL_01
{
    public partial class frmFaculty : Form
    {


        public frmFaculty()
        {
            InitializeComponent();
        }

        private void frmFaculty_Load(object sender, EventArgs e)
        {
            using (var context = new Model1())
            {
                var listFaculties = context.Faculties.ToList();
                BindGrid(listFaculties);
                txtTotalProfessor.Text = "0";
            }
        }

        private void BindGrid(List<Faculty> listFaculties)
        {
            dgvFaculty.Rows.Clear();
            foreach (var faculty in listFaculties)
            {
                int index = dgvFaculty.Rows.Add();
                dgvFaculty.Rows[index].Cells[0].Value = faculty.FacultyID;
                dgvFaculty.Rows[index].Cells[1].Value = faculty.FacultyName;
                dgvFaculty.Rows[index].Cells[2].Value = faculty.TotalProfessor;
            }
        }



        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtFacultyID.Text, out int facultyID))
            {
                MessageBox.Show("ID không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new Model1())
                {
                    var faculty = context.Faculties.FirstOrDefault(f => f.FacultyID == facultyID);
                    if (faculty != null)
                    {
                        DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            context.Faculties.Remove(faculty);
                            context.SaveChanges();
                            MessageBox.Show("Xóa khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            BindGrid(context.Faculties.ToList());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khoa cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa khoa: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ClearForm();
        }


        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void dgvFaculty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu người dùng click vào dòng hợp lệ (không phải tiêu đề)
            if (e.RowIndex >= 0)
            {
                // Lấy dữ liệu từ dòng được chọn
                DataGridViewRow selectedRow = dgvFaculty.Rows[e.RowIndex];

                // Đồng bộ thông tin vào các TextBox
                txtFacultyID.Text = selectedRow.Cells[0].Value.ToString();
                txtFacultyName.Text = selectedRow.Cells[1].Value.ToString();
                txtTotalProfessor.Text = selectedRow.Cells[2].Value?.ToString() ?? "0";

                // Cập nhật tổng số giáo sư (số lượng giáo sư)
                
            }
        }

        

        private void btnThemSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu nhập vào
            if (string.IsNullOrWhiteSpace(txtFacultyName.Text))
            {
                MessageBox.Show("Tên khoa không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new Model1())
                {
                    // Lấy mã khoa từ ô nhập liệu
                    int facultyID = int.Parse(txtFacultyID.Text);

                    // Tìm xem mã khoa có tồn tại trong cơ sở dữ liệu chưa
                    var existingFaculty = context.Faculties.FirstOrDefault(f => f.FacultyID == facultyID);

                    if (existingFaculty == null) // Mã khoa chưa tồn tại, thêm mới
                    {
                        var newFaculty = new Faculty
                        {
                            FacultyID = facultyID,
                            FacultyName = txtFacultyName.Text,
                            TotalProfessor = string.IsNullOrWhiteSpace(txtTotalProfessor.Text) ? (int?)null : int.Parse(txtTotalProfessor.Text)
                        };

                        context.Faculties.Add(newFaculty);
                        context.SaveChanges();
                        MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else // Mã khoa đã tồn tại, cập nhật
                    {
                        existingFaculty.FacultyName = txtFacultyName.Text;
                        existingFaculty.TotalProfessor = string.IsNullOrWhiteSpace(txtTotalProfessor.Text) ? (int?)null : int.Parse(txtTotalProfessor.Text);
                        context.SaveChanges();
                        MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Sau khi thêm hoặc sửa, cập nhật lại Grid và làm trống các ô nhập liệu
                    BindGrid(context.Faculties.ToList());
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm hoặc sửa khoa: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtFacultyID.Clear();
            txtFacultyName.Clear();
            txtTotalProfessor.Clear();
            txtFacultyID.Focus();
        }

        
    }
    }
