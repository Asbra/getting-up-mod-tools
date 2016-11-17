using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace st2dds
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

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage st2dds filename.st");
                return;
            }

            string file = args[0];
            Console.WriteLine("Opening " + file + " ..");
            Stream fileStream = File.OpenRead(file);

            Console.WriteLine("Getting file info ..");
            FileInfo fileInfo = new FileInfo(file);
            long fileSize = fileInfo.Length;
            string fileName = fileInfo.Name;

            Console.WriteLine("Reading file bytes ..");
            byte[] fileBytes = new byte[fileSize];
            fileStream.Read(fileBytes, 0, (int)fileSize);

            fileStream.Close();

            if (fileBytes[0] != 'S' && fileBytes[1] != 'T')
            {
                Console.WriteLine("!!FATAL ERROR!! " + fileName + " is not a valid ST file!");
                return;
            }

            Console.WriteLine("Texture information");

            string textureName = "";
            for (int i = 0x40; fileBytes[i] != 0; i++)
            {
                textureName += (char)fileBytes[i];
            }
            Console.WriteLine("Name: " + textureName);

            int width = fileBytes[0xC] | fileBytes[0xD] << 8 | fileBytes[0xE] << 16 | fileBytes[0xF] << 24;
            int height = fileBytes[0x10] | fileBytes[0x11] << 8 | fileBytes[0x12] << 16 | fileBytes[0x13] << 24;
            Console.WriteLine("Dimensions: " + width + "x" + height);

            int dxt_version = fileBytes[0x1C] > 0 ? 5 : 1;

            // Console.WriteLine("Text: " + fileBytes[0x36].ToString("X2"));

            PixelData[] pixels = new PixelData[(fileSize - 0x184) / 16];
            int p = 0;
            
            for (int i = 0x184; i < fileSize; i += 16)
            {
                pixels[p++] = new PixelData(fileBytes, i);
            }

            p = p * 16;
            Console.WriteLine("Texture data: " + p);

            ///////////////////////////////////////////////////////////////////
            // Convert to DDS

            byte[] outBytes = new byte[(pixels.Length * 16) + 128]; // each pixel is 16 bytes, 128 DDS header size

            byte[] nullBytes = new byte[100];

            using (MemoryStream ms = new MemoryStream())
            {
                // DDS_HEADER
                ms.Write(Encoding.ASCII.GetBytes("DDS "), 0, 4);

                ms.Write(BitConverter.GetBytes(124), 0, 4); // dwSize
                ms.Write(BitConverter.GetBytes(0x081007), 0, 4); // dwFlags

                ms.Write(BitConverter.GetBytes(height), 0, 4); // dwHeight
                ms.Write(BitConverter.GetBytes(width), 0, 4); // dwWidth

                ms.Write(BitConverter.GetBytes(width == height ? 0x4000 : 0x2000), 0, 4); // dwPitchOrLinearSize

                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwDepth
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwMipMapCount

                // dwReserved1[11]
                ms.Write(nullBytes, 0, 11);

                // DDS_PIXELFORMAT
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwSize
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwFlags
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwFourCC
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwRGBBitCount
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwRBitMask
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwGBitMask
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwBBitMask
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwABitMask

                ms.WriteByte(0); // ?
                ms.Write(BitConverter.GetBytes(0x20), 0, 4); // ?
                ms.Write(BitConverter.GetBytes(0x04), 0, 4); // ?

                ms.Write(Encoding.ASCII.GetBytes("DXT" + dxt_version), 0, 4);
                ms.Write(nullBytes, 0, 20);

                ms.Write(BitConverter.GetBytes(0x1000), 0, 4); // dwCaps
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwCaps2
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwCaps3
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwCaps4
                ms.Write(BitConverter.GetBytes(0), 0, 4); // dwReserved2

                foreach (PixelData pixel in pixels)
                {
                    ms.WriteByte(pixel.alpha);
                    ms.WriteByte(pixel.unk01);
                    ms.WriteByte(pixel.unk02);
                    ms.WriteByte(pixel.unk03);
                    ms.WriteByte(pixel.unk04);
                    ms.WriteByte(pixel.unk05);
                    ms.WriteByte(pixel.unk06);
                    ms.WriteByte(pixel.unk07);
                    ms.WriteByte(pixel.unk08);
                    ms.WriteByte(pixel.unk09);
                    ms.WriteByte(pixel.unk0A);
                    ms.WriteByte(pixel.unk0B);
                    ms.WriteByte(pixel.unk0C);
                    ms.WriteByte(pixel.unk0D);
                    ms.WriteByte(pixel.unk0E);
                    ms.WriteByte(pixel.unk0F);
                }

                // That should be 0x80 bytes total

                outBytes = ms.ToArray();
                ms.Close();
            }

            string fileoutName = fileName.Replace(".st", ".dds");
            Console.WriteLine("Output to " + fileoutName);

            using (Stream fileoutStream = File.OpenWrite(fileoutName))
            {
                fileoutStream.Write(outBytes, 0, outBytes.Length);
                fileoutStream.Close();
            }
        }
    }
}
