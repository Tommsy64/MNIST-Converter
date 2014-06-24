using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MnistConverter
{
    public class MnistConverterEngine
    {
        private void SaveList(IEnumerable<CharAndImage> vList, string path)
        {
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            CounterList<char> counter = new CounterList<char>();
            foreach (CharAndImage item in vList)
            {
                counter.Put(item.Char);
                try
                {
                    item.Image.Save(Path.Combine(path, (int)item.Char + "_" + counter.GetValue(item.Char) + ".bmp"));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "An error has occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0, true);
                }
            }
        }

        private List<Image> MakeImageList(List<int> pixelList, int numberOfImages, int columnNumber, int rowNumber)
        {
            List<Image> rImageList = new List<Image>();
            if (pixelList != null)
            {
                Bitmap vBitmap;
                int vPixelCounter = 0;
                while (true)
                {
                    bool setBreak = false;
                    vBitmap = new Bitmap(columnNumber, rowNumber);
                    for (int y = 0; y < rowNumber; y++)
                    {
                        for (int x = 0; x < columnNumber; x++)
                        {
                            if (vPixelCounter >= pixelList.Count)
                            {
                                setBreak = true;
                                break;
                            }
                            int colorUint = (int)pixelList[vPixelCounter];
                            if (colorUint < 0)
                            {
                                colorUint = 0;
                            }

                            Color newColor = Color.FromArgb(colorUint, colorUint, colorUint);
                            vBitmap.SetPixel(x, y, newColor);
                            vPixelCounter++;
                        }
                        if (setBreak)
                        {
                            break;
                        }
                    }

                    rImageList.Add(vBitmap);
                    if (setBreak)
                    {
                        vBitmap.Save("LastImage.bmp");
                        break;
                    }
                }
            }
            return rImageList;
        }


        private string GetLabelFile(string iPath, bool isTrainingData)
        {
            return Path.Combine(iPath, isTrainingData ? "train-labels.idx1-ubyte" : "t10k-labels.idx1-ubyte");
        }
        private string GetImageFile(string iPath, bool isTrainingData)
        {
            return Path.Combine(iPath, isTrainingData ? "train-images.idx3-ubyte" : "t10k-images.idx3-ubyte");
        }

        List<Image> imageBitmapList = new List<Image>();
        List<char> imageCharList = new List<char>();

        public bool ConvertMnist(string path, bool isTrainData)
        {
            string labelFile = GetLabelFile(path, isTrainData);
            string imageFile = GetImageFile(path, isTrainData);
            string subFolder = Path.GetFileNameWithoutExtension(labelFile);
            if (File.Exists(GetLabelFile(path, isTrainData)) && File.Exists(GetImageFile(path, isTrainData)))
            {
                #region getting image
                using (StreamReader reader = new StreamReader(imageFile, Encoding.ASCII))
                {
                    char[] buffer4 = new char[4];
                    char[] buffer1 = new char[1];

                    List<int> pixelList = new List<int>();

                    reader.Read(buffer4, 0, buffer4.Length);
                    var magivnumber = buffer4;

                    reader.Read(buffer4, 0, buffer4.Length);
                    int number1 = Convert.ToInt32(buffer4[buffer4.Length - 1]);
                    int number2 = Convert.ToInt32(buffer4[buffer4.Length - 2]);

                    int vNumberOfImages = int.Parse(number2.ToString() + number1.ToString());

                    reader.Read(buffer4, 0, buffer4.Length);
                    int vNumberOfRows = buffer4[buffer4.Length - 1];

                    reader.Read(buffer4, 0, buffer4.Length);
                    int vNumberOfColumns = buffer4[buffer4.Length - 1];


                    while (reader.Read(buffer1, 0, buffer1.Length) > 0)
                        pixelList.Add(buffer1[0] * 2);

                    imageBitmapList = MakeImageList(pixelList, vNumberOfImages, vNumberOfColumns, vNumberOfRows);
                }
                #endregion
                #region getting labels
                using (StreamReader reader = new StreamReader(labelFile, Encoding.Default))
                {
                    char[] buffer4 = new char[4];
                    char[] buffer1 = new char[1];

                    reader.Read(buffer4, 0, buffer4.Length);
                    var magivnumber = buffer4;

                    reader.Read(buffer4, 0, buffer4.Length);
                    int number1 = Convert.ToInt32(buffer4[buffer4.Length - 1]);
                    int number2 = Convert.ToInt32(buffer4[buffer4.Length - 2]);

                    int vNumberOfImages = int.Parse(number2.ToString() + number1.ToString());

                    while (reader.Read(buffer1, 0, buffer1.Length) > 0)
                        imageCharList.Add(buffer1[0]);
                }
                #endregion

                IEnumerable<CharAndImage> vList = imageCharList.Zip(imageBitmapList, (c, i) => new CharAndImage { Image = i, Char = c });

                String vFolder = Path.Combine(path, subFolder);
                SaveList(vList, vFolder);
                return true;
            }
            return false;
        }
    }
}
