using Lab05.BUS;
using Lab05.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05
{
    public partial class Form2 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                var listFacultys = facultyService.GetAll();
                FillFalcultyCombobox(listFacultys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);
                var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                BindGrid(listStudents);
            }
        }
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[1].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[2].Value = item.FullName;
                if (item.Faculty != null)
                    dgvStudent.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[4].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[5].Value = item.Major.Name + "";
            }
        }
        private void FillMajorCombobox(List<Major> listMajor)
        {
            listMajor = listMajor.Where(m => m != null).ToList();

            // Gắn dữ liệu vào combobox
            cmbMajor.DataSource = listMajor;
            cmbMajor.DisplayMember = "Name"; // Hiển thị thuộc tính "Name" của chuyên ngành
            cmbMajor.ValueMember = "MajorID"; // Giá trị thực tế là "MajorID"
        }

        private void cmbMajor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvStudent.SelectedRows.Count == 0)
            {
                MessageBox.Show(" Vui lòng chọn một sinh viên");
            }
            if (cmbMajor.SelectedItem != null && cmbFaculty.SelectedItem != null)
            {
                Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
                Major selectedMajor = cmbMajor.SelectedItem as Major;
                if (selectedFaculty != null && selectedMajor != null)
                {
                    string studentID = dgvStudent.SelectedRows[0].Cells[1].Value.ToString();
                    if (!string.IsNullOrEmpty(studentID))
                    {
                        Student student = studentService.FindById(studentID);
                        if (student != null)
                        {
                            student.MajorID = selectedMajor.MajorID;
                            student.FacultyID = selectedFaculty.FacultyID;
                            studentService.InsertUpdate(student);
                            MessageBox.Show("Đăng ký chuyên nghành thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sinh viên");
                        }
                    }
                    else
                    {
                        MessageBox.Show("ID sinh viên không hợp lệ");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn cả khoa và chuyên nghành");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn chuyên nghành");
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvStudent.Rows[e.RowIndex];
                    string studentID = row.Cells[1].Value.ToString();
                    Student selectedStudent = studentService.FindById(studentID);
                    if (selectedStudent != null)
                    {
                        if (selectedStudent.FacultyID != null)
                        {
                            cmbFaculty.SelectedValue = selectedStudent.FacultyID;
                            var listMajors = majorService.GetAllByFaculty((int)selectedStudent.FacultyID);
                            FillMajorCombobox(listMajors);
                            if (selectedStudent.MajorID != null)
                            {
                                cmbMajor.SelectedValue = selectedStudent.MajorID;

                            }
                            else
                            {
                                cmbMajor.SelectedIndex = -1;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sinh vien khong co khoa");
                    }
                }
            }
        }
    }
}
