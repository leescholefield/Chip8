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
    public class SetVxToTest
    {

        [TestMethod()]
        public void Vx_Is_Set_To_Opcode_Value()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x6105
            };

            Opcodes.SetVxTo.Invoke(c);

            Assert.AreEqual(5, c.Registers[1]);
        }

        [TestMethod()]
        public void ProgramCounter_Is_Incremented_By_2()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x6105,
                ProgramCounter = 0
            };
            Opcodes.SetVxTo.Invoke(c);

            Assert.AreEqual(2, c.ProgramCounter);
        }
    }
}
