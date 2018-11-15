# Groove

A WebSockets transport layer for Mirror. In case you've made your way here, this project is in development and is not fully implemented yet.

## Features

* Run a Mirror client (including WebGL) and server (not using WebGL) using WebSockets
* Use the power of Mirror to develop your game while being able to support the extreme reach of the WebGL target

## Downloads

* Current version (0.5)

## Considerations

In its current iteration, Groove requires you to use a separate MonoBehaviour as a "coroutine host" for the client. Simply add this "ClientCoroutineHostBehaviour" to a GameObject in your Scene and you are good to go, as long as you are using the Groove transport layer.

You can use the Groove transport layer by overriding the following in your NetworkManager after importing the Groove scripts:

```C#
public override void InitializeTransport()
{
	Transport.layer = new GrooveTransport();
}
```

## Support

If you have any problems using Groove, please file an issue and be as descriptive as possible and I will attempt to answer as best I can.