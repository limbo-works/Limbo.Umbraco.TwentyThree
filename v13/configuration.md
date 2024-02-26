# Configuration

## App Settings

In order to access the TwentyThree API, the package needs to be configured with a set of credentials, which should be added in your appSettings.json file like this:

The package introduces a new `Limbo:TwentyThree` section in the appsettings.json file.

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

### Credentials

The credentials represent the information and OAuth 1.0a tokens used for accessing the TwentyThree API. `Limbo:TwentyThree:Credentials` is an array with the information for each account.

#### Key
The `Key` property should be a GUID value that can be used as a unique identifier for the credentials. A randomly generated GUID should be fine for this.

#### Name
The `Name` property should be used to identify the credentials when shown in the UI.

#### Description
The value of the `Description` property is currently not used, but are meant to be shown in the UI to identify the credentials to the user.

#### Domains
The `Domains` property should be a string array with one or more domains associated with the TwentyThree account.

#### ConsumerKey
An OAuth 1.0a consumer key obtained from the TwentyThree management portal.

#### ConsumerSecret
An OAuth 1.0a consumer key obtained from the TwentyThree management portal.

#### AccessToken
An OAuth 1.0a consumer key obtained from the TwentyThree management portal.

#### AccessTokenSecret
An OAuth 1.0a consumer key obtained from the TwentyThree management portal.