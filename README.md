# MockAuthProvider

[![codecov](https://codecov.io/gh/mattncott/MockAuthProvider/branch/main/graph/badge.svg?token=EZ7sJpMkbx)](https://codecov.io/gh/mattncott/MockAuthProvider)

THIS SHOULD NEVER BE USED IN PRODUCTION. THE INTENTION OF THIS IS FOR TEST AND DEVELOPMENT PURPOSES ONLY!.

This is a Mock OIDC provider that allows for uses to generate test access tokens and refresh tokens for authentication.

## Required configuration values

### Jwt
This should contain all the information required to generate a JWT token using this provider

#### Example
```json
  "Jwt": {
    "Key": "YourSecretKeyForAuthenticationOfApplication",
    "Issuer": "localhost",
    "Audience": "https://localhost"
  },
```

### Clients
This should contain all the information about any OIDC clients that can be used for authentication

#### Example
```json
  "Clients": [
    {
      "ClientId": "TestClient",
      "ClientSecret": "abc123",
      "RedirectUri": "http://localhost/redirect"
    }
  ]
```

### Accounts
This should contain all the information about any user accounts that you want to allow authentication for.

#### Example
```json
  "Accounts": [
    {
      "Id": "a35e3495-becb-4ad1-9698-6a9c8a5e0ece",
      "Username": "testuser",
      "Password": "password",
      "Name": "Tester"
    }
  ],
```

## Usage

To use the authentication provider is simple. The following endpoint(s) have been provided:

### /authorize
This is the main endpoint for the provider. This endpoint will accept a AuthorizeRequestContract which will then be validated. If all criteria is correct an access token and a refresh token will be returned.

The clientId, clientSecret, redirectUri and user data must all match those in the configuration files.

If isConfidential is provided and set to TRUE, then the clientSecret can be omitted.

#### Example Request
```json
{
	"clientId": "TestClient",
	"clientSecret": "abc123",
	"isConfidential": false,
	"redirectUri": "http://localhost/redirect",
	"username": "testuser",
	"password": "testpass"
}
```

#### Example Response
If an error occurs then details will be provided in the errorMessage and success will be set to false.

```json
{
	"success": true,
	"accessToken": "...",
	"refreshToken": "...",
	"expiresIn": 3600,
	"errorMessage": null
}
```