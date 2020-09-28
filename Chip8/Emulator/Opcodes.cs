using System;

namespace Chip8.Emulator
{
    /// <summary>
    /// Contains all of the Chip-8 Opcodes as static <see cref="Action"/>. For the sake of simplicity these Actions will recieve an instance 
    /// of <see cref="Chip8Intepreter"/> and they can alter the public properties of any member.
    /// 
    /// Rather than name these according to their Opcode values they are named from the function they serve; for example, 
    /// 'DXYN' is named 'DrawSprite'. Although it can be a little tricky to mentally map an opcode to its corresponding Action I feel 
    /// it is easier to follow a cycles flow with this naming convention. See the remarks section for a map between Opcodes 
    /// and their equivelant function.
    /// </summary>
    /// <remarks>
    /// Opcode to function map:
    ///  -- 00E0 : <see cref="ClearScreen"/>
    ///  -- 00EE : <see cref="ReturnFromSubroutine"/>
    ///  -- 1NNN : <see cref="JumpToAddress"/>
    ///  -- 2NNN : <see cref="CallSubroutine"/>
    ///  -- 3XNN : <see cref="SkipInstructionIfVxEquals"/>
    ///  -- 4XNN : <see cref="SkipInstructionIfVxNotEquals"/>
    ///  -- 5XY0 : <see cref="SkipInstructionIfVxEqualsVy"/>
    ///  -- 6XNN : <see cref="SetVxTo"/>
    ///  -- 7XNN : <see cref="AddToVx"/>
    ///  -- 8XY0 : <see cref="SetVxToVy"/>
    ///  -- 8XY1 : <see cref="SetVxToVxOrVy"/>
    ///  -- 8XY2 : <see cref="SetVxToVxAndVy"/>
    ///  -- 8XY3 : <see cref="SetVxToVxXorVy"/>
    ///  -- 8XY4 : <see cref="AddVyToVx"/>
    ///  -- 8XY5 : <see cref="SubtractVyFromVx"/>
    ///  -- 8XY6 : <see cref="ShiftVxRightByOne"/>
    ///  -- 8XY7 : <see cref="SetVxToVyMinusVx"/>
    ///  -- 8XYE : <see cref="ShiftVxLeftByOne"/>
    ///  -- 9XY0 : <see cref="SkipInstructionIfVxNotEqualsVy"/>
    ///  -- ANNN : <see cref="SetIndexTo"/>
    ///  -- BNNN : <see cref="JumpToAddressPlusV0"/>
    ///  -- CXNN : <see cref="SetVxToRandomNumber"/>
    ///  -- DXYN : <see cref="DrawSprite"/>
    ///  -- EX9E : <see cref="SkipInstructionIfKeyPressed"/>
    ///  -- EXA1 : <see cref="SkipInstructionIfKeyNotPressed"/>
    ///  -- FX07 : <see cref="SetVxToDelayTimer"/>
    ///  -- FX0A : <see cref="WaitForKeyPress"/>
    ///  -- FX15 : <see cref="SetDelayTimerToVx"/>
    ///  -- FX18 : <see cref="SetSoundTimerToVx"/>
    ///  -- FX1E : <see cref="AddVxToIndex"/>
    ///  -- FX29 : <see cref="SetIndexToVx"/>
    ///  -- FX33 : <see cref="StoreDecimalRepresentationOfVxAtIndex"/>
    ///  -- FX55 : <see cref="StoreV0ThroughVxToMemory"/>
    ///  -- FX65 : <see cref="FillV0ThroughVxWithMemoryValues"/>
    /// </remarks>
    public abstract class Opcodes
    {

        #region Display

        /// <summary>
        /// Sets all the Values in the Graphics array to off
        /// </summary>
        /// // 00E0
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

        // DXYN
        public static Action<Chip8Intepreter> DrawSprite
        {
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

        #endregion

        #region Flow

        // 00EE 
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

        //1NNN
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

        //2NNN
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

        //BNNN
        public static Action<Chip8Intepreter> JumpToAddressPlusV0
        {
            get
            {
                return (c) =>
                {
                    c.ProgramCounter = (c.Opcode & 0x0FFF) + c.Registers[0];
                };
            }
        }

        #endregion

        #region Condition

        //3XNN
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

        //4XNN
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
        // 5XY0
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

        // 9XY0
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

        #endregion

        #region Const

        //6XNN
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

        //7XNN
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

        #endregion

        #region Assign

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

        #endregion

        #region BitOp

        //8XY1
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

        //8XY2 
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

        //8XY3
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

        // 8XY6
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

        // 8XYE
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

        #endregion

        #region Math

        // 8XY4
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

        //8XY5
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

        // 8XY7
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

        #endregion

        #region MEM

        //ANNN
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

        // FX1E
        public static Action<Chip8Intepreter> AddVxToIndex
        {
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

        //FX29 
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

        //FX55
        public static Action<Chip8Intepreter> StoreV0ThroughVxToMemory
        {
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

        //FX65
        public static Action<Chip8Intepreter> FillV0ThroughVxWithMemoryValues
        {
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

        #endregion

        #region Random
        //CXNN
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

        #endregion

        #region Key Press

        //EX9E
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

        //EXA1
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

        //FX0A
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

        #endregion

        #region Timer

        //FX07
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

        //FX15
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

        #endregion

        #region Sound

        //FX18 
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

        #endregion

        #region BCD

        //FX33
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

        #endregion
    }
}
