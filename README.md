# ManiaMap.Godot

[![Tests](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/tests.yml/badge.svg)](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/tests.yml)
[![Docs](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/docs.yml/badge.svg?event=push)](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/docs.yml)
![Godot .NET 4.0](https://img.shields.io/badge/Godot%20.NET-4.2-blue)
![Version](https://img.shields.io/github/v/tag/mpewsey/ManiaMap.Godot?label=Version)

## About

This package provides components for interfacing the [ManiaMap](https://github.com/mpewsey/ManiaMap) procedural layout generator with Godot .NET.

![BigMap](https://user-images.githubusercontent.com/23442063/158001876-cb3962a8-9826-44e9-bb19-a5779e3f99d6.png)

## Installation

1. Install the `MPewsey.ManiaMap` NuGet package in your Godot project. In Visual Studio, this can be done through `Project > Manage NuGet Packages...`, then searching for the package.
2. Copy the `addons/mpewsey.maniamap` directory from this repository into the `addons` folder of your Godot project.
3. Build your Godot project solution, so that the newly incorporated C# addon scripts are compiled.
4. Enable the addon by going to `Project > Project Settings` and clicking the enable checkbox next to the `ManiaMap.Godot` plugin.

## Documentation

For information on the scripting API, as well as instructions related to procedurally generating a room layout, see the [documentation](https://mpewsey.github.io/ManiaMap.Godot/md_pages_layout_generation_instructions.html).

## Samples

Sample scenes are available in the `samples` directory. To explore them, clone this repository, open it in Godot, and perform Steps 3 through 4 under [Installation](#Installation). The requisite NuGet package dependencies should automatically be downloaded when building in Visual Studio.

<table style='text-align: center'>
    <tr>
        <td width='50%' align='center'>
            <img width=450 alt='layout map book' src='https://github.com/mpewsey/ManiaMap.Godot/assets/23442063/a7b18eac-1432-402c-aec0-44b0882397e5'><br>
            Layout tile map book sample with onionskin map display.
        </td>
        <td width='50%' align='center'>
            <img width=450 alt='layout map' src='https://github.com/mpewsey/ManiaMap.Godot/assets/23442063/cf86ca2b-5dd8-4480-9932-375fd1c933e6'><br>
            Layout tile map sample.
        </td>
    </tr>
    <tr>
        <td width='50%' align='center'>
            <img width=450 alt='2d layout' src='https://github.com/mpewsey/ManiaMap.Godot/assets/23442063/07c9da75-0489-4362-8957-05634932d28c'><br>
            2D layout generation sample.
        </td>
        <td width='50%' align='center'>
            <img width=450 alt='3d layout' src='https://github.com/mpewsey/ManiaMap.Godot/assets/23442063/d8cbda20-e74c-4fc4-967b-c8a2c8333bbb'><br>
            Multi-layer 3D layout generation sample.
        </td>
    </tr>
</table>
