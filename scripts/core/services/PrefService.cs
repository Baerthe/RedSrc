namespace Core;
using Core.Interface;
using Godot;
using System;
public sealed class PrefService : IPrefService
{
    public PrefService()
    {
        GD.PrintRich("[color=#00ff88]PrefService initialized.[/color]");
    }
}