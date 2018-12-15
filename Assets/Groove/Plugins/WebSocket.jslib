var LibraryWebSockets = {
$webSocketInstances: [],

SocketCreate: function(url, ConnectedCallback, DataReceived, ErrorCallback, DisconnectedCallback)
{
	var str = Pointer_stringify(url);
	console.log("connecting to: "+str+"in JS layer");
	var socket = {
		socket: new WebSocket(str),
		buffer: new Uint8Array(0),
		error: null,
		messages: []
	}

	socket.socket.binaryType = 'arraybuffer';

	socket.socket.onopen = function(e){
		console.log("connected successfully in JS layer");
		Runtime.dynCall('v', ConnectedCallback, 0);
		console.log("dispatched back to JS layer");
	}

	socket.socket.onmessage = function (e) {
		console.log("Received data");
		if (e.data instanceof Blob)
		{
			var reader = new FileReader();
			reader.addEventListener("loadend", function() {
				var array = new Uint8Array(reader.result);
				RaiseMirrorData(array);
			});
			reader.readAsArrayBuffer(e.data);
		}
		else if (e.data instanceof ArrayBuffer)
		{
			var array = new Uint8Array(e.data);
			RaiseMirrorData(array);
		}
	}

	socket.socket.onclose = function (e) {
		if (e.code != 1000)
		{
			Runtime.dynCall('vi', ErrorCallback, [e.code]);
		}
		Runtime.dynCall('v', DisconnectedCallback, 0);
	}
	var instance = webSocketInstances.push(socket) - 1;
	return instance;

	function RaiseMirrorData(array){
		var buffer = _malloc(array.byteLength);
		HEAPU8.set(array, buffer);

		var args = [buffer, array.byteLength];
		console.log("calling native code");
		Runtime.dynCall('vii', DataReceived, args);
	}
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
