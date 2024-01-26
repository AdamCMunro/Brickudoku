using Brickdoku.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
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
        const int generatedSize = 30;
        SoundPlayer music = new SoundPlayer(Properties.Resources.Brickudoku_Music);

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
                // System.IndexOutOfRangeException error keeps appearing here on some runs only and i dont know why
                // @adam
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

        void generateShape(int index, int x, int y, int size, int[] generatedNumbers) // creates a shape using the data in the shapes array
        {
            Console.WriteLine("Y:" + y);
            for (int i = 0; i < shapes[index].getSize(); i++) // creates a button for each block required to make the shape
            {
                shapes[index].getBlockAtIndex(i).SetBounds(x + shapes[index].getXOffset(i), y + shapes[index].getYOffset(i), size, size); //blocks are placed with respect to their offset data
                shapes[index].getBlockAtIndex(i).BackColor = Color.Crimson;
                shapes[index].getBlockAtIndex(i).ForeColor = Color.Crimson;
                shapes[index].getBlockAtIndex(i).Text = index.ToString(); // text is set to the index of the shape in the array for easy identification later
                shapes[index].getBlockAtIndex(i).Click += new EventHandler((sender, e) => clickGeneratedShape(sender, e, x, y, generatedNumbers));
                Controls.Add(shapes[index].getBlockAtIndex(i));
                // doesn't allow a disabled button to have its forecolor changed so i have enabled the button again
                //shapes[index].getBlockAtIndex(i).Enabled = false; // highlight is disabled
            }
        }

        /**
         * Shapes array is the indexes of all shapes currently on the page
         */
        void clickGeneratedShape(object sender, EventArgs e, int x, int y, int[] generatedNumbers)
        {
            selectedShape = int.Parse(((Button)sender).Text); // on click, selected shape is set to the index 
            Console.WriteLine("Shape clicked!");
            // first need to make any previously selected shapes normal colour and size again
            // delete all shapes from the page and regenerate
            for (int i=0; i < generatedNumbers.Length; i++)
            {
                for (int j = 0; j < shapes[generatedNumbers[i]].getSize(); j++)
                {
                    
                    shapes[generatedNumbers[i]].getBlockAtIndex(j).BackColor = Color.Crimson;
                    shapes[generatedNumbers[i]].getBlockAtIndex(j).ForeColor = Color.Crimson;
                }
            }

           
            for (int i = 0; i < shapes[selectedShape].getSize(); i++)
            {
                shapes[selectedShape].getBlockAtIndex(i).BackColor = Color.DarkRed;
                shapes[selectedShape].getBlockAtIndex(i).ForeColor = Color.DarkRed;
                
            }

            // this stuff all makes the size change too but is pretty inefficient and slow
            // Controls.Remove(shapes[generatedNumbers[i]].getBlockAtIndex(j));
            // generate each shape again with original properties
            /*for (int i = 0; i < generatedNumbers.Length; i++)
            {
                generateShape(generatedNumbers[i], x, 100 + (i*150), generatedSize, generatedNumbers);
            }*/

            // for each button on the clicked shape, make it bigger and change the colour
            //generateShape(selectedShape, x, y, 40, generatedNumbers); // kind of works to make it bigger but looks a bit messy
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        public Form1()
        {
            InitializeComponent(); // Initialise the new item
            initialiseShapes();
            playMusic();

        }

        /**
         * Play music function
         */
        void playMusic()
        {
            // used this to help https://stackoverflow.com/questions/14491431/playing-wav-file-with-c-sharp
            music.PlayLooping(); // play music in a loop
        }

        /**
         * Function to create the grid
         */
        void createGrid()
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                
                    btn[x, y] = new Button();
                    btn[x, y].SetBounds(275 + (49 * x), 60 + (49 * y), 50, 50);
                    // change background colour to white to differentiate squares
                    if ((x>2 && x<6 && y<3) || (x<3 && y>2 && y<6) ||(x>5 && y>2 && y<6) || (x>2 && x<6 && y>5))
                    {
                        btn[x, y].BackColor = Color.White;
                    }
                    else
                    {
                        btn[x, y].BackColor = Color.LightPink;
                    }
                    // btn[x, y].Text = Convert.ToString((x + 1) + "," + (y + 1)); for debugging purposes
                    btn[x, y].Click += new EventHandler(this.btnEvent_Click);
                    Controls.Add(btn[x, y]);
                    //remove the hover over highlight by setting 
                    btn[x, y].Enabled = false;
                    // slow down the appearing of the grid slightly
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        void placeShapes()
        {
            // generate 3 random numbers and use that to generate three shapes
            // it seems to bug if two numbers are the same so need to check for that
            int[] generatedNumbers = new int[3];
            for (int i=0; i<3; i++)
            {
                Random random = new Random();
                int number = random.Next(38); // generate random number < 38
                // check number is not already in list of already generated and remove if it is
                while (generatedNumbers.Contains(number))
                {
                    // if it already contains the number generate a new number till a unique one is found
                    random = new Random();
                    number = random.Next(38);
                }
                // add the number to the array so it is not used again
                generatedNumbers[i] = number;
       
                // write randomly generated number to output
                Console.WriteLine("Random number: " + number);
                generateShape(number, 100, 100 + (i*150), generatedSize, generatedNumbers);
            }
            
        }
        void btnEvent_Click(object sender, EventArgs e) {

            Console.WriteLine(((Button)sender).Text); // SAME handler asbefore
        }


        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            // when start button is clicked, hide all the items in the current menu screen
            // and display the grid game form
            BtnExit.Hide();
            BtnStart.Hide();
            
            // create a title label for the top of the screen
            Label lblTitle = new Label();
            Controls.Add(lblTitle);
            lblTitle.Font = new System.Drawing.Font("OCR A Extended", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle.Text = "Brickudoku";
            lblTitle.ForeColor = System.Drawing.Color.Crimson;
            lblTitle.SetBounds(400, 10, 250, 40);
            this.BackgroundImage = null; // remove image from gameplay screen
            createGrid();
            placeShapes();

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        // The volume control button
        private void BtnMute_Click(object sender, EventArgs e)
        {
            // when clicked if playing, volume turns off, and image changes to muted image
            
            if (BtnMute.Text == "p")
            {
                Console.WriteLine("button pressed - muting");
                music.Stop();
                BtnMute.Text = "m";
                try
                {
                    BtnMute.BackgroundImage = Properties.Resources.muted;
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine("Error finding image!");
                }
            }
            else
            {
                Console.WriteLine("button pressed - playing");
                music.PlayLooping();
                BtnMute.Text = "p";
                try
                {
                    BtnMute.BackgroundImage = Properties.Resources.playing;
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine("Error finding image!");
                }
            }

        }
    }
}
