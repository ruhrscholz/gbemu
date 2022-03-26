namespace gbemu;

public class Cpu
{
    private Gameboy _gameboy;
    internal Registers _registers;

    internal class Registers
    {
        byte A;
        byte B;
        byte C;
        byte D;
        byte E;
        byte H;
        byte L;
        byte Flags;
        ushort SP; // Stack Pointer
        internal ushort PC; // Program Counter
        
        ushort AF
        {
            get => (ushort)((ushort)(A << 4) ^ Flags);
            set {
                A = (byte)(value >> 4);
                Flags = (byte)(value & 0xF);
            }
        }
        
        ushort BC
        {
            get => (ushort)((ushort)(B << 4) ^ C);
            set {
                B = (byte)(value >> 4);
                C = (byte)(value & 0xF);
            }
        }
        
        ushort DE
        {
            get => (ushort)((ushort)(D << 4) ^ E);
            set {
                D = (byte)(value >> 4);
                E = (byte)(value & 0xF);
            }
        }
        
        ushort HL
        {
            get => (ushort)((ushort)(H << 4) ^ L);
            set {
                H = (byte)(value >> 4);
                L = (byte)(value & 0xF);
            }
        }
    }

    public Cpu(Gameboy gameboy)
    {
        this._gameboy = gameboy;
        this._registers = new Registers();
    }

    internal void run()
    {
        while (true)
        {
            switch (_gameboy._memory.Get(_registers.PC))
            {
                case 0xC3:
                    byte lo = _gameboy._memory.Get((ushort)(_registers.PC+0x0008));
                    byte hi = _gameboy._memory.Get((ushort)(_registers.PC+0x0016));
                    _registers.PC = (ushort)(hi << 4 | lo);
                    break;
                default:
                    throw new NotImplementedException(
                        $"OP-Code {_gameboy._memory.Get(_registers.PC).ToString("x")} is not implemented");
                    break;
            }
        }
            
    }
}