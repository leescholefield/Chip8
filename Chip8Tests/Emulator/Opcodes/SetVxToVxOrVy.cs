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
    public class SetVxToVxOrVy
    {

        [TestMethod()]
        public void Vx_Set_To_Vx_Bitwise_Or_Vy()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x8121
            };
            c.Registers[1] = 6;
            c.Registers[2] = 10;

            Opcodes.SetVxToVxOrVy.Invoke(c);

            Assert.AreEqual(14, c.Registers[1]);
        }


        [TestMethod()]
        public void ProgramCounter_Incremented_By_2()
        {
            var c = new Chip8Intepreter
            {
                Opcode = 0x8121,
                ProgramCounter = 0
            };

            Opcodes.SetVxToVxOrVy.Invoke(c);

            Assert.AreEqual(2, c.ProgramCounter);
        }
    }
}
