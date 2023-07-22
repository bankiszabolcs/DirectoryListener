# DirectoryListener
Az alkalmazás Windows környezetben képes a lokális fájlrendszer egy megadott könyvtárának tartalmát figyelni, annak változása esetén – 
amint a fájlok már nem íródnak - , egy http POST request-ben az ott található fájlok közül a kiválasztási szempontoknak megfelelőt egy szerverre feltölteni.)

> Az alkalmazást egy a GitHubon publikosan hozzáférhető fájlszerverrel teszteltem (`https://github.com/rafaelfgx/StorageService.git`) 
> Az POST kérést úgy írtam meg, hogy megfeleljen a szerver kritériumainak és a URI cím ennek megfelelően lett az alkalmazásba "bele égetve".

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

## Alkalmazás indítása
1. Forkolni kell az adott GitHub repository tartalmát:
https://github.com/bankiszabolcs/DirectoryListener

2.  A célgépre le kell klónozni az adott GitHub repository tartalmát.
    Terminálba:
    `git clone https://github.com/bankiszabolcs/DirectoryListener.git`

3.  Futtatás
   + Visual Studioból
       - Projekt gyökerében nyitsd meg a DirectoryListener.sln fájlt
       - Nyomd le az F5-öt
   
   + Windows Intézőből
        - Projekt gyökerében lépj be a DirectoryListener > bin > Debug > net6.0 mappába és indítsd el a DirectoryListener.exe fájlt.

## Továbbfejlesztési lehetőségek:
- Több megfigyelhető fájlformátum hozzáadása
- Választási lehetőség hogy a fájlok melyik módosítása érdekel minket (átnevezés, módosítás, törlés stb.)
- Szerver URI-jének megadási lehetősége
