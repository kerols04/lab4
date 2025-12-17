# Laboration 4 – Konsumera web-API (GitHub + Zippopotam)

Det här är en C#/.NET konsolapplikation som gör HTTP GET-anrop mot två publika REST-API:er,
läser JSON och deserialiserar till C#-objekt med `System.Text.Json`.

## Funktioner

### GitHub API
Programmet hämtar repositories från GitHub för organisationen `.NET` via:
https://api.github.com/orgs/dotnet/repos

Följande fält skrivs ut i konsolen :
- name
- description
- html_url
- homepage
- watchers
- pushed_at

`name` deserialiseras till C#-property `Name` med `JsonPropertyName`.

###  Zippopotam.us
Programmet hämtar platsdata för **Montvale, New Jersey** via:
https://api.zippopotam.us/us/nj/montvale

Programmet skriver ut:
- postnummer
- latitud
- longitud

## Körning

### Krav
- .NET 8 SDK

### Starta programmet
Kör i projektmappen:

```bash
dotnet restore
dotnet run
