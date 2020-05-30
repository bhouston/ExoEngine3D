# ExoEngine3D

A basic 3D engine built using C# and the .NET platform

July 27, 2002

## Introduction

For my fourth year computer graphics course I wrote a little 3D engine using C# and the .NET platform.  I decided to go this route as opposed to the C++ route that everyone else took in the course because I wanted to see whether C# lived up to it's hype.  Surprising, after writing about 600kB of code in C# it seems like it is a decent language after all and possibly an effective replacement for the C++ even in demanding field of real-time graphics.  When I compare C# to C++ I find it's best features are garbage collection, less convoluted syntax and true object orientation.

Just a quick disclaimer before I go too far -- please remember that I am only a student (and a cognitive science/neuroscience student at that) and not John Carmack thus don't get your expectations too high.

## How the Engine Works

I must immediately give credit for the OpenGL/C# library that Lloyd Dupont created -- it is an amazingly easy library to use.  I have modified the library in the course of creating this project but not significantly.

This 3D engine imports it's level/worlds data from the popular Worldcraft editor .  Strangely, Worldcraft outputs it's world/level data in sets of bounding planes which define the contours of solid objects.  Thus one has to convert the bounding plane sets into their respective sets of polygons.  The resulting set of faces is then quickly optimized to remove hidden/redundancy faces created by adjacent objects.  Then this face set is converted into a binary space partition tree (commonly just called a "BSP tree") representation for both collision detection purposes and efficient visibility calculations.  There is also some auxiliary code that recognizes specifically defined entities in the Worldcraft data such as the animated pond and the various duck sprites -- but that is pretty simple.

This engine allows for polygons to be rendered using reflection mapping, (fake) Phong shading, Gouraud shading or just simple flat shading.  The engine uses reflection mapping to get the somewhat realistic look of the pond's waves.  The (fake) Phong shading is used on the ducks in order to make them look shiny and smoothly rounded -- the primary effect of Phong shading (usually called specular reflection) is the viewer/camera dependent white highlights.

The 600kB of code responsible for this engine is divided into three parts: "ExocortexNative", a C++ support library for OpenGL and TIFF images, "Exocortex", a C# library that I am using across projects, and "ExoEngine", the code that is specific to this application.  The "Exocortex" library actually contains some fairly reusable classes for 3D applications such as OpenGL compatible matrix, vector and quaternion classes as well as more specialized classes for Marching Cubes and multidimensional fast Fourier transforms.
