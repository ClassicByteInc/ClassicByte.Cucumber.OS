using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassicByte.App.MinShell
{
    public partial class MainForm : Form
    {
        public static StringBuilder Command { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("wdnm","dw",MessageBoxButtons.OK,MessageBoxIcon.Hand);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        public String ReadLine()
        {
            InputBox.ReadOnly = false;
            var command = new StringBuilder();
            int time = 0;
            while (true)
            {

                var theChar = InputBox.Text.ToCharArray()[time];
                if ((theChar=='\n')||time== InputBox.Text.ToCharArray().Length)
                {
                    InputBox.ReadOnly = true;
                    return command.ToString();
                }
                command.Append(theChar);
                time++;
            }
        }

        private void 操作ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ReadLine());

        }
    }
}
