# Groove

A WebSockets transport layer for Mirror. WebGL is fully supported (for Mirror clients). Please try to break it.

## Features

* Run a Mirror client (including WebGL) and server (on Windows, macOS or Linux) using WebSockets
* Use the power of Mirror to develop your game while being able to support the extreme reach of the WebGL target

## Installation & Using Instructions

* Download the current version package from this page.
* Import Mirror into your project using your favorite method (Asset Store, package, or building from source).
* Import the current version package you downloaded from this page.
* Override the Mirror transport layer to use Groove (see Considerations below). You're done.

## Downloads

* Current version (0.8)

## Considerations

To use Groove, you must override the following in your NetworkManager after importing the Groove scripts:

```C#
public override void InitializeTransport()
{
	Transport.layer = new GrooveTransport();
}
```

## Support

If you have any problems using Groove, please file an issue and be as descriptive as possible and I will attempt to answer as best I can.
