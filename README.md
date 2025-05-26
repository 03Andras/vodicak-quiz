# Vodičák – Webalapú Kvízrendszer

Ez a projekt a **Selye János Egyetem** *Adatbázis alkalmazások* kurzusának **kombinált beadandó feladataként** készült.

##  Cél

A projekt célja egy olyan  webalkalmazás létrehozása, amely segíti a felhasználókat a **jogosítványhoz szükséges közlekedési szabályok** gyakorlásában és tesztelésében.

A rendszer lehetővé teszi:
- Regisztrációt és bejelentkezést
- Kvíz indítását (véletlenszerűen kiválasztott kérdésekkel)
- Válaszadás után az eredmény azonnali megjelenítését
- Korábbi kvízek visszakeresését
- Kétnyelvű (szlovák / angol) felület használatát

---

##  Technológiák

- Backend: C# (.NET WebAPI)
- Frontend: HTML, JavaScript (jQuery), Bootstrap (opcionális)
- Adatbázis: SQLite
- Fejlesztőkörnyezet: Visual Studio Code / dotnet CLI

---

##  Fájlstruktúra

- `/Controllers/` – WebAPI logika (felhasználók, kvízek)
- `/wwwroot/` – HTML oldalak (index, regisztráció, kvíz, eredmények)
- `vodicak.db` – SQLite adatbázis
- `schema.sql` – adatbázis struktúra (DDL)

---

##  Fő funkciók

- Felhasználók regisztrálása (`signup.html`)
-  Bejelentkezés jelszóval, SHA-256 hash + salt használatával
-  Session ID alapú bejelentkezés, cookie-ban tárolva
-  Véletlenszerű kérdésekből generált kvíz (10 db kérdés, kevert válaszok)
-  Visszaszámláló (5 perc), automatikus beküldés
-  Eredmények mentése + kijelzése
-  Korábbi kvízek listázása (`dashboard.html`)
-  Kvíz részleteinek megtekintése (`kviz_detail.html`)
-  Kétnyelvű interfész (szlovák / angol)

---

##  Adatbázis táblák

- `Pouzivatel` – név, email, jelszó hash, só
- `Session` – munkamenet azonosító
- `Otazka` – kérdés szövege, kép elérési útvonala (null is lehet)
- `Odpoved` – válasz szövege, kapcsolódó kérdés ID, helyesség
- `Kviz` – kitöltött kvíz példányai felhasználónként
- `Zaznam` – rögzített válaszok egy kvízhez

---

## Eredeti beadandó szöveg 

**Kombinált beadandó (frissítve 2025-04-02, 25 pont, határidő 2025-06-01)**  
A kombinált beadandó célja egy olyan oldal elkészítése amely segítségeket ad a jogosítványhoz szükséges jogszabályi ismeretek ellenőrzésére és gyakorlására.  
A weboldal egy adatbázisból kérdéseket jelenít meg (kérdésbank), amelyeket a felhasználók megválaszolhatnak.  
A válaszokat a rendszer rögzíti és a felhasználók saját fiókjaikban megtekinthetik az adott válaszaikat. A weboldalra felhasználók regisztrálhatnak.

A gyakorlatot a Moodle-ba kell feltölteni:  https://e-learning.ujs.sk/mod/assign/view.php?id=24974

### Követelmények (részletezve):
- A program három részből áll:  
  - WebApi (.NET, C#)
  - Front-end (JQuery, Bootstrap)
  - SQLite adatbázis back-end

### Minimum funkcionalitás:
- [x] Minden kérdésnek legalább 2 válaszlehetősége van
- [x] Minden kérdésnek van szöveges leírása
- [x] Minden kérdéshez nulla vagy több kép tartozhat
- [x] Egy kvíz 10 kérdésből áll, véletlenszerűen választva
- [x] Válaszok kevert sorrendben jelennek meg

### Oldalak:
- [x] index.html (kezdőlap)
- [x] signup.html (regisztráció)
- [x] dashboard.html (felhasználói fiók + előzmények)
- [x] kviz.html (kvíz indítása)
- [x] kviz_detail.html (részletek megtekintése)

### Felhasználókezelés:
- [x] Regisztráció + jelszó tárolása sózva, hash-elve
- [x] Bejelentkezés POST-tal, SessionID generálás
- [x] Cookie alapú munkamenet-kezelés
- [x] Session adatbázisban tárolva (visszakereshető)

### Kvízek:
- [x] Kvíz rögzítése a rendszerben
- [x] Felhasználók visszanézhetik saját eredményeiket
- [x] Időkorlát beállítása (pl. 5 perc)

### Adatbázis elvárások:
- `User` tábla: név, email, hash+salt
- `Question`, `Answer`, `Quiz`, `Session`, `Record` táblák
- Legalább 20 valós kérdés szerepel a kérdésbankban

---

##  Leadási forma

- Visual Studio Code projekt
- `.NET WebAPI` + SQLite adatbázis
- `vodicak.db` és `schema.sql` mellékelve
- Minden fájl egy `.zip` csomagban vagy GitHub repóként benyújtva

---

##  Szerző

**Nagy András**  
Selye János Egyetem  
Komárno – 2025  
