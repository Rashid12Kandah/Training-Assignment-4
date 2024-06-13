using System;
using System.Drawing;

public class Concat{

    public static Bitmap Concate(Bitmap img1, Bitmap img2, int axis){
        if(axis == 0){
            int width = img1.Width + img2.Width;
            int height = Math.Max(img1.Height, img2.Height);
            Bitmap concatImage = new Bitmap(width, height);
            for(int y = 0; y<height; y++){
                for(int x = 0; x<width; x++){
                    if (x < img1.Width && y < img1.Height)
                    {
                        concatImage.SetPixel(x, y, img1.GetPixel(x, y));
                    }
                    else if (x >= img1.Width && y < img2.Height)
                    {
                        concatImage.SetPixel(x, y, img2.GetPixel(x - img1.Width, y));
                    }

                }
            }
            return concatImage;

        }else if(axis == 1){
            int width = Math.Max(img1.Width, img2.Width);
            int height = img1.Height + img2.Height;
            Bitmap concatImage = new Bitmap(width, height);
            for(int y = 0; y<height; y++){
                for(int x = 0; x<width; x++){
                    if(y< img1.Height && x<img1.Width){
                        concatImage.SetPixel(x, y, img1.GetPixel(x,y));
                    }
                    if(y>=img1.Height && x<width){
                        concatImage.SetPixel(x, y, img2.GetPixel(x, y-img1.Height));
                    }
                }
            }
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

        using(var temp1 = Image.FromFile(img1Path)){
            using(var temp2 = Image.FromFile(img2Path)){
                Bitmap img1 = new Bitmap(temp1);
                Bitmap img2 = new Bitmap(temp2);
                Bitmap concatImage = Concate(img1, img2, axis);
                concatImage.Save(savePath);
            }
        }
    }
}