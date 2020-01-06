using System;

namespace Chip8.Emulator
{
    public class Keypad
    {

        private readonly byte[] _keys;

        public int Length => _keys.Length;

        public Keypad()
        {
            _keys = new byte[16];
        }

        public byte this[int index]
        {
            get
            {
                if (index > 16)
                    throw new InvalidOperationException("Index must not be greater than 16");

                return _keys[index];
            }

            set
            {
                if (index > 16)
                    throw new InvalidOperationException("Index must not be greater than 16");
                _keys[index] = value;
            }
        }

        public void KeyPressed(byte key)
        {
            if (key > 16)
                throw new InvalidOperationException("Key must not be greater than 16");

            _keys[key] = 1;
        }

        public void KeyReleased(byte key)
        {
            if (key > 16)
                throw new InvalidOperationException("Key must not be greater than 16");

            _keys[key] = 0;
        }
    }
}
