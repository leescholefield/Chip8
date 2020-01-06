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
    public class AddToVxTest
    {

        [TestMethod()]
        public void Opcode_Value_Is_Added_To_Vx()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x7107
            };
            c.Registers[1] = 2;

            Opcodes.AddToVx.Invoke(c);

            Assert.AreEqual(9, c.Registers[1]);
        }

        [TestMethod()]
        public void ProgramCounter_Is_Incremented_By_2()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x7107,
                ProgramCounter = 0
            };

            Opcodes.AddToVx.Invoke(c);

            Assert.AreEqual(2, c.ProgramCounter);
        }
    }
}
