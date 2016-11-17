using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace dds2st
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
            if (args.Length < 1)
            {
                Console.WriteLine("Usage dds2st filename.dds [name]");
                return;
            }

            string file = args[0];
            Console.WriteLine("Opening " + file + " ..");
            Stream fileStream = File.OpenRead(file);
            
            string fileExtraName = file.Replace(".dds", ".dat");
            Console.WriteLine("Opening extra data file " + fileExtraName + " ..");
            Stream fileExtraStream = File.OpenRead(fileExtraName);
            byte[] temp = new byte[32];
            fileExtraStream.Read(temp, 0, 32);
            string textureName = Encoding.ASCII.GetString(temp);
            int _0008 = fileExtraStream.ReadByte(); // 0008 in ST
            int dxt_byte = fileExtraStream.ReadByte(); // 001C in ST
            int _003C = fileExtraStream.ReadByte(); // 003C in ST
            fileExtraStream.Close();

            Console.WriteLine("Getting file info ..");
            FileInfo fileInfo = new FileInfo(file);
            long fileSize = fileInfo.Length;
            string fileName = fileInfo.Name;

            Console.WriteLine("Reading file bytes ..");
            byte[] fileBytes = new byte[fileSize];
            fileStream.Read(fileBytes, 0, (int)fileSize);

            fileStream.Close();

            if (fileBytes[0] != 'D' && fileBytes[1] != 'D' && fileBytes[2] != 'S')
            {
                Console.WriteLine("!!FATAL ERROR!! " + fileName + " is not a valid DDS file!");
                return;
            }

            Console.WriteLine("Texture information");

            Console.WriteLine("Name: " + textureName);

            int width = fileBytes[0x10] | fileBytes[0x11] << 8 | fileBytes[0x12] << 16 | fileBytes[0x13] << 24;
            Console.WriteLine("Width: " + width);
            int height = fileBytes[0x0C] | fileBytes[0x0D] << 8 | fileBytes[0x0E] << 16 | fileBytes[0x0F] << 24;
            Console.WriteLine("Height: " + height);

            // int dxt_byte = fileBytes[0x57] == 0x31 ? 0x00 : 0xFF;
            
            PixelData[] pixels = new PixelData[(fileSize - 0x80) / 16];
            int p = 0;

            for (int i = 0x80; i < fileSize; i += 16)
            {
                pixels[p++] = new PixelData(fileBytes, i);
            }

            p = p * 16;
            Console.WriteLine("Texture data: " + p);
            
            ///////////////////////////////////////////////////////////////////
            // Convert to ST

            long outputSize = fileSize + 0x100;
            Console.WriteLine("Output size: " + outputSize);
            byte[] outBytes = new byte[outputSize]; // each pixel is 16 bytes, 256 ST header size
            byte[] nullbytes = new byte[128];
            
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(Encoding.ASCII.GetBytes("ST"), 0, 2);
                ms.Write(BitConverter.GetBytes(0), 0, 2);
                ms.Write(BitConverter.GetBytes(4), 0, 4); // 0004
                ms.Write(BitConverter.GetBytes(_0008), 0, 4); // 0008

                ms.Write(BitConverter.GetBytes(width), 0, 4); // 000C
                ms.Write(BitConverter.GetBytes(height), 0, 4); // 0010
                ms.Write(BitConverter.GetBytes(width), 0, 4); // 0014
                ms.Write(BitConverter.GetBytes(height), 0, 4); // 0018

                ms.Write(BitConverter.GetBytes(dxt_byte), 0, 4); // 001C
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 0020
                ms.Write(BitConverter.GetBytes(1), 0, 4); // 0024
                ms.Write(BitConverter.GetBytes(1), 0, 4); // 0028
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 002C
                ms.Write(BitConverter.GetBytes(1), 0, 4); // 0030
                ms.Write(BitConverter.GetBytes(0x00008080), 0, 4); // 0034
                ms.Write(BitConverter.GetBytes(0x80), 0, 4); // 0038
                ms.Write(BitConverter.GetBytes(_003C), 0, 4); // 003C
                
                ms.Write(Encoding.ASCII.GetBytes(textureName), 0, Encoding.ASCII.GetBytes(textureName).Length); // 0040
                ms.Write(nullbytes, 0, 32 - Encoding.ASCII.GetBytes(textureName).Length); // 0040

                ms.Write(BitConverter.GetBytes(0), 0, 4); // 0060
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 0064
                ms.Write(BitConverter.GetBytes(2), 0, 4); // 0068
                ms.Write(BitConverter.GetBytes(10), 0, 4); // 006C
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 0070
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 0074
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 0078
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 007C
                ms.Write(BitConverter.GetBytes(0), 0, 4); // 0080
                ms.Write(BitConverter.GetBytes(0x00008080), 0, 4); // 0084
                ms.Write(BitConverter.GetBytes(0x00000080), 0, 4); // 0088
                ms.Write(BitConverter.GetBytes(0x00008080), 0, 4); // 008C
                ms.Write(BitConverter.GetBytes(0x00008080), 0, 4); // 0090
                ms.Write(nullbytes, 0, 0x70); // 0094
                Console.WriteLine("Wrote ST header");

                ms.Write(Encoding.ASCII.GetBytes("BIR"), 0, 3); // 104
                ms.Write(BitConverter.GetBytes(0), 0, 1); // 107
                ms.Write(nullbytes, 0, 0x7C); // 0108
                Console.WriteLine("Wrote BIR header");

                // 0184
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
                Console.WriteLine("Wrote texture data");

                outBytes = ms.ToArray();
                ms.Close();
            }

            string fileoutName = fileName.Replace(".dds", ".st");
            Console.WriteLine("Output to " + fileoutName);

            using (Stream fileoutStream = File.OpenWrite(fileoutName))
            {
                fileoutStream.Write(outBytes, 0, outBytes.Length);
                fileoutStream.Close();
            }
        }
    }
}
