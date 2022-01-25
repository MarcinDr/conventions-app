# Conventions web app

This is an example application of Conventions application for creating events, presentations and signing up for those. 

App contains solution with following projects:

- Conventions.Web - contains basic react app and .net6 backend - frontend is not done, it is still the basic create-react-app.
- Conventions.Api - backend service that exposes API (protected with JWT)
- Conventions.Domain - project containing db definition (models + migrations) 

## Running solution locally

To run these application locally we should:

- provide all required env variables in `launchSettings.json` file for both Web and Api projects
- run `docker-compose up` in the root folder
- run `yarn install && yarn start` in the `Conventions.Web/ClientApp` folder
- run `dotnet run --project Conventions.Web/Conventions.Web.csproj -c Debug` in the root folder
- run `dotnet run --project Conventions.Api/Conventions.Api.csproj -c Debug` in the root folder

After that we can navigate to `https://localhost:7058` that should immediately redirect us to auth0 login page, after successful login
it should redirect back to default react app and we should have a cookie (developer tools -> application -> cookies)

To communicate with our Api we could use received JWT (we could request it directly from postman using our clientId and clientSecret) and postman collection included in the repository (Conventions_API.json in the root folder).

## Limitations 
- no user management
- test users ids are hardcoded in the `AppInitializationService`

