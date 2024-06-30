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

        int width, height;

        if(axis == 0){
            width = img1.Width + img2.Width;
            height = Math.Max(img1.Height, img2.Height);
        }else{
            width = Math.Max(img1.Width, img2.Width);
            height = img1.Height + img2.Height;
        }

        Bitmap concatImage = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

        BitmapData img1Data = img1.LockBits(new Rectangle(0,0, img1.Width, img1.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        BitmapData img2Data = img2.LockBits(new Rectangle(0,0, img2.Width, img2.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        BitmapData concatData = concatImage.LockBits(new Rectangle(0,0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

        int img1Stride = img1Data.Stride;
        int img2Stride = img2Data.Stride; 
        int concatStride = concatData.Stride;

        if(axis == 0){
            unsafe{
                byte * img1Ptr = (byte *)(void *) img1Data.Scan0;
                byte * img2Ptr = (byte *)(void *) img2Data.Scan0;
                byte * concatPtr = (byte *)(void *) concatData.Scan0;

                for(int y = 0; y<height; y++){
                    for(int x = 0; x<width; x++){
                        if(x<img1.Width && y<img1.Height){
                            concatPtr[0] = img1Ptr[0];
                            img1Ptr += 1;
                        }else if(x>=img1.Width && y<img1.Height){
                            concatPtr[0] = img2Ptr[0];
                            img2Ptr += 1;
                        }else if(y>=img1.Height){
                            // concatPtr[0] = img2Ptr[0];
                            concatPtr[0] = 255;
                            img2Ptr += 1;
                        }
                        concatPtr += 1;
                    }
                    img1Ptr += img1Stride;
                    img2Ptr += img2Stride;
                    concatPtr += concatStride;
                }
            }
            return concatImage;

        }else if(axis == 1){
            unsafe{
                byte * img1Ptr = (byte *)(void *) img1Data.Scan0;
                byte * img2Ptr = (byte *)(void *) img2Data.Scan0;
                byte * concatPtr = (byte *)(void *) concatData.Scan0;

                for(int y = 0; y<height; y++){
                    for(int x = 0; x<width; x++){
                        if(y<img1.Height && x<img1.Width){
                            concatPtr[0] = img1Ptr[0];
                            img1Ptr += 1;
                        }else if(y>=img1.Height && x<img1.Width){
                            concatPtr[0] = img2Ptr[0];
                            img2Ptr += 1;
                        }else if(x>=img1.Width){
                            concatPtr[0] = img2Ptr[0];
                            img2Ptr += 1;
                        }
                        concatPtr += 1;
                    }
                    img1Ptr += img1Stride;
                    img2Ptr += img2Stride;
                    concatPtr += concatStride;
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