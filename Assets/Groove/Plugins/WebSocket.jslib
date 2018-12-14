var LibraryWebSockets = {
$webSocketInstances: [],

SocketCreate: function(url, MsgRcvd, ConnectedCallback, DisconnectedCallback, ErrorCallback)
{
	var str = Pointer_stringify(url);
	var socket = {
		socket: new WebSocket(str),
		buffer: new Uint8Array(0),
		error: null
	}

	socket.socket.binaryType = 'arraybuffer';

	socket.socket.onopen = function(e){
		Runtime.dynCall('v', ConnectedCallback, 0);
	};

	socket.socket.onmessage = function (e) {
		console.log("received data");
		// Groove should only ever send and receive ArrayBuffers
		if (e.data instanceof ArrayBuffer)
		{
			var array = new Uint8Array(e.data);
			var buffer = _malloc(array.byteLength);
			HEAPU8.set(array, buffer);

			var args = [buffer, array.byteLength];
			console.log("calling native code");
			Runtime.dynCall('vii', MsgRcvd, args);
		}
	};

	socket.socket.onclose = function (e) {
		if (e.code != 1000)
		{
			Runtime.dynCall('vi', ErrorCallback, [e.code]);
		}
		else{
			Runtime.dynCall('v', DisconnectedCallback, 0);
		}
	}
	var instance = webSocketInstances.push(socket) - 1;
	return instance;
},

SocketState: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	return socket.socket.readyState;
},

SocketError: function (socketInstance, ptr, bufsize)
{
 	var socket = webSocketInstances[socketInstance];
 	if (socket.error == null)
 		return 0;
    var str = socket.error.slice(0, Math.max(0, bufsize - 1));
    writeStringToMemory(str, ptr, false);
	return 1;
},

SocketSend: function (socketInstance, ptr, length)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.send (HEAPU8.buffer.slice(ptr, ptr+length));
},

SocketClose: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.close();
}
};

autoAddDeps(LibraryWebSockets, '$webSocketInstances');
mergeInto(LibraryManager.library, LibraryWebSockets);
