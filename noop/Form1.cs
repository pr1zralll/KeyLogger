using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace noop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Jook.Start();
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Jook.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
