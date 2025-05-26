# Vodičák – Webalapú jogosítványteszt kvízrendszer

Ez a dokumentum a projekt során felhasznált nyilvános könyvtárak, kódrészletek, leírások és képek forrásait tartalmazza.

---

##  Felhasznált könyvtárak (Libraries used)

### 1. [jQuery 3.6.0](https://jquery.com/)
- CDN: https://code.jquery.com/jquery-3.6.0.min.js
- Használat: DOM-kezelés, AJAX POST és GET hívások egyszerűsítéséhez.
- Licenc: MIT License

---

## Kódalap és inspiráció

### 2. ASP.NET WebAPI sablon (dotnet CLI)
- Parancs: `dotnet new webapi`
- Dokumentáció: https://learn.microsoft.com/en-us/aspnet/core/web-api/
- Cél: REST API végpontok kezelése (regisztráció, bejelentkezés, kvízkezelés)

### 3. SQLite ADO.NET
- Könyvtár: `Microsoft.Data.Sqlite`
- Forrás: https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/
- Cél: könnyűsúlyú adatbázis-kezelés fájl alapú rendszerrel

### 4. Jelszavak hash-elése és sózása
- SHA256 + random salt technika
- Alapötlet: Microsoft dokumentáció alapján
- Forrás: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256

---

## Képek és kérdések forrása

### 5. Vizsga kérdések és közlekedési témák
- Saját kézzel szerkesztett, valamint inspiráció:
  - https://www.minv.sk/?testy-na-vodicske-opravnenie
  - https://www.autoskolatest.sk/
  - https://www.testy-auto.sk/
- Kérdések egy része átfogalmazott / kombinált

### 6. Felhasznált képek (pl. gyalogosok.png, kriz1.png stb.)
- Forrás: nyílt Google képkeresés
- Csak illusztrációs céllal, oktatási használatra (non-commercial use)

---

## Egyéb technikai segítség

### 7. ChatGPT (OpenAI)
- Kódrészletek strukturálása, hibakeresés, magyarázatok
- Prompt-alapú segítségnyújtás: .NET konfigurációk, JS nyelvváltás, adatbázis létrehozás

### 8. StackOverflow
- Hibák keresése, megoldások:
  - Példa: `"Unable to find the required services. Please add AddControllers"` – https://stackoverflow.com/a/60484791
  - `"SQLite no such table"` – https://stackoverflow.com/a/13253942

---

##  Licenc és felhasználás

Ez a projekt **oktatási céllal** készült. A kód szabadon felhasználható nem kereskedelmi célokra.  
A kérdések és képek kizárólag demonstrációs célokat szolgálnak.

---

## Szerző

**Nagy András**  
Selye János Egyetem, Komárno  
2025 – Adatbázis alkalmazások (kombinált beadandó)
