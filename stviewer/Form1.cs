using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace stviewer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PixelData
    {
        public byte alpha;
        public byte unk01;
        public byte unk02;
        public byte unk03;
        public byte unk04;
        public byte unk05;
        public byte unk06;
        public byte unk07;
        public byte unk08; // c0
        public byte unk09; // c0
        public byte unk0A; // c1
        public byte unk0B; // c1
        public byte unk0C; // Color indexes
        public byte unk0D; // ^
        public byte unk0E; // ^
        public byte unk0F; // ^

        public PixelData(byte[] buffer, int index)
        {
            alpha = buffer[index + 0];
            unk01 = buffer[index + 1];
            unk02 = buffer[index + 2];
            unk03 = buffer[index + 3];
            unk04 = buffer[index + 4];
            unk05 = buffer[index + 5];
            unk06 = buffer[index + 6];
            unk07 = buffer[index + 7];
            unk08 = buffer[index + 8];
            unk09 = buffer[index + 9];
            unk0A = buffer[index + 10];
            unk0B = buffer[index + 11];
            unk0C = buffer[index + 12];
            unk0D = buffer[index + 13];
            unk0E = buffer[index + 14];
            unk0F = buffer[index + 15];
        }
    }

    public partial class formMain : Form
    {
        string filePath = "";

        string textureName = "";
        int textureWidth = 0;
        int textureHeight = 0;
        int dxt_version = 0;
        Bitmap bmp = null;

        public formMain()
        {
            InitializeComponent();
            this.AllowDrop = true;
            texture.AllowDrop = true;

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                filePath = args[1];

                if (!LoadST(filePath))
                {
                    MessageBox.Show("Failed to load " + Path.GetFileName(filePath));
                }
            }
        }

        private bool LoadST(string path)
        {
            Stream fileStream = File.OpenRead(path);
            
            FileInfo fi = new FileInfo(path);
            int fileSize = (int)fi.Length;
            string fileName = fi.Name;

            byte[] fileBytes = new byte[fi.Length];
            fileStream.Read(fileBytes, 0, fileSize);

            fileStream.Close();

            //////////////////////////////////////////////////
            // Parse ST texture files
            if (fileBytes[0] != 'S' || fileBytes[1] != 'T')
            {
                return false;
            }

            textureName = "";
            for (int i = 0x40; fileBytes[i] != 0; i++)
            {
                textureName += (char)fileBytes[i];
            }
            textureWidth = fileBytes[0xC] | fileBytes[0xD] << 8 | fileBytes[0xE] << 16 | fileBytes[0xF] << 24;
            textureHeight = fileBytes[0x10] | fileBytes[0x11] << 8 | fileBytes[0x12] << 16 | fileBytes[0x13] << 24;

            dxt_version = fileBytes[0x1C] > 0 ? 5 : 1;

            PixelData[] pixels = new PixelData[(fileSize - 0x184) / 16];
            int p = 0;

            for (int i = 0x184; i < fileSize; i += 16)
            {
                pixels[p++] = new PixelData(fileBytes, i);
            }

            p = p * 16;

            ///////////////////////////////////////////////////////////////////
            // Load texture into PictureBox control

            bmp = new Bitmap(textureWidth, textureHeight, PixelFormat.Format32bppArgb);
            
            bool done = false;
            int y;
            for (y = 0; y < textureHeight; y += 4)
            {
                for (int x = 0; x < textureWidth; x += 4)
                {
                    if (x + textureWidth * y >= pixels.Length * 16)
                    {
                        done = true;
                        break;
                    }

                    PixelData pixel = pixels[x / 4 + textureWidth / 4 * (y / 4)];

                    int[] alphan = new int[8];
                    alphan[0] = pixel.alpha;
                    alphan[1] = pixel.unk01;
                    if (alphan[0] > alphan[1])
                    {
                        alphan[2] = (6 * alphan[0] + alphan[1]) / 7; alphan[3] = (5 * alphan[0] + 2 * alphan[1]) / 7; alphan[4] = (4 * alphan[0] + 3 * alphan[1]) / 7; alphan[5] = (3 * alphan[0] + 4 * alphan[1]) / 7; alphan[6] = (2 * alphan[0] + 5 * alphan[1]) / 7; alphan[7] = (alphan[0] + 6 * alphan[1]) / 7;
                    }
                    else
                    {
                        alphan[2] = (4 * alphan[0] + alphan[1]) / 5; alphan[3] = (3 * alphan[0] + 2 * alphan[1]) / 5; alphan[4] = (2 * alphan[0] + 3 * alphan[1]) / 5; alphan[5] = (alphan[0] + 4 * alphan[1]) / 5; alphan[6] = 0; alphan[7] = 255;
                    }

                    int a = 255;//pixel.alpha;
                    ulong alphatable = (ulong)(pixel.unk02 | (pixel.unk03 << 8) | (pixel.unk04 << 16) | (pixel.unk05 << 24) | (pixel.unk06 << 32) | (pixel.unk07 << 40));

                    int r = 255, g = 255, b = 255;
                    Color[] c = new Color[4];
                    byte b1, b2;

                    b1 = pixel.unk08;
                    b2 = pixel.unk09;

                    r = (b2 & 0xF8) >> 3;
                    g = ((b1 & 0xE0) >> 5) | ((b2 & 0x7) << 3);
                    b = b1 & 0x1F;

                    r = (r * 255) / 31;
                    g = (g * 255) / 63;
                    b = (b * 255) / 31;

                    c[0] = Color.FromArgb(a, r, g, b);
                    int c0_s = c[0].R + c[0].G + c[0].B;//b1 | (b2 << 8);

                    b1 = pixel.unk0A;
                    b2 = pixel.unk0B;

                    r = (b2 & 0xF8) >> 3;
                    g = ((b1 & 0xE0) >> 5) | ((b2 & 0x7) << 3);
                    b = b1 & 0x1F;

                    r = (r * 255) / 31;
                    g = (g * 255) / 63;
                    b = (b * 255) / 31;

                    c[1] = Color.FromArgb(a, r, g, b);
                    int c1_s = c[1].R + c[1].G + c[1].B;//b1 | (b2 << 8);
                    
                    c[2] = Color.FromArgb(a, (2 * c[0].R + c[1].R) / 3, (2 * c[0].G + c[1].G) / 3, (2 * c[0].B + c[1].B) / 3);
                    c[3] = Color.FromArgb(a, (c[0].R + 2 * c[1].R) / 3, (c[0].G + 2 * c[1].G) / 3, (c[0].B + 2 * c[1].B) / 3);

                    ulong indexes = (ulong)(pixel.unk0C | (pixel.unk0D << 8) | (pixel.unk0E << 16) | (pixel.unk0F << 24));

                    r = g = b = 0;

                    int[] alphas = new int[16];
                    for (int i = 0; i < 16; i++)
                    {
                        ulong index = alphatable & 7;
                        alphatable = alphatable >> 3;
                        alphas[i] = alphan[index];
                    }
                    
                    for (int i = 0; i < 16; i++)
                    {
                        ulong index = indexes & 3;
                        indexes = indexes >> 2; //shift two bytes right (same as indexes = indexes >> 2)

                        bmp.SetPixel(x + i % 4, y + i / 4, Color.FromArgb(alphas[i], c[index].R, c[index].G, c[index].B));
                    }
                }

                if (done != false)
                {
                    break;
                }
            }
            
            this.Size = new Size(textureWidth, textureHeight + 28);
            this.Text = textureName + " [" + textureWidth + "x" + textureHeight + "] (DXT" + dxt_version + ")";
            texture.Size = new Size(textureWidth, textureHeight);
            texture.Image = bmp;
            
            return true;
        }

        private void formMain_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void formMain_DragDrop(object sender, DragEventArgs e)
        {
            // MessageBox.Show((string)e.Data.GetData("FileName"));
        }
    }
}
