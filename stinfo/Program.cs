using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace stinfo
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
                Console.WriteLine("Usage stinfo <path>");
                return;
            }

            string path = args[0].TrimEnd('"');
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Stream fileStream = File.OpenRead(file);

                FileInfo fileInfo = new FileInfo(file);
                long fileSize = fileInfo.Length;
                string fileName = fileInfo.Name;

                byte[] fileBytes = new byte[fileSize];
                fileStream.Read(fileBytes, 0, (int)fileSize);

                fileStream.Close();

                if (fileBytes[0] != 'S' && fileBytes[1] != 'T')
                {
                    continue;
                }

                string textureName = "";
                for (int i = 0x40; fileBytes[i] != 0; i++)
                {
                    textureName += (char)fileBytes[i];
                }

                int width = fileBytes[0xC] | fileBytes[0xD] << 8 | fileBytes[0xE] << 16 | fileBytes[0xF] << 24;
                int height = fileBytes[0x10] | fileBytes[0x11] << 8 | fileBytes[0x12] << 16 | fileBytes[0x13] << 24;

                /*
                PixelData[] pixels = new PixelData[(fileSize - 0x184) / 16];
                int p = 0;

                for (int i = 0x184; i < fileSize; i += 16)
                {
                    pixels[p++] = new PixelData(fileBytes, i);
                }

                p = p * 16;
                */

                Console.Write(fileName + "\t");
                Console.Write(textureName + "\t" + (textureName.Length >= 16 ? "" : "\t"));
                Console.Write(width + "x" + height + "\t\t");
                Console.Write("DXT 0x" + fileBytes[0x1C].ToString("X2") + "\t");
                // Console.Write(p + "\t");
                Console.Write(Environment.NewLine);
            }
        }
    }
}
