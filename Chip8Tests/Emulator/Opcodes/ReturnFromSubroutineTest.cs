using Chip8.Emulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chip8Tests.Emulator
{
    [TestClass()]
    public class ReturnFromSubroutineTest
    {

        [TestMethod()]
        public void StackPointer_Is_Decremented()
        {
            var Chip8 = new Chip8Intepreter
            {
                StackPointer = 2
            };

            Opcodes.ReturnFromSubroutine.Invoke(Chip8);

            Assert.AreEqual(1, Chip8.StackPointer);
        }

        [TestMethod()]
        public void ProgramCounter_Set_To_Previous_Stack_Value_Incremented_By_Two()
        {
            var Chip8 = new Chip8Intepreter
            {
                StackPointer = 2,

            };
            Chip8.Stack[1] = 7;

            Opcodes.ReturnFromSubroutine.Invoke(Chip8);

            Assert.AreEqual(9, Chip8.ProgramCounter);
        }
    }
}
