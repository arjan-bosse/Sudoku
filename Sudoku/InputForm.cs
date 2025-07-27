using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        // Clear the textbox.
        public void Clear()
        {
            textBox1.Clear();
        }

        // Add lines to the textbox.
        public void WriteLines(string lines)
        {
            textBox1.AppendText(lines);
        }

        // Read lines from the textbox.
        public string ReadLines()
        {
            return textBox1.Text;
        }
    }
}
