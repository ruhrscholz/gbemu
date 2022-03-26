namespace gbemu;

public class Gameboy
{
    internal Cpu _cpu;
    internal Memory _memory;
    public Gameboy()
    {
        this._cpu = new Cpu(this);
        this._memory = new Memory(this);
    }

    public void loadRom(byte[] rom)
    {
        this._memory.loadRom(rom);
    }

    public void start()
    {
        this._cpu.run();
    }
}