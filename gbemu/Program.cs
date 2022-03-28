// See https://aka.ms/new-console-template for more information

using gbemu;


mainWindow = new Window();
mainWindow.Title = "Canvas Sample";

Gameboy gameboy = new Gameboy();

byte[] rom = File.ReadAllBytes("tetris.gb");



gameboy.loadRom(rom);
gameboy.start();