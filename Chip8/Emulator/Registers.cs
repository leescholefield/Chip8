namespace Chip8.Emulator
{

    /// <summary>
    /// Contains 16 8-bit registers. These are V0 through VF.
    /// </summary>
    public class Registers
    {

        private byte[] V;

        public byte this[int index]
        {
            get
            {
                return V[index];
            }
            set
            {
                V[index] = value;
            }
        }

        public Registers()
        {
            V = new byte[16];
        }
    }
}
