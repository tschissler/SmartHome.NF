using System;

namespace Iot.Device.Max7219
{
    public class MonoSpaceFont : IFont
    {
        private readonly byte[] _data;
        private readonly int _bytesPerCharacter;
        private readonly byte[] _space;
        private readonly int _pixelsPerCharacter;

        /// <summary>
        /// Constructs FixedSizeFont instance
        /// </summary>
        /// <param name="bytesPerCharacter">number of bytes per character</param>
        /// <param name="data">Font data</param>
        /// <param name="spaceWidth">Space width</param>
        public MonoSpaceFont(int bytesPerCharacter, byte[] data, int spaceWidth = 8, int pixelsPerCharacter = 8)
        {
            _data = data;
            _bytesPerCharacter = bytesPerCharacter;
            _space = new byte[spaceWidth];
            _pixelsPerCharacter = pixelsPerCharacter;
        }

        /// <summary>
        /// Get character information
        /// </summary>
        public ListByte this[char chr]
        {
            get
            {
                int start = chr * _bytesPerCharacter;
                int end = start + _pixelsPerCharacter;
                if (end > _data.Length)
                {
                    return new ListByte(_space); // character is not defined
                }

                if (chr == ' ')
                {
                    return new ListByte(_space);
                }

                return new ListByte(new SpanByte(_data, start, end - start).ToArray());
            }
        }

    }
}
