

using Chip8.Emulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chip8Tests.Emulator
{
    [TestClass()]
    public class ClearScreenTest
    {
        private Chip8Intepreter Chip8 = new Chip8Intepreter();

        [TestMethod()]
        public void Resets_Grphics_Array()
        {
            Chip8.GraphicsArray[2] = 5;
            Opcodes.ClearScreen.Invoke(Chip8);

            Assert.AreEqual(0, Chip8.GraphicsArray[2]);
        }

        [TestMethod()]
        public void Increments_ProgramCounter_By_Two()
        {
            var c = new Chip8Intepreter();
            var prev = c.ProgramCounter;
            Opcodes.ClearScreen.Invoke(c);

            Assert.AreEqual(prev + 2, c.ProgramCounter);
        }
    }
}
