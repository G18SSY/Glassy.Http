# Glassy.Http
Provides utility functionality for HTTP interaction in .NET

![Stable](https://img.shields.io/nuget/v/Glassy.Http.AspNetCore)
![PreReleaseBuild](https://img.shields.io/github/workflow/status/G18SSY/Glassy.Http/Package - Pre-release?label=pre-release%20build)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

- [Visual Studio](https://visualstudio.microsoft.com/) 2019

### Installing

Clone the repo:

```
git clone https://github.com/G18SSY/Glassy.Http.git
```

Open Glassy.Http.sln in Visual Studio 2019

## Running the tests

### Quality Test

TBC

### Style Tests

TBC

## Deployment

TBC

## Features

- Parameter parser for extracting parameters from [Microsoft.AspNetCore.Http.HttpRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httprequest). This was originally designed for simplifying the production of RESTful HTTP APIs in [Azure Functions](https://azure.microsoft.com/en-gb/services/functions/) apps (>= v2) and can extract parameters from:
  - Headers
  - Query strings
  - Route

## Contributing

TBC

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/G18SSY/Glassy.Http/releases). 

## Authors

* **Brad Glass** - *Initial work* - [G18SSY](https://github.com/G18SSY)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

* Fluent API design was based on the fantastic [Autofac](https://github.com/autofac/Autofac) API
