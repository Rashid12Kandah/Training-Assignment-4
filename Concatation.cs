using System;
using System.Drawing;
using System.Drawing.Imaging;

public class Concat{
    public static Bitmap GrayScale(Bitmap image){

        Bitmap tempImg = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);

        ColorPalette palette = tempImg.Palette;
        for (int i = 0; i < 256; i++)
        {
            palette.Entries[i] = Color.FromArgb(i, i, i);
        }
        tempImg.Palette = palette;

        BitmapData imgData = image.LockBits(new Rectangle(0,0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb); 

        BitmapData tempData = tempImg.LockBits(new Rectangle(0,0, tempImg.Width, tempImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

        int imgStride = imgData.Stride;
        int imgOffset = imgStride - (image.Width*3);

        int tempStride = tempData.Stride;
        int tempOffset = tempStride - tempImg.Width;


        unsafe{

            byte * imgPtr = (byte *)(void *) imgData.Scan0;
            byte * tempPtr = (byte *)(void *) tempData.Scan0;
            byte red, green, blue;

            for(int y = 0; y < image.Height; y++){
                for(int x = 0; x<image.Width; x++){
                    red = imgPtr[0];
                    green = imgPtr[1];
                    blue = imgPtr[2];

                    tempPtr[0] = (byte)(.299 * red + .587 * green + .114 * blue);

                    imgPtr +=3;
                    tempPtr += 1;
                }
                imgPtr += imgOffset;
                tempPtr += tempOffset;

            }
            }

        image.UnlockBits(imgData);
        tempImg.UnlockBits(tempData);

        return tempImg;
    }

    public static Bitmap Concate(Bitmap img1_O, Bitmap img2_O, int axis){
        Bitmap img1 = GrayScale(img1_O);
        Bitmap img2 = GrayScale(img2_O);

        int width1 = img1.Width, height1 = img1.Height;
        int width2 = img2.Width, height2 = img2.Height;

        int conWidth, conHeight;

        if(axis == 0){
            conWidth = width1 + width2;
            conHeight = Math.Max(height1, height2);
        }else if(axis == 1){
            conWidth = Math.Max(width1, width2);
            conHeight = height1 + height2;
        }else{
            return -1;
        }

        Bitmap concatImage = new Bitmap(conWidth, conHeight, PixelFormat.Format8bppIndexed);

        BitmapData img1Data = img1.LockBits(new Rectangle(0,0, width1, height1), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        BitmapData img2Data = img2.LockBits(new Rectangle(0,0, width2, height2), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        BitmapData concatData = concatImage.LockBits(new Rectangle(0,0, conWidth, conHeight), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

        int img1Stride = img1Data.Stride;
        int img1Offset = img1Stride - width1;
        
        int img2Stride = img2Data.Stride; 
        int img2Offset = img2Stride - width2;
        
        int concatStride = concatData.Stride;
        int concatOffset = concatStride - conWidth

        if(axis == 0){
            unsafe{
                byte * img1Ptr = (byte *)(void *) img1Data.Scan0;
                byte * img2Ptr = (byte *)(void *) img2Data.Scan0;
                byte * concatPtr = (byte *)(void *) concatData.Scan0;

                for(int y = 0; y<conHeight; y++){
                    for(int x = 0; x<conWidth; x++){
                        if(x<width1 && y<height1){
                            concatPtr[0] = img1Ptr[0];
                            img1Ptr += 1;
                        }else if(x>=width1 && y<height1){
                            concatPtr[0] = img2Ptr[0];
                            img2Ptr += 1;
                        }else if(y>=height1){
                            // concatPtr[0] = img2Ptr[0];
                            concatPtr[0] = 255;
                            img2Ptr += 1;
                        }
                        concatPtr += 1;
                    }
                    img1Ptr += img1Offset;
                    img2Ptr += img2Offset;
                    concatPtr += concatOffset;
                }
            }
            return concatImage;

        }else if(axis == 1){
            unsafe{
                byte * img1Ptr = (byte *)(void *) img1Data.Scan0;
                byte * img2Ptr = (byte *)(void *) img2Data.Scan0;
                byte * concatPtr = (byte *)(void *) concatData.Scan0;

                for(int y = 0; y<conHeight; y++){
                    for(int x = 0; x<conWidth; x++){
                        if(y<height1 && x<width1){
                            concatPtr[0] = img1Ptr[0];
                            img1Ptr += 1;
                        }else if(y>=height1 && x<width1){
                            concatPtr[0] = img2Ptr[0];
                            img2Ptr += 1;
                        }else if(x>=width1){
                            concatPtr[0] = img2Ptr[0];
                            img2Ptr += 1;
                        }
                        concatPtr += 1;
                    }
                    img1Ptr += img1Offset;
                    img2Ptr += img2Offset;
                    concatPtr += concatOffset;
                }
            }
            img1.UnlockBits(img1Data);
            img2.UnlockBits(img2Data);
            concatImage.UnlockBits(concatData);
            return concatImage;
        }
        return null;
    }

    public static void Main(string[] args){
        if(args.Length != 3){
            Console.WriteLine("Usage: Concatation.exe <img1Path> <img2Path> <axis>");
            return;
        }
        string img1Path = args[0];
        string img2Path = args[1];
        int axis = int.Parse(args[2]);

        string uuid = Guid.NewGuid().ToString();

        string savePath = $"{axis}_{uuid}.jpg";

        
        Bitmap img1 = new Bitmap(img1Path);
        Bitmap img2 = new Bitmap(img2Path);
        Bitmap concatImage = Concate(img1, img2, axis);
        concatImage.Save(savePath);

    }
}
