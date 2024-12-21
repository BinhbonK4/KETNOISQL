using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KETNOISQL_01.Model;

namespace KETNOISQL_01
{
    public partial class Form1 : Form
    {

        private Model1 context;
        private Student selectedStudent;
        public Form1()
        {
            InitializeComponent();
        }


        private void UpdateStudentCount()
        {
            int totalStudents = context.Students.Count();
            txtTongSV.Text = totalStudents.ToString();
        }
        private void LoadData()
        {
            try
            {
                context = new Model1();
                List<Faculty> listFalcutys = context.Faculties.ToList();
                List<Student> listStudents = context.Students.ToList();
                FillFalcutyComboBox(listFalcutys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearForm()
        {
            txtMSSV.Clear();
            txtHoTen.Clear();
            cmbKhoa.SelectedIndex = -1;
            txtDiemTB.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            UpdateStudentCount();
        }
        private void FillFalcutyComboBox(List<Faculty> listFalcutys)
        {
            this.cmbKhoa.DataSource = listFalcutys;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvSinhVien.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dgvSinhVien.Rows.Add();
                dgvSinhVien.Rows[index].Cells[0].Value = item.StudentID;
                dgvSinhVien.Rows[index].Cells[1].Value = item.FullName;
                dgvSinhVien.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvSinhVien.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        private bool ValidateInputs()
        {
            // Kiểm tra các thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra mã số sinh viên: phải là chuỗi số có đúng 10 ký tự
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtMSSV.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Mã số sinh viên không hợp lệ. Vui lòng nhập 10 chữ số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra điểm trung bình: phải là số thập phân trong khoảng từ 0 đến 10
            if (!double.TryParse(txtDiemTB.Text, out double averageScore) || averageScore < 0 || averageScore > 10)
            {
                MessageBox.Show("Điểm trung bình sinh viên không hợp lệ. Vui lòng nhập giá trị từ 0 đến 10!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra tên sinh viên: phải là chữ, không chứa ký tự đặc biệt, từ 3 đến 100 ký tự
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtHoTen.Text, @"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯăâêôơư\s]{3,100}$"))
            {
                MessageBox.Show("Tên sinh viên không hợp lệ. Vui lòng nhập từ 3 đến 100 ký tự không chứa ký tự đặc biệt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
                {
                    Model1 context = new Model1();
                    List<Student> listStudents = context.Students.ToList();
                    if (listStudents.Any(s => s.StudentID == txtMSSV.Text))
                    {
                        MessageBox.Show("Mã số sinh viên đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var newStudent = new Student()
                    {
                        StudentID = txtMSSV.Text,
                        FullName = txtHoTen.Text,
                        FacultyID = Convert.ToInt32(cmbKhoa.SelectedValue),
                        AverageScore = double.Parse(txtDiemTB.Text)
                    };

                    context.Students.Add(newStudent);
                    context.SaveChanges();

                    MessageBox.Show("Thêm sinh viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadData();
                    UpdateStudentCount();

            }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Student> listStudents = context.Students.ToList();
                var student = listStudents.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
                if (student != null) 
                {
                    
                    student.FullName = txtHoTen.Text;
                    student.FacultyID = int.Parse(cmbKhoa.SelectedValue.ToString());
                    student.AverageScore = double.Parse(txtDiemTB.Text);

                    context.SaveChanges();

                    BindGrid(context.Students.ToList());
                    MessageBox.Show("Cập nhật sinh viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadData();
                    UpdateStudentCount();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Student> listStudents = context.Students.ToList();
                var student = listStudents.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
                if (student != null)
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xóa sinh viên", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        context.Students.Remove(student);
                        context.SaveChanges();


                        BindGrid(context.Students.ToList());
                        MessageBox.Show("Xóa sinh viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm();
                        LoadData();
                        UpdateStudentCount();
                    }
                }
                else
                {
                    MessageBox.Show("Mã sinh viên không tồn tại trong hệ thống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
                txtMSSV.Text = row.Cells[0].Value.ToString();
                txtHoTen.Text = row.Cells[1].Value.ToString();
                txtDiemTB.Text = row.Cells[3].Value.ToString();
                cmbKhoa.SelectedValue = context.Students.Find(txtMSSV.Text).FacultyID;


            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void quảnLýKhoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFaculty frmQuanLyKhoa = new frmFaculty();
            frmQuanLyKhoa.ShowDialog();

        }

        private void tìmKiếmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTimkiem frmTimKiem = new frmTimkiem();
            frmTimKiem.ShowDialog();
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        
    }
}
