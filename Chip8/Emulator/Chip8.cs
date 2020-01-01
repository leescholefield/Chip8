using System;
using System.IO;

namespace Chip8.Emulator
{
    /// <summary>
    /// ref https://github.com/ilkkavi/Chip-8/blob/master/Emulator/Chip8.cs
    /// </summary>
    public class Chip8
    {

        #region Properties

        /// <summary>
        /// 16 8-bit registers used for storing data.
        /// </summary>
        public Registers Registers { get; private set; }

        public byte[] Memory { get; private set; }

        public int Index { get; private set; }

        /// <summary>
        /// Currently executing opcode.
        /// </summary>
        public int Opcode { get; private set; }

        public int ProgramCounter { get; private set; }

        /// <summary>
        /// Simple flag used to detect if a ROM file has been loaded.
        /// </summary>
        private bool Loaded = false;

        #endregion

        #region Initialization

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
                Console.WriteLine("Could not open file: ", e.ToString());
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
            Opcode = (Memory[ProgramCounter << 8]) | Memory[ProgramCounter + 1];
            ExecuteOpcode(Opcode);
        }

        private void ExecuteOpcode(int opcode)
        {
            int X = (opcode & 0x0F00) >> 8;
            int Y = (opcode & 0x00F0) >> 4;
            int NN = (opcode & 0x00FF);
            int N = (opcode & 0x000F);

            var func = GetOpcodeAction(opcode);
        }

        private Action<Chip8> GetOpcodeAction(int opcode)
        {
            Action<Chip8> result = null;

            switch(opcode & 0xF000)
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
            }


            return result;
        }

    }
}
