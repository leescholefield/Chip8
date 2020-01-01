using System;

namespace Chip8.Emulator
{
    public abstract class Opcodes
    {

        public static Action<Chip8> ClearScreen
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> ReturnFromSubroutine
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> JumpToAddress
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> CallSubroutine
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> SkipInstructionIfVxEquals
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> SkipInstructionIfVxNotEquals
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> SkipInstructionIfVxEqualsVy
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> SetVxTo
        {
            get
            {
                return null;
            }
        }

        public static Action<Chip8> AddToVx
        {
            get
            {
                return null;
            }
        }


    }
}
