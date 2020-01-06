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
    public class SkipInstructionIfVxEqualTest
    {

        [TestMethod()]
        public void ProgramCounter_Incremented_By_4_When_Opcode_Equals()
        {
            var chip8 = new Chip8Intepreter
            {
                Opcode = 0x0105,
                ProgramCounter = 0
            };
            chip8.Registers[1] = 5;

            Opcodes.SkipInstructionIfVxEquals.Invoke(chip8);

            Assert.AreEqual(4, chip8.ProgramCounter);
        }

        [TestMethod()]
        public void ProgramCounter_Incremented_By_2_When_Opcode_Not_Equals()
        {
            var chip8 = new Chip8Intepreter
            {
                Opcode = 0x0105,
                ProgramCounter = 0
            };

            Opcodes.SkipInstructionIfVxEquals.Invoke(chip8);

            Assert.AreEqual(2, chip8.ProgramCounter);
        }
    }
}
