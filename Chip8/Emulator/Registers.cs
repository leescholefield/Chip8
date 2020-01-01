namespace Chip8.Emulator
{
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
