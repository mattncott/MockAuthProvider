# MockAuthProvider

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