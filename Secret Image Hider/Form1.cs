using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Secret_Image_Hider
{
    public partial class Form1 : Form
    {
        //fields
        PictureBuilder picture;  //global instance of class
        Bitmap firstPic;
        Bitmap secondPic;
        string[] info;
        double click_x = 0;
        double click_y = 0;
        int coord_x;
        int coord_y;


        //events
        public Form1()
        {
            InitializeComponent();
            picture = new PictureBuilder();  //initialize class
        }


        private void importImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {

                openFile.InitialDirectory = "C:\\temp\\";
                openFile.Filter = "PPM P3 ASCII (*.ppm)|*.ppm|PPM P6 RAW (*.ppm)|*.ppm";
                openFile.FilterIndex = 1;
                openFile.RestoreDirectory = true;

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    switch (openFile.FilterIndex)
                    {
                        case 1:
                            info = File.ReadAllLines(openFile.FileName).ToArray();
                            firstPic = picture.MakePPM(info);
                            break;

                        case 2:
                            info = File.ReadAllText(openFile.FileName).Split(' ').ToArray();
                            //convert the bytes to ints
                            firstPic = picture.MakePPM(info);
                            break;
                    }

                    pictureBox1.Image = firstPic;
                    label1.Text = "Select Position On Image";
                    pictureBox1.Cursor = Cursors.Cross;
                }

            }//end using opendialog box
        }//end import



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //treat everything as a double while calculating coordinates for accuracy then return as an int
            coord_x = (int)((click_x * (double)firstPic.Width) / (double)pictureBox1.Width);
            coord_y = (int)((click_y * (double)firstPic.Height) / (double)pictureBox1.Height);

            textBox1.Text = String.Format("X = {0}, Y = {1}", coord_x, coord_y);

            textBox2.ReadOnly = false;
            label1.Text = "Type Message";
        }//end picturebox1 click get coordinates


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            label1.Text = "Click Encode to Integrate Message";
        }//end textbox1 text changed


        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            secondPic = picture.LoadPPM(textBox2.Text, coord_x, coord_y);
            pictureBox2.Image = secondPic;
        }//end button1 click


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFile = new SaveFileDialog())
            {
                saveFile.InitialDirectory = "C:\\temp\\";
                saveFile.Filter = "Portable PicMap (*.ppm)|*.ppm|Bitmap Image (*.bmp)|*.bmp";
                saveFile.Title = "Save an Image File";
                saveFile.FilterIndex = 1;
                saveFile.RestoreDirectory = true;
                
                if(saveFile.ShowDialog() == DialogResult.OK){
                    if (saveFile.FileName != "")
                    {
                        switch (saveFile.FilterIndex)
                        {
                            case 1:
                                string[] save_text = picture.SavePPM((Bitmap)pictureBox2.Image);
                                File.WriteAllLines(saveFile.FileName, save_text);
                                break;

                            case 2:
                                secondPic.Save(saveFile.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;  
                        }
                    }
                }
            }
        }//end save


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            click_x = e.X;
            click_y = e.Y;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }//class
}//name

//instructions wrongly change if load picture is canceled