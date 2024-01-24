using Brickdoku.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brickdoku
{
    public partial class Brickudoku : Form
    {
        public Brickudoku()
        {
            InitializeComponent();
        }

        private void Brickudoku_Load(object sender, EventArgs e)
        {

        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            // when start button is clicked, hide all the items in the current menu screen
            // and display the grid game form
            btnExit.Hide();
            btnStart.Hide();
            this.lblTitle.Font = new System.Drawing.Font("OCR A Extended", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle.SetBounds(200, 30, 500, 60);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
        
        /**
         * Display dialog box with information about the game
         */
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Brickudoku game in C# by Adam Munro, Marylou Das Chagas E Silva and Laura Clark (c) 2024", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // display game rules somehow
        }
    }
}
