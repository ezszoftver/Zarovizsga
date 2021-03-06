﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Tao.OpenGl;

namespace MeshReducer
{
    class Texture
    {
        public int[] id = null;

        public Texture()
        {
            id = new int[1];
        }

        public Texture(Texture b)
        {
            id = new int[1];

            id[0] = b.id[0];
        }

        public bool Load(int red, int green, int blue)
        {
            byte[] data = new byte[8 * 8 * 4];
            for (int i = 0; i < data.Length; i+=4)
            {
                data[i + 0] = (byte)blue;
                data[i + 1] = (byte)green;
                data[i + 2] = (byte)red;
                data[i + 3] = (byte)255;
            }

            Gl.glGenTextures(1, id);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, id[0]);

            float[] maxAnisotropy = new float[1];
            Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, maxAnisotropy);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maxAnisotropy[0]);

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGBA8, 8, 8, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, data);

            data = null;
            return true;
        }

        public bool Load(string filename)
        {
            if (!File.Exists(filename)) { return false; }

            Bitmap textureImage = null;
            try
            {
                textureImage = new Bitmap(filename);
            }
            catch
            {
                return false;
            }

            textureImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle rectangle = new Rectangle(0, 0, textureImage.Width, textureImage.Height);
            BitmapData bitmapData = textureImage.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Gl.glGenTextures(1, id);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, id[0]);

            float[] maxAnisotropy = new float[1];
            Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, maxAnisotropy);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maxAnisotropy[0]);

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGBA8, textureImage.Width, textureImage.Height, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

            textureImage.UnlockBits(bitmapData);
            textureImage.Dispose();

            return true;
        }

        public void Release()
        {
            if (id != null)
            {
                Gl.glDeleteTextures(1, id);
            }
        }

        public static void SaveToBMP(string dir_and_filename, int red, int green, int blue, int alpha)
        {
            byte[] data = new byte[8 * 8 * 4];
            for (int i = 0; i < data.Length; i += 4)
            {
                data[i + 0] = (byte)alpha;
                data[i + 1] = (byte)red;
                data[i + 2] = (byte)green;
                data[i + 3] = (byte)blue;
            }
            
            Bitmap bitmap = GetDataPicture(8, 8, red, green, blue, alpha);
            bitmap.Save(dir_and_filename, ImageFormat.Bmp);

            data = null;
        }

        static public Bitmap GetDataPicture(int w, int h, int red, int green, int blue, int alpha)
        {
            Bitmap pic = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color c = Color.FromArgb(alpha, red, green, blue);
                    pic.SetPixel(x, y, c);
                }
            }

            return pic;
        }
    }
}
