// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Threading;

namespace Iot.Device.Max7219
{
    /// <summary>
    /// Graphical functions for a MAX7219 device
    /// </summary>
    public class MatrixGraphics
    {
        private readonly Max7219 _device;

        /// <summary>
        /// Constructs MatrixGraphics instance
        /// </summary>
        /// <param name="device">Max7219 device</param>
        /// <param name="font">Font to use for drawing text</param>
        public MatrixGraphics(Max7219 device, IFont font)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            Font = font ?? throw new ArgumentNullException(nameof(font));
        }

        /// <summary>
        /// Font used for drawing text
        /// </summary>
        public IFont Font { get; set; }

        /// <summary>
        /// Writes a char to the given device with the specified font.
        /// </summary>
        public void WriteLetter(int deviceId, char chr, bool flush = true)
        {
            var charBytes = Font[chr];
            int end = Math.Min(charBytes.Count, Max7219.NumDigits);
            for (int col = 0; col < end; col++)
            {
                _device[new DeviceIdDigit(deviceId, col)] = charBytes[col];
            }

            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        ///  Scrolls the underlying buffer (for all cascaded devices) up one pixel
        /// </summary>
        public void ScrollUp(bool flush = true)
        {
            for (var i = 0; i < _device.Length; i++)
            {
                _device[i] = (byte)(_device[i] >> 1);
            }

            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Scrolls the underlying buffer (for all cascaded devices) down one pixel
        /// </summary>
        public void ScrollDown(bool flush = true)
        {
            for (var i = 0; i < _device.Length; i++)
            {
                _device[i] = (byte)((_device[i] << 1) & 0xff);
            }

            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Scrolls the underlying buffer (for all cascaded devices) to the left
        /// </summary>
        public void ScrollLeft(byte value, bool flush = true)
        {
            for (var i = 1; i < _device.Length; i++)
            {
                _device[i - 1] = _device[i];
            }

            _device[_device.Length - 1] = value;
            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Scrolls the underlying buffer (for all cascaded devices) to the right
        /// </summary>
        public void ScrollRight(byte value, bool flush = true)
        {
            for (var i = _device.Length - 1; i > 0; i--)
            {
                _device[i] = _device[i - 1];
            }

            _device[0] = value;
            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Shows a message on the device.
        /// If it's longer then the total width (or <see paramref="alwaysScroll"/> == true),
        /// it transitions the text message across the devices from right-to-left.
        /// </summary>
        /// <param name="alwaysScroll">Scroll text even if it is short enough to fit the display</param>
        /// <param name="characterSpace">Amount of pixels between characters</param>
        /// <param name="delayInMilliseconds">Speed for scrolling</param>
        /// <param name="monoSpaced">If true, all characters have the same width (8 pixels), otherwise empty areas are trimmed</param>
        /// <param name="text">The text to show</param>
        public void ShowMessage(string text, int delayInMilliseconds = 50, bool alwaysScroll = false, bool monoSpaced = false, int characterSpace = 1)
        {
            // Original code uses Linq
            // IEnumerable<ListByte> textCharBytes = text.Select(chr => Font[chr]);
            // int textBytesLength = textCharBytes.Sum(x => x.Count) + text.Length - 1;

            ArrayList textCharBytes = new ArrayList();
            int textBytesLength = 0;
            foreach (char car in text)
            {
                ListByte font = Font[car];
                textCharBytes.Add(font);
                textBytesLength += font.Count;
            }

            textBytesLength += (text.Length - 1) * characterSpace;

            bool scroll = alwaysScroll || textBytesLength > _device.Length;
            if (scroll)
            {
                var pos = _device.Length - 1;
                _device.ClearAll(false);
                foreach (ListByte arr in textCharBytes)
                {
                    foreach (byte b in arr)
                    {
                        ScrollLeft(b, true);
                        Thread.Sleep(delayInMilliseconds);

                    }

                    ScrollLeft(0, true);
                    Thread.Sleep(delayInMilliseconds);

                }

                for (; pos > 0; pos--)
                {
                    ScrollLeft(0, true);
                    Thread.Sleep(delayInMilliseconds);
                }
            }
            else
            {
                // calculate margin to display text centered
                var margin = (_device.Length - textBytesLength) / 2;
                _device.ClearAll(false);
                var pos = margin;
                foreach (ListByte arr in textCharBytes)
                {
                    foreach (byte b in arr)
                    {
                        _device[pos++] = b;
                    }

                    pos+=characterSpace;
                }

                _device.Flush();
            }
        }
    }
}
