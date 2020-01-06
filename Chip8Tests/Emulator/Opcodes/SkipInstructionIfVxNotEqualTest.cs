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
    public class SkipInstructionIfVxNotEqualTest
    {

        [TestMethod()]
        public void ProgramCounter_Incremented_By_4_When_Vx_Not_Equal()
        {
            var c = new Chip8Intepreter
            {
                ProgramCounter = 0,
                Opcode = 0x0108
            };
            c.Registers[1] = 6;

            Opcodes.SkipInstructionIfVxNotEquals(c);

            Assert.AreEqual(4, c.ProgramCounter);
        }

        [TestMethod()]
        public void ProgramCounter_Incremented_By_2_When_Vx_Is_Equal()
        {
            var c = new Chip8Intepreter
            {
                ProgramCounter = 0,
                Opcode = 0x0108
            };
            c.Registers[1] = 8;

            Opcodes.SkipInstructionIfVxNotEquals(c);

            Assert.AreEqual(2, c.ProgramCounter);
        }
    }
}
