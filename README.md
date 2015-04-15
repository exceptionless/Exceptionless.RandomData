# Exceptionless.RandomData
[![Build status](https://ci.appveyor.com/api/projects/status/jiw0i4c33c70ofqq?svg=true)](https://ci.appveyor.com/project/Exceptionless/exceptionless-randomdata) [![Gitter](https://badges.gitter.im/Join Chat.svg)](https://gitter.im/exceptionless/Discuss)

Utility class to easily generate random data. This makes generating good unit test data a breeze!

## Getting Started (Development)

[This package](https://www.nuget.org/packages/Exceptionless.RandomData/) can be installed via the [NuGet package manager](https://docs.nuget.org/consume/Package-Manager-Dialog). If you need help, please contact us via in-app support or [open an issue](https://github.com/exceptionless/Exceptionless.RandomData/issues/new). We’re always here to help if you have any questions!

1. You will need to have [Visual Studio 2013](http://www.visualstudio.com/products/visual-studio-community-vs) installed.
2. Open the `Exceptionless.RandomData.sln` Visual Studio solution file.

## Using RandomData

Below is a small sample of what you can do, so check it out!

```csharp
private int[] _numbers = new[] { 1, 2, 3, 4, 5 };

private enum _days {
    Monday,
    Tuesday
}

int value = RandomData.GetInt(1, 5);
// or
value = _numbers.Random();
    
var day = RandomData.GetEnum<_days>();
```
