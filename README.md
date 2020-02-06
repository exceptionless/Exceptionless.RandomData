# Exceptionless.RandomData
[![Build status](https://github.com/Exceptionless/Exceptionless.RandomData/workflows/Build/badge.svg)](https://github.com/Exceptionless/Exceptionless.RandomData/actions)
[![NuGet Version](http://img.shields.io/nuget/v/Exceptionless.RandomData.svg?style=flat)](https://www.nuget.org/packages/Exceptionless.RandomData/)
[![Slack Status](https://slack.exceptionless.com/badge.svg)](https://slack.exceptionless.com)

Utility class to easily generate random data. This makes generating good unit test data a breeze!

## Getting Started (Development)

[This package](https://www.nuget.org/packages/Exceptionless.RandomData/) can be installed via the [NuGet package manager](https://docs.nuget.org/consume/Package-Manager-Dialog). If you need help, please contact us via in-app support or [open an issue](https://github.com/exceptionless/Exceptionless.RandomData/issues/new). We’re always here to help if you have any questions!

1. You will need to have [Visual Studio Code](https://code.visualstudio.com/) installed.
2. Open the root folder.

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
