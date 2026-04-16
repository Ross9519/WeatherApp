# Weather App (Open-Meteo API)

## Project Overview
Questa applicazione è una web app sviluppata in Blazor che permette di ottenere informazioni meteo partendo dal nome di una città. L’utente inserisce una città, il sistema utilizza l’API di geocoding di Open-Meteo per trovare le possibili corrispondenze e, dopo la selezione, recupera i dati meteo tramite le coordinate geografiche.

I risultati vengono mostrati in una dashboard sotto forma di card. È possibile salvare fino a 5 ricerche e il sistema include un meccanismo di caching per ridurre le chiamate API. L’architettura è organizzata con servizi, orchestrator e test unitari per la logica backend.

---

## App Features
- Ricerca città tramite geocoding Open-Meteo  
- Selezione tra più risultati possibili  
- Recupero meteo tramite coordinate lat/long  
- Visualizzazione in card nella dashboard  
- Supporto fino a 5 ricerche salvate  
- Caching dei dati meteo  
- Architettura a servizi con orchestrator  
- Test unitari per services, cache e orchestrator  

---

## How to Navigate & Run the Code

Clona la repository:

git clone https://github.com/Ross9519/WeatherApp.git

Apri la solution:

WeatherApp.sln

Esegui l’app da Visual Studio oppure con:

dotnet run --project WeatherApp

---

## Project Structure

WeatherApp/  
 ├── Cache/              # Caching dati meteo  
 ├── Components/         # Componenti UI Blazor  
 ├── Models/             # Modelli dati  
 ├── Orchestrators/      # Coordinamento logica applicativa  
 ├── Pages/              # Pagine Blazor  
 ├── Services/           # Integrazione API Open-Meteo  
 └── Program.cs  

WeatherApp.Tests/  
 ├── Orchestrator/       # Test orchestrator  
 ├── Services/           # Test servizi  
 └── TestUtils/          # Utility per testing  

---

## What I Learned
- Uso dell'AI per la creazione di un piano di azione, di produzione codice e test cases
- Integrazione di API esterne
- Gestione di flussi asincroni multi-step  
- Implementazione frontend interattivo tramite Blazor  
- Implementazione di caching applicativo

---

## Challenges
La parte più complessa è stata gestire il flusso completo tra ricerca città, selezione del risultato e recupero meteo mantenendo uno stato coerente. Anche la gestione dei components razor, e il loro render corretto, mi ha richiesto molta attenzione.

---

## Future Improvements
- Miglioramento della UI e del layout  
- Aggiunta icone per condizioni meteo  
- Persistenza delle ricerche salvate  
- Caching con TTL e invalidazione avanzata  
- Refactor verso Clean Architecture  
