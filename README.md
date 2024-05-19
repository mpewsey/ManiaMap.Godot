# ManiaMap.Godot

[![Docs](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/docs.yml/badge.svg?event=push)](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/docs.yml)
![Godot .NET 4.0](https://img.shields.io/badge/Godot%20.NET-4.0-blue)
![Version](https://img.shields.io/github/v/tag/mpewsey/ManiaMap.Godot?label=Version)

## About

This package provides components for interfacing the [ManiaMap](https://github.com/mpewsey/ManiaMap) procedural layout generator with Godot .NET.

![BigMap](https://user-images.githubusercontent.com/23442063/158001876-cb3962a8-9826-44e9-bb19-a5779e3f99d6.png)

## Installation

1. Install the `MPewsey.ManiaMap` NuGet package in your Godot project. In Visual Studio, this can be done through `Project > Manage NuGet Packages...`, then searching for the package.
2. Copy the `addons/mpewsey.maniamap` directory from this repository into the `addons` folder of your Godot project.
3. Build your Godot project solution, so that the newly incorporated C# addon scripts are compiled.
4. Enable the addon by going to `Project > Project Settings` and clicking the enable checkbox next to the `ManiaMap.Godot` plugin.
