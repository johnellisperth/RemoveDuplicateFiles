using RemoveDuplicateFiles.ViewModel;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RemoveDuplicateFiles
{
    public partial class FormMain : Form
    {
        IDuplicateFilesViewModel DuplicateFileSearcher_;
        public FormMain(IDuplicateFilesViewModel duplicateFileSearcher)
        {
            InitializeComponent();
            DuplicateFileSearcher_ = duplicateFileSearcher;
        }

        private void buttonFolderSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DuplicateFileSearcher_.SelectedFolderPath = folderBrowserDialog1.SelectedPath;
                textBoxSelectedFolderPath.Text = DuplicateFileSearcher_.SelectedFolderPath;
            }
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            string folder = textBoxSelectedFolderPath.Text;
            var results= DuplicateFileSearcher_.SearchFolderForDuplicates();

            //translate and bind
            DataTable dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            results
                .ToList()
                .ForEach(kvp => dt.Rows.Add(new object[] { kvp.Key, string.Join(',', kvp.Value) }));

            //allows you to add uniqueness constraint to the key column :-)
            dt.Constraints.Add("keyconstraint", dt.Columns[0], true);


            dataGridView1.DataSource = dt;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DuplicateFileSearcher_.RemoveAllDuplicates();
        }
    }
}
