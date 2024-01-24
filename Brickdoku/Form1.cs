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
        class Shape
        {
            int size;
            int x;
            int y;
            Button[] blocks;

            public Shape()
            {
                size = 0;
                x = 0;
                y = 0;
            }

            public Shape(int size, int x, int y)
            {
                this.size = size;
                this.x = x;
                this.y = y;
                blocks = new Button[size];
                for (int i = 0; i < size; i++)
                {
                    blocks[i] = new Button();
                }
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

            public void setX(int x)
            {
                this.x = x;
            }

            public int getX()
            {
                return x;
            }

            public void setY(int y)
            {
                this.y = y;
            }

            public int getY()
            {
                return y;
            }
        }

        Shape[] shapes = new Shape[1];
        void initialiseShapes()
        {
            shapes[0] = new Shape(4, 1, 1);
            for (int i = 0; i < 4; i++)
            {
                switch(i)
                {
                    case 0:
                        shapes[0].getBlockAtIndex(i).SetBounds(shapes[0].getX(), shapes[0].getY(), 50, 50);
                        break;
                    case 1:
                        shapes[0].getBlockAtIndex(i).SetBounds(shapes[0].getX() + 50, shapes[0].getY(), 50, 50);
                        break;
                    case 2:
                        shapes[0].getBlockAtIndex(i).SetBounds(shapes[0].getX(), shapes[0].getY() + 50, 50, 50);
                        break;
                    case 3:
                        shapes[0].getBlockAtIndex(i).SetBounds(shapes[0].getX() + 50, shapes[0].getY() + 50, 50, 50);
                        break;

                }
                shapes[0].getBlockAtIndex(i).BackColor = Color.PowderBlue;
                shapes[0].getBlockAtIndex(i).ForeColor = Color.PowderBlue;
                shapes[0].getBlockAtIndex(i).Text = "0";
                Controls.Add(shapes[0].getBlockAtIndex(i));
            }


        }
        public Form1()
        {
            InitializeComponent();
            initialiseShapes();
        }
    }
}
