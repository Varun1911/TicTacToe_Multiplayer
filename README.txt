-> create a network manager, there should be only one, its a singleton
-> select a transport layer, can use unity transport layer but its not necessary
-> make a player prefab which will be spawned every time a player connects, it should have the network object script attached to it, this is not necessary
-> populate the network prefabs list, anything that needs to be on the network should be on this list, player prefab is added by default


-> server : just the server with no player 
-> client : a player connected to the server
-> host : both server and a client

-> now for every client a player is spawned, but the positions are not synced and if we add movement logic and move one player, it controls all the other players as well. To fix this we derive from NetworkBehaviour instead of Monobehaviour. It has fields like IsHost, IsServer, IsClient, IsOwner etc. We can use IsOwner to run the logic only for the player that the client/host owns.

-> to synchronise the movement, we need a component called network transform, however bandwidth is really imp, so donot send any unneccesary data


-> However this creates an issue, the client recieves data from the server to sync, but the client cannot send data to the server, so it cannot update its own transform data. You hvae two options here - 1) client can tell the server that it would like to move and then server would move it and send the data back, this is useful when you dont want to trust the client. 2) but for a casual game, you can add the ClientNetworkTransform component instead of NetworkTranform to the player prefab. However this is not available by default and you need to add the Multiplayer Sample Utilities package from the following github link : https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop.git?path=/Packages/com.unity.multiplayer.samples.coop#main


-> You can just make your own ClientNetworkTransform script by deriving from NetworkTransform and overriding the function OnIsServerAuthoritative() and return false.


-> NetworkVariable<T> : A variable that is synced over the network. The class containing network variable must derive from NetwrokBehaviour. Also it must be initialized during definition or as soon as the object is created. You cannnot initialze it later.
-> Updating NetworkVariable : varName.value = newVal;

-> The client cannot update their network varibale unless you explicitly allow them during initialization.
 NetworkVariable<int> var1 = new NetworkVariable(1, NetworkVaribaleReadPermission.Everyone, NetworkVariableWritePermission.Owner)
 A client can only write to their own network variables and cannot write to other clients' networks vars. Server can write to all network variables.

-> OnValueChanged : Network varibales have a event that gets triggered on value change

-> NetworkBehaviour has a virtual funtion which gets called when the host/client is spawned : OnNetworkSpawn(). You can override this function and never use awake and start for network objects.


-> Custom data types for Network variables : you can only use data types that are value types and not reference types i.e you cannot use data types that can be null. 
-> But you can create custom types to organise data but they should also contain all valid types. You can create a struct but not a class as structs are value types. You also need to implement the interface INetworkSerializable in your struct. serializer.SerializeValue(ref varName);

-> To send strings you need to use FixedStringxBytes data type


-> spawning objects on the network : turn the obj into a prefab and add the NetworkObject component and add the prefab to the network prefabs list on the network manager. Instantiate the obj but this will only spawn the obj on the owner so we need to spawn it on the network. Call the Spawn function on the network object to spawn it on the network. 
You can only spawn network objects on the server and not on the client, to spawn on client you need to use serverRPC

-> To despawn you can either destroy the object by using Destroy(obj) or desapwn from the network by using Despawn() func on the network obj. Desapawn() takes parameters which tells whether to destroy the object as well or not.


-> Syncing Animations : to sync animations we need to add the NetworkAnimator component to the network object and the animations will be synced up automatically. But this only works for the host cause this is server authoritative. We can make our own client network animator which derives from network animator and overrides the function OnIsServerAuthoritative() and returns false;


-> Connecting to a server on the same network : you can provide the ip of the server in unity transport component but the firewall will prevent this so you need to setup port forwarding for the router to connect the client to the server. To avoid this you can use a relay server. Its a third party server on the internet and everyone can connect to it. Unity has a official relay tool as part of Unity Gaming Services.

RPC : Remote Procedure Call. They're ways to call methods on objects that aren't in the same executable.
ServerRPC :  only runs on the server but is triggered by clients
ClientRPC : only called on the server but is executed on all clients

A serverRpc function uses the attribute [ServerRpc] and the name of the function must end in *ServerRpc

Network Object Reference -> a way to pass a reference to a network object. You just need to pass the network object and receive a NetworkObjectReference. To get the Network object from the network object refernce you need to use the TryGet() function. Eg : networkObjectReference.TryGet(out NetworkObject networkObject)


NetworkList<T> can be used 