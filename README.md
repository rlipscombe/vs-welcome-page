vs-welcome-page
===============

When you open a solution, this extension automatically looks for a ReadMe
file and renders and displays it in the embedded Visual Studio web browser.

This allows you to provide easy-to-find documentation for new team members,
or others who are new to your project.

It supports Markdown text, meaning that it works completely transparently for
GitHub projects, and has preliminary support for GitHub wiki pages.

Feel free to visit the [home page](http://rlipscombe.github.io/vs-welcome-page).
You can download it from the [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/0aff56ff-edf3-4939-a8f5-400f1279ac2a).

Debugging vs-welcome-page
--

1. Set 'WelcomePage' as the Startup Project.
2. Open the 'Debug' pane in the properties for 'WelcomePage'.
3. Select 'Start external program' and enter `C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe`.
4. Set 'Command line arguments' to `/rootsuffix Exp`.

### Disabling ReSharper ###

If, like me, you've got ReSharper installed, you might find that loading it
into the Experimental instance of Visual Studio slows down your debugging.
If you're not developing a ReSharper plugin, you might want to disable it.

To do this:

1. Start debugging your Visual Studio extension.
2. In the experimental instance of Visual Studio, open the Command Window
   (View -> Other Windows -> Command Window).
3. Enter the command `ReSharper_Suspend`.
4. Close the experimental instance of Visual Studio.

You only need to do this once.

While ReSharper will still be loaded in the experimental instance of Visual
Studio, it will be disabled, and shouldn't get in your way.
