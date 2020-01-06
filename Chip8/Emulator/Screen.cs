using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8.Emulator
{
    public class Screen
    {

        private readonly int[] ScreenArray;

        public int Length => 64 * 32;

        public Screen()
        {
            ScreenArray = new int[64 * 32];
        }

        /// <summary>
        /// Clears all pixel values and sets everything to its default state.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < ScreenArray.Length; i++)
            {
                ScreenArray[i] = 0;
            }
        }

        public bool this[int index]
        {
            get
            {
                if (index < ScreenArray.Length)
                    return ScreenArray[index] != 0;
                else
                    throw new InvalidOperationException("Index: " + index + " is greater than screen size");
            }

            set
            {
                if (index < ScreenArray.Length)
                    ScreenArray[index] = value ? 1 : 0;
                else
                    throw new InvalidOperationException("Index: " + index + " is greater than screen size");
            }
        }

        public bool this [int x, int y]
        {
            get
            {
                var loc = x + (y * 64);

                if (loc < ScreenArray.Length)
                    return ScreenArray[loc] != 0;
                else
                    throw new InvalidOperationException("Location: " + loc + " is greater than the screen size");
            }
            set
            {
                var loc = x + (y * 64);

                if (loc < ScreenArray.Length)
                    ScreenArray[loc] = value ? 1 : 0;
                else
                    throw new InvalidOperationException("Location: " + loc + " is greater than the screen size");
            }
        }
    }
}
