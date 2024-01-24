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
    public partial class Form1 : Form
    {
        Button[,] btn = new Button[9, 9]; // Create 2D array of buttons
        public Form1()
        {
            InitializeComponent(); // Initialise the new item
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {   
                    btn[x, y] = new Button();
                    btn[x, y].SetBounds(400 + (49 * x), 55 + (49 * y), 50, 50); 
                    btn[x, y].BackColor = Color.LightPink; 
                    // btn[x, y].Text = Convert.ToString((x + 1) + "," + (y + 1)); for debugging purposes
                    btn[x, y].Click += new EventHandler(this.btnEvent_Click);
                    Controls.Add(btn[x, y]);
                    //remove the hover over highlight by setting 
                    btn[x,y].Enabled = false;
                }
            }

            //Differentiate 3x3 grid sections with white colour

            for (int x = 3; x < 6; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    btn[x, y].BackColor = Color.White;
                }
            }

            for (int x = 0; x < 3; x++)
            {
                for (int y = 3; y < 6; y++)
                {
                    btn[x, y].BackColor = Color.White;
                }
            }

            for (int x = 6; x < 9; x++)
            {
                for (int y = 3; y < 6; y++)
                {
                    btn[x, y].BackColor = Color.White;
                }
            }


            for (int x = 3; x < 6; x++)
            {
                for (int y = 6; y < 9; y++)
                {
                    btn[x, y].BackColor = Color.White;
                }
            }

        }
        void btnEvent_Click(object sender, EventArgs e) {

            Console.WriteLine(((Button)sender).Text); // SAME handler asbefore
        }

        private void Form1_Load(object sender, EventArgs e) //REQUIRED
        {

        }                                                    


    }
}
