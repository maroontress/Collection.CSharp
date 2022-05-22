# Collection.CSharp

Collection.CSharp is a C# class library containing some _collection_ classes,
which we don't need frequently but sometimes. It depends on .NET Standard 2.1.

It contains the following classes:

- [`ImmutableLinkedHashMap`](doc/ImmutableLinkedHashMap.md)
- [`InternMap`](doc/InternMap.md)
- [`LinkedHashSet`](doc/LinkedHashSet.md)

<!--
## Get started

Collection.CSharp is available as
[the ![NuGet-logo][nuget-logo] NuGet package][nuget-maroontress.collection].
-->

## API Reference

- [Maroontress.Collection][apiref-maroontress.collection] namespace

## How to build

### Requirements for build

- Visual Studio 2022 (Version 17.2)
  or [.NET 6.0 SDK (SDK 6.0.300)][dotnet-sdk]

### Build

```plaintext
git clone URL
cd Collection.CSharp
dotnet build
```

### Get the test coverage report with Coverlet

Install [ReportGenerator][report-generator] as follows:

```plaintext
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Run all tests and get the report in the file `Coverlet-html/index.html`:

```plaintext
rm -rf MsTestResults
dotnet test --collect:"XPlat Code Coverage" --results-directory MsTestResults \
  && reportgenerator -reports:MsTestResults/*/coverage.cobertura.xml \
    -targetdir:Coverlet-html
```

[report-generator]:
  https://github.com/danielpalme/ReportGenerator
[dotnet-sdk]:
  https://dotnet.microsoft.com/en-us/download
[apiref-maroontress.collection]:
  https://maroontress.github.io/Collection-CSharp/api/latest/html/Maroontress.Collection.html
[nuget-maroontress.collection]:
  https://www.nuget.org/packages/Maroontress.Collection/
[nuget-logo]:
  https://maroontress.github.io/images/NuGet-logo.png
