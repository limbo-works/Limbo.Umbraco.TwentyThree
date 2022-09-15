# Limbo TwentyThree [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.TwentyThree.svg)](https://www.nuget.org/packages/Limbo.Umbraco.TwentyThree) [![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.TwentyThree.svg)](https://www.nuget.org/packages/Limbo.Umbraco.TwentyThree) [![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/backoffice-extensions/limbo-twentythree/)

This package features a property editor for inserting (via URL or embed code) a TwentyThree video. The property editor saves a bit of information about the video, which then will be availble in C#.




<br /><br />
## Installation

The package is only available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.TwentyThree/2.0.0-alpha001). To install the package, you can use either .NET CLI:

```
dotnet add package Limbo.Umbraco.TwentyThree --version 2.0.0-alpha001
```

or the older NuGet Package Manager:

```
Install-Package Limbo.Umbraco.TwentyThree -Version 2.0.0-alpha001
```







<br /><br />
## Configuration

In order to access the TwentyThree API, the package needs to be configured with a set of credentials, which should be added in your `appSettings.json` file like this:

```json
{
  "Limbo": {
    "TwentyThree": {
      "Credentials": [
        {
          "Key": "00000000-0000-0000-0000-000000000000",
          "Name": "MyProject",
          "Description": "A description about the credentials.",
          "Domains": "The domain(s) of your account here.",
          "ConsumerKey": "Your consumer key here.",
          "ConsumerSecret": "Your consumer secret here.",
          "AccessToken": "Your access token here.",
          "AccessTokenSecret": "Your access token secret here."
        }
      ]
    }
  }
}
```

**Key** should be a randomly generated GUID which will be used as a unique identifier for the credentials.

**Name** should be used to identify the credentials when shown in the UI.

**Description** is currently not used, but are meant to be shown in the UI to identify the credentials to the user.

**Domains** a string array with one or more domains associated with the TwentyThree account.

**ConsumerKey** an OAuth 1.0a consumer key obtained from the TwentyThree management portal.

**ConsumerSecret** an OAuth 1.0a consumer key obtained from the TwentyThree management portal.

**AccessToken** an OAuth 1.0a consumer key obtained from the TwentyThree management portal.

**AccessTokenSecret** an OAuth 1.0a consumer key obtained from the TwentyThree management portal.
