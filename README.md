![SharpGDX Logo](.github/sharpgdx-h.png)

## Cross-platform Game Development Framework
**[SharpGDX](https://sharpgdx.com) is a C# port of Java's cross-platform game development framework, libGDX, based on OpenGL (ES), designed for Windows, Linux, macOS.** It provides a robust and well-established environment for rapid prototyping and iterative development. Unlike other frameworks, ShrpGDX does not impose a specific design or coding style, allowing you the freedom to create games according to your preferences.

## Status
While this is considered to be in the earlier stages of development, and most features do work, I do expect bugs.

I also plan to clean up all the code to make it more consistent with the best practices of C#. This will include changes such as moving get/set methods to properties, cleaning up the documentation and solidifying access (C# and Java differe on the inherit access level of objects that is unspecified).

When I started this project, libGDX was at 1.12.1, over the next few weeks I plan to updgrade this to 1.13.0, though I may wait for 1.13.1.

I am also toying with the idea of making a 'backend' that uses SDL2 instead of GLFW and OpenTK to reduce dependencies on other 3rd party libraries.

## Demos
This project itself has a 'Drop' demo, which is the older version of the libGDX 'Simple Game' from their tutorials.

There is also a demo here: [Super Jumper](https://github.com/SharpGDX/SharpGDX-Demo-Super-Jumper)

Other demos are almost complete, I am fixing bugs as I port them (Cuboc is down to 1 bug). I will also try and port some of the older libGDX demos that are not up to date for current version of libGDX.

## NuGet
You can find an alpha (pre-release) version on NuGet [here](https://www.nuget.org/packages/SharpGDX.Desktop/). This works on Windows and possibly works on Mac and Linux (I do not have a system running Mac or Linux, so cannot be sure, but I see no reason it won't work).

This NuGet relies on OpenTK and the base SharpGDX library, which will be added for you.

## Community & Contribution
For engaging discussions and support, join our official [SharpGDX Discord](https://discord.gg/CcqEzx2q)

## Extensions
- SharpGDX.Controllers
    - This has some hidden dependencies I need to port.
- SharpGDX.Ashley
    - This is complete, though I do plan to clean it up since generics have some subtle differences in C# and Java (lots of unnecessary 'Type' passing in the C# version.
- SharpGDX.Box2D
    - The Java to C# portion is complete. But I have to create all the externs and a DLL for the C++ portion (I don't have all the nice tools that are used for JNI).
SharpGDX.AI
    - I have not started this yet.

--- 

> [!NOTE]
> This is a conversion of the Java framework libGDX, located [here](https://www.libgdx.com). All original code and assets belong to their respective owners.

---

<div>
    <a href="https://www.jetbrains.com/?from=SharpGDX" align="right">
    <img src=".github/jetbrains.svg" alt="JetBrains" class="logo-footer" width="72" align="left">
    <a><br/>

Special thanks to [JetBrains](https://www.jetbrains.com/?from=SharpGDX) for supporting us with open-source licenses for their IDEs. </a>
</div>