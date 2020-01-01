using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chip8.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8.Emulator.Tests
{
    [TestClass()]
    public class RegistersTests
    {
        private readonly Registers Registers = new Registers();

        [TestMethod()]
        public void Setting_Register_Value()
        {
            Registers[1] = 5;
        }

        [TestMethod()]
        public void Getting_Register_Value()
        {
            Registers[3] = 7;
            var val = Registers[3];
            Assert.AreEqual(7, val);
        }
    }
}