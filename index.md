---
layout: default
title: About Roger
---

vs-welcome-page
==

When you open a solution, this extension automatically looks for a ReadMe
file and renders and displays it in the embedded Visual Studio web browser.

This allows you to provide easy-to-find documentation for new team members,
or others who are new to your project.

It supports Markdown text, meaning that it works completely transparently for
GitHub projects, and has preliminary support for GitHub wiki pages.

You can download it from the [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/0aff56ff-edf3-4939-a8f5-400f1279ac2a).

Questions
--

### How does this work? ###

It's installed as a Visual Studio extension (a `.VSIX` file). When Visual Studio is run, it subscribes to the *solution open* event. When you open a solution, it starts an embedded web server, which hosts a simple web application. This web application is responsible for looking for, and rendering, any `README` file that it finds in the solution folder.

### What formats are supported? ###

Currently, just GitHub-flavoured Markdown. I might add other formats in future.

### What files does it look for? ###

It will use the first file it finds from the following list.

* `Index.md`
* `Home.md`
* `README.md`

### How do I install it? ###

You will need Visual Studio 2012 Professional or better.

1. [Download](http://visualstudiogallery.msdn.microsoft.com/0aff56ff-edf3-4939-a8f5-400f1279ac2a) it from the Visual Studio Gallery.
2. Double-click the `.VSIX` file.

### Why did you do this? ###

Two reasons:

1. It's often difficult when joining a new project to work out where everything lives, how to build the source code, how to fire up a test environment, etc. I figured that if I could display a welcome page for new developers right there in the IDE, this might improve.
2. I wanted to see if I could knock out a Visual Studio extension in a weekend.
