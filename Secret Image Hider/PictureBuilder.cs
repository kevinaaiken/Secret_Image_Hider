using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Secret_Image_Hider
{
    class PictureBuilder
    {
        //fields
        public Bitmap ppmBitmap {get; set;}
        
        //methods
        public Bitmap MakePPM(string[] ppm_info)
        {
            string[] wh =ppm_info[2].Split();

            int width = Int16.Parse(wh[0]);
            int height = Int16.Parse(wh[1]);
            
            ppmBitmap = new Bitmap(width, height);

            int i = 4;
            for (int y = 0; y < height; y ++){
                for (int x = 0; x < width; x ++){

                    Color newColor = Color.FromArgb(Int16.Parse(ppm_info[i]), Int16.Parse(ppm_info[i+1]), Int16.Parse(ppm_info[i+2]));
                    ppmBitmap.SetPixel(x, y, newColor);

                    i += 3;
                }//end for width
            }//end for height
            
            return ppmBitmap;
        }//end load ppm method


        public Bitmap LoadPPM(string text_info, int start_x, int start_y)
        {
            bool setup = true;
            bool key_set = false;
            bool len_set = false;
            bool msg_set = false;
            Color changedColor;
            int i = 0;
            
            Bitmap ppmBitmap1 = (Bitmap)ppmBitmap.Clone();

            //hide message
            for (int y = 0; y < ppmBitmap1.Height; y ++){
                for (int x = 0; x < ppmBitmap1.Width; x ++){

                    Color col_pixel = ppmBitmap1.GetPixel(x, y);
                    
                    //FIRST
                    //make pixels odd (0+1 or 254+1) till reach selected coords
                    if (setup){
                        if (col_pixel.B % 2 == 0)
                        {
                            changedColor = Color.FromArgb(col_pixel.R, col_pixel.G, (col_pixel.B + 1));
                            ppmBitmap1.SetPixel(x, y, changedColor);
                            if (x == start_x && y == start_y)
                            {
                                setup = false;
                                key_set = true;
                            }
                        }else{
                            if (x == start_x && y == start_y)
                            {
                                setup = false;
                                key_set = true;
                            }
                        }
                    //SECOND
                    //make pixel even (1-1 or 255-1)
                    }else if (key_set){
                        if (col_pixel.B % 2 == 1){
                            changedColor = Color.FromArgb(col_pixel.R, col_pixel.G, col_pixel.B - 1);
                            ppmBitmap1.SetPixel(x, y, changedColor);
                            key_set = false;
                            len_set = true;
                        }else{
                            key_set = false;
                            len_set = true;
                        }
                    //THIRD
                    }else if (len_set){ 
                        if (i < text_info.Length){
                            changedColor = Color.FromArgb(col_pixel.R, col_pixel.G, (int)text_info.Length);
                            ppmBitmap1.SetPixel(x, y, changedColor);
                            len_set = false;
                            msg_set = true;
                        }
                    //FOURTH
                    }else if (msg_set){ 
                        if (i < text_info.Length){
                            changedColor = Color.FromArgb(col_pixel.R, col_pixel.G, (int)text_info[i]);
                            ppmBitmap1.SetPixel(x, y, changedColor);
                            i += 1;
                        }else{
                            return ppmBitmap1;
                        }
                    }
                    
                }//end for width
            }//end for height

            return ppmBitmap1;
        }//end make ppm method


        public string[] SavePPM(Bitmap pic)
        {
            string[] aryText = new string[((pic.Width * pic.Height) * 3) + 4];

            aryText[0] = "P3";
            aryText[1] = "# Created by GIMP version 2.10.8 PNM plug-in";
            aryText[2] = String.Format("{0} {1}", pic.Width, pic.Height);
            aryText[3] = "255";

            int i = 4;
            for (int y = 0; y < pic.Height; y ++){
                for (int x = 0; x < pic.Width; x ++){

                    Color col_pixel = pic.GetPixel(x, y);

                    aryText[i] = Convert.ToString(col_pixel.R);
                    aryText[i + 1] = Convert.ToString(col_pixel.G);
                    aryText[i + 2] = Convert.ToString(col_pixel.B);

                    i += 3;
                }//end for width
            }//end for height

            return aryText;
        }//end save ppm method


        //NOTES FOR FUTURE IMPLEMENTATION

        //have each ascii chara be split into 3 parts (possibly more....) and each part is put in the last digit of the red channel
        //concatenate them to make a chara, so the hidden message length will be split into 3 parts (possibly more....)
        //if a value is >250 skip it, or get it to 240
        
    }//class
}//name
