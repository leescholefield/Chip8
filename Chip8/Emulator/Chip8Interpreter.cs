using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Chip8.Emulator
{
    /// <summary>
    /// ref https://github.com/ilkkavi/Chip-8/blob/master/Emulator/Chip8.cs
    /// </summary>
    public class Chip8Intepreter
    {

        #region Properties

        /// <summary>
        /// 16 8-bit registers used for storing data.
        /// </summary>
        public Registers Registers { get; private set; }

        public byte[] Memory { get; private set; }

        public int Index { get; set; }

        /// <summary>
        /// Currently executing opcode.
        /// </summary>
        public int Opcode { get; set; }

        public int ProgramCounter { get; set; }

        public Screen GraphicsArray { get; set; }

        public int[] Stack { get; private set; }
        public int StackPointer { get; set; }

        public byte DelayTimer { get; set; }

        public byte SoundTimer { get; set; }

        public Keypad KeypadArray { get; set; }

        public bool Redraw { get; set; }


        /// <summary>
        /// Simple flag used to detect if a ROM file has been loaded.
        /// </summary>
        private bool Loaded = false;

        // Fonts are constructed from a 4 bit nibble of each byte, five bytes per character
        private readonly byte[] fontset = {0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
                                           0x20, 0x60, 0x20, 0x20, 0x70, // 1
                                           0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
                                           0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
                                           0x90, 0x90, 0xF0, 0x10, 0x10, // 4
                                           0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
                                           0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
                                           0xF0, 0x10, 0x20, 0x40, 0x40, // 7
                                           0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
                                           0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
                                           0xF0, 0x90, 0xF0, 0x90, 0x90, // A
                                           0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
                                           0xF0, 0x80, 0x80, 0x80, 0xF0, // C
                                           0xE0, 0x90, 0x90, 0x90, 0xE0, // D
                                           0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
                                           0xF0, 0x80, 0xF0, 0x80, 0x80  // F
                                          };

        #endregion

        #region Initialization

        public Chip8Intepreter()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes all components to their default values.
        /// </summary>
        private void Initialize()
        {
            Registers = new Registers();
            Memory = new byte[1024 * 4];
            Index = 0;
            Opcode = 0;
            ProgramCounter = 0x200;
            GraphicsArray = new Screen();
            Stack = new int[16];
            StackPointer = 0;
            KeypadArray = new Keypad();
            Redraw = false;

            for (int i = 0; i < 80; i++)
                Memory[i] = fontset[i];
        }

        public void LoadRom(string loc)
        {
            Initialize();

            try
            {
                byte[] buffer = File.ReadAllBytes(loc);

                // copy buffer into memory starting at 0x200
                for (int i = 0; i < buffer.Length; i++)
                {
                    Memory[i + 512] = buffer[i];
                }

                Loaded = true;
            }
            catch (IOException e)
            {
                throw new Exception("Could not open file: ", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loadin file: ", e.ToString());
            }
        }

        #endregion

        /// <summary>
        /// Proccesses the next Game cycle. This consists of getting the next opcode from memory and executing it.
        /// </summary>
        public void Cycle()
        {
            if (Loaded == false)
                throw new ApplicationException("No rom has been loaded");

            Opcode = (Memory[ProgramCounter] << 8) | Memory[ProgramCounter + 1];
            ExecuteOpcode(Opcode);

            if (DelayTimer > 0)
                DelayTimer--;
            if (SoundTimer > 0)
            {
                if (SoundTimer == 1)
                    // play sound
                SoundTimer--;
            }
        }

        public void KeyPressed(byte key)
        {
            if (key > 16)
                throw new InvalidOperationException("Key must not be greater than 16");

            KeypadArray[key] = 1;
        }

        public void KeyReleased(byte key)
        {
            if (key > 16)
                throw new InvalidOperationException("Key must not be greater than 16");

            KeypadArray[key] = 0;
        }

        private void ExecuteOpcode(int opcode)
        {
            var func = GetOpcodeAction(opcode);
            func.Invoke(this);
        }

        private Action<Chip8Intepreter> GetOpcodeAction(int opcode)
        {
            Action<Chip8Intepreter> result;
            switch (opcode & 0xF000)
            {
                case 0x000: // 0x00E0 clear screen || 0x00EE return from subroutine
                    switch(opcode & 0x000F)
                    {
                        case 0x0000:
                            result = Opcodes.ClearScreen;
                            break;
                        case 0x000E:
                            result = Opcodes.ReturnFromSubroutine;
                            break;
                        default:
                            throw new IOException("Unknown opcode: " + opcode);

                    }
                    break;

                case 0x1000:
                    result = Opcodes.JumpToAddress;
                    break;

                case 0x2000:
                    result = Opcodes.CallSubroutine;
                    break;

                case 0x3000:
                    result = Opcodes.SkipInstructionIfVxEquals;
                    break;

                case 0x4000:
                    result = Opcodes.SkipInstructionIfVxNotEquals;
                    break;

                case 0x5000:
                    result = Opcodes.SkipInstructionIfVxEqualsVy;
                    break;

                case 0x6000:
                    result = Opcodes.SetVxTo;
                    break;

                case 0x7000:
                    result = Opcodes.AddToVx;
                    break;

                case 0x8000:
                    switch (opcode & 0x000F)
                    {
                        case 0x0000:
                            result = Opcodes.SetVxToVy;
                            break;
                        case 0x0001:
                            result = Opcodes.SetVxToVxOrVy;
                            break;
                        case 0x0002: 
                            result = Opcodes.SetVxToVxAndVy;
                            break;
                        case 0x0003:
                            result = Opcodes.SetVxToVxXorVy;
                            break;
                        case 0x0004:
                            result = Opcodes.AddVyToVx;
                            break;
                        case 0x0005:
                            result = Opcodes.SubtractVyFromVx;
                            break;
                        case 0x0006:
                            result = Opcodes.ShiftVxRightByOne;
                            break;
                        case 0x0007:
                            result = Opcodes.SetVxToVyMinusVx;
                            break;
                        case 0x000E:
                            result = Opcodes.ShiftVxLeftByOne;
                            break;
                        default:
                            throw new ApplicationException("Unknown opcode: " + opcode);
                    }
                    break;

                case 0x9000:
                    result = Opcodes.SkipInstructionIfVxNotEqualsVy;
                    break;

                case 0xA000:
                    result = Opcodes.SetIndex;
                    break;

                case 0xB000:
                    result = Opcodes.JumpToAddressPlusV0;
                    break;

                case 0xC000:
                    result = Opcodes.SetVxToRandomNumber;
                    break;

                case 0xD000:
                    result = Opcodes.DrawSprite;
                    break;

                case 0xE000: // Two opcodes start with 0xE
                    switch (opcode & 0x000F)
                    {
                        case 0x000E:
                            result = Opcodes.SkipInstructionIfKeyPressed;
                            break;
                        case 0x0001:
                            result = Opcodes.SkipInstructionIfKeyNotPressed;
                            break;
                        default:
                            throw new ApplicationException("Unknown opcode: " + opcode);
                    }
                    break;

                case 0xF000: // 9 opcodes start with 0xFX
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007: // FX07: Sets VX to the value of the delay timer
                            result = Opcodes.SetVxToDelayTimer;
                            break;
                        case 0x000A: // FX0A: A key press is awaited, and then stored in VX
                            result = Opcodes.WaitForKeyPress;
                            break;
                        case 0x0015: // FX15: Sets the delay timer to VX
                            result = Opcodes.SetDelayTimerToVx;
                            break;
                        case 0x0018: // FX18: Sets the sound timer to VX
                            result = Opcodes.SetSoundTimerToVx;
                            break;
                        case 0x001E: // FX1E: Adds VX to I
                            result = Opcodes.AddVxToIndex;
                            break;
                        case 0x0029: // FX29: Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font
                            result = Opcodes.SetIndexToVx;
                            break;
                        case 0x0033:
                            result = Opcodes.StoreDecimalRepresentationOfVxAtIndex;
                            break;
                        case 0x0055: // FX55: Stores V0 to VX in memory starting at address I
                            result = Opcodes.StoreV0ThroughVxToMemory;
                            break;
                        case 0x0065: // FX65: Fills V0 to VX with values from memory starting at address I
                            result = Opcodes.FillV0ThroughVxWithMemoryValues;
                            break;
                        default:
                            throw new ApplicationException("Unknown opcode: " + opcode);
                    }
                    break;
                default:
                    throw new ApplicationException("Unknown opcode: " + opcode);
            }


            return result;
        }

    }
}
