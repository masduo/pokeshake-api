# PokeShake API (Pokemon but Shakespearean) 

![Build and Tests](https://github.com/masduo/pokeshake-api/workflows/Greet%20Everyone/badge.svg?branch=workflows)

A fresh perspective at the world of Pokemon! PokeShake API gets a Pokemon species name, and translates its flavor text to Shakespearean style.

[Poke API's Pokemon Species endpoint](https://pokeapi.co/api/v2/pokemon-species) is used to get flavor texts:

- Different versions of the Pokemon game have different flavor texts localised in different languages. The **Ruby** version and **English** language are assumed here.
- Pokemon species' Uniform Resource Identifiers are case-sensitive. The same design principle is also assumed here.

[Fun Translations API's Shakespearean endpoint](https://api.funtranslations.com/translate/shakespeare) is used to translate texts to Shakespearean style.

- Ther is a rate limit of 5 requests per hour. For production environment an API Key can be acquired and set during deployment. See below under Helm.

# Run

Depending on the use case, choose one of the following three approaches to run the API locally.

Then browse to:

- http://localhost:5000/swagger
- http://localhost:5000/healthcheck

## 1) Run with .Net CLI

To run and/or debug locally. Also to get secure connection:

1. Install [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

2. In your favourite shell run

   ```sh
   dotnet run -p src

   # or with file watcher
   dotnet watch -p src run
   ```

3. To get secure connection, trust the dotnet development certs `dotnet dev-certs https --trust` and then browse to https://localhost:5001/swagger

## 2) Run with Docker or Docker Compose

To run in Docker and test out the Dockerfile:

1.  Install [Docker Desktop](https://www.docker.com/products/docker-desktop) (_tested on docker desktop community v2.5 - engine v19.03_)

2.  Then run

    ```sh
    docker-compose up --build

    # or do it yourself
    docker build -t pokeshake-api .
    docker run -p 5000:5000 pokeshake-api
    ```

## 3) Run with Skaffold

To run in the local Kubernetes cluster. Since Skaffold is using Helm to install and deploy the API, this approach is particularly useful to test precisely how the API changes will play out in test or production Kubernetes clusters:

1.  Install

    - [Docker Desktop](https://www.docker.com/products/docker-desktop) (_tested on docker desktop v2.5 - engine v19.03_)
    - Enable Kubernetes in Docker Desktop (_tested with K8s v1.19_)
    - [Helm](https://github.com/helm/helm/releases/tag/v3.3.4) (_tested on v3.3_)
    - [Skaffold](https://skaffold.dev/docs/install/) (_tested on v1.15_)

2.  Run

    ```sh
    skaffold run --tail

    # or with file watcher
    skaffold dev
    ```

# Features

### Stylecop

To enforce basic unobtrusive coding standards for consistency.

### Serilog

To enable structured logging with log templates, and to allow logging with different formats: templated text for development and compact json for production. It is configurable for different environments via `appsettings<.env>.json`.

The API logs to console (stdout). Another service (fluentd or filebeat) will be required to harvest the logs files and transfer them to log stores (elasticsearch, splunk, etc.).

### Open API Specification (Swagger)

To add live/auto-generated documentation to the API.

### Docker and Docker Compose

To enable building and running the API inside Docker.

### Helm

To enable deploying the API to Kubernetes instance. Skaffold uses it to deploy locally. The Fun Translations API secret can be set when installing the PokeShake API in the production cluster:

```yaml
# In a pseudo CI specification file
script:
  - helm upgrade --install pokeshake-api ./helm -f ./helm/values/production.yaml
    --set deployment.fun_translations_api_secret=$FUN_TRANSLATIONS_API_SECRET
```

### Skaffold

To deploy the API to Kubernetes insance running inside Docker Desktop.

### Versioning

Add versioning via urls. `/v1/` is the current default version.

### DotNetEnv

To sets environment variables for local development. Add a (_gitignored and dockerignored_) `.env` file to the root and write environemnt variables:

```sh
# to run with production configuration
ASPNETCORE_ENVIRONMENT=production
# to set Fun Translations API secret
FUN_TRANSLATIONS_API_SECRET=[redacted]
```

### Caching and API Rate Limits

Free version of Fun Translations API has a rate limits in place. To avoid re-requesting the API for the same translation, an in-memory cache is used.

The cache has an absolute expiry timespan of 12 hours (43200ticks), to align with Fun Translations API's `Cache-Control` header:

```
Cache-Control: private, max-age=43200, pre-check=86400, post-check=43200
```

In order to scale, this would need to be upgraded to a distributed caching system like Redis.

# Tests

Two sets of test: Unit Tests, and Integration Tests. Run them using the dotnet CLI:

```sh
dotnet test tests.unit

dotnet test tests.integration
```

Tests are also run in `Dockerfile` to stop faulty code being built into a Docker image.

# TODO

- `FunTranslationsApiClient` does not have automated tests. They would be similar to `PokeApiClientTests`.

- `PokemonController` has integration tests, but it could have some unit tests to check the caching and the logs.

- Caching layer can move to its own `CacheService` and be added as a depedency to `PokemonController`. And unit tested separately.

- Caching mechanism doesn't scale and should be upgraded to use a distributed cache system. This can be achieved by using `IDistributedCache` and running a Redis instance inside Docker.

- Poke API flavor texts includes escape character such as line feed (`\n`), tab (`\t`), etc. These could be stripped out or appropriated before sending them off to be translated.

- Hypremedia could be implemented by adding links to the API response. In case a client would want to interact with the API response origin:

  ```json
  {
    "name": "pikachu",
    "description": "Whenever pikachu cometh across something new...",
    "links": [
      {
        "href": "https://pokeapi/api/v2/pikachu",
        "rel": "origin",
        "method": "get"
      }
    ]
  }
  ```
