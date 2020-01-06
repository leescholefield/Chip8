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
    public class SetVxEqualToVy
    {

        [TestMethod()]
        public void Vx_Is_Set_To_Vy_Value()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x8120
            };
            c.Registers[2] = 7;

            Opcodes.SetVxToVy.Invoke(c);

            Assert.AreEqual(7, c.Registers[1]);
        }

        [TestMethod()]
        public void ProgramCounter_Is_Incremented_By_2()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x8120,
                ProgramCounter = 0
            };

            Opcodes.SetVxToVy.Invoke(c);

            Assert.AreEqual(2, c.ProgramCounter);
        }

    }
}
