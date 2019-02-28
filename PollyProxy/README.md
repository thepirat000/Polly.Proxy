# Polly.Proxy

Configure [**Polly**](https://github.com/App-vNext/Polly#polly) policies via a proxy.

A combination of [**Polly**](https://github.com/App-vNext/Polly#polly) and [**Castle Dynamic Proxy**](https://github.com/castleproject/Core#castle-core) as an alternative way to configure _Polly policies_ 
to any virtual method being called without modifying the existing code.

## [NuGet](https://www.nuget.org/packages/Polly.Proxy/)

[![NuGet Status](https://img.shields.io/nuget/v/Polly.Proxy.svg?style=flat)](https://www.nuget.org/packages/Polly.Proxy/)
[![NuGet Count](https://img.shields.io/nuget/dt/Polly.Proxy.svg)](https://www.nuget.org/packages/Polly.Proxy/)

To install the package run the following command on the Package Manager Console:

```
PM> Install-Package Polly.Proxy
```

## Usage

Create a new proxy for a class (or interface) by using `PollyProxy.Create<T>()` helper method:

```c#
var proxy = PollyProxy.Create<Interface>(new Class(), policies => policies
    .For("MyMethod", Policy.Handle<Exception>().Retry(3))
    .Default(Policy.NoOp()));
```

## Example

Given an existing interface/class pair:

```c#
public interface IYourInterface
{
    void YourMethod();
}

public class YourClass : IYourInterface
{
    public void YourMethod()
    {
        // ...
    }
}
```

That is used in some other place:

```c#
public class AnotherClass
{
    private IYourInterface _instance = new YourClass();

    public void Run()
    {
        _instance.YourMethod();
        // ...
    }
}
```

You can set Polly policies to the method calls for your instance of `YourClass` by
creating a new proxy instead:

```c#
using Polly;

public class AnotherClass
{
    private IYourInterface _instance; // <-- Will proxy this instance

    public AnotherClass()
    {
        _instance = PollyProxy.Create<IYourInterface>(new YourClass(), p => p
            .Default(Policy.Handle<Exception>().Retry(2)));
    }

    public void Run()
    {
        _instance.YourMethod(); // <-- This call will be handled by Polly
        // ...
    }
}
```

## Configuration

You can use the fluent API to map the policies _rules_:

- `.For(string methodName, Policy policy)`: Maps a policy for a method by its case-sentitive name.
- `.When(Func<MethodInfo, bool> selector, Policy policy)`: Specifies a policy to be used under certain condition on the [`MethodInfo`](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo) being called.
- `.Map(Func<MethodInfo, Policy> policyBuilder)`: A function of the `MethodInfo` being called that returns the policy to use (or NULL).
- `.Default(Policy policy)`: Defines a default Policy to use for any other method not mapped by other rules.

> Notes:
> - The order in which the fluent API methods are called, defines the precedence for the rules evaluation, except for the `.Default(Policy)` which can be called in any order.
> - You can set/return a NULL policy for a rule to indicate the rule is not matching any policy.

## Creating proxies

The `PollyProxy.Create<T>()` method returns a proxy object that _inherits from the proxied class_ / _implements proxied interface_ and forwards calls to the real object.

Give special attention to the generic type argument `T`, it can be:
- **An interface**: Will generate an _interface proxy_ to log all the interface member calls. (Recommended)
- **A class type**: Will generate a _class proxy_ to log virtual member calls. 
 
#### IMPORTANT NOTE

- When using an _interface proxy_, the interception is limited to the members of the interface. 
- When using a _class proxy_, the interception is limited to its virtual members: **non-virtual methods can't be automatically proxied**. 

