using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brickdoku
{
    public partial class Form1 : Form
    {
        Button[,] btn = new Button[9, 9]; // Create 2D array of buttons
        int selectedShape = -1;
        const int generatedSize = 20;

        class Shape
        {
            int size; // the number of blocks used to make the shape
            Button[] blocks; // the array of blocks
            int[] yOffsetData; // an array storing how the blocks should be offset realtive to the central block along the x-axis
            int[] xOffsetData; // an array storing how the blocks should be offset realtive to the central block along the y-axis

            public Shape()
            {
                size = 0;
            }

            public Shape(int size, int[] xOffsetData, int[] yOffsetData)
            {
                this.size = size;
                blocks = new Button[size];
                for (int i = 0; i < size; i++)
                {
                    blocks[i] = new Button();
                }
                this.xOffsetData = xOffsetData;
                this.yOffsetData = yOffsetData;
            }

            public Button getBlockAtIndex(int i)
            {
                return blocks[i];
            }

            public void setSize(int size)
            {
                this.size = size;
            }

            public int getSize()
            {
                return size;
            }

            public int getXOffset(int i)
            {
                return xOffsetData[i];
            }

            public int getYOffset(int i)
            {
                return yOffsetData[i];
            }
        }

        Shape[] shapes = new Shape[38]; // the array of all possible shapes
        void initialiseShapes() // shape objects are created and the array is populated
        {
            shapes[0] = new Shape(1, new int[] { 0 }, new int[] { 0 }); // 1x1 square
            shapes[1] = new Shape(2, new int[] { 0, generatedSize }, new int[] { 0, 0 }); // 1x2 horizontal
            shapes[2] = new Shape(2, new int[] { 0, 0 }, new int[] { 0, generatedSize }); // 1x2 vertical
            shapes[3] = new Shape(3, new int[] { 0, generatedSize, -(generatedSize) }, new int[] { 0, 0, 0 }); // 1x3 horizontal
            shapes[4] = new Shape(3, new int[] { 0, 0, 0 }, new int[] { 0, generatedSize, -(generatedSize) }); // 1x3 vertical
            shapes[5] = new Shape(3, new int[] { 0, 0, generatedSize }, new int[] { 0, -(generatedSize), 0 }); // 3 piece 'L' shape
            shapes[6] = new Shape(3, new int[] { 0, 0, -(generatedSize) }, new int[] { 0, generatedSize, 0 }); // 3 piece 'L' shape rotated 90 degrees clockwise
            shapes[7] = new Shape(3, new int[] { 0, 0, -(generatedSize) }, new int[] { 0, generatedSize, 0 }); // 3 piece 'L' shape rotated 180 degrees clockwise
            shapes[8] = new Shape(3, new int[] { 0, 0, -(generatedSize) }, new int[] { 0, -(generatedSize), 0 }); // 3 piece 'L' shape rotated 270 degrees clockwise
            shapes[9] = new Shape(4, new int[] { 0, generatedSize, 0, generatedSize }, new int[] { 0, 0, generatedSize, generatedSize }); // 2x2 square
            shapes[10] = new Shape(4, new int[] { 0, generatedSize, 2*(generatedSize), -(generatedSize) }, new int[] { 0, 0, 0, 0 }); // 1x4 horizontal
            shapes[11] = new Shape(4, new int[] { 0, 0, 0, 0 }, new int[] { 0, generatedSize, 2*(generatedSize), -(generatedSize) }); // 1x4 vertical
            shapes[12] = new Shape(4, new int[] { 0, generatedSize, 2*(generatedSize), -(generatedSize) }, new int[] { 0, -(generatedSize), -2*(generatedSize), generatedSize }); // 1x4 diagonal '/'
            shapes[13] = new Shape(4, new int[] { 0, generatedSize, 2* (generatedSize), -(generatedSize) }, new int[] { 0, generatedSize, 2*(generatedSize), -(generatedSize) }); // 1x4 diagonal '\'
            shapes[14] = new Shape(4, new int[] { 0, -(generatedSize), -(generatedSize), 0 }, new int[] { 0, 0, -(generatedSize), generatedSize }); // squiggly
            shapes[15] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, 0 }, new int[] { 0, 0, -(generatedSize), -(generatedSize) }); // squiggly rotated 90 degrees clockwise
            shapes[16] = new Shape(4, new int[] { 0, generatedSize, -(generatedSize), 0 }, new int[] { 0, 0, 0, generatedSize }); // 4 piece 'T'
            shapes[17] = new Shape(4, new int[] { 0, -(generatedSize), 0, 0 }, new int[] { 0, 0, -(generatedSize), generatedSize }); // 4 piece 'T' rotated 90 degrees clockwise
            shapes[18] = new Shape(4, new int[] { 0, generatedSize, -(generatedSize), 0 }, new int[] { 0, 0, 0, -(generatedSize) }); // 4 piece 'T' rotated 180 degrees clockwise
            shapes[19] = new Shape(4, new int[] { 0, generatedSize, 0, 0 }, new int[] { 0, 0, -(generatedSize), generatedSize }); // 4 piece 'T' rotated 270 degress clockwise
            shapes[20] = new Shape(4, new int[] { 0, 0, 0, generatedSize }, new int[] { 0, -(generatedSize), generatedSize, generatedSize }); // 4 piece 'L' shape
            shapes[21] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize) }, new int[] { 0, 0, 0, generatedSize }); // 4 piece 'L' shape rotated 90 degrees clockwise
            shapes[22] = new Shape(4, new int[] { 0, 0, 0, -(generatedSize) }, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize) }); // 4 piece 'L' shape rotated 180 degrees clockwise
            shapes[23] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, generatedSize }, new int[] { 0, 0, 0, -(generatedSize) }); // 4 piece 'L' shape rotated 270 degrees clockwis
            shapes[24] = new Shape(5, new int[] { 0, -(generatedSize), -2*(generatedSize), generatedSize, 2*(generatedSize) }, new int[] { 0, 0, 0, 0, 0 }); // 1x5 horizontal
            shapes[25] = new Shape(5, new int[] { 0, 0, 0, 0, 0 }, new int[] { 0, -(generatedSize), -2*(generatedSize), generatedSize, 2*(generatedSize) }); // 1x5 vertical
            shapes[26] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), generatedSize }, new int[] { 0, generatedSize, -2*(generatedSize), -(generatedSize), -(generatedSize) }); // 5 piece 'T' shape
            shapes[27] = new Shape(5, new int[] { 0, generatedSize, generatedSize -(generatedSize), generatedSize }, new int[] { 0, generatedSize, -(generatedSize), 0, 0 }); // 5 piece 'T' shape rotated 90 degrees clockwise
            shapes[28] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), generatedSize }, new int[] { 0, generatedSize, -(generatedSize), generatedSize, generatedSize }); // 5 piece 'T' shape rotated 180 degrees clockwise
            shapes[29] = new Shape(5, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, -(generatedSize) }, new int[] { 0, generatedSize, -(generatedSize), 0, 0 }); // 5 piece 'T' shape rotated 270 degrees clockwise
            shapes[30] = new Shape(5, new int[] { 0, 0, 0, generatedSize, 2*(generatedSize) }, new int[] { 0, -(generatedSize), -2*(generatedSize), 0, 0 }); // 5 piece 'L' shape
            shapes[31] = new Shape(5, new int[] { 0, 0, 0, generatedSize, 2*(generatedSize) }, new int[] { 0, generatedSize, 2*(generatedSize), 0, 0 }); // 5 piece 'L' shape rotated 90 degrees clockwise
            shapes[32] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -2*(generatedSize) }, new int[] { 0, generatedSize, 2*(generatedSize), 0, 0 }); // 5 piece 'L' shape rotated 180 degrees clockwise
            shapes[33] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -2*(generatedSize) }, new int[] { 0, -(generatedSize), -2*(generatedSize), 0, 0 }); // 5 piece 'L' shape rotated 270 degrees clockwise
            shapes[34] = new Shape(5, new int[] { 0, 0, 0, generatedSize, generatedSize }, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, generatedSize }); // 'C' shape
            shapes[35] = new Shape(5, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, generatedSize }, new int[] { 0, 0, generatedSize, 0, generatedSize }); // 'C' shape rotated 90 degrees
            shapes[36] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -(generatedSize) }, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, generatedSize }); // 'C' shape rotated 180 degrees
            shapes[37] = new Shape(5, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, generatedSize }, new int[] { 0, 0, -(generatedSize), 0, -(generatedSize) }); // 'C' shape rotated 270 degrees
        }

        void generateShape(int index, int x, int y) // creates a shape using the data in the shapes array
        {
            for (int i = 0; i < shapes[index].getSize(); i++) // creates a button for each block required to make the shape
            {
                shapes[index].getBlockAtIndex(i).SetBounds(x + shapes[index].getXOffset(i), y + shapes[index].getYOffset(i), generatedSize, generatedSize); //blocks are placed with respect to their offset data
                shapes[index].getBlockAtIndex(i).BackColor = Color.DarkRed;
                shapes[index].getBlockAtIndex(i).ForeColor = Color.DarkRed;
                shapes[index].getBlockAtIndex(i).Text = index.ToString(); // text is set to the index of the shape in the array for easy identification later
                shapes[index].getBlockAtIndex(i).Click += new EventHandler(this.clickGeneratedShape);
                Controls.Add(shapes[index].getBlockAtIndex(i));
                shapes[index].getBlockAtIndex(i).Enabled = false; // highlight is disabled
            }
        }

        void clickGeneratedShape(object sender, EventArgs e)
        {
            selectedShape = int.Parse(((Button)sender).Text); // on click, selected shape is set to the index 
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        public Form1()
        {
            InitializeComponent(); // Initialise the new item
            initialiseShapes();
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

            generateShape(23, 100, 100);

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


    }
}
