using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brickdoku
{
    public partial class Form1 : Form
    {
        class Shape
        {
            int size;
            Button[] blocks;
            int[] yOffsetData;
            int[] xOffsetData;

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

        Shape[] shapes = new Shape[34];
        void initialiseShapes()
        {
            shapes[0] = new Shape(1, new int[] {0}, new int[] {0}); // 1x1 square
            shapes[1] = new Shape(2, new int[] { 0, 50}, new int[] { 0, 0}); // 1x2 horizontal
            shapes[2] = new Shape(2, new int[] { 0, 0 }, new int[] { 0, 50 }); // 1x2 vertical
            shapes[3] = new Shape(3, new int[] { 0, 50, -50 }, new int[] { 0, 0, 0 }); // 1x3 horizontal
            shapes[4] = new Shape(3, new int[] { 0, 0, 0 }, new int[] { 0, 50, -50 }); // 1x3 vertical
            shapes[5] = new Shape(3, new int[] { 0, 0, 50 }, new int[] { 0, -50, 0 }); // 3 piece 'L' shape
            shapes[6] = new Shape(3, new int[] { 0, 0, -50 }, new int[] { 0, 50, 0 }); // 3 piece 'L' shape rotated 90 degrees clockwise
            shapes[7] = new Shape(3, new int[] { 0, 0, -50 }, new int[] { 0, 50, 0 }); // 3 piece 'L' shape rotated 180 degrees clockwise
            shapes[8] = new Shape(3, new int[] { 0, 0, -50 }, new int[] { 0, -50, 0 }); // 3 piece 'L' shape rotated 270 degrees clockwise
            shapes[9] = new Shape(4, new int[] { 0, 50, 0, 50 }, new int[] {0, 0, 50, 50}); // 2x2 square
            shapes[10] = new Shape(4, new int[] { 0, 50, 100, -50 }, new int[] { 0, 0, 0, 0 }); // 1x4 horizontal
            shapes[11] = new Shape(4, new int[] { 0, 0, 0, 0 }, new int[] { 0, 50, 100, -50 }); // 1x4 vertical
            shapes[12] = new Shape(4, new int[] { 0, 50, 100, -50 }, new int[] { 0, -50, -100, 50 }); // 1x4 diagonal '/'
            shapes[13] = new Shape(4, new int[] { 0, 50, 100, -50 }, new int[] { 0, 50, 100, -50 }); // 1x4 diagonal '\'
            shapes[14] = new Shape(4, new int[] { 0, -50, -50, 0}, new int[] { 0, 0, -50, 50}); // squiggly
            shapes[15] = new Shape(4, new int[] { 0, -50, 50, 0 }, new int[] { 0, 0, -50, -50 }); // squiggly rotated 90 degrees clockwise
            shapes[16] = new Shape(4, new int[] { 0, 50, -50, 0 }, new int[] { 0, 0, 0, 50 }); // 4 piece 'T'
            shapes[17] = new Shape(4, new int[] { 0, -50, 0, 0 }, new int[] { 0, 0, -50, 50 }); // 4 piece 'T' rotated 90 degrees clockwise
            shapes[18] = new Shape(4, new int[] { 0, 50, -50, 0 }, new int[] { 0, 0, 0, -50 }); // 4 piece 'T' rotated 180 degrees clockwise
            shapes[19] = new Shape(4, new int[] { 0, 50, 0, 0 }, new int[] { 0, 0, -50, 50 }); // 4 piece 'T' rotated 270 degress clockwise
            shapes[20] = new Shape(4, new int[] { 0, 0, 0, 50 }, new int[] { 0, -50, 50, 50 }); // 4 piece 'L' shape
            shapes[21] = new Shape(4, new int[] { 0, -50, 50, -50 }, new int[] { 0, 0, 0, 50 }); // 4 piece 'L' shape rotated 90 degrees clockwise
            shapes[22] = new Shape(4, new int[] { 0, 0, 0, -50 }, new int[] { 0, -50, 50, -50 }); // 4 piece 'L' shape rotated 180 degrees clockwise
            shapes[23] = new Shape(4, new int[] { 0, -50, 50, 50 }, new int[] { 0, 0, 0, -50 }); // 4 piece 'L' shape rotated 270 degrees clockwis
            shapes[24] = new Shape(5, new int[] { 0, -50, -100, 50, 100 }, new int[] { 0, 0, 0, 0, 0 }); // 1x5 horizontal
            shapes[25] = new Shape(5, new int[] { 0, 0, 0, 0, 0 }, new int[] { 0, -50, -100, 50, 100 }); // 1x5 vertical
            shapes[26] = new Shape(5, new int[] { 0, 0, 0, -50, 50 }, new int[] { 0, 50, -50, -50, -50 }); // 5 piece 'T' shape
            shapes[27] = new Shape(5, new int[] { 0, 50, 50, -50, 50 }, new int[] { 0, 50, -50, 0, 0 }); // 5 piece 'T' shape rotated 90 degrees clockwise
            shapes[28] = new Shape(5, new int[] { 0, 0, 0, -50, 50 }, new int[] { 0, 50, -50, 50, 50 }); // 5 piece 'T' shape rotated 180 degrees clockwise
            shapes[29] = new Shape(5, new int[] { 0, -50, -50, 50, -50 }, new int[] { 0, 50, -50, 0, 0 }); // 5 piece 'T' shape rotated 270 degrees clockwise
            shapes[30] = new Shape(5, new int[] { 0, 0, 0, 50, 100 }, new int[] { 0, -50, -100, 0, 0 }); // 5 piece 'L' shape
            shapes[31] = new Shape(5, new int[] { 0, 0, 0, 50, 100 }, new int[] { 0, 50, 100, 0, 0 }); // 5 piece 'L' shape rotated 90 degrees clockwise
            shapes[32] = new Shape(5, new int[] { 0, 0, 0, -50, -100 }, new int[] { 0, 50, 100, 0, 0 }); // 5 piece 'L' shape rotated 180 degrees clockwise
            shapes[33] = new Shape(5, new int[] { 0, 0, 0, -50, -100 }, new int[] { 0, -50, -100, 0, 0 }); // 5 piece 'L' shape rotated 270 degrees clockwise
            // need to add 'C'


        }

        void generateShape(int index, int x, int y)
        {
            for (int i = 0; i < shapes[index].getSize(); i++)
            {
                shapes[index].getBlockAtIndex(i).SetBounds(x + shapes[index].getXOffset(i), y + shapes[index].getYOffset(i), 50, 50);
                shapes[index].getBlockAtIndex(i).BackColor = Color.PowderBlue;
                shapes[index].getBlockAtIndex(i).ForeColor = Color.PowderBlue;
                shapes[index].getBlockAtIndex(i).Text = "0";
                Controls.Add(shapes[index].getBlockAtIndex(i));
            }
        }
        public Form1()
        {
            InitializeComponent();
            initialiseShapes();
            for (int i = 0; i < shapes.Length; i++)
            {
                generateShape(i, 200, 200 + (i*200));
            }
        }
    }
}
