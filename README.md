# ManiaMap.Godot

[![Tests](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/tests.yml/badge.svg)](https://github.com/mpewsey/ManiaMap.Godot/actions/workflows/tests.yml)
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

## Samples

Sample projects are available in the `samples` directory. To explore them, clone this repository and open it like any other Godot project.

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

## Usage

The following subsections outline how to procedurally generate a layout.

### Step 1: Create Rooms and Room Templates

The procedural generator creates layouts by pulling from user-defined room templates. To the generator, a room is a collection of cells in a grid, with information, such as the available door connections, assigned to them.

#### Creating a Room

1. From an empty scene in Godot, create a `RoomNode2D` or `RoomNode3D` depending on whether you wish to develop in 2D or 3D. One of these node types must serve as the root of your room scene.
2. In the inspector, specify the number of cell rows and columns, along with the cell size to create the bounding shape of the room.
3. When the room node is selected, a toolbar will appear in the Godot main window (Circled in red in the screenshot below). The options on this toolbar may be used to edit the cell activities to further define the shape of the room. For instance, selecting the toggle edit mode button, then clicking or dragging over cells in the view will toggle the targeted cells on or off.
    * Note that for `RoomNode3D`, you must be in the top view in order to edit cell activities. The top view can be navigated to by clicking the positive Y axis on the axis gizmo or selecting `Top View` from the 3D main window's perspective menu. The toolbar will show a check mark when the cells can be edited.

In the below screenshot, the cells for an angle shaped room have been created, with the active cells shown in blue and inactive cells shown in red and crossed-out. The active cells are the regions where we intend to build our scene.

![Screenshot 2024-05-31 141058](https://github.com/mpewsey/ManiaMap.Godot/assets/23442063/15c94ef3-387c-43fa-8b46-a6bd2341dba4)

#### Creating Doors

Doors define the locations where two rooms can be connected. At least one door must be added to a room.

1. Add a `DoorNode2D` or `DoorNode3D` as a child of the room based on your room type.
   * To save time, you may wish to create your doors as separate scenes with any related nodes, such as sprites or collision, as children. These scenes can then be referenced in your room scene.
2. Position the door within the room and assign its direction and connection constraints.
3. To auto assign the closest cell and direction to the door, make sure the applicable flags are selected in the inspector and click the `Auto Assign` button on the room toolbar. The assigned door direction will be based on its location relative to the center of its assigned cell.

Note: Additional room child nodes such as `CollectableSpot2D`, `Feature2D`, `RoomFlag2D`, or their respective 3D nodes can also be added to the room if you wish them to be included.

![Screenshot 2024-05-31 150604](https://github.com/mpewsey/ManiaMap.Godot/assets/23442063/f7bff083-d1a2-452f-9d57-c79f68b1b31e)

#### Exporting Room Templates

The procedural generator uses one or more `RoomTemplateResource` exported from `RoomNode2D` or `RoomNode3D` to generate layouts.

To perform auto assignment and save the room template for an individual room, click the `Update Room Template` button on the room toolbar. This will create a `RoomTemplateResource` with the `.room_template.tres` extension at the same path as the scene.

Alternately, to perform this operation on multiple saved rooms within a project, select the `Project > Tools > Mania Map > Batch Update Room Templates` option from the menu.
