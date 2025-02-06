# MockAuthProvider

[![codecov](https://codecov.io/gh/mattncott/MockAuthProvider/branch/main/graph/badge.svg?token=EZ7sJpMkbx)](https://codecov.io/gh/mattncott/MockAuthProvider)

THIS SHOULD NEVER BE USED IN PRODUCTION. THE INTENTION OF THIS IS FOR TEST AND DEVELOPMENT PURPOSES ONLY!.

This is a Mock OIDC provider that allows for uses to generate test access tokens and refresh tokens for authentication.

## Docker Usage

### Docker Compose

The below example can be inserted into any docker compose file to get you up and running. For configuring the Account/Clients, please see their respective below settings. Clients can support multiple clients, all you need to do is increment the number value.

```yaml
auth:
  container_name: dev-auth
  ports:
    - 7184:443
  image: mattncott/mockauthprovider:main
  volumes:
    - ~/.aspnet/dev-certs/https:/root/.dotnet/corefx/cryptography/x509stores/my/
  environment:
    - ASPNETCORE_URLS=https://+443
    - ConnectionStrings:DefaultConnection=User ID=${POSTGRES_USER-postgres};Password=${POSTGRES_PASSWORD-password};Host=db;Port=5432;Database=${POSTGRES_DATABASE-MockAuthProvider};
    
    - Account:Id=2abbd92d-d80e-415b-be12-ff9524ef086c
    - Account:Username=${AccountUsername}
    - Account:Password=${AccountPassword}
    - Account:Name=${AccountName}

    - Clients:0:ClientId=${OidcClientId}
    - Clients:0:ClientSecret=${OidcClientSecret}
    - Clients:0:RedirectUri=${OidcRedirectUri}
```

## Required configuration values for appsettings.json

### Connection String
This should contain all the information required to connect to the PostgreSQL database.
```json
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=password;Host=localhost;Port=5432;Database=MockAuthProvider;"
  }
```

### Clients
This should contain all the information about any OIDC clients that can be used for authentication

Please note, if a ClientSecret is an empty string or NULL then, the Client will be created as a `public` client.

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

### Account
This should contain all the information about any user accounts that you want to allow authentication for.

#### Example
```json
  "Account": {
      "Id": "a35e3495-becb-4ad1-9698-6a9c8a5e0ece",
      "Username": "testuser",
      "Password": "password",
      "Name": "Tester",
      "Phone": "07845198198",
      "Email": "email@email.com",
      "Roles": [
        "user"
      ]
    },
```

## Usage

To use the authentication provider is simple. The following endpoint(s) have been provided:

### /authorize
This endpoint supports the `authorization_code` grant type. When using this endpoint, the user provided in the `Account` environment setting will automatically be logged in (eventually this functionality will be toggled). After successfully authenticating, a code will be generated and the user will be redirected to the `/token` endpoint.

### /token
This endpoint supports both the `client_credentials` and then `authorization_code` grant types (more to be added in the future). If successful, an access token will be generated and returned to the user.