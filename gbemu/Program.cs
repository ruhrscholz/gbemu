using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace gbemu;

static class Program
{
    private static IWindow _window;
    private static Gameboy _gameboy;
    private static void Main(string[] args){
        _gameboy = new Gameboy();

        byte[] rom = File.ReadAllBytes("tetris.gb");



        _gameboy.loadRom(rom);
        
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(160, 144);
        options.VSync = false;
        options.UpdatesPerSecond = 1 << 22;

        _window = Window.Create(options);
        
        //Assign events.
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;

        //Run the window.
        _window.Run();
    }

    private static void OnLoad()
    {
        //Set-up input context.
        IInputContext input = _window.CreateInput();
        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += KeyDown;
        }
    }

    private static void OnRender(double obj)
    {
        //Here all rendering should be done.
    }

    private static void OnUpdate(double obj)
    {
        _gameboy.step();
    }

    private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
    {
        //Check to close the window on escape.
        if (arg2 == Key.Escape)
        {
            _window.Close();
        }
    }
}