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
    public class JumpToAddressTest
    {

        [TestMethod()]
        public void ProgramCounter_Set_To_Last_3_Digits_Of_Current_Opcode()
        {
            var chip8 = new Chip8Intepreter
            {
                Opcode = 0x0234
            };

            Opcodes.JumpToAddress.Invoke(chip8);

            Assert.AreEqual(564, chip8.ProgramCounter);
        }

    }
}
