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
    public class CallSubroutineTest
    {

        [TestMethod()]
        public void Current_ProgramCounter_Is_Added_To_Stack()
        {
            var chip8 = new Chip8Intepreter
            {
                ProgramCounter = 23
            };

            Opcodes.CallSubroutine.Invoke(chip8);

            Assert.AreEqual(23, chip8.Stack[0]);
        }

        [TestMethod()]
        public void StackPointer_Is_Incremented()
        {
            var chip8 = new Chip8Intepreter
            {
                StackPointer = 3
            };

            Opcodes.CallSubroutine.Invoke(chip8);

            Assert.AreEqual(4, chip8.StackPointer);
        }

        [TestMethod()]
        public void ProgramCounter_Set_To_Value_Of_Opcode()
        {
            var chip8 = new Chip8Intepreter
            {
                Opcode = 0x0234
            };

            Opcodes.CallSubroutine.Invoke(chip8);

            Assert.AreEqual(564, chip8.ProgramCounter);
        }
    }
}
