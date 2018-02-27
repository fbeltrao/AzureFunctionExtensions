# Azure Function Extensions

Azure functions allows you to write code without worrying too much about the infrastructure behind (aka Serverless).

Serverless is often used in an [event driven architecture](https://docs.microsoft.com/en-us/azure/architecture/guide/architecture-styles/event-driven), where your code react as events are happening around it: a file is created, a new user is registered, etc.

This repository contains a few Azure Function extensions that I write to help me succeed using the framework.


## Custom Bindings

A function has a trigger (the reason why it should run) and usually an output (what is the end result of running it).

|Triggers|Outputs|
| ------------- |-------------|
|<ul><li>An item was added to a queue</li><li>A direct HTTP call</li><li>A timer (every x minutes)</li><li>[many more](https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings#supported-bindings)</li>|<ul><li>Add an item to a queue</li><li>Send an email</li><li>Write to a database</li></ul>|

Azure Functions framework allow you to write your own triggers and outputs. Triggers are a bit complicated to implement, since you need to implement a way to notify the function host once a trigger happens. Outputs, on the other hand, are simple to be [customized](https://github.com/Azure/azure-webjobs-sdk/wiki/Creating-custom-input-and-output-bindings).

### Why should I write my own output binding?

Let's imagine you work in a team that is using Azure Functions heavily. Some of your functions need to write computed results to Redis, which will be consumed by a Frontend App. Without using custom bindings your code would look like this:

```C#
public static void WriteToRedisFunction1()
{
    // do some magic
    var computedValue = new { magicNumber = 10 };

    // here goes the "write to redis code"
    var db = ConnectionMultiplexer.Connect("connection-string-to-redis").GetDatabase();
    db.StringSet("myKeyValue", computedValue);
}

public static void WriteToRedisFunction2()
{
    // do yet another magic
    var anotherComputedValue = new { magicNumber = 20 };

    // "write to redis" again
    var db = ConnectionMultiplexer.Connect("connection-string-to-redis").GetDatabase();
    db.StringSet("myKeyValue2", anotherComputedValue);
}
```

You already see a code smell: repeating code. You could move all the Redis specific code to a shared class/library, but that would mean using DI to this library in your unit tests.

What if you could write your functions like this:

```C#
public static void WriteToRedisFunction1(
    [RedisOutput] IAsyncCollector<RedisItem> redisOutput)
{
    // do some magic
    var computedValue = new { magicNumber = 10 };

    // write to a collection
    redisOutput.AddAsync(new RedisItem()
    {
        ObjectValue = computedValue
    });

}

public static void WriteToRedisFunction2(
    [RedisOutput] IAsyncCollector<RedisItem> redisOutput
)
{
    // do yet another magic
    var anotherComputedValue = new { magicNumber = 20 };

    // write to a collection
    redisOutput.AddAsync(new RedisItem()
    {
        ObjectValue = computedValue
    });
}
```

Testing is easier. You check the contents of the "redisOutput" to ensure the expected items were created. No DI needed.


### Creating a custom output binding

Creating a custom binding requires a [few steps](https://github.com/Azure/azure-webjobs-sdk/wiki/Creating-custom-input-and-output-bindings):
1. Implement the attribute that glues the IAsyncCollector to your code
```CSharp
/// <summary>
/// Binds a function parameter to write to Redis
/// </summary>
[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
[Binding]
public sealed class RedisOutputAttribute : Attribute, IConnectionProvider
{
    /// <summary>
    /// Redis item key
    /// </summary>
    [AutoResolve(Default = "Key")]
    public string Key { get; set; }

    /// <summary>
    /// Defines the Redis server connection string
    /// </summary>
    [AutoResolve(Default = "ConnectionString")]
    public string Connection { get; set; }

    /// <summary>
    /// Send items in batch to Redis
    /// </summary>
    public bool SendInBatch { get; set; } = true;

    /// <summary>
    /// Sets the operation to performed in Redis
    /// Default is <see cref="RedisOutputOperation.SetKeyValue"/>
    /// </summary>
    public RedisOutputOperation Operation { get; set; } = RedisOutputOperation.SetKeyValue;
    
    /// <summary>
    /// Time to live in Redis
    /// </summary>
    public TimeSpan? TimeToLive { get; set; }
}
```

2. Implement the Redis item definition
```CSharp
/// <summary>
/// Defines a Redis item to be saved into the database
/// </summary>
public class RedisOutput 
{
    /// <summary>
    /// Redis item key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Defines the value as text to be set
    /// </summary>
    [JsonProperty("textValue")]
    public string TextValue { get; set; }

    /// <summary>
    /// Defines the value as object to be set. The content will be converted to json using JsonConvert.
    /// </summary>
    [JsonProperty("objectValue")]
    public object ObjectValue { get; set; }

    /// <summary>
    /// Defines the value as a byte array to be set
    /// </summary>
    [JsonProperty("binaryValue")]
    public byte[] BinaryValue { get; set; }

    /// <summary>
    /// Sets the operation to performed in Redis
    /// </summary>
    [JsonProperty("operation")]
    public RedisOutputOperation Operation { get; set; }

    /// <summary>
    /// Time to live in Redis
    /// </summary>
    [JsonProperty("ttl")]
    public TimeSpan? TimeToLive { get; set; }

    /// <summary>
    /// Value to increment by when used in combination with <see cref="RedisOutputOperation.IncrementValue"/>
    /// Default: 1
    /// </summary>
    [JsonProperty("incrementValue")]
    public long IncrementValue { get; set; } = 1;
}
```

3. Implement the part that sends a single/multiple RedisOutput to Redis, by implementing an IAsyncCollector:
```CSharp
/// <summary>
/// Collector for <see cref="RedisItem"/>
/// </summary>
public class RedisOutputAsyncCollector : IAsyncCollector<RedisOutput>
{

    ...

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="config"></param>
    /// <param name="attr"></param>
    public RedisItemAsyncCollector(RedisExtensionConfigProvider config, RedisOutputAttribute attr) : this(config, attr, RedisDatabaseManager.GetInstance())
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="config"></param>
    /// <param name="attr"></param>
    public RedisItemAsyncCollector(RedisExtensionConfigProvider config, RedisOutputAttribute attr, IRedisDatabaseManager redisDatabaseManager)
    {
        this.config = config;
        this.attr = attr;
        this.redisDatabaseManager = redisDatabaseManager;
        this.redisOutputCollection = new List<RedisOutput>();
    }

    /// <summary>
    /// Adds item to collection to be sent to redis
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddAsync(RedisOutput item, CancellationToken cancellationToken = default(CancellationToken))
    {
        ...
    }

    /// <summary>
    /// Flushs all items to redis
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        ...
    }
    
}
```

4. Implement the extension configuration provider which will instruct the Azure Function Host how to bind attributes, items and collectors together:
```CSharp
/// <summary>
/// Initializes the Redis binding
/// </summary>
public class RedisConfiguration : IExtensionConfigProvider
{
    ...

    /// <summary>
    /// Initializes attributes, configuration and async collector
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(ExtensionConfigContext context)
    {
        // Converts json to RedisItem
        context.AddConverter<JObject, RedisOutput>(input => input.ToObject<RedisOutput>());

        // Redis output binding
        context
            .AddBindingRule<RedisOutputAttribute>()
            .BindToCollector<RedisOutput>(attr => new RedisOutputAsyncCollector(this, attr));

        // Redis database (input) binding
        context
            .AddBindingRule<RedisDatabaseAttribute>()
            .BindToInput<IDatabase>(ResolveRedisDatabase);
    }
}
```

Important: parts of the code were removed to keep the post simple. Check the source code for the complete implementation.

### Sample Functions
#### Redis
```CSharp
/// <summary>
/// Sets a value in Redis
/// </summary>
/// <param name="req"></param>
/// <param name="redisItem"></param>
/// <param name="log"></param>
/// <returns></returns>
[FunctionName(nameof(SetValueInRedis))]
public static IActionResult SetValueInRedis(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    [RedisOutput(Connection = "%redis_connectionstring%", Key = "%redis_setvalueinredis_key%")]  out RedisOutput redisItem,
    TraceWriter log)
{
    string requestBody = new StreamReader(req.Body).ReadToEnd();

    redisItem = new RedisItem()
    {
        TextValue = requestBody
    };

    return new OkResult();
}
```

#### HttpCall
```CSharp
/// <summary>
/// Calls a web site to notify about a change
/// </summary>
/// <param name="messages"></param>
/// <param name="log"></param>
/// <returns></returns>
[FunctionName(nameof(CallWebsite))]
public static async Task CallWebsite(     
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    [HttpCall] IAsyncCollector<HttpCallMessage> messages, 
    TraceWriter log)
{    
    // compute something
    var computedValue = new { payload: "3123213" };

    // computed value will be posted to another web site
    await messages.AddAsync(new HttpCallMessage("url-to-webhook")
        .AsJsonPost(computedValue)
        );   
}
```


### What output bindings are available in this library?

| Type | Description |
|--|--|
|Redis| Allows a function to interact with Redis. Following operations are currently support: add/insert item to lists, set a key, increment a key value in Redis. For more read (or more complex operations) you can use the [RedisDatabase] attribute that will resolve a IDatabase to your function |
|HttpCall| Allows a function to make an HTTP call easily, handy when you need to call a Webhook or a callback web site to notify a change.|

Have a suggestion? Create an issue and I will take a look.
I am looking into adding custom event grid topic messages as output.