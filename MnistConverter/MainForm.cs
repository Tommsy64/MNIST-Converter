using System;
using System.Windows.Forms;

namespace MnistConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                MnistConverterEngine engine = new MnistConverterEngine();
                if (engine.ConvertMnist(path, checkBox1.Checked == false))
                {
                    progressBar1.Value = 100;
                    MessageBox.Show("Output can be found in " + path, "Conversion done!", MessageBoxButtons.OK);
                }
            }
        }
    }
}
