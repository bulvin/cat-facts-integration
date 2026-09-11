# Cat Facts Integration

Aplikacja Web API napisana w .NET, która pobiera ciekawostki o kotach z zewnętrznego API, zapisuje je lokalnie w pliku <code>Data/cat-facts.txt</code> oraz umożliwia ich wyszukiwanie i paginację.

## Funkcjonalności

* pobieranie ciekawostek z zewnętrznego API,
* zapisywanie danych w pliku tekstowym w formacie JSON,
* prosta paginacja wyników
* wyszukiwanie po fragmencie tekstu
* globalna obsługa wyjątków

## Technologie

* .NET 10
* ASP.NET Minimal API
* xUnit
* NSubstitute
* OpenAPI, Scalar

#

## Uruchomienie

Przywróć zależności:

```bash
dotnet restore
```

Uruchom aplikację:

```bash
dotnet run --project CatFactsIntegration.WebApi
```

Dokumentacja Scalar jest dostępna w środowisku deweloperskim pod adresem:

```text
/scalar/v1
```

## Przykłady
http://localhost:5280/fact

<img width="680" height="152" alt="image" src="https://github.com/user-attachments/assets/8cf6f1ca-0b54-41e0-a5d2-ef1419e67979" />

http://localhost:5280/facts?page=1&limit=2&phrase=cat

<img width="1240" height="242" alt="image" src="https://github.com/user-attachments/assets/17d192a6-672d-4b6f-9e1b-ee323147ff35" />


## Testy

Uruchomienie wszystkich testów:

```bash
dotnet test
```
