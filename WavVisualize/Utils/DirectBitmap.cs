﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WavVisualize
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap;
        public int[] Bits;
        public bool Disposed;
        public int Height;
        public int Width;
        protected GCHandle BitsHandle;

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, int color)
        {
            Bits[x + y * Width] = color;
        }

        public int GetPixel(int x, int y)
        {
            return Bits[x + y * Width];
        }

        public void Clear()
        {
            int c = 0xFFFFFF | (0xFF << 24);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetPixel(x, y, c);
                }
            }
        }

        public void Copy(DirectBitmap b)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    SetPixel(x, y, b.GetPixel(x, y));
                }
            }
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
            GC.SuppressFinalize(this);
        }

        public static implicit operator Bitmap(DirectBitmap directBitmap)
        {
            return directBitmap.Bitmap;
        }
    }
}