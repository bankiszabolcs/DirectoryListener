# DirectoryListener
Az alkalmazás Windows környezetben képes a lokális fájlrendszer egy megadott könyvtárának tartalmát figyelni, annak változása esetén – 
amint a fájlok már nem íródnak - , egy http POST request-ben az ott található fájlok közül a kiválasztási szempontoknak megfelelőt egy szerverre feltölteni.)

## Leírás
- Megadhatjuk, hogy mely mappa tartalmát figyelje. 
- Az alkalmazás jelzi ha egy fájlt módosítottak, töröltek, átneveztek vagy újat hoztak létre. 
- A "figyelés" az alkönyvtárakra is vonatkozik.
- Kiválaszthatjuk, milyen formátumú fájlokat figyeljen meg. (Csak a listában szereplőket képes megfigyelni.)
- A megfigyelés ideje alatt folyamatosan listázódnak a keresésnek megfelelő elemek.
- Találat esetén az adott fájl azonnal továbbküldődik egy 'Post' kéréssel, ha a távoli szerver elérhető.
- A fájl továbbküldésének eredményességét a találati doboz utolsó oszlopában található ikon jelzi.
- Megfigyelést leállíthatjuk, és újból újraindíthatjuk.
- A figyelés eredményét 'txt' fájlba kimenthetjük.

## Indítás

## Továbbfejlesztési lehetőségek:
- Több megfigyelhető fájlformátum hozzáadása
- Választási lehetőség hogy a fájlok melyik módosítása érdekel minket (átnevezés, módosítás, törlés stb.)
- Szerver URI-jének megadási lehetősége
