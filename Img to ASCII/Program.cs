using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Runtime.InteropServices;
using System.ComponentModel.Design;

namespace Img_to_ASCII
{
    internal class Program
    {
        //static string betuk = "Ñ@#W$9876543210?!abc;:+=-,._       ";
        static string betuk = "     _.,-=+:;cba!?01234hncmkfdgxbvnxndgjgksrjyzdh56789$W#@Ñ";
        static char[,] charsMaped;
        static Bitmap bitmap;
        public static int GeneratedPicSize = 400;
        public static bool eredetiNagysag = false;
        public static string picPath;

        static void Main(string[] args)
        {
            if (args.Length > 0 && args.Length < 4)
            {
               // Console.Write("Nem jooo!");
                picPath = args[0];
                picPath.Trim();
                if (picPath[0] == '\"')picPath = picPath.Replace("\"", "");

                Init(true,false,false);


                for (int i = 1; i < args.Length; i++)
                {
                    if (i == 1)
                    {
                        string seged = args[i];
                        if (!bool.TryParse(seged, out eredetiNagysag))
                        {
                            if (seged == "yes" || seged == "y" || seged == "Y")
                            {
                                eredetiNagysag = true;
                                break;
                            }
                            else if (seged == "no" || seged == "n" || seged == "N")
                            {
                                eredetiNagysag = false;
                            }
                            else Console.WriteLine("not a valid response! Defaulting to no!");

                            if (args.Length < 2)
                            {
                                Init(false, false, true);
                            }
                        }

                    }
                    

                    else if (i == 2)
                    {
                        if (!int.TryParse(args[2], out GeneratedPicSize))
                        {
                            Console.WriteLine("Invalid Custom pic size!Setting to default 100*100");
                            GeneratedPicSize = 100;
                        }
                        LoadImage(picPath);
                    }
                }
            }
            else
            {
                Init(true,true,true);
                
            }
           
            charsMaped = new char[bitmap.Height, bitmap.Width];
            ProcessImage();
            Kiir();
            Console.ReadKey();
        }
        /// <summary>
        /// initialize
        /// </summary>
        /// <param name="a">paht select</param>
        /// <param name="b">EredetiNagyasg </ param>
        /// <param name="c">Custom size If b is false!</param>
        private static void Init(bool a,bool b,bool c)
        {
            ///a harom boolt a harom szekcio keresere fogom hasznalni
            ///lesz 3 whileom a 3 ifembe 

            if (a)
            {
                while (!LoadImage(picPath))
                {
                    Console.WriteLine("Please enter a valid path for your picture!");
                    picPath = Console.ReadLine();
                    picPath.Trim();
                    if (picPath[0] == '\"')
                    {
                       
                       picPath= picPath.Replace("\"","");
                    }
                        
                }
            }
           

            if(b )
            {
                Console.WriteLine("Do you want to output with the original size? [y/n]");
                string seged = Console.ReadLine();
                while (!bool.TryParse(seged, out eredetiNagysag) )
                {
                    if (seged == "yes"||seged=="y"||seged=="Y") {
                        eredetiNagysag = true;
                        break;
                    }
                    else if (seged == "no"||seged=="n"||seged=="N")
                    {
                        eredetiNagysag= false;
                        break;
                    }
                    Console.WriteLine("Invalid response please specify yes or no [y/n]");
                    
                }
            }

            if (c && !eredetiNagysag)
            {
                Console.WriteLine("Then please specify the wanted size NxN");
             
                while (!int.TryParse(Console.ReadLine(),out GeneratedPicSize))
                {
                    Console.WriteLine("Invalid input ,pleas inputr an integer number!");
                }
              

            }

            LoadImage(picPath);
        }

        private static void Kiir()
        {
            StreamWriter ki = new StreamWriter("ki.txt");
            for (int i = 0; i < charsMaped.GetLength(0); i++)
            {
                for (int j = 0; j < charsMaped.GetLength(1); j++)
                {
                    Console.Write(charsMaped[j, i]);
                    ki.Write(charsMaped[j, i]);

                }
                Console.WriteLine();
                ki.WriteLine();

            }
            ki.Close();
        }

        private static void ProcessImage()
        {
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    int bright;
                    // int pixelBerightnes = (j + i * bitmap.Width) * 4;

                    double r = bitmap.GetPixel(i, j).R;
                    double g = bitmap.GetPixel(i, j).G;
                    double b = bitmap.GetPixel(i, j).B;


                    bright = (int)(r + g + b) / 3;

                    charsMaped[i, j] = getDensity(bright);
                }
            }
        }

        private static bool LoadImage(string picPath)
        {
            Image img;
            try
            {
                img = Image.FromFile(picPath);
            }
            catch (Exception e)
            {
                return false;
            }

            Size i = img.Size;
            int h = Math.Min(i.Width, i.Height);
            Console.WriteLine(i);

            if (eredetiNagysag)
                bitmap = new Bitmap(img, new Size(h, h));
            else
                bitmap = new Bitmap(img, new Size(GeneratedPicSize, GeneratedPicSize));


            return true;

        }

        static char getDensity(int value)
        {
            // Since we don't have 255 characters, we have to use percentages
            int charValue = map(value, 0, 255, 0, betuk.Length - 1);
           
            return betuk[charValue];
        }

        private static int map(int value, int fromLow, int fromHigh, int toLow, int toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }
    }


}