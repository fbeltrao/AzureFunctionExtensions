# Available Bindings in this library

## Redis
Allows a function to interact with Redis. Following operations are currently supported: add/insert item to lists, set a key, increment a key value in Redis. For read or more complex operations you can use the [RedisDatabase] attribute that will resolve a IDatabase to your function

## HttpCall
Allows a function to make an HTTP call easily, handy when you need to call a Webhook or an url to notify a change|

## EventGrid
Allows a function to easily publish Azure Event Grid events. **Important**: Subscribing to an Event Grid topic is already part of the Azure Function runtime, no need to use a custom binding

## Azure SignalR
Allows a function to easily send messages to Azure SignalR connected clients. Allowed scopes are: broadcast to hub, to one or more groups and to one or more users. For more information about Azure SignalR Service [check here](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-overview)|
