using Brickdoku.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Brickdoku
{
    public partial class Form1 : Form
    {
        Button[,] btn = new Button[9, 9]; // Create 2D array of buttons
        int selectedShape = -1;
        const int generatedSize = 40;
        int numberOfShapes = 0;
        SoundPlayer music = new SoundPlayer(Properties.Resources.Brickudoku_Music);

        //added for grid interaction
        private bool dragging = false;
        private Shape draggedShape = null;
        private Point offset;

        bool[,] gridOccupied = new bool[9, 9]; //keep track of the occupied grid squares

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

            // sometimes errors here
            public int getXOffset(int i)
            {
                return xOffsetData[i];
            }

            public int getYOffset(int i)
            {
                return yOffsetData[i];
            }
        }

        Shape[] shapes = new Shape[48]; // the array of all possible shapes
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
            shapes[10] = new Shape(4, new int[] { 0, generatedSize, 2 * (generatedSize), -(generatedSize) }, new int[] { 0, 0, 0, 0 }); // 1x4 horizontal
            shapes[11] = new Shape(4, new int[] { 0, 0, 0, 0 }, new int[] { 0, generatedSize, 2 * (generatedSize), -(generatedSize) }); // 1x4 vertical
            shapes[12] = new Shape(4, new int[] { 0, generatedSize, 2 * (generatedSize), -(generatedSize) }, new int[] { 0, -(generatedSize), -2 * (generatedSize), generatedSize }); // 1x4 diagonal '/'
            shapes[13] = new Shape(4, new int[] { 0, generatedSize, 2 * (generatedSize), -(generatedSize) }, new int[] { 0, generatedSize, 2 * (generatedSize), -(generatedSize) }); // 1x4 diagonal '\'
            shapes[14] = new Shape(4, new int[] { 0, -(generatedSize), -(generatedSize), 0 }, new int[] { 0, 0, -(generatedSize), generatedSize }); // squiggly
            shapes[15] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, 0 }, new int[] { 0, 0, -(generatedSize), -(generatedSize) }); // squiggly rotated 90 degrees clockwise
            shapes[16] = new Shape(4, new int[] { 0, generatedSize, generatedSize, 0 }, new int[] { 0, 0, -(generatedSize), generatedSize }); // squiggly mirrored
            shapes[17] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, 0 }, new int[] { 0, 0, generatedSize, generatedSize }); // squiggly mirrored rotated 90 degrees clockwise
            shapes[18] = new Shape(4, new int[] { 0, generatedSize, -(generatedSize), 0 }, new int[] { 0, 0, 0, generatedSize }); // 4 piece 'T'
            shapes[19] = new Shape(4, new int[] { 0, -(generatedSize), 0, 0 }, new int[] { 0, 0, -(generatedSize), generatedSize }); // 4 piece 'T' rotated 90 degrees clockwise
            shapes[20] = new Shape(4, new int[] { 0, generatedSize, -(generatedSize), 0 }, new int[] { 0, 0, 0, -(generatedSize) }); // 4 piece 'T' rotated 180 degrees clockwise
            shapes[21] = new Shape(4, new int[] { 0, generatedSize, 0, 0 }, new int[] { 0, 0, -(generatedSize), generatedSize }); // 4 piece 'T' rotated 270 degress clockwise
            shapes[22] = new Shape(4, new int[] { 0, 0, 0, generatedSize }, new int[] { 0, -(generatedSize), generatedSize, generatedSize }); // 4 piece 'L' shape
            shapes[23] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize) }, new int[] { 0, 0, 0, generatedSize }); // 4 piece 'L' shape rotated 90 degrees clockwise
            shapes[24] = new Shape(4, new int[] { 0, 0, 0, -(generatedSize) }, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize) }); // 4 piece 'L' shape rotated 180 degrees clockwise
            shapes[25] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, generatedSize }, new int[] { 0, 0, 0, -(generatedSize) }); // 4 piece 'L' shape rotated 270 degrees clockwise
            shapes[26] = new Shape(4, new int[] { 0, 0, 0, -(generatedSize) }, new int[] { 0, -(generatedSize), generatedSize, generatedSize }); // 4 piece 'L' shape mirrored
            shapes[27] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize) }, new int[] { 0, 0, 0, -(generatedSize) }); // 4 piece 'L' shape mirrored rotated 90 degrees clockwise
            shapes[28] = new Shape(4, new int[] { 0, 0, 0, generatedSize }, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize) }); // 4 piece 'L' shape mirrored rotated 180 degrees clockwise
            shapes[29] = new Shape(4, new int[] { 0, -(generatedSize), generatedSize, generatedSize }, new int[] { 0, 0, 0, generatedSize }); // 4 piece 'L' shape mirrored rotated 270 degrees clockwise
            shapes[30] = new Shape(5, new int[] { 0, -(generatedSize), -2 * (generatedSize), generatedSize, 2 * (generatedSize) }, new int[] { 0, 0, 0, 0, 0 }); // 1x5 horizontal
            shapes[31] = new Shape(5, new int[] { 0, 0, 0, 0, 0 }, new int[] { 0, -(generatedSize), -2 * (generatedSize), generatedSize, 2 * (generatedSize) }); // 1x5 vertical
            shapes[32] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), generatedSize }, new int[] { 0, generatedSize, -(generatedSize), -(generatedSize), -(generatedSize) }); // 5 piece 'T' shape
            shapes[33] = new Shape(5, new int[] { 0, generatedSize, generatedSize - (generatedSize), generatedSize }, new int[] { 0, generatedSize, -(generatedSize), 0, 0 }); // 5 piece 'T' shape rotated 90 degrees clockwise
            shapes[34] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), generatedSize }, new int[] { 0, generatedSize, -(generatedSize), generatedSize, generatedSize }); // 5 piece 'T' shape rotated 180 degrees clockwise
            shapes[35] = new Shape(5, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, -(generatedSize) }, new int[] { 0, generatedSize, -(generatedSize), 0, 0 }); // 5 piece 'T' shape rotated 270 degrees clockwise
            shapes[36] = new Shape(5, new int[] { 0, 0, 0, generatedSize, 2 * (generatedSize) }, new int[] { 0, -(generatedSize), -2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape
            shapes[37] = new Shape(5, new int[] { 0, 0, 0, generatedSize, 2 * (generatedSize) }, new int[] { 0, generatedSize, 2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape rotated 90 degrees clockwise
            shapes[38] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -2 * (generatedSize) }, new int[] { 0, generatedSize, 2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape rotated 180 degrees clockwise
            shapes[39] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -2 * (generatedSize) }, new int[] { 0, -(generatedSize), -2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape rotated 270 degrees clockwise
            shapes[40] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -2 * (generatedSize) }, new int[] { 0, -(generatedSize), -2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape mirrored
            shapes[41] = new Shape(5, new int[] { 0, 0, 0, generatedSize, 2 * (generatedSize) }, new int[] { 0, -(generatedSize), -2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape mirrored rotated 90 degrees clockwise
            shapes[42] = new Shape(5, new int[] { 0, 0, 0, generatedSize, 2 * (generatedSize) }, new int[] { 0, generatedSize, 2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape mirrored rotated 180 degrees clockwise
            shapes[43] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -2 * (generatedSize) }, new int[] { 0, generatedSize, 2 * (generatedSize), 0, 0 }); // 5 piece 'L' shape rotated 270 degrees clockwise
            shapes[44] = new Shape(5, new int[] { 0, 0, 0, generatedSize, generatedSize }, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize), generatedSize }); // 'C' shape
            shapes[45] = new Shape(5, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, generatedSize }, new int[] { 0, 0, generatedSize, 0, generatedSize }); // 'C' shape rotated 90 degrees
            shapes[46] = new Shape(5, new int[] { 0, 0, 0, -(generatedSize), -(generatedSize) }, new int[] { 0, -(generatedSize), generatedSize, -(generatedSize), generatedSize }); // 'C' shape rotated 180 degrees
            shapes[47] = new Shape(5, new int[] { 0, -(generatedSize), -(generatedSize), generatedSize, generatedSize }, new int[] { 0, 0, -(generatedSize), 0, -(generatedSize) }); // 'C' shape rotated 270 degrees
        }

        void generateShape(int index, int x, int y, int[] generatedNumbers) // creates a shape using the data in the shapes array
        {

            for (int i = 0; i < shapes[index].getSize(); i++) // creates a button for each block required to make the shape
            {
                Button block = shapes[index].getBlockAtIndex(i);

                block.SetBounds(x + shapes[index].getXOffset(i), y + shapes[index].getYOffset(i), generatedSize, generatedSize); //blocks are placed with respect to their offset data
                block.BackColor = Color.Crimson;
                block.ForeColor = Color.Crimson;
                block.Text = index.ToString(); // text is set to the index of the shape in the array for easy identification later

                // Add the button to the main form
                Controls.Add(block);
                block.Enabled = true; // highlight is enabled for the event handlers to work

                // bring shape to front
                block.BringToFront();

                //event handlers for drag and drop
                block.MouseDown += new MouseEventHandler((sender, e) => btnEvent_MouseDown(sender, e, generatedNumbers));
                block.MouseMove += new MouseEventHandler(this.btnEvent_MouseMove);
                block.MouseUp += new MouseEventHandler(this.btnEvent_MouseUp);

            }
        }

        private void InitialiseGridOccupancy()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    gridOccupied[i, j] = false;
                }
            }
        }

        private void MarkGridOccupied(int startX, int startY, int width, int height)
        {
            for (int i = startX; i < startX + width; i++)
            {
                for (int j = startY; j < startY + height; j++)
                {
                    gridOccupied[i, j] = true;
                }
            }
        }

        //used in dragging and dropping the shapes
        private void btnEvent_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragging) //if dragging is in progress
            {
                dragging = false; //reset flag

                PlaceShapeToGrid(draggedShape);
            }
        }

        //used in dragging and dropping the shapes
        private void btnEvent_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)  //if dragging is in progress
            {
                Shape shape = draggedShape;

                // Move each button in shape to its new location
                for (int i = 0; i < shape.getSize(); i++)
                {
                    Button button = shape.getBlockAtIndex(i);

                    // Calculate new location for each button in the shape
                    int newLeft = e.X + button.Left - offset.X;
                    int newTop = e.Y + button.Top - offset.Y;

                    button.Location = new Point(newLeft, newTop);
                }

                Refresh(); // stop the shape from lagging
            }
        }
        //
        //used in dragging and dropping the shapes
        private void btnEvent_MouseDown(object sender, MouseEventArgs e, int[] generatedNumbers)
        {
            // if left mouse button is pressed
            if (e.Button == MouseButtons.Left)
            {
                // set colour of clicked shape to be dark red
                selectedShape = int.Parse(((Button)sender).Text); // on click, selected shape is set to the index 
                for (int i = 0; i < generatedNumbers.Length; i++)
                {
                    for (int j = 0; j < shapes[generatedNumbers[i]].getSize(); j++)
                    {
                        // change any previously clicked shapes to be back to normal
                        shapes[generatedNumbers[i]].getBlockAtIndex(j).BackColor = Color.Crimson;
                        shapes[generatedNumbers[i]].getBlockAtIndex(j).ForeColor = Color.Crimson;
                    }
                }

                for (int i = 0; i < shapes[selectedShape].getSize(); i++)
                {
                    // change current shape to be dark red
                    shapes[selectedShape].getBlockAtIndex(i).BackColor = Color.DarkRed;
                    shapes[selectedShape].getBlockAtIndex(i).ForeColor = Color.DarkRed;

                }
                dragging = true; //set dragging flag to true

                // Find the shape containing the button
                Button clickedButton = (Button)sender;
                Shape foundShape = null;

                // iterate through shapes to find the shape for the clicked button
                foreach (Shape shape in shapes)
                {
                    for (int i = 0; i < shape.getSize(); i++)
                    {
                        if (shape.getBlockAtIndex(i) == clickedButton)
                        {
                            foundShape = shape;
                            break;
                        }
                    }
                    if (foundShape != null)
                    {
                        break;
                    }
                }
                draggedShape = foundShape; // set the found shape as the dragged shape
                offset = new Point(e.X, e.Y); //set the initial offset for dragging
            }
        }

        // places the shape to grid, ensuring it aligns
        private void PlaceShapeToGrid(Shape shape)
        {
            if (IsShapeFullyOnGrid(shape))
            {
                for (int i = 0; i < shape.getSize(); i++)
                {
                    Button button = shape.getBlockAtIndex(i);

                    // Change the color of the grid squares underneath the shape
                    ChangeGridColor(button);



                    // Make the original shape disappear
                    button.Visible = false;
                }

                // Decrement the number of shapes
                numberOfShapes--;
                regenerateShapes();
            }
            else
            {
                // If the shape is not fully on the grid, move it off the grid
                MoveShapeOffGrid(shape);
            }
        }



        // Change the color of the grid squares without placing the shape on the grid
        private void ChangeGridColor(Button button)
        {
            int centerX = button.Location.X + button.Width / 2;
            int centerY = button.Location.Y + button.Height / 2;
            int gridX = (centerX - 275) / 50;
            int gridY = (centerY - 60) / 50;

            // Change the color of the grid square underneath the shape to a different color (e.g., Color.Gray)
            btn[gridX, gridY].BackColor = Color.Gray;
            gridOccupied[gridX, gridY] = true;
        }

        //helper function for SnapShapeToGrid function
        private bool IsShapeFullyOnGrid(Shape shape)
        {
            for (int i = 0; i < shape.getSize(); i++)
            {
                Button button = shape.getBlockAtIndex(i);

                if (!IsButtonEntirelyOnGrid(button))
                {
                    return false;
                }
            }

            return true;
        }

        //helper function for IsShapeFullyOnGrid function
        // checks if all edges of the button are within the grid boundaries
        // and if the corresponding grid squares are unoccupied
        private bool IsButtonEntirelyOnGrid(Button button)
        {
            int buffer = -20; // Adjust this value based on the desired buffer width

            int leftEdge = button.Location.X - buffer;
            int rightEdge = button.Location.X + button.Width + buffer;
            int topEdge = button.Location.Y - buffer;
            int bottomEdge = button.Location.Y + button.Height + buffer;

            // Check if all edges of the button are within the grid boundaries
            if (leftEdge < 275 || rightEdge > 275 + 9 * 50 || topEdge < 50 || bottomEdge > 50 + 9 * 50)
            {
                return false;
            }

            // Check if the squares on the grid are unoccupied
            int gridX = (button.Location.X - 275) / 50;
            int gridY = (button.Location.Y - 60) / 50;

            for (int i = gridX; i < gridX + button.Width / 50; i++)
            {
                for (int j = gridY; j < gridY + button.Height / 50; j++)
                {
                    if (gridOccupied[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //helper function for SnapShapeToGrid function.
        //moves the shape off if it doesn't fit on the grid
        private void MoveShapeOffGrid(Shape shape)
        {
            // Calculate the average position of the shape buttons
            int avgX = 0;
            int avgY = 0;

            for (int i = 0; i < shape.getSize(); i++)
            {
                Button button = shape.getBlockAtIndex(i);
                avgX += button.Location.X + button.Width / 2;
                avgY += button.Location.Y + button.Height / 2;
            }

            avgX /= shape.getSize();
            avgY /= shape.getSize();

            // Move the entire shape off the grid
            int offsetX = avgX - 100;
            int offsetY = avgY - 60;

            for (int i = 0; i < shape.getSize(); i++)
            {
                Button button = shape.getBlockAtIndex(i);
                button.Location = new Point(button.Location.X - offsetX, button.Location.Y - offsetY);
            }
        
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
            this.BackgroundImage = null; // remove image from gameplay screen
            // create a title label for the top of the screen
            Label lblTitle = new Label();
            Controls.Add(lblTitle);
            lblTitle.Font = new System.Drawing.Font("OCR A Extended", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle.Text = "Brickudoku";
            lblTitle.ForeColor = System.Drawing.Color.Crimson;
            lblTitle.SetBounds(400, 10, 250, 40);
            placeShapes();
            createGrid();
            InitialiseGridOccupancy();
        }

        

        void placeShapes()
        {
            // generate 3 random numbers and use that to generate three shapes
            // it seems to bug if two numbers are the same so need to check for that
            int[] generatedNumbers = new int[3];
            for (int i = 0; i < 3; i++)
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
                generateShape(number, 100, 100 + (i * 150), generatedNumbers);
                numberOfShapes++;
            }

        }

        /**
         * Check if new shapes are needed and regenerate
         */
        void regenerateShapes()
        {
            // if there are no shapes, generate 3 new ones
            if (numberOfShapes == 0)
            {
                placeShapes();
            }
            else
            {
                Console.WriteLine("Num shapes: " + numberOfShapes);
            }
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
