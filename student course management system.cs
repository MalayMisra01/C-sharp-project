using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StudentCourseManager
{
    public partial class Form1 : Form
    {
        private TextBox txtStudentID, txtStudentName, txtCourseCode, txtCourseTitle;
        private Button btnAddStudent, btnAddCourse, btnEnroll;
        private ComboBox cmbStudents, cmbCourses;
        private NumericUpDown numGrade;
        private ListBox lstEnrollments;
        private Label lblGPA;

        private List<Student> students = new List<Student>();
        private List<Course> courses = new List<Course>();

        public Form1()
        {
            InitializeComponent();
            RefreshDropdowns();
        }

        private void InitializeComponent()
        {
            this.Text = "Student Course Management System";
            this.ClientSize = new System.Drawing.Size(600, 400);

            txtStudentID = new TextBox { Left = 20, Top = 20, Width = 100, PlaceholderText = "Student ID" };
            txtStudentName = new TextBox { Left = 130, Top = 20, Width = 150, PlaceholderText = "Student Name" };
            btnAddStudent = new Button { Left = 300, Top = 20, Width = 120, Text = "Add Student" };
            btnAddStudent.Click += BtnAddStudent_Click;

            txtCourseCode = new TextBox { Left = 20, Top = 60, Width = 100, PlaceholderText = "Course Code" };
            txtCourseTitle = new TextBox { Left = 130, Top = 60, Width = 150, PlaceholderText = "Course Title" };
            btnAddCourse = new Button { Left = 300, Top = 60, Width = 120, Text = "Add Course" };
            btnAddCourse.Click += BtnAddCourse_Click;

            cmbStudents = new ComboBox { Left = 20, Top = 110, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStudents.SelectedIndexChanged += CmbStudents_SelectedIndexChanged;

            cmbCourses = new ComboBox { Left = 180, Top = 110, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            numGrade = new NumericUpDown { Left = 340, Top = 110, Width = 60, Minimum = 0, Maximum = 100 };
            btnEnroll = new Button { Left = 420, Top = 110, Width = 120, Text = "Enroll / Update" };
            btnEnroll.Click += BtnEnroll_Click;

            lstEnrollments = new ListBox { Left = 20, Top = 150, Width = 520, Height = 160 };
            lblGPA = new Label { Left = 20, Top = 320, Width = 200, Text = "GPA: 0.00" };

            Controls.AddRange(new Control[] {
                txtStudentID, txtStudentName, btnAddStudent,
                txtCourseCode, txtCourseTitle, btnAddCourse,
                cmbStudents, cmbCourses, numGrade, btnEnroll,
                lstEnrollments, lblGPA
            });
        }

        private void BtnAddStudent_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtStudentID.Text) && !string.IsNullOrWhiteSpace(txtStudentName.Text))
            {
                students.Add(new Student
                {
                    StudentID = txtStudentID.Text,
                    Name = txtStudentName.Text
                });
                txtStudentID.Clear();
                txtStudentName.Clear();
                RefreshDropdowns();
                MessageBox.Show("Student added.");
            }
        }

        private void BtnAddCourse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCourseCode.Text) && !string.IsNullOrWhiteSpace(txtCourseTitle.Text))
            {
                courses.Add(new Course
                {
                    CourseCode = txtCourseCode.Text,
                    Title = txtCourseTitle.Text
                });
                txtCourseCode.Clear();
                txtCourseTitle.Clear();
                RefreshDropdowns();
                MessageBox.Show("Course added.");
            }
        }

        private void BtnEnroll_Click(object sender, EventArgs e)
        {
            if (cmbStudents.SelectedItem is Student student && cmbCourses.SelectedItem is Course course)
            {
                var existing = student.Enrollments.FirstOrDefault(e => e.Course.CourseCode == course.CourseCode);
                if (existing != null)
                {
                    existing.Grade = (double)numGrade.Value;
                    MessageBox.Show("Grade updated.");
                }
                else
                {
                    student.Enrollments.Add(new Enrollment
                    {
                        Course = course,
                        Grade = (double)numGrade.Value
                    });
                    MessageBox.Show("Student enrolled.");
                }
                DisplayEnrollments(student);
            }
        }

        private void CmbStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStudents.SelectedItem is Student student)
                DisplayEnrollments(student);
        }

        private void DisplayEnrollments(Student student)
        {
            lstEnrollments.Items.Clear();
            foreach (var e in student.Enrollments)
                lstEnrollments.Items.Add($"{e.Course.Title}: Grade {e.Grade}");

            lblGPA.Text = $"GPA: {student.CalculateGPA():0.00}";
        }

        private void RefreshDropdowns()
        {
            cmbStudents.DataSource = null;
            cmbCourses.DataSource = null;
            cmbStudents.DataSource = students;
            cmbCourses.DataSource = courses;
        }
    }

    public class Student
    {
        public string StudentID { get; set; }
        public string Name { get; set; }
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public double CalculateGPA()
        {
            if (Enrollments.Count == 0) return 0;
            return Enrollments.Average(e => e.Grade);
        }

        public override string ToString() => $"{StudentID} - {Name}";
    }

    public class Course
    {
        public string CourseCode { get; set; }
        public string Title { get; set; }

        public override string ToString() => $"{CourseCode} - {Title}";
    }

    public class Enrollment
    {
        public Course Course { get; set; }
        public double Grade { get; set; }
    }

    // Entry point
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Form1());
        }
    }
}
