# Limbo TwentyThree

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.TwentyThree.svg)](https://www.nuget.org/packages/Limbo.Umbraco.TwentyThree)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.TwentyThree.svg)](https://www.nuget.org/packages/Limbo.Umbraco.TwentyThree)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.twentythree)

**Limbo.Umbraco.TwentyThree** is a package for Umbraco 13 that features a property editor for inserting (via URL or embed code) a TwentyThree video. The property editor saves a bit of information about the video, which then will be availble in C#.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="./LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>
      Umbraco 13
    </td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>
      .NET 8
    </td>
  </tr>
</table>



<br /><br />
## Installation

The package is only available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.TwentyThree/13.0.0). To install the package, you can use either .NET CLI:

```
dotnet add package Limbo.Umbraco.TwentyThree --version 13.0.0
```

or the NuGet Package Manager:

```
Install-Package Limbo.Umbraco.TwentyThree -Version 13.0.0
```

## Umbraco 10, 11 and 12
For the Umbraco 10-12 version of this package, see the [**v2/main**](https://github.com/limbo-works/Limbo.Umbraco.TwentyThree/tree/v2/main) branch instead.






<br /><br />
## Dependencies

- [**Umbraco 13**](https://github.com/umbraco/Umbraco-CMS)  
The `v13.x` releases are build against Umbraco 13.

- [**Skybrud.Essentials.Http**](https://github.com/skybrud/Skybrud.Essentials.Http)  
Our package for making HTTP requests.

- [**Skybrud.Social.TwentyThree**](https://github.com/abjerner/Skybrud.Social.TwentyThree)  
Our integration package and API wrapper for the Twenty Three API.

- [**Limbo.Umbraco.Video**](https://github.com/limbo-works/Limbo.Umbraco.Video)  
Base package with common functionality for our various video pickers.





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
          "Domains": [ "The domain(s) of your account here." ],
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


<br /><br />
## Screenshots

![image](https://user-images.githubusercontent.com/3634580/190511380-6f3a9338-9ee6-4e66-9f52-adbeb32c898b.png)
*Example view of the property editor.*

![image](https://user-images.githubusercontent.com/3634580/190514237-60cc32d0-5467-4adf-9688-9491b63ba754.png)
*Overlay for picking videos from the configured account(s).*
