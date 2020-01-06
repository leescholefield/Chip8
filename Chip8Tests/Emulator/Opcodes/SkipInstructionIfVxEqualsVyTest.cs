using Chip8.Emulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Tests.Emulator
{
    [TestClass()]
    public class SkipInstructionIfVxEqualsVyTest
    {

        [TestMethod()]
        public void ProgramCounter_Incremented_By_4_If_Vx_Equals_Vy()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x5120,
                ProgramCounter = 0
            };
            c.Registers[1] = 6;
            c.Registers[2] = 6;

            Opcodes.SkipInstructionIfVxEqualsVy.Invoke(c);

            Assert.AreEqual(4, c.ProgramCounter);
        }

        [TestMethod()]
        public void ProgramCounter_Incremented_By_2_If_Vx_Not_Equals_Vy()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x5120,
                ProgramCounter = 0
            };
            c.Registers[1] = 5;
            c.Registers[2] = 6;

            Opcodes.SkipInstructionIfVxEqualsVy.Invoke(c);

            Assert.AreEqual(2, c.ProgramCounter);
        }
    }
}
