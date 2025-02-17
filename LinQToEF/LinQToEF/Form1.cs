using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinQToEF
{
    public partial class Form1 : Form
    {
        QLSACHEntities db = new QLSACHEntities();
        public Form1()
        {
            InitializeComponent();
        }

        #region Methods
        public void LoadDataCategories()
        {
            // Cach 1: Query Syntax
            var result1 = from category in db.CATEGORIES select category;

            // Cach 2: Method Syntax
            var result2 = db.CATEGORIES.Select(category => category);

            this.cboCategories.DataSource = result1.ToList();
            this.cboCategories.DisplayMember = "Name";
            this.cboCategories.ValueMember = "ID";
        }
        public void LoadDataBooks()
        {
            var result = from book in db.BOOKS
                         select new
                         {
                             ID = book.ID,
                             Name = book.NAME,
                             Price = book.PRICE,
                             Category = book.CATEGORIES.NAME,
                         };
            this.dgvBooks.DataSource = result.ToList();
            this.dgvBooks.Columns["ID"].Visible = false;
            this.dgvBooks.Columns["Name"].HeaderText = "Tên sách";
            this.dgvBooks.Columns["Price"].HeaderText = "Giá";
            this.dgvBooks.Columns["Category"].HeaderText = "Thể loại";
        }

        public void ClearInput()
        {
            this.txtName.Text = "";
            this.txtPrice.Text = "";
            this.cboCategories.SelectedIndex = -1;
        }

        public bool CheckInput()
        {
            if (string.IsNullOrEmpty(this.txtName.Text))
                return false;
            if (string.IsNullOrEmpty(this.txtPrice.Text))
                return false;
            if (this.cboCategories.SelectedIndex == -1)
                return false;
            return true;
        }

        public bool CheckBookExists(string name)
        {
            // Cách 1:
            //BOOKS book = db.BOOKS.Where(p => p.NAME == name).SingleOrDefault();
            //if (book != null)
            //    return true;
            // return false;

            // Cách 2:
            var result = from book in db.BOOKS
                         where book.NAME == name
                         select book;

            if (result.Count() > 0)
                return true;
            return false;
        }

        public void AddBook()
        {
            if(!this.CheckInput() || this.CheckBookExists(this.txtName.Text))
            {
                MessageBox.Show("Dữ liệu nhập không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BOOKS book = new BOOKS();
            book.NAME = this.txtName.Text;
            book.PRICE = float.Parse(this.txtPrice.Text);
            book.ID_CATEGORY = (int)this.cboCategories.SelectedValue;
            db.BOOKS.Add(book);
            
            MessageBox.Show("Thêm sách thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            db.SaveChanges();
        }

        public void DeleteBook()
        {
            int id = (int)this.dgvBooks.SelectedRows[0].Cells["ID"].Value;
            BOOKS book = db.BOOKS.Find(id); // Tìm kiếm sách theo Primary Key
            db.BOOKS.Remove(book);
            db.SaveChanges();
        }

        public void EditBook()
        {
            int id = (int)this.dgvBooks.SelectedRows[0].Cells["ID"].Value;
            
            if(!this.CheckInput())
            {
                MessageBox.Show("Dữ liệu nhập không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            BOOKS book = db.BOOKS.Find(id);
            book.NAME = this.txtName.Text;
            book.PRICE = float.Parse(this.txtPrice.Text);
            book.ID_CATEGORY = (int)this.cboCategories.SelectedValue;

            db.SaveChanges();
        }
        #endregion

        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadDataCategories();
            this.LoadDataBooks();
            this.cboCategories.SelectedIndex = -1;
            this.txtName.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.AddBook();
            this.LoadDataBooks();
            this.ClearInput();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(DialogResult.Yes == MessageBox.Show("Bạn có muốn xóa sách này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                this.DeleteBook();

            this.LoadDataBooks();
            this.ClearInput();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.EditBook();
            this.LoadDataBooks();
            this.ClearInput();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           this.txtName.Text = this.dgvBooks.SelectedRows[0].Cells["Name"].Value.ToString();
            this.txtPrice.Text = this.dgvBooks.SelectedRows[0].Cells["Price"].Value.ToString();
            this.cboCategories.Text = this.dgvBooks.SelectedRows[0].Cells["Category"].Value.ToString();
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(DialogResult.Yes == MessageBox.Show("Bạn có muốn thoát chương trình không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
