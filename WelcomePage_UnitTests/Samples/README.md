Samples
=======

This folder contains some sample markdown documents, suitable for interactive testing of the renderer.

To use it, start `WelcomePage.WebServer` pointing at this filesystem folder. For example:

    WelcomePage.WebServer http://localhost:15220 D:\Source\vs-welcome-page\WelcomePage_UnitTests\Samples

Test Cases
----------

* Bulleted List
* With an external link to [github](http://github.com/rlipscombe/vs-welcome-page/) in it.
* With a link to [another page](another-page) in the same folder.
* With a [[github-wiki]] style link in it.
* What about a link to a page in a subdirectory? [Try it](subdir/foo).

### links with spaces ###

If you create a link like this: `[link with spaces](link with spaces)`:

For example: [link with spaces](link with spaces)

1. MarkdownPad 2 doesn't recognise it as a link.
2. `vs-welcome-page` doesn't recognise it as a link.
3. GitHub wiki leaves the spaces in the name, and doesn't check to see if it exists. 

### github wiki style ###

If you create a link like this: `[[github wiki style]]`:

For example: [[github wiki style]]

1. MarkdownPad 2 doesn't recognise it as a link.
2. `vs-welcome-page` doesn't recognise it as a link.
3. GitHub wiki converts it to `Github-wiki-style`. If it doesn't exist, it's coloured red, and takes you to the edit page. 

Code
----

This is a snippet of C# code with some strings, numbers, etc. in it.

    namespace Foo
	{
		// Program entry point.
		static void Main(string[] args)
		{
		    Console.WriteLine("Hello World");
			Console.WriteLine("Meaning of life = {0}", 42);
		}
	}

Images
------

### Remote Image ###

![Remote Image](http://lorempixel.com/300/200/cats/)
