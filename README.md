# <img src="https://raw.githubusercontent.com/RobThree/ITimer/main/logo.png" alt="Logo" width="32" height="32"> ITimer
Provides a testable abstraction and alternative to `System.Threading.Timer` and `System.Timers.Timer`. Targets netstandard2.0 and higher.

## Why and how
Timer related code is, or should I say _used to be_, hard to unittest. When you have timer related code, you (probably) don't want to wait until the timer elapses in your unittest which would in turn make your unittests slower than strictly necessary.

The basis for this library is the [`ISignaler`](ITimer/ISignaler.cs) interface which defines an interface for timers to implement that allow you to replace those timers with the [`TestTimer`](ITimer/TestTimer.cs) in your unittests so you have total control over when the timer fires the `Elapsed` event.

This library provides the two most common timers: [`System.Threading.Timer`](https://docs.microsoft.com/en-us/dotnet/api/system.threading.timer) and [`System.Timers.Timer`](https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer) wrapped in the [`ThreadingTimer`](ITimer/ThreadingTimer.cs) and [`SystemTimer`](ITimer/SystemTimer.cs) classes respectively. Other, custom, timers should be simple to implement by simply implementing the [`ISignaler`](ITimer/ISignaler.cs) interface.

## ISignaler? Why not ITimer?

Agreed, [`ISignaler`](ITimer/ISignaler.cs) is not the best name. `ITimer` would have been a much better choice, but that conflicts with the namespace. That would require you to write `ITimer.ITimer` everywhere this interface is used. And since we wanted a simple package-ID and simple (root) namespace we opted for `ITimer` as namespace and [`ISignaler`](ITimer/ISignaler.cs) as interface name. If you have any better suggestions, please let us know and we'll consider it for the next major version.

## Quickstart

In your code:

```c#
public class MyClass
{
    private readonly ISignaler _timer;

    public MyClass(ISignaler timer)
    {
        _timer = timer ?? throw new ArgumentNullException(nameof(timer));
        _timer.Elapsed += (s, e) => { 
            // Do work here...
            Console.WriteLine($"Tick tock! {e.SignalTime}"); 
        };

    }

    public void Start() {
      _timer.Start();
    }

    public void Stop() {
      _timer.Stop();
    }
}

using (var myTimer = new SystemTimer()) {
  var myclass = new MyClass(myTimer);
  myClass.Start();
  //...
}
```
Or, even better, using Dependency Injection: 
```c#
public void ConfigureServices(IServiceCollection services)
{
    // Register SystemTimer as ISignaler
    services.AddScoped<ISignaler, SystemTimer>();
    // ...
}
```

For usage in unittests, see the [`TestTimer`](#testtimer) below.

## ISignaler

The [`ISignaler`](ITimer/ISignaler.cs) interface defines the `Start()`  and `Stop()`  methods to start and stop the timer raise the `Elapsed` event. The `Interval` property gets the timer's interval and the `AutoReset` property returns whether or not the timer should fire the event once and then stop, or keep going in a fire event / wait cycle. 

## ThreadingTimer and SystemTimer

As mentioned before, these timers encapsulate (or "wrap") the [`System.Threading.Timer`](https://docs.microsoft.com/en-us/dotnet/api/system.threading.timer) and [`System.Timers.Timer`](https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer) timers and provide a unified interface because they both implement the [`ISignaler`](ITimer/ISignaler.cs) interface. The difference between these two is perhaps best explained by [Jon Skeet](https://jonskeet.uk/csharp/threads/timers.html) (archived version [here](https://archive.is/eXhQS) or [here](https://web.archive.org/web/20190303143427/http://jonskeet.uk/csharp/threads/timers.html)). The [`System.Windows.Forms.Timer`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.timer) is not provided by this library but should be simple to implement. 

Both timers are simplified versions of the underlying timers unified to a single, simple, interface. If you need a more specific implementation then you may want to implement them again in your own class, also implementing the [`ISignaler`](ITimer/ISignaler.cs) interface. 

## TestTimer

With the [`TestTimer`](ITimer/TestTimer.cs) you are in complete control over when, and how often, the timer fires. You even control the `SignalTime` so you can specify at what (pretend) time the timer fired. Ofcourse, this is very useful in unittests.

The TestTimer offers some extra properties like the `TickCount`, `StartCount` and `StopCount` that keep track of how often the `Elapsed` event has been raised and the timer has been started and stopped respectively (all of which can be reset with the `Reset()` method). The `Elapsed` event for the [`TestTimer`](ITimer/TestTimer.cs) provides the `TestTimerElapsedEventArgs` which also contains a `TickCount` property.

Most important, however, for the TestTimer are the `Tick(DateTimeOffset?)`, `Tick(IEnumerable<DateTimeOffset>)` and `Tick(Int32, Func<Int32, DateTimeOffset>)` methods. These methods will raise the `Elapsed` event on the [`TestTimer`](ITimer/TestTimer.cs) and allow you to specify the `SignalTime`.

Given the above example, we can now replace the [`SystemTimer`](ITimer/SystemTimer.cs) with a [`TestTimer`](ITimer/TestTimer.cs) for our tests:

```c#
using (var myTestTimer = new TestTimer()) {
  var myclass = new MyClass(myTestTimer);

  myTestTimer.Tick(); // Raise elapsed event
  Assert.IsTrue(...);
}
```
The different overloads of the `Tick()` method allow you to raise the event multiple times. The [`TestTimer`](ITimer/TestTimer.cs) has a constructor argument `requireStart` that allows you to specify wether you require the timer to be started before it will start raising events or not; this defaults to the latter, making your unittests more concise not having to start the [`TestTimer`](ITimer/TestTimer.cs) each time. When this value equals `true` the event won't be raised by any of the `Tick()` methods unless `Start()` is called.

## License

Licensed under MIT license. See [LICENSE](LICENSE) for details. 

## Attribution

Icon made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com/free-icon/experience_2755467).
