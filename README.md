# Mod tools for Marc Ecko's Getting Up
Simple tools/utilities to assist in modding the game [Marc Ecko's Getting Up: Contents Under Pressure](http://store.steampowered.com/app/260190/)

## Game information
```cpp
ST header is 260 (0x104) bytes
BIR header is 128 (0x80) bytes
DDS header is 128 (0x80) bytes

ST header
0000 ST\0\0
0004 4
0008 4
000C Width
0010 Height
0014 Width
0018 Height
001C // 0 if DXT1 otherwise DXT5
0020 0
0024 1
0028 1
002C 0
0030 1
0034 0x00008080
0038 0x80
003C 0x52
0040 char[32] Filename
0060 0
0064 0
0068 2
006C 0x0A
0070 0
0074 0
0078 0
007C 0
0080 0
0084 0x00008080
0088 0x00000080
008C 0x00008080
0090 0x00008080
0094 [0x70 null bytes]
size 0104
0104 starts BIR header
size 0080
0184 starts texture data

DDS header
https://msdn.microsoft.com/en-us/library/windows/desktop/bb943982(v=vs.85).aspx
0000 char[4] "DDS "
0004 DWORD dwSize
0008 DWORD dwFlags;
000C DWORD dwHeight;
000F DWORD dwWidth;
0010 DWORD dwPitchOrLinearSize;
0014 DWORD dwDepth;
0018 DWORD dwMipMapCount;
001C DWORD dwReserved1[11]; 0x2C
0048 DDS_PIXELFORMAT ddspf; https://msdn.microsoft.com/en-us/library/windows/desktop/bb943984(v=vs.85).aspx
+ 0000 "DXT1"/"DXT3"/"DXT5"
+ 0004 DWORD dwSize;
+ 0008 DWORD dwFlags;
+ 000C DWORD dwFourCC;
+ 0010 DWORD dwRGBBitCount;
+ 0014 DWORD dwRBitMask;
+ 0018 DWORD dwGBitMask;
+ 001C DWORD dwBBitMask;
+ 0020 DWORD dwABitMask;
006C DWORD dwCaps;
0070 DWORD dwCaps2;
0074 DWORD dwCaps3;
0078 DWORD dwCaps4;
007C DWORD dwReserved2;
size 0080
```

## st2dds
Converts ST texture files from the game into DDS texture files

*Usage* `st2dds.exe filename.st`

### Example
```
gettingup\texture_tools>st2dds wp_tranem10_08_f.st
Opening wp_tranem10_08_f.st ..
Getting file info ..
Reading file bytes ..
Texture information
Name: WP_TraneM10_08_F
Dimensions: 512x256
Texture data count: 65536
Output to wp_tranem10_08_f.dds
```

## dds2st
Converts DDS textures to ST texture files

*Usage* `dds2st.exe filename.dds Texture_Name`

### Example
```
C:\Projects\gettingup\texture_tools>dds2st wp_tranem10_08_f.dds
Opening wp_tranem10_08_f.dds ..
Getting file info ..
Reading file bytes ..
Input texture information ..
Name: WP_TraneM10_08_F
Width: 512
Height: 256
Texture data count: 65536
Output size: 65920
Wrote ST header
Wrote BIR header
Wrote texture data
Output to wp_tranem10_08_f.st
```