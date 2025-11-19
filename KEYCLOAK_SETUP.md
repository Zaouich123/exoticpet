# Keycloak Configuration Guide

## Overview

This application uses Keycloak for authentication and authorization. Follow these steps to properly configure Keycloak for the exoticPet application.

## Prerequisites

- Keycloak is running on `http://localhost:8090`
- Admin credentials: `admin` / `admin`

## Step-by-Step Setup

### 1. Login to Keycloak Admin Console

1. Open browser to: `http://localhost:8090/admin`
2. Login with credentials:
   - Username: `admin`
   - Password: `admin`

### 2. Create or Select Realm

1. Click on the dropdown at the top-left (showing "Master")
2. Click "Create Realm"
3. Enter realm name: `exoticpet`
4. Click "Create"

### 3. Configure Client for Blazor WebApp

1. Go to **Clients** section in the left menu
2. Click **Create client**
3. Enter client ID: `blazor-client`
4. Click **Next**
5. Enable **Client authentication** toggle
6. Click **Next** and then **Save**
7. In the **Credentials** tab, copy the **Client Secret** (you'll need this)

### 4. Configure Valid Redirect URIs

1. In the `blazor-client` settings, go to the **Access** tab
2. Set the following:
   - **Root URL**: `http://localhost:5000`
   - **Home URL**: `http://localhost:5000`
   - **Valid redirect URIs**:
     \`\`\`
     http://localhost:5000/*
     \`\`\`
   - **Valid post logout redirect URIs**:
     \`\`\`
     http://localhost:5000
     \`\`\`
3. **Save**

### 5. Configure Client Scopes

1. Go to **Client Scopes** section
2. Click on `profile` scope
3. Go to **Mappers** tab
4. Ensure these mappers exist and are configured:
   - `username`
   - `email`
   - `given name`
   - `family name`

### 6. Create Test User

1. Go to **Users** section
2. Click **Add user**
3. Enter username: `testuser`
4. Enable **Email verified**
5. Click **Create**
6. Go to **Credentials** tab
7. Set password: `testpassword`
8. Disable **Temporary** toggle
9. Click **Set Password**

### 7. Assign Roles to User

1. Stay in the user details page
2. Go to **Role mapping** tab
3. Go to **Assign role**
4. If needed, create a realm role `gestionnaire`:
   - Go to **Realm roles** in left menu
   - Click **Create role**
   - Name: `gestionnaire`
   - Click **Create**
5. Back to user role mapping, assign the `gestionnaire` role

### 8. Update Application Configuration

Make sure your `appsettings.json` files have the correct values:

**exoticPet.WebApp/appsettings.json:**
\`\`\`json
{
  "Authentication": {
    "OIDC": {
      "Authority": "http://localhost:8090/realms/exoticpet",
      "ClientId": "blazor-client",
      "ClientSecret": "YOUR_CLIENT_SECRET"
    }
  }
}
\`\`\`

**exoticPet.ApiService/appsettings.json:**
\`\`\`json
{
  "Authentication": {
    "OIDC": {
      "Authority": "http://localhost:8090/realms/exoticpet"
    }
  }
}
\`\`\`

## Testing the Setup

1. Start the application: `dotnet run --project exoticPet.AppHost`
2. Navigate to `http://localhost:5000`
3. Click on login
4. You should be redirected to Keycloak login page
5. Login with `testuser` / `testpassword`
6. You should be redirected back to the application
7. Try adding an animal - this requires the `gestionnaire` role

## Troubleshooting

### "Invalid redirect_uri" error
- Check that the Blazor client has `http://localhost:5000/*` in Valid Redirect URIs

### "Invalid client secret" error
- Verify the ClientSecret in `appsettings.json` matches the one in Keycloak admin console

### "Token validation failed" error
- Ensure `RequireHttpsMetadata = false` is set in Program.cs
- Verify the Authority URL is correct: `http://localhost:8090/realms/exoticpet`

### User cannot create animals (403 Forbidden)
- Ensure the user has the `gestionnaire` role assigned
- Check the server logs to see which roles the token contains

### CORS errors when calling API
- Ensure HTTP is used throughout (not HTTPS)
- Verify the API is running on `http://localhost:5251`
- Check TokenHandler is properly injecting the Bearer token

## Common Issues

1. **Redirect loop**: Clear browser cookies for localhost and try again
2. **Token expired**: Browser will automatically refresh using the refresh token
3. **Role not appearing in token**: Restart Keycloak container or refresh the browser
\`\`\`

```csharp file="" isHidden
