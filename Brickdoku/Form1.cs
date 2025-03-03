﻿/**
 * Brickudoku grid game by team 4
 * Team members:
 * - Adam Munro
 * - Marylou Das Chagas e Silva
 * - Laura Clark
 * 
 * // await time delay - got from this link - https://stackoverflow.com/questions/24136390/thread-sleep-without-freezing-the-ui#:~:text=The%20simplest%20way%20to%20use,asynchronous%20add%20the%20async%20modifier.&text=Now%20you%20can%20use%20the,asynchronous%20tasks%2C%20in%20your%20case.
 */
using Brickdoku.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Brickdoku
{
    public partial class Brickudoku : Form
    {
        Button[,] btn = new Button[9, 9]; // Create 2D array of button
        const int generatedSize = 50;
        int numberOfShapes = 0;
        int[] generatedNumbers = { -1, -1, -1 };
        bool[] placeable = new bool[] { true, true, true };
        bool[] placed = { false, false, false };
        int numberNotPlaceable = 0;
        List<int> rows = new List<int>();
        List<int> columns = new List<int>();
        List<Tuple<int, int>> squares = new List<Tuple<int, int>>();

        //music and sound effects
        SoundPlayer background_music;
        SoundPlayer pop = new SoundPlayer("pop_sound.wav"); //sound effect for placing shape on grid
        SoundPlayer points = new SoundPlayer("points_sound.wav"); // sound effect for winning points
        bool bgPlaying = false;

        Color palePink = ColorTranslator.FromHtml("#ffccd4"); // light pink sqaure colour
        Color midPink = ColorTranslator.FromHtml("#ff667d"); // mid pink colour for hovering
        //added for grid interaction
        private bool dragging = false;
        private Shape draggedShape = null;
        private Shape foundShape = null;
        private Point offset;

        // scoring variables
        int totalScore = 0;
        int numberOfCompleted;
        int streak = 0;

        bool AI = false;
        bool AI2 = false;
        bool clicked = false;

        Button clickedButton = null;

        bool[,] gridOccupied = new bool[9, 9]; //keep track of the occupied grid squares

        Label lblCombination = new Label(); // label to display combination
        Label lblStreak = new Label(); // label to display streaks
        Label lblScore = new Label();   // score label   
        Label lblDisplayScore = new Label(); // score header label
        Label lblScoreIncrease = new Label();  // score increase label

        Form RulesForm = new Form(); // form for rules

        /**
         * Class for the playable shapes
         */
        class Shape
        {
            int size; // the number of blocks used to make the shape
            Button[] blocks; // the array of blocks
            int[] yOffsetData; // an array storing how the blocks should be offset realtive to the central block along the x-axis
            int[] xOffsetData; // an array storing how the blocks should be offset realtive to the central block along the y-axis


            private List<Point> initialPositions = new List<Point>(); //used to store the postions of the shapes
            /**
             * method to store the initial positions of each of the generated shapes
            */
            public void StoreInitialPositions()
            {
                initialPositions.Clear();
                for (int i = 0; i < getSize(); i++)
                {
                    Button button = getBlockAtIndex(i);
                    initialPositions.Add(button.Location);
                }
            }

            /** 
             * method to restore initial positions
              */
            public void RestoreInitialPositions()
            {
                for (int i = 0; i < getSize(); i++)
                {
                    Button button = getBlockAtIndex(i);
                    button.Location = initialPositions[i];
                }
            }

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
                    blocks[i] = new Button(); // initaliases a new button for each tile in the shape
                }
                this.xOffsetData = xOffsetData;
                this.yOffsetData = yOffsetData;
            }

            /**
             * Returns the block at a given index
             */
            public Button getBlockAtIndex(int i)
            {
                return blocks[i];
            }

            /**
             * Returns the size of a shape
             */
            public int getSize()
            {
                return size;
            }

            /**
             * Returns the xOffsetData for the block at a given index
             */
            public int getXOffset(int i)
            {
                return xOffsetData[i];
            }

            /**
             * Returns the yOffsetData for the block at a given index
             */
            public int getYOffset(int i)
            {
                return yOffsetData[i];
            }
        }

        Shape[] shapes = new Shape[48]; // the array of all possible shapes
        void InitialiseShapes() // shape objects are created and the array is populated
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
            shapes[33] = new Shape(5, new int[] { 0, generatedSize, generatedSize, -(generatedSize), generatedSize }, new int[] { 0, generatedSize, -(generatedSize), 0, 0 }); // 5 piece 'T' shape rotated 90 degrees clockwise
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

        /**
         * Create a shape using the the index from the shapes and the x and y positions to place it to
         */
        void GenerateShape(int index, int x, int y) // creates a shape using the data in the shapes array
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
                if (AI == false)
                {
                    block.Enabled = true; // highlight is enabled for the event handlers to work
                }

                block.Visible = true;

                // bring shape to front
                block.BringToFront();

                //event handlers for drag and drop
                if (AI == false)
                {
                    block.MouseDown += new MouseEventHandler(this.BtnEvent_MouseDown);
                    block.MouseMove += new MouseEventHandler(this.BtnEvent_MouseMove);
                    block.MouseUp += new MouseEventHandler(this.BtnEvent_MouseUp);
                }

                // Add these lines to store and maintain the initial positions
                shapes[index].StoreInitialPositions();

            }
        }

        /**
         * Initialise all spaces in the grid to be unoccupied
         */
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

        /**
        * Event handler for the MouseUp event on the shape buttons.
        * Used in dragging and dropping the shapes.
        * This method is called to handle the end of the drag operation when mouse id released.
        */
        private void BtnEvent_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragging) //check if dragging is in progress
            {
                dragging = false; //reset flag
                PlaceShapeToGrid(draggedShape);
            }
        }

        /**
        * Event handler for the MouseMove event on the shape buttons.
        * Used in dragging and dropping the shapes.
        * This method is called to update the shape's position when the mouse moves.
        */
        private void BtnEvent_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)  //check if dragging is in progress
            {
                Shape shape = draggedShape;

                // Move each button in shape to its new location
                for (int i = 0; i < shape.getSize(); i++)
                {
                    Button button = shape.getBlockAtIndex(i);

                    // Calculate new location for each button in the shape
                    int newLeft = e.X + button.Left - offset.X;
                    int newTop = e.Y + button.Top - offset.Y;

                    //set the new location
                    button.Location = new Point(newLeft, newTop);
                }

                Refresh(); // stop the shape from lagging
            }
        }
        /**
       * Event handler for the MouseDown event on the shape buttons.
       * Used in dragging and dropping the shapes.
       * This method is called to start the drag operation, when the left mouse button is pressed on a shape button.
       */
        private void BtnEvent_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // check if left mouse button is pressed
            {
                clicked = true;
                // change any previously clicked shapes to be back to normal colour
                for (int i = 0; i < generatedNumbers.Length; i++)
                {
                    for (int j = 0; j < shapes[generatedNumbers[i]].getSize(); j++)
                    {
                        if (placeable[i] == true)
                        {
                            shapes[generatedNumbers[i]].getBlockAtIndex(j).BackColor = Color.Crimson;
                            shapes[generatedNumbers[i]].getBlockAtIndex(j).ForeColor = Color.Crimson;
                        }
                    }
                }

                dragging = true; //set dragging flag to true

                // Find the shape containing the button
                clickedButton = (Button)sender;
                foundShape = null;

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
                // change current shape to be dark red
                for (int i = 0; i < foundShape.getSize(); i++)
                {
                    foundShape.getBlockAtIndex(i).BackColor = Color.DarkRed;
                    foundShape.getBlockAtIndex(i).ForeColor = Color.DarkRed;

                }
                draggedShape = foundShape; // set the found shape as the dragged shape
                offset = new Point(e.X, e.Y); //set the initial offset for dragging
            }
        }

        /**
        * Places the shape onto the grid, ensuring it aligns properly.
        * @param shape object to be placed onto the grid is used.
        */
        private void PlaceShapeToGrid(Shape shape)
        {
            //Console.WriteLine("Fully on grid? " + IsShapeFullyOnGrid(shape));
            if (IsShapeFullyOnGrid(shape) == true)
            {
                clicked = false;
                if (BtnMute.Text == "m") //play pop sound effect if not muted
                {
                    pop.Stop();
                }
                else
                {
                    pop.Play();
                }

                for (int j = 0; j < 3; j++)
                {
                    if (generatedNumbers[j] == Int32.Parse(shape.getBlockAtIndex(0).Text))
                    {
                        placed[j] = true;
                        break;
                    }
                }
                for (int i = 0; i < shape.getSize(); i++)
                {
                    Button button = shape.getBlockAtIndex(i);

                    // Change the color of the grid squares underneath the shape
                    ChangeGridColour(button);

                    // Make the original shape disappear
                    button.Visible = false;
                    Controls.Remove(button);
                    button.MouseDown -= new MouseEventHandler(this.BtnEvent_MouseDown);
                    button.MouseMove -= new MouseEventHandler(this.BtnEvent_MouseMove);
                    button.MouseUp -= new MouseEventHandler(this.BtnEvent_MouseUp);
                }

                CheckComplete();
                DisplayStreakAndCombo(numberOfCompleted);
                CalculateScore(shape, numberOfCompleted, streak);
                // Decrement the number of shapes and check for regeneration
                numberOfShapes--;
                numberNotPlaceable = 0;
                for (int i = 0; i < 3; i++)
                {
                    // remove the shape we have  from the generated numbers array by resestting to zero
                    if (placed[i] == false)
                    {
                        // Console.WriteLine("Checking");
                        placeable[i] = CheckShapeFits(generatedNumbers[i]);
                    }
                }
                RegenerateShapes();
            }
            else
            {
                // If the shape is not fully on the grid, move it off the grid
                shape.RestoreInitialPositions();
            }
        }


        /**
         * Calculate and display streak and combination information
         * @param numberOfCompleted - the number of completed shapes to be used to calculate streak and combo
         */
        async void DisplayStreakAndCombo(int numberOfCompleted)
        {
            if (numberOfCompleted > 0)
            {
                streak++;
            }
            else
            {
                streak = 0;
            }
            // display streak bonus
            if (streak > 1)
            {
                lblStreak.Text = "x " + streak.ToString() + " streak!";
                lblStreak.Visible = true;
                // add in time delay - got from this link - https://stackoverflow.com/questions/24136390/thread-sleep-without-freezing-the-ui#:~:text=The%20simplest%20way%20to%20use,asynchronous%20add%20the%20async%20modifier.&text=Now%20you%20can%20use%20the,asynchronous%20tasks%2C%20in%20your%20case.
                await Task.Delay(1500);
                lblStreak.Visible = false;
                if (BtnMute.Text == "m") //play streak/combo sound effect if not muted
                {
                    points.Stop();
                }
                else
                {
                    points.Play();
                }
            }

            // display combination bonus
            if (numberOfCompleted > 1)
            {
                Console.WriteLine(numberOfCompleted.ToString());
                lblCombination.Text = "x " + numberOfCompleted.ToString() + " combo!";
                lblCombination.Visible = true;
                // add in time delay - got from this link - https://stackoverflow.com/questions/24136390/thread-sleep-without-freezing-the-ui#:~:text=The%20simplest%20way%20to%20use,asynchronous%20add%20the%20async%20modifier.&text=Now%20you%20can%20use%20the,asynchronous%20tasks%2C%20in%20your%20case.
                await Task.Delay(1500);
                lblCombination.Visible = false;

                if (BtnMute.Text == "m") //play streak/combo sound effect if not muted
                {
                    points.Stop();
                }
                else
                {
                    points.Play();
                }
            }
        }


        /**
        * Change the color of the grid squares underneath the shape 
        * without placing the shape on the grid.
        * @param button representing a block of the shape is used.
        */
        private void ChangeGridColour(Button button)
        {
            // calculate the center coordinates of the button
            int centerX = button.Location.X + button.Width / 2;
            int centerY = button.Location.Y + button.Height / 2;
            int gridX = (centerX - 275) / 50;
            int gridY = (centerY - 60) / 50;
            Console.WriteLine("x - test" + gridX + " y: " + gridY);

            // Change the colour of the grid square underneath the shape to a different colour 
            btn[gridX, gridY].BackColor = Color.Crimson;
            gridOccupied[gridX, gridY] = true;
        }

        /**
        * Check if the entire shape is on the grid.
        * @param shape object to be checked is used.
        * @return true if the shape is fully on the grid; otherwise, false.
        */
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

        /**
        * Helper function for determining if the entire button is within the grid boundaries 
        * and if the corresponding grid squares are unoccupied.
        * @param button representing a block of the shape is used
        * @returns true if all edges of the button are within the grid boundaries and the corresponding grid squares are unoccupied; otherwise, false.
        */
        private bool IsButtonEntirelyOnGrid(Button button)
        {
            int buffer = -22; // Adjust this value based on the desired buffer width 

            // calculate the edges of the button including the buffer
            int leftEdge = button.Location.X - buffer;
            int rightEdge = button.Location.X + button.Width + buffer;
            int topEdge = button.Location.Y - buffer;
            int bottomEdge = button.Location.Y + button.Height + buffer;

            // Check if all edges of the button are within the grid boundaries
            if (leftEdge < 275 || rightEdge > 275 + 9 * 50 || topEdge < 50 || bottomEdge > 50 + 9 * 50)
            {
                return false;
            }

            // Check if the squares on the grid, including a buffer around each button, are unoccupied
            int gridX = (button.Location.X - buffer - 275) / 50; // left-side buffer
            int gridY = (button.Location.Y - buffer - 60) / 50; // top-side buffer

            for (int i = gridX; i < gridX + button.Width / 50; i++)
            {
                for (int j = gridY; j < gridY + button.Height / 50; j++)
                {
                    if (i >= 0 && i < 9 && j >= 0 && j < 9 && gridOccupied[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            // required

        }

        public Brickudoku()
        {
            InitializeComponent(); // Initialise the new item
            InitialiseShapes();
            /** used this to help with music https://stackoverflow.com/questions/14491431/playing-wav-file-with-c-sharp
            music.PlayLooping();lay music in a loop */
            RulesSetUp(); // set up rules

            MXP.URL = @"Brickudoku_Music.mp3";
            MXP.settings.playCount = 9999; //Repeating the music when it ends
            MXP.Ctlcontrols.play();
            MXP.Visible = false;

        }

        /**
         * Function to create the grid
         */
        void CreateGrid()
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {

                    btn[x, y] = new Button();
                    btn[x, y].SetBounds(275 + (49 * x), 60 + (49 * y), 50, 50);
                    // change background colour to white to differentiate squares
                    if ((x > 2 && x < 6 && y < 3) || (x < 3 && y > 2 && y < 6) || (x > 5 && y > 2 && y < 6) || (x > 2 && x < 6 && y > 5))
                    {
                        btn[x, y].BackColor = Color.White;
                    }
                    else
                    {
                        btn[x, y].BackColor = palePink;
                    }

                    //btn[x, y].Text = Convert.ToString((x + 1) + "," + (y + 1)); // for debugging purposes
                    // add common event handler
                    btn[x, y].Click += new EventHandler(this.BtnEvent_Click);
                    btn[x, y].MouseEnter += new EventHandler(this.Btn_Enter);
                    btn[x, y].MouseLeave += new EventHandler(this.Btn_Leave);
                    Controls.Add(btn[x, y]);

                    //remove the hover over highlight by setting 
                    btn[x, y].Enabled = true;
                    // slow down the appearing of the grid slightly
                    if (AI == false)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    
                }
            }
        }

        /**
         * Event handler for grid buttons
         * Function to hover the location the button will be placed to when the mouse enters the grid space when a shape is clicked
         */
        private void Btn_Enter(object sender, EventArgs e)
        {
            // check if a shape is clicked
            if (clicked == true)
            {
                // from one button hovered over, get locations of other buttons in the shape and colour
                int x = 0;
                int y = 0;
                int xMod;
                int yMod;

                int number = Int32.Parse(foundShape.getBlockAtIndex(0).Text); // index shape number

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if ((Button)sender == btn[i, j])
                        {
                            x = i;
                            y = j;
                            break;
                        }
                    }
                }
                // for every block of the generated shape;
                for (int i = 0; i < shapes[number].getSize(); i++)
                {

                    xMod = shapes[number].getXOffset(i) / generatedSize;
                    yMod = shapes[number].getYOffset(i) / generatedSize;

                    if (x + xMod >= 0 && x + xMod < 9 && y + yMod >= 0 && y + yMod < 9 && !gridOccupied[x + xMod, y + yMod])
                    {
                        // set all the grid coordinates that will be under the shaep hover to mid pink
                        btn[x + xMod, y + yMod].BackColor = midPink;
                    }
                }
            }

        }


        /**
         * Event handler for the grid buttons to make any hovered grid buttons back to original colour when mouse leaves
         */
        private void Btn_Leave(object sender, EventArgs e)
        {
            Button hovered = (Button)sender;
            // for the whole grid, if the color is midPink change back
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (btn[x, y].BackColor == midPink)
                    {
                        // change background colour to white to differentiate squares
                        if ((x > 2 && x < 6 && y < 3) || (x < 3 && y > 2 && y < 6) || (x > 5 && y > 2 && y < 6) || (x > 2 && x < 6 && y > 5))
                        {
                            btn[x, y].BackColor = Color.White;
                        }
                        else
                        { 
                            btn[x, y].BackColor = palePink;
                        }
                    }

                }
            }
        }


        /**
         * Event handler for grid buttons
         * When a button is clicked make the hovered shapes filled in and occupied
         */
        private void BtnEvent_Click(object sender, EventArgs e)
        {
            int count = 0;
            // set all midPink buttons to be filled and coloured crimson
            // keep a count of how many squares are green to compare to shape size
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {

                    if (btn[x, y].BackColor == midPink)
                    {
                        count++;
                    }
                }
            }
            // if the shape is fully on the grid
            if (count == foundShape.getSize())
            {
                for (int y = 0; y < 9; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        // check hover is fully on grid and all pieces of shape are placeable
                        if (btn[x, y].BackColor == midPink)
                        {
                            btn[x, y].BackColor = Color.Crimson;
                            gridOccupied[x, y] = true;
                            clicked = false;
                        }
                    }
                }

                if (BtnMute.Text == "m") //play pop sound effect if not muted
                {
                    pop.Stop();
                }
                else
                {
                    pop.Play();
                }
                // set placed to be true for shape that was placed
                for (int j = 0; j < 3; j++)
                {
                    if (generatedNumbers[j] == Int32.Parse(foundShape.getBlockAtIndex(0).Text))
                    {
                        placed[j] = true;
                        break;
                    }
                }
                for (int i = 0; i < foundShape.getSize(); i++)
                {
                    Button button = foundShape.getBlockAtIndex(i);

                    // Make the original shape disappear
                    button.Visible = false;
                    Controls.Remove(button);
                    button.MouseDown -= new MouseEventHandler(this.BtnEvent_MouseDown);
                    button.MouseMove -= new MouseEventHandler(this.BtnEvent_MouseMove);
                    button.MouseUp -= new MouseEventHandler(this.BtnEvent_MouseUp);
                }

                CheckComplete();
                DisplayStreakAndCombo(numberOfCompleted);
                CalculateScore(foundShape, numberOfCompleted, streak);
                // Decrement the number of shapes and check for regeneration
                numberOfShapes--;
                numberNotPlaceable = 0;
                for (int i = 0; i < 3; i++)
                {
                    // re check if other remaining shapes fit 
                    if (placed[i] == false)
                    {
                        // Console.WriteLine("Checking");
                        placeable[i] = CheckShapeFits(generatedNumbers[i]);
                    }
                }
                RegenerateShapes();
            }
        }


        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        /**
         * Event handler for start button to start the game 
         */
        private void BtnStart_Click(object sender, EventArgs e)
        {
            // when start button is clicked, hide all the items in the current menu screen
            // and display the grid game form
            BtnExit.Hide();
            BtnStart.Hide();
            BtnAI.Hide();
            this.BackgroundImage = null; // remove image from gameplay screen
            this.BackColor = Color.Linen;
            menuStrip1.BackColor = Color.Linen;
            CreateGrid();
            SetUpLabels();
            InitialiseGridOccupancy();
            GenerateRandomShapeNumbers();
        }

        /**
         * Set information for the labels needed for the game 
         * set up title, combo, streak, and scoring labels and place and position them
         */
        void SetUpLabels()
        {
            // create a title label for the top of the screen
            Label lblTitle = new Label();
            Controls.Add(lblTitle);
            lblTitle.Font = new System.Drawing.Font("OCR A Extended", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle.Text = "Brickudoku";
            lblTitle.BringToFront();
            lblTitle.ForeColor = System.Drawing.Color.Crimson;
            lblTitle.SetBounds(400, 10, 250, 40);

            // combinations label
            lblCombination.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblCombination.ForeColor = System.Drawing.Color.Crimson;
            lblCombination.SetBounds(750, 100, 250, 40);
            lblCombination.Visible = false;
            Controls.Add(lblCombination);

            // streaks label
            lblStreak.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblStreak.ForeColor = System.Drawing.Color.Crimson;
            lblStreak.SetBounds(750, 150, 250, 40);
            lblStreak.Visible = false;
            Controls.Add(lblStreak);

            // create a display score label
            lblDisplayScore.Font = new System.Drawing.Font("OCR A Extended", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblDisplayScore.Text = "Score";
            lblDisplayScore.ForeColor = System.Drawing.Color.Crimson;
            lblDisplayScore.SetBounds(780, 300, 250, 40);
            Controls.Add(lblDisplayScore);

            // add actual score label
            lblScore.Font = new System.Drawing.Font("OCR A Extended", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblScore.Text = "0";
            lblScore.ForeColor = System.Drawing.Color.Crimson;
            lblScore.SetBounds(820, 350, 250, 40);
            Controls.Add(lblScore);

            // label for the score increase on that turn
            lblScoreIncrease.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblScoreIncrease.Text = "";
            lblScoreIncrease.ForeColor = System.Drawing.Color.Crimson;
            lblScoreIncrease.SetBounds(820, 250, 250, 40);
            lblScoreIncrease.Visible = false;
            Controls.Add(lblScoreIncrease);
        }

        /**
         * Generate 3 random numbers to be used to generate 3 shapes
         * Call function to generate shapes at the correct positions
         */
        void GenerateRandomShapeNumbers()
        {
            // generate 3 random numbers and use that to generate three shapes
            numberNotPlaceable = 0; // reset back to 0
            for (int i = 0; i < 3; i++)
            {
                placed[i] = false;
                Random random = new Random();
                int number = random.Next(48); // generate random number < 48
                 // check number is not already in list of already generated
                while (generatedNumbers.Contains(number))
                {
                    // if it already contains the number generate a new number till a unique one is found
                    random = new Random();
                    number = random.Next(38);

                }
                // add the number to the array so it is not used again
                generatedNumbers[i] = number;

                //Console.WriteLine("Random number: " + number);
                GenerateShape(number, 100, 80 + (i * 220));
                numberOfShapes++;
            }
            // check shape fits for all generated numbers
            for (int i = 0; i < 3; i++)
            {
                placeable[i] = CheckShapeFits(generatedNumbers[i]);
            }
        }

        /**
         * Check if new shapes are needed and regenerate
         */
        void RegenerateShapes()
        {
            // if there are no shapes, generate 3 new ones
            if (numberOfShapes == 0)
            {
                GenerateRandomShapeNumbers();
            }
        }

        /**
         * Function to check for a complete line or square
         * Update a list of rows, columns and sqaures that are complete
         * Keep a total of how many were completed to check for streaks and comboss
         */
        void CheckComplete()
        {
            // clear previous rows, columns and squares
            rows.Clear();
            squares.Clear();
            columns.Clear();
            numberOfCompleted = 0;

            // lsit to store columns that we know are empty so we don't check twice
            List<int> emptyColumns = new List<int>();

            // check rows
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (gridOccupied[x, y] == false)
                    {
                        // column x must therefore also be empty so don't check it later
                        emptyColumns.Add(x);
                        // if one x is not occupied, then the whole line is not occupied
                        break; // go to next row
                    }
                    // if x is 8, then everyhting in that row is occupied, so clear
                    if (x == 8)
                    {
                        Console.WriteLine("row: " + y + " fully occupied");
                        rows.Add(y);
                        numberOfCompleted++;
                    }
                }
            }
            // check columns
            for (int x = 0; x < 9; x++)
            {
                if (emptyColumns.Contains(x))
                {
                    continue;
                }
                for (int y = 0; y < 9; y++)
                {
                    if (gridOccupied[x, y] == false)
                    {
                        //Console.WriteLine("false" + x + "," + y);
                        // if one in column is not occupied, then we can break
                        break;
                    }
                    // if we reach y = 8 then there is a full column
                    if (y == 8)
                    {
                        Console.WriteLine("Column: " + x + " fully occupied");
                        columns.Add(x);
                        numberOfCompleted++;
                    }
                }
            }
            // check for squares
            // for each square 
            bool occupied; // to check whether one space of a square is occupied to avoid having to loop the whole square
            for (int s = 0; s < 9; s++)
            {
                occupied = true;
                int occupiedSquares = 0;
                // for each row in the square

                for (int x = 0; x < 9; x++)
                {
                    if (occupied == false)
                    {
                        break;
                    }
                    else
                    {
                        // for squares 0,3,6 x < 2
                        // if s is one of these values and x > 2, skip to next loop
                        if ((s == 0 || s == 3 || s == 6) && x > 2)
                        {
                            continue;
                        }
                        // for sqaures 1,4,7, x>2, x<6
                        else if ((s == 1 || s == 4 || s == 7) && (x < 3 || x > 5))
                        {
                            continue;
                        }
                        // for sqaures 2,5,8 x > 5
                        else if ((s == 2 || s == 5 || s == 8) && x < 6)
                        {
                            continue;
                        }
                        // for each column in the square
                        for (int y = 0; y < 9; y++)
                        {
                            // for squares 0,1,2 y < 2
                            // if s is one of these values and x > 2, continue
                            if ((s == 0 || s == 1 || s == 2) && y > 2)
                            {
                                continue;
                            }
                            // for sqaures 3,4,5, y>2, y<6
                            else if ((s == 3 || s == 4 || s == 5) && (y < 3 || y > 5))
                            {
                                continue;
                            }
                            // for sqaures 6,7,8 y > 5
                            else if ((s == 6 || s == 7 || s == 8) && y < 6)
                            {
                                continue;
                            }
                            // if any column in the square is unoccupied, break
                            if (gridOccupied[x, y] == false)
                            {
                                occupied = false;
                                break;
                            }
                            else
                            {
                                occupiedSquares++;
                            }
                            // if we have reach the end of x and y without breaking, the square is full
                            if (occupiedSquares == 9)
                            {
                                Console.WriteLine("Sqaure " + s + " is full");
                                for (int i = 0; i < 3; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        // store coordinates of that are in the square to then re colour the coordinates later
                                        squares.Add(new Tuple<int, int>(x - i, y - j));
                                    }
                                }
                                numberOfCompleted++;
                            }
                        }
                    }
                }
            }
            // if anything has been completed, remove completed
            if (numberOfCompleted > 0)
            {
                removeCompleted();
            }
        }

        /**
         * Function to remove the completed rows once they have been found
         */
        void removeCompleted()
        {
            // colour rows
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(30); // pause between removing to animate slightly
                    gridOccupied[j, rows[i]] = false;
                    // set colour back to original
                    if ((rows[i] > 2 && rows[i] < 6 && (j < 3 || j > 5)) || (j > 2 && j < 6 && (rows[i] < 3 || rows[i] > 5)))
                    {
                        btn[j, rows[i]].BackColor = Color.White;
                    }
                    else
                    {
                        btn[j, rows[i]].BackColor = palePink;
                    }
                }
            }
            // change colours for all columns that are full
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(30);
                    gridOccupied[columns[i], j] = false;
                    // set column colour back to original
                    if ((j > 2 && j < 6 && (columns[i] < 3 || columns[i] > 5)) || (columns[i] > 2 && columns[i] < 6 && (j < 3 || j > 5)))
                    {
                        btn[columns[i], j].BackColor = Color.White;
                    }
                    else
                    {
                        btn[columns[i], j].BackColor = palePink;
                    }
                }
            }

            // change colours for all sqaures that are full
            for (int i = 0; i < squares.Count; i++)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(30);
                int x = squares[i].Item1;
                int y = squares[i].Item2;
                gridOccupied[x, y] = false;
                if ((x > 2 && x < 6 && y < 3) || (x < 3 && y > 2 && y < 6) || (x > 5 && y > 2 && y < 6) || (x > 2 && x < 6 && y > 5))
                {
                    btn[x, y].BackColor = Color.White;
                }
                else
                {
                    btn[x, y].BackColor = palePink;
                }
            }
        }

        /**
        * Calculate the total score by adding together the number of pieces in the shape, the combination, and the streak
        * Update the score label to display the score 
        */
        async void CalculateScore(Shape shape, int numComplete, int streak)
        {
            int score = 0;
            score += shape.getSize(); // add size of shape to score
            score += 18 * numComplete; // add completed/combo bonus
            if (streak > 1)
            {
                score += 27; // add 27 for a streak bonus
            }
            
            totalScore += score;
            lblScore.Text = totalScore.ToString();

            // if score is greater than zero, display label showing what it is increased by
            if (score > 0)
            {
                lblScoreIncrease.Text = "+ " + score.ToString();
                lblScoreIncrease.Visible = true;
                // add in time delay - got from this link - https://stackoverflow.com/questions/24136390/thread-sleep-without-freezing-the-ui#:~:text=The%20simplest%20way%20to%20use,asynchronous%20add%20the%20async%20modifier.&text=Now%20you%20can%20use%20the,asynchronous%20tasks%2C%20in%20your%20case.
                await Task.Delay(1000);
                lblScoreIncrease.Visible = false;
            }
        }

        /**
         * Event handler for exit button to exit the game
         */
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /**
         * Mute button to play or mute background music and sound effects
         */
        private void BtnMute_Click(object sender, EventArgs e)
        {
            // when clicked if playing, volume turns off, and image changes to muted image

            if (BtnMute.Text == "p")
            {
                Console.WriteLine("button pressed - muting");
                MXP.Ctlcontrols.stop();
                BtnMute.Text = "m";
                try
                {
                    // change image on button to muted
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
                PlayMediaPlayer();
                BtnMute.Text = "p";
                try
                {
                    // change image on button to playing
                    BtnMute.BackgroundImage = Properties.Resources.playing;
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine("Error finding image!");
                }
            }
        }

        private void PlayMediaPlayer()
        {

            if (bgPlaying == true)
            {

                background_music.Stop();
                bgPlaying = false;
            }

            MXP.Ctlcontrols.play();

        }
        // USed this tutorial to use MediaPlayer https://www.youtube.com/watch?v=Ch7yY3ti_aI
        private void PlayBackgroundSoundPlayer(object sender, EventArgs e)
        {
            background_music = new SoundPlayer(@"Brikudoku_Music.wav");
            background_music.PlayLooping();
            MXP.Ctlcontrols.stop(); // stop the media player
            bgPlaying = true; // set playing to true
        }

        /**
         * Check if a shape fits on the grid
         */
        bool CheckShapeFits(int number)
        {
            int xMod;
            int yMod;
            int count = 0; // the number of blocks that fit


            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    // for every block of the generated shape;
                    for (int i = 0; i < shapes[number].getSize(); i++)
                    {

                        xMod = shapes[number].getXOffset(i) / generatedSize; // gets the offset in terms of the 2d array, i.e +1/-1, +2/-2, 0 etc..
                        yMod = shapes[number].getYOffset(i) / generatedSize;

                        if (x + xMod >= 0 && x + xMod < 9 && y + yMod >= 0 && y + yMod < 9 && !gridOccupied[x + xMod, y + yMod]) // checks that the block is on the grid and the space isn't already occupied
                        {
                            count++;
                        }
                        if (count == shapes[number].getSize()) // if every block fits on the grid
                        {
                            if (shapes[number].getBlockAtIndex(0).BackColor == Color.Gray)
                            {
                                ColourShape(number);
                            }
                            //Console.WriteLine("true");
                            if (numberNotPlaceable != 0)
                            {
                                numberNotPlaceable--;
                            }

                            if (AI && AI2)
                            {
                                AiPlaceShape(x, y, shapes[number]); // if the AI is playing, the shape is placed in the available space
                                
                            }

                            return true; //placeable[i] == true
                        }
                    }
                    count = 0; // once the whole shape is checked for given values of x and y count is reset to 0
                }
            }
            GreyOutShape(number);
            numberNotPlaceable++;
            //Console.WriteLine("false");
            Console.WriteLine("Number not placeable: " + numberNotPlaceable);
            Console.WriteLine("Number of shapes: " + numberOfShapes);
            if (numberNotPlaceable == numberOfShapes) // if all remaining shapes are deemed not placeable, the game ends
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                //reset values back to 0 for Ai
                if (AI == true)
                {
                    numberOfShapes = 0;
                    numberNotPlaceable = 0;
                }
                DisplayGameOverScreen();
            }
            return false;
        }

        /**
         * Place shape in the AI version of the game
         */
        void AiPlaceShape(int x, int y, Shape shape)
        {
            int xMod;
            int yMod;
            int number = Int32.Parse(shape.getBlockAtIndex(0).Text);

            for (int i = 0; i < shape.getSize(); i++)
            {
                xMod = shape.getXOffset(i) / generatedSize;
                yMod = shape.getYOffset(i) / generatedSize;

                btn[x + xMod, y + yMod].BackColor = Color.Crimson;
                gridOccupied[x + xMod, y + yMod] = true; //places each block at its correct position relative to the central coordinates
                shape.getBlockAtIndex(i).Hide();
            }
            
            numberOfShapes--;
            numberNotPlaceable = 0;
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            CheckComplete();
            DisplayStreakAndCombo(numberOfCompleted);
            CalculateScore(shape, numberOfCompleted, streak);
            RegenerateShapes();
            AI2 = false; // temporarily set to false
            for (int i = 0; i < 3; i++)
            {
                // remove the shape we have  from the generated numbers array by resestting to zero
                if (placed[i] == false)
                {
                    // Console.WriteLine("Checking");
                    placeable[i] = CheckShapeFits(generatedNumbers[i]);
                }
            }
            AI2 = true;
        }

        /**
         * A shape is turned grey and its event handlers are removed
         */
        void GreyOutShape(int index)
        {
            for (int i = 0; i < shapes[index].getSize(); i++)
            {
                //Console.WriteLine("grey");
                shapes[index].getBlockAtIndex(i).BackColor = Color.Gray;
                shapes[index].getBlockAtIndex(i).ForeColor = Color.Gray;
                shapes[index].getBlockAtIndex(i).MouseMove -= BtnEvent_MouseMove;
                shapes[index].getBlockAtIndex(i).MouseUp -= BtnEvent_MouseUp;
                shapes[index].getBlockAtIndex(i).MouseDown -= BtnEvent_MouseDown;
            }
        }

        /**
         * A shape is coloured and its event handlers are added
         */
        void ColourShape(int index)
        {
            for (int i = 0; i < shapes[index].getSize(); i++)
            {
                shapes[index].getBlockAtIndex(i).BackColor = Color.Crimson;
                shapes[index].getBlockAtIndex(i).ForeColor = Color.Crimson;
                shapes[index].getBlockAtIndex(i).MouseMove += BtnEvent_MouseMove;
                shapes[index].getBlockAtIndex(i).MouseUp += BtnEvent_MouseUp;
                shapes[index].getBlockAtIndex(i).MouseDown += BtnEvent_MouseDown;
            }
        }

        /**
         * Displays a game over screen to the user and handles high score
         */
        async void DisplayGameOverScreen()
        {
            HideGrid();
            HideShapes();
            lblScore.Hide();
            lblDisplayScore.Hide();
            Label lblGameOver = new Label();
            Button btnPlayAgain = new Button();
            Button btnExit = new Button();
            Label lblGameOverScoreHeader = new Label(); // all the GUI elements for the game over screen are initialised
            Label[] lblHighScores = new Label[5];
            Label lblPlayerFinalScore = new Label();
            Label lblEnterName = new Label();
            TextBox txtBoxUserName = new TextBox();
            string highScoresText = System.IO.File.ReadAllText("..\\..\\HighScores.txt"); // high score data is read from file, found: https://stackoverflow.com/questions/7569904/easiest-way-to-read-from-and-write-to-files
            string[] highScoreUsers = new string[5];
            int[] highScoreScores = new int[5];
            int newHighScoreIndex = -1;
            int j = 0;
            int k = 0;

            string[] fileTextArr = highScoresText.Split(':'); // splits the file data into individual usernames and scores

            for (int i = 0; i < fileTextArr.Length; i++)
            {
                Console.WriteLine(fileTextArr[i]);
                if (fileTextArr[i] != "---" && fileTextArr[i] != "0") // checks if file data is null
                {
                    if (i % 2 == 0) // even/odd indexes are used to distinguish the usernames and scores in the file data array
                    {
                        highScoreUsers[j] = fileTextArr[i];
                        j++;
                    }
                    else
                    {
                        highScoreScores[k] = int.Parse(fileTextArr[i]);
                        k++;
                    }
                }
                else if (i % 2 == 0)
                {
                    highScoreUsers[j] = "---"; //usernames are set to a basic value if not found in the file
                    j++;
                }
                else
                {
                    highScoreScores[k] = 0; //scores are set to a basic value if not found in the file
                    k++;
                }
            }

            Controls.Add(lblGameOver);
            lblGameOver.Font = new System.Drawing.Font("OCR A Extended", 45F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblGameOver.Text = "GAME OVER";
            lblGameOver.ForeColor = System.Drawing.Color.Crimson;
            lblGameOver.SetBounds(320, 50, 500, 100);

            Controls.Add(btnExit);
            btnExit.Font = new System.Drawing.Font("OCR A Extended", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnExit.Text = "Exit";
            btnExit.BackColor = Color.LightPink;
            btnExit.SetBounds(495, 500, 100, 75);
            btnExit.Click += new EventHandler(this.BtnExit_Click);

            Controls.Add(lblGameOverScoreHeader);
            lblGameOverScoreHeader.Font = new System.Drawing.Font("OCR A Extended", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))); // basic styling for game over GUI elements
            lblGameOverScoreHeader.Text = "Scores:";
            lblGameOverScoreHeader.ForeColor = System.Drawing.Color.Crimson;
            lblGameOverScoreHeader.SetBounds(160, 140, 200, 50);

            for (int i = 0; i < lblHighScores.Length; i++)
            {
                lblHighScores[i] = new Label();
                Controls.Add(lblHighScores[i]);
                lblHighScores[i].Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))); // high scores are added to the GUI
                DisplayHighScoreData(lblHighScores[i], highScoreUsers[i], highScoreScores[i]);
                lblHighScores[i].ForeColor = System.Drawing.Color.Crimson;
                lblHighScores[i].SetBounds(160, 190 + ((i * 50)), 200, 50);

            }

            if (AI == false)
            {
                Controls.Add(btnPlayAgain);
                btnPlayAgain.Font = new System.Drawing.Font("OCR A Extended", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                btnPlayAgain.Text = "Play Again";
                btnPlayAgain.BackColor = Color.LightPink;
                btnPlayAgain.SetBounds(385, 500, 100, 75);
                btnPlayAgain.Click += (sender, e) => BtnPlayAgain_Click(sender, e, lblGameOver, btnPlayAgain, btnExit, lblPlayerFinalScore, lblHighScores, lblGameOverScoreHeader);

            }

            Controls.Add(lblPlayerFinalScore);
            lblPlayerFinalScore.Font = new System.Drawing.Font("OCR A Extended", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblPlayerFinalScore.Text = "Score: " + totalScore;
            lblPlayerFinalScore.ForeColor = System.Drawing.Color.Crimson;
            lblPlayerFinalScore.SetBounds(600, 250, 400, 100);

            if (totalScore > highScoreScores[highScoreScores.Length - 1]) // checks if player score is greater than the lowest saved high score
            {
                newHighScoreIndex = highScoreScores.Length - 1;
                for (int i = 0; i < highScoreScores.Length - 1; i++)
                {
                    if (totalScore > highScoreScores[i]) // cycles through remaining saved high scores searching for the highest high score it beats
                    {
                        newHighScoreIndex = i;
                        break;
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    lblHighScores[newHighScoreIndex].Hide(); // score to be replaced with the new high score flashes briefly
                    await Task.Delay(300);
                    lblHighScores[newHighScoreIndex].Show();
                    await Task.Delay(300);
                }
                for (int i = highScoreScores.Length - 1; i >= newHighScoreIndex; i--)
                {
                    if (i == newHighScoreIndex)
                    {
                        highScoreScores[i] = totalScore; // score to be replaced is changed
                        DisplayHighScoreData(lblHighScores[i], "---", totalScore); // name is tempoorarily made blank
                    }
                    else
                    {
                        highScoreScores[i] = highScoreScores[i - 1];
                        highScoreUsers[i] = highScoreUsers[i - 1]; // scores below the new high score are moved down the list with the last one being replaced
                        DisplayHighScoreData(lblHighScores[i], highScoreUsers[i], highScoreScores[i]);
                    }
                }

                Controls.Add(lblEnterName);
                lblEnterName.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                lblEnterName.ForeColor = System.Drawing.Color.Crimson;
                lblEnterName.Text = "ENTER NAME";
                lblEnterName.SetBounds(410, 225, 200, 50);

                Controls.Add(txtBoxUserName);
                txtBoxUserName.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                txtBoxUserName.ForeColor = System.Drawing.Color.Crimson;
                txtBoxUserName.MaxLength = 3; // a text box for the user to enter their name with maximum length 3
                txtBoxUserName.TextAlign = HorizontalAlignment.Center;
                txtBoxUserName.SetBounds(460, 275, 75, 100);
                txtBoxUserName.KeyDown += (sender, e) => TxtBoxUserName_Enter(sender, e, lblEnterName, lblHighScores, totalScore, highScoreScores, newHighScoreIndex, btnPlayAgain);

            }
        }

        /**
         * Displays the username and score of a given high score with correct formatting
         */
        void DisplayHighScoreData(Label lbl, string name, int score)
        {
            if (score == 0)
            {
                lbl.Text = name + " : 00000";
            }
            else if (score < 10000 && score >= 1000)
            {
                lbl.Text = name + " : 0" + score;
            }
            else if (score < 1000 && score >= 100)
            {
                lbl.Text = name + " : 00" + score;
            }
            else if (score < 100 && score >= 10)
            {
                lbl.Text = name + " : 000" + score;
            }
            else
            {
                lbl.Text = name + " : 0000" + score;
            }
        }

        /**
         * Event listener for when the enter key is pressed after entering your name
         */
        void TxtBoxUserName_Enter(object sender, KeyEventArgs e, Label lbl, Label[] lblName, int score, int[] highScoreScores, int index, Button playAgain)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ((Control)sender).Hide();
                lbl.Hide();
                DisplayHighScoreData(lblName[index], ((Control)sender).Text, score);

                int currentScore;

                string fileOutputText = "";
                for (int i = 0; i < highScoreScores.Length; i++)
                {
                    string currentName = lblName[i].Text.Substring(0, 3); // gets the name from the label text
                    Console.WriteLine("name: " + currentName);
                    currentScore = highScoreScores[i];
                    fileOutputText = fileOutputText + currentName + ":" + currentScore + ":"; // appends file output text
                }
                fileOutputText = fileOutputText.Remove(fileOutputText.Length - 1, 1); // code from https://www.c-sharpcorner.com/blogs/remove-last-character-from-string-in-c-sharp1
                System.IO.File.WriteAllText("..\\..\\HighScores.txt", fileOutputText); // writes to file

            }
        }

        /**
         * Event handler for play again button
         * When play again button is clicked, hide the game over screen, reset all data back to initial values and display the game screen
         */
        private void BtnPlayAgain_Click(object sender, EventArgs e, Label lblGameOver, Button btnPlayAgain, Button btnExit, Label finalScore, Label[] highScores, Label gameOverHead)
        {
            // when start button is clicked, hide all the items in the current menu screen
            // and display the grid game form
            btnExit.Hide();
            btnPlayAgain.Hide();
            lblGameOver.Hide();
            finalScore.Hide();
            gameOverHead.Hide();
            foreach (Label l in highScores)
            {
                l.Hide();
            }
            InitialiseGridOccupancy(); // reset occupancy
            this.BackColor = Color.Linen;
            // make playable again
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    // redisplay grid with correct colours
                    if ((x > 2 && x < 6 && y < 3) || (x < 3 && y > 2 && y < 6) || (x > 5 && y > 2 && y < 6) || (x > 2 && x < 6 && y > 5))
                    {
                        btn[x, y].BackColor = Color.White;
                    }
                    else
                    {
                        btn[x, y].BackColor = palePink;
                    }
                    btn[x, y].Show();
                }
            }
            // reset all placeable to true
            for (int i = 0; i < placeable.Length; i++)
            {
                placeable[i] = true;
            }
            // reset all generated numbers to 0
            for (int i = 0; i < generatedNumbers.Length; i++)
            {
                generatedNumbers[i] = -1;
            }
            numberOfShapes = 0;
            totalScore = 0;
            streak = 0;
            dragging = false;
            draggedShape = null;
            numberOfCompleted = 0;
            lblScore.Text = "0";
            lblScore.Show();
            lblDisplayScore.Show();
            InitialiseGridOccupancy();
            GenerateRandomShapeNumbers();
        }

        /**
         * Hide all buttons in the grid
         */
        void HideGrid()
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    btn[x, y].Hide();
                }
            }
        }

        /**
         * Hide all shapes that were leftover at the end of the previous game
         */
        void HideShapes()
        {
            for (int i = 0; i < generatedNumbers.Length; i++)
            {
                Shape shape = shapes[generatedNumbers[i]];
                for (int j = 0; j < shape.getSize(); j++)
                {
                    // if not already hidden, hide
                    if (shape.getBlockAtIndex(j).Visible == true)
                    {
                        shape.getBlockAtIndex(j).Visible = false;
                        Controls.Remove(shape.getBlockAtIndex(j));
                        shape.getBlockAtIndex(j).MouseMove -= new MouseEventHandler(this.BtnEvent_MouseMove);
                        shape.getBlockAtIndex(j).MouseUp -= new MouseEventHandler(this.BtnEvent_MouseUp);
                        shape.getBlockAtIndex(j).MouseUp -= new MouseEventHandler(this.BtnEvent_MouseDown);
                    }
                }
            }
        }


        /**
         * Exit option in the menu to close the program
         */
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /**
         * Event handler for about option in the menu
         * Shows a message box with basic information about the game
         */
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Brickudoku by Adam Munro, Marylou Das Chagas e Silva and Laura Clark, 2024", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /**
         * Set up all needed variables to display the rules 
         */
        void RulesSetUp()
        {
            RulesForm.AutoScroll = true; // make scroll automatically

            RulesForm.Icon = Properties.Resources.BrickudokuIcon;
            RulesForm.Text = "Brickudoku Rules";
            RulesForm.BackColor = Color.Linen;
            RulesForm.MaximizeBox = false;
            RulesForm.MinimizeBox = false;
            RulesForm.MaximumSize = new System.Drawing.Size(800, 500);
            RulesForm.MinimumSize = new System.Drawing.Size(800, 500);
            // center to center of the parent form
            RulesForm.StartPosition = FormStartPosition.CenterParent;

            Label LblRulesTitle = new Label();
            LblRulesTitle.Font = new System.Drawing.Font("OCR A Extended", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblRulesTitle.Text = "Brickudoku Rules";
            LblRulesTitle.ForeColor = System.Drawing.Color.Crimson;
            LblRulesTitle.SetBounds(50, 20, 700, 50);
            RulesForm.Controls.Add(LblRulesTitle);

           

            // gameplay
            Label LblPlayTitle = new Label();
            LblPlayTitle.Text = "Playing the Game: ";
            LblPlayTitle.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblPlayTitle.ForeColor = System.Drawing.Color.Crimson;
            LblPlayTitle.SetBounds(50, 100, 400, 30);
            RulesForm.Controls.Add(LblPlayTitle);

            Label LblPlay = new Label();
            LblPlay.Font = new System.Drawing.Font("Palatino Linotype", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblPlay.ForeColor = System.Drawing.Color.Black;
            LblPlay.SetBounds(50, 140, 700, 100);
            LblPlay.Text = "Once you have started a new game, 3 different blocks will be automatically generated at the Left Hand Side. To place the blocks you have two options: 1. Click on a block and drag it to the point on the grid where you would like to place it. 2. Click on a block and hover over the location on the grid where you would like to place it. The location the shape will be placed to will be highlighted pink. Click the grid at a position where the shape is fully placeable to place it.";
            RulesForm.Controls.Add(LblPlay);

            Label LblPlayContinued = new Label();
            LblPlayContinued.Font = new System.Drawing.Font("Palatino Linotype", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblPlayContinued.ForeColor = System.Drawing.Color.Black;
            LblPlayContinued.SetBounds(50, 240, 700, 50);
            LblPlayContinued.Text = " Continue to place the rest of your blocks. Once you have used all of your shapes, 3 new shapes for you to place will be generated. The aim of the game is to get as high a score as possible.";
            RulesForm.Controls.Add(LblPlayContinued);

            // points
            Label LblPointsTitle = new Label();
            LblPointsTitle.Text = "Scoring Points: ";
            LblPointsTitle.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblPointsTitle.ForeColor = System.Drawing.Color.Crimson;
            LblPointsTitle.SetBounds(50, 300, 400, 30);
            RulesForm.Controls.Add(LblPointsTitle); 

            Label LblPoints = new Label(); 
            LblPoints.Font = new System.Drawing.Font("Palatino Linotype", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblPoints.ForeColor = System.Drawing.Color.Black;
            LblPoints.SetBounds(50, 340, 700, 100);
            LblPoints.Text = "To score points, place blocks on the grid. For more points, aim to complete rows, columns and squares. A complete sqaure must be a 3x3 sqaure on one of the squares outlined in the grid background. You can score extra points by getting a combo or a streak. A combo is when you clear multiple rows, columns or squares in one turn. A streak is when you complete a row, column or square multiple turns in a row. See the pictures below for examples of a row, column, square and combo: ";
            RulesForm.Controls.Add(LblPoints);

            CreateGamePlayImages(); // images to demonstrate gameplay

            // end game
            Label LblEndTitle = new Label(); 
            LblEndTitle.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblEndTitle.ForeColor = System.Drawing.Color.Crimson;
            LblEndTitle.SetBounds(50, 630, 400, 30);
            LblEndTitle.Text = "Ending the Game: ";
            RulesForm.Controls.Add(LblEndTitle);

            Label LblEnd = new Label(); 
            LblEnd.Font = new System.Drawing.Font("Palatino Linotype", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblEnd.ForeColor = System.Drawing.Color.Black;
            LblEnd.SetBounds(50, 670, 700, 150);
            LblEnd.Text = "The game will automatically end when you have no pieces left to play that will fit. Pieces that generate and will not fit are coloured grey and you are unable to move them. The game checks after each piece played and updates which pieces are playable. If you have no playable pieces, the game ends. Once the game ends you will see a game over screen with your score, previous high scores, and an option to exit or play the game again. If you have made it onto the top 5 high scores, the list will be updated and your score will be added to the high scores list. You can also choose to exit the game at any point by choosing the menu option or clicking the cross.";
            RulesForm.Controls.Add(LblEnd);

            // sound
            Label LblSoundTitle = new Label();
            LblSoundTitle.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblSoundTitle.ForeColor = System.Drawing.Color.Crimson;
            LblSoundTitle.SetBounds(50, 840, 500, 30);
            LblSoundTitle.Text = "Music and Sound Effects: ";
            RulesForm.Controls.Add(LblSoundTitle);

            Label LblSound = new Label();
            LblSound.Font = new System.Drawing.Font("Palatino Linotype", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblSound.ForeColor = System.Drawing.Color.Black;
            LblSound.SetBounds(50, 880, 700, 60);
            LblSound.Text = "The background music that is playing can be muted and un-muted at any point in the game by clicking the mute button in the top right hand corner of the screen. Muting the background music will also mute the sound effects that happen during gameplay.";
            RulesForm.Controls.Add(LblSound);

            // AI
            Label LblAITitle = new Label();
            LblAITitle.Font = new System.Drawing.Font("OCR A Extended", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblAITitle.ForeColor = System.Drawing.Color.Crimson;
            LblAITitle.SetBounds(50, 960, 400, 30);
            LblAITitle.Text = "AI Mode: ";
            RulesForm.Controls.Add(LblAITitle);

            Label LblAI = new Label();
            LblAI.Font = new System.Drawing.Font("Palatino Linotype", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            LblAI.ForeColor = System.Drawing.Color.Black;
            LblAI.SetBounds(50, 1000, 700, 60);
            LblAI.Text = "The AI mode can be selected to play from the starting screen. The AI will play automatically, trying to get as high a score as possible. The AI is a very basic initial version and it doesn't always work!";
            RulesForm.Controls.Add(LblAI);

            // create back button
            Button BtnRulesClose = new Button();
            BtnRulesClose.Text = "Close Rules";
            BtnRulesClose.SetBounds(50, 1100, 125, 50);
            BtnRulesClose.Font = new System.Drawing.Font("OCR A Extended", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            BtnRulesClose.BackColor = Color.LightPink;
            BtnRulesClose.Click += new EventHandler(this.BtnRulesClose_Click);
            RulesForm.Controls.Add(BtnRulesClose);

            // add blank space underneath
            Label LblSpacing = new Label();
            LblSpacing.SetBounds(0, 1160, 800, 30);
            RulesForm.Controls.Add(LblSpacing);
        }

        /**
         * Event handler for button to close the rules page
         */
        private void BtnRulesClose_Click(object sender, EventArgs e)
        {
            RulesForm.Close();
        }

        /**
         * Event handler to show the rules form when rules menu option is clicked
         */
        private void RulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show the rules form
            RulesForm.ShowDialog();
        }

        /**
         * Function to set and place the gamepleay images
         */
        void CreateGamePlayImages()
        {
            PictureBox rowImage = new PictureBox();
            rowImage.Image = Properties.Resources.row;
            rowImage.SetBounds(50, 450, 150, 150);
            rowImage.SizeMode = PictureBoxSizeMode.StretchImage;
            RulesForm.Controls.Add(rowImage);

            PictureBox colImage = new PictureBox();
            colImage.Image = Properties.Resources.column;
            colImage.SetBounds(230, 450, 150, 150);
            colImage.SizeMode = PictureBoxSizeMode.StretchImage;
            RulesForm.Controls.Add(colImage);

            PictureBox squareImage = new PictureBox();
            squareImage.Image = Properties.Resources.square;
            squareImage.SetBounds(410, 450, 150, 150);
            squareImage.SizeMode = PictureBoxSizeMode.StretchImage;
            RulesForm.Controls.Add(squareImage);

            PictureBox comboImage = new PictureBox();
            comboImage.Image = Properties.Resources.combo;
            comboImage.SetBounds(590, 450, 150, 150);
            comboImage.SizeMode = PictureBoxSizeMode.StretchImage;
            RulesForm.Controls.Add(comboImage);
        }

        /**
         * Event handler for the AI button
         * When AI button clicked launch the AI version of the game
         */
        private void BtnAI_Click(object sender, EventArgs e)
        {
            // initialise game the same as start game does
            BtnExit.Hide();
            BtnStart.Hide();
            BtnAI.Hide();
            AI = true; // so other parts of the game know that it is playing the AI version
            AI2 = true;
            this.BackgroundImage = null; // remove image from gameplay screen
            this.BackColor = Color.Linen;
            menuStrip1.BackColor = Color.Linen;
            CreateGrid();
            SetUpLabels();
            InitialiseGridOccupancy();
            GenerateRandomShapeNumbers();
        }
    }
}

