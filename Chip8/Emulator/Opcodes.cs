using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chip8.Emulator
{
    public abstract class Opcodes
    {

        /// <summary>
        /// Sets all the Values in the Graphics array to 0
        /// </summary>
        public static Action<Chip8Intepreter> ClearScreen
        {
            get
            {
                return (c) =>
                {
                    c.GraphicsArray.Clear();
                    c.ProgramCounter += 2;
                    c.Redraw = true;
                };
            }
        }

        public static Action<Chip8Intepreter> ReturnFromSubroutine
        {
            get
            {
                return (c) =>
                {
                    c.StackPointer--;
                    c.ProgramCounter = c.Stack[c.StackPointer];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> JumpToAddress
        {
            get
            {
                return (c) =>
                {
                    c.ProgramCounter = c.Opcode & 0x0FFF;
                };
            }
        }

        public static Action<Chip8Intepreter> CallSubroutine
        {
            get
            {
                return (c) =>
                {
                    c.Stack[c.StackPointer] = c.ProgramCounter;
                    c.StackPointer++;
                    c.ProgramCounter = c.Opcode & 0x0FFF;
                };
            }
        }

        public static Action<Chip8Intepreter> SkipInstructionIfVxEquals
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var NN = (c.Opcode & 0x00FF);

                    if (c.Registers[X] == NN)
                    {
                        c.ProgramCounter += 4;
                    }
                    else
                    {
                        c.ProgramCounter += 2;
                    }
                };
            }
        }

        public static Action<Chip8Intepreter> SkipInstructionIfVxNotEquals
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var NN = (c.Opcode & 0x00FF);

                    if (c.Registers[X] != NN)
                    {
                        c.ProgramCounter += 4;
                    }
                    else
                    {
                        c.ProgramCounter += 2;
                    }
                };
            }
        }
        // 0x5XY0
        public static Action<Chip8Intepreter> SkipInstructionIfVxEqualsVy
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    if (c.Registers[X] == c.Registers[Y])
                        c.ProgramCounter += 4;
                    else
                        c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SetVxTo
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var NN = (c.Opcode & 0x00FF);

                    c.Registers[X] = (byte)NN;

                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> AddToVx
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var NN = (c.Opcode & 0x00FF);

                    c.Registers[X] += (byte)NN;

                    c.ProgramCounter += 2;
                };
            }
        }

        // 0x8XY0
        public static Action<Chip8Intepreter> SetVxToVy
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    c.Registers[X] = c.Registers[Y];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SetVxToVxOrVy
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    c.Registers[X] |= c.Registers[Y];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SetVxToVxAndVy
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    c.Registers[X] &= c.Registers[Y];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SetVxToVxXorVy
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    c.Registers[X] ^= c.Registers[Y];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> AddVyToVx
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    if (c.Registers[Y] > (0xFF - c.Registers[X]))
                        c.Registers[0xF] = 1;
                    else
                        c.Registers[0xF] = 0;

                    c.Registers[X] += c.Registers[Y];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SubtractVyFromVx
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    if (c.Registers[X] < c.Registers[Y])
                        c.Registers[0xF] = 0;
                    else
                        c.Registers[0xF] = 1;

                    c.Registers[X] -= c.Registers[Y];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> ShiftVxRightByOne
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;

                    c.Registers[0xF] = (byte)(c.Registers[X] & 0x1);
                    c.Registers[X] >>= 1;
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SetVxToVyMinusVx
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    if (c.Registers[X] > c.Registers[Y])
                        c.Registers[0xF] = 0;
                    else
                        c.Registers[0xF] = 1;

                    c.Registers[X] = (byte)(c.Registers[Y] - c.Registers[X]);
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> ShiftVxLeftByOne
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;

                    c.Registers[0xF] = (byte)(c.Registers[X] >> 7);
                    c.Registers[X] <<= 1;
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SkipInstructionIfVxNotEqualsVy
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    if (c.Registers[X] != c.Registers[Y])
                        c.ProgramCounter += 4;
                    else
                        c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SetIndex {
            get
            {
                return (c) =>
                {
                    c.Index = c.Opcode & 0x0FFF;
                    c.ProgramCounter += 2;
                };
            }
        }
        public static Action<Chip8Intepreter> JumpToAddressPlusV0 {
            get
            {
                return (c) =>
                {
                    c.ProgramCounter = (c.Opcode & 0x0FFF) + c.Registers[0];
                };
            }
        }
        public static Action<Chip8Intepreter> SetVxToRandomNumber {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var NN = (c.Opcode & 0x00FF);

                    byte rand = (byte)(new Random()).Next(0, 0xFF);
                    c.Registers[X] = (byte)(rand & NN);
                    c.ProgramCounter += 2;
                };
            }
        }
        public static Action<Chip8Intepreter> DrawSprite {
            get
            {
                return (c) =>
                {
                    var N = (c.Opcode & 0x000F);
                    var X = (c.Opcode & 0x0F00) >> 8;
                    var Y = (c.Opcode & 0x00F0) >> 4;

                    // reset register
                    c.Registers[0xF] = 0;

                    for (int row = 0; row < N; row++)
                    {
                        var pixel = c.Memory[c.Index + row];

                        for (int col = 0; col < 8; col++)
                        {
                            // check if current pixel is 1
                            if ((pixel & (0x80 >> col)) != 0)
                            {
                                // check for collision
                                if (c.GraphicsArray[c.Registers[X] + col, c.Registers[Y] + row])
                                {
                                    c.Registers[0xF] = 1;
                                }

                                // set pixel value
                                c.GraphicsArray[c.Registers[X] + col, c.Registers[Y] + row] ^= true;
                                    
                            }
                        }
                    }

                    c.ProgramCounter += 2;
                    c.Redraw = true;
                };
            }
        }

        public static Action<Chip8Intepreter> SkipInstructionIfKeyPressed {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    if (c.KeypadArray[c.Registers[X]] == 1)
                        c.ProgramCounter += 4;
                    else
                        c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SkipInstructionIfKeyNotPressed
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    if (c.KeypadArray[c.Registers[X]] == 0)
                        c.ProgramCounter += 4;
                    else
                        c.ProgramCounter += 2;
                };
            }
        }
        public static Action<Chip8Intepreter> SetVxToDelayTimer {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    c.Registers[X] = c.DelayTimer;
                    c.ProgramCounter += 2;
                };
            }
        }
        public static Action<Chip8Intepreter> WaitForKeyPress
        {
            get
            {
                return (c) =>
                {
                    bool pressed = false;

                    var X = (c.Opcode & 0x0F00) >> 8;
                    
                    for (int i = 0; i < c.KeypadArray.Length; i++)
                    {
                        if (c.KeypadArray[i] == 1)
                        {
                            c.Registers[X] = (byte)i; 
                            pressed = true;
                        }
                    }

                    if (!pressed)
                        return;
                    else
                        c.ProgramCounter += 2;
                };
            }
        }
        public static Action<Chip8Intepreter> SetDelayTimerToVx {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    c.DelayTimer = c.Registers[X];
                    c.ProgramCounter += 2;
                };
            }
        }
        public static Action<Chip8Intepreter> SetSoundTimerToVx
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;
                    c.SoundTimer = c.Registers[X];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> AddVxToIndex {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;

                    c.Index += c.Registers[X];
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> SetIndexToVx
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;

                    c.Index = c.Registers[X] * 5;
                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> StoreDecimalRepresentationOfVxAtIndex
        {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;

                    c.Memory[c.Index] = unchecked((byte)(c.Registers[X] / 100));
                    c.Memory[c.Index + 1] = unchecked((byte)((c.Registers[X] / 10) % 10));
                    c.Memory[c.Index + 2] = unchecked((byte)((c.Registers[X] % 100) % 10));

                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> StoreV0ThroughVxToMemory {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;

                    for (int i = 0; i <= X; i++)
                    {
                        c.Memory[c.Index + i] = c.Registers[i];
                    }

                    c.ProgramCounter += 2;
                };
            }
        }

        public static Action<Chip8Intepreter> FillV0ThroughVxWithMemoryValues {
            get
            {
                return (c) =>
                {
                    var X = (c.Opcode & 0x0F00) >> 8;

                    for (int i = 0; i <= X; i++)
                    {
                        c.Registers[i] = c.Memory[c.Index + i];
                    }
                    c.ProgramCounter += 2;
                };
            }
        }
    }
}
