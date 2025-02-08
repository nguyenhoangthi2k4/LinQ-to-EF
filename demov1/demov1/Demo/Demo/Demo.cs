using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class frmDemo : Form
    {
        QLSACHEntities db = new QLSACHEntities();
        public frmDemo()
        {
            InitializeComponent();
           
            this.LoadData();
            this.LoadCategory();
            this.cboCategory.SelectedIndex = -1;
            //this.AddBinding();
        }

        #region Methods
        //void AddBinding()
        //{
        //    this.txtName.DataBindings.Add(new Binding("Text", this.dgvBooks.DataSource, "Name", true, DataSourceUpdateMode.Never));
        //    this.txtPrice.DataBindings.Add(new Binding("Text", this.dgvBooks.DataSource, "Price", true, DataSourceUpdateMode.Never));
        //    this.cboCategory.DataBindings.Add(new Binding("Text", this.dgvBooks.DataSource, "Category", true, DataSourceUpdateMode.Never));
        //}

        bool CheckInput()
        {
            if (string.IsNullOrEmpty(this.txtName.Text))
                return false;

            if (string.IsNullOrEmpty(this.txtPrice.Text))
                return false;

            if (this.cboCategory.SelectedIndex == -1)
                return false;

            return true;
        }

        bool CheckUniqueBook()
        {
            // Cách 1:
            //BOOKS book = db.BOOKS.Where(p => p.NAME == this.txtName.Text).SingleOrDefault();
            //if (book != null)
            //    return false;

            // Cách 2:
            var result  = from b in db.BOOKS
                          where b.NAME == this.txtName.Text
                          select b;
            if(result.Count() > 0)
                return false;

            return true;
        }

        void LoadData()
        {
            // select tất cả dữ liệu từ bảng BOOKS
            // Cách 1:
            // var reuslt = from book in db.BOOKS select book;
            // this.dgvBooks.DataSource = result.ToList();

            // Cách 2:
            // this.dgvBooks.DataSource = db.BOOKS.ToList();

            // select cột tùy ý
            var result = from book in db.BOOKS
                         select new
                         {
                             ID = book.ID,
                             Name = book.NAME,
                             Price = book.PRICE,
                             Category = book.CATEGORY.NAME
                         };
            this.dgvBooks.DataSource = result.ToList();
            this.dgvBooks.Columns[0].Visible = false;
            this.dgvBooks.Columns[1].HeaderText = "Tên sách";
            this.dgvBooks.Columns[2].HeaderText = "Đơn giá";
            this.dgvBooks.Columns[3].HeaderText = "Thể loại";
        }

        void LoadCategory()
        {
            this.cboCategory.DataSource = db.CATEGORY.ToList();
            this.cboCategory.DisplayMember = "NAME";
            this.cboCategory.ValueMember = "ID";
        }

        void AddBook()
        {
            if (!this.CheckInput() || !this.CheckUniqueBook())
            {
                MessageBox.Show("Dữ liệu không hợp lệ");
                return;
            }

            BOOKS book = new BOOKS();
            book.NAME = this.txtName.Text;
            book.PRICE = float.Parse(this.txtPrice.Text);
            book.ID_CATEGORY = (int)this.cboCategory.SelectedValue;

            db.BOOKS.Add(book);
            db.SaveChanges();
        }

        void DeleteBook()
        {
            int id1 = int.Parse(this.dgvBooks.SelectedRows[0].Cells["ID"].Value.ToString());
            int id = int.Parse(this.dgvBooks.SelectedCells[0].OwningRow.Cells["ID"].Value.ToString());
            BOOKS book = db.BOOKS.Find(id);
            if (DialogResult.OK == MessageBox.Show("Bạn có chắc muốn xóa?", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question))
            {
                db.BOOKS.Remove(book);
                db.SaveChanges();
                MessageBox.Show("Xóa thành công");
            }
        }

        void UpdateBook()
        {
            int id = int.Parse(this.dgvBooks.SelectedCells[0].OwningRow.Cells["ID"].Value.ToString());

            if(!this.CheckInput())
            {
                MessageBox.Show("Dữ liệu không hợp lệ");
                return;
            }

            BOOKS book = db.BOOKS.Find(id);
            book.NAME = this.txtName.Text;
            book.PRICE = float.Parse(this.txtPrice.Text);
            book.ID_CATEGORY = (int)this.cboCategory.SelectedValue;

            db.SaveChanges();
        }

        #endregion

        #region Events
        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.AddBook();
            this.LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.DeleteBook();
            this.LoadData();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.UpdateBook();
            this.LoadData();
        }        

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(DialogResult.Yes == MessageBox.Show("Bạn có muốn thoát?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void dgvBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txtName.Text = this.dgvBooks.SelectedRows[0].Cells["Name"].Value.ToString();
            this.txtPrice.Text = this.dgvBooks.SelectedRows[0].Cells["Price"].Value.ToString();
            this.cboCategory.Text = this.dgvBooks.SelectedRows[0].Cells["Category"].Value.ToString();
        }
        #endregion

    }
}
