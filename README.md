# Cat Facts Integration

A .NET Web API application that retrieves cat facts from an external API, saves them locally in <code>Data/cat-facts.txt</code>, and supports searching and pagination.

## Features

* Fetching cat facts from an external API
* Saving data to a text file in JSON format
* Simple result pagination
* Searching by partial text matches
* Global exception handling

## Technologies

* .NET 10
* ASP.NET Minimal API
* xUnit
* NSubstitute
* OpenAPI, Scalar

## Running the Application

Restore dependencies:

```bash
dotnet restore
```

Run the application:

```bash
dotnet run --project CatFactsIntegration.WebApi
```

Scalar documentation is available in the development environment at:

```text
/scalar/v1
```

## Examples
http://localhost:5280/fact

<img width="680" height="152" alt="image" src="https://github.com/user-attachments/assets/8cf6f1ca-0b54-41e0-a5d2-ef1419e67979" />

http://localhost:5280/facts?page=1&limit=2&phrase=cat

<img width="1240" height="242" alt="image" src="https://github.com/user-attachments/assets/17d192a6-672d-4b6f-9e1b-ee323147ff35" />


## Tests

Run all tests:

```bash
dotnet test
```
