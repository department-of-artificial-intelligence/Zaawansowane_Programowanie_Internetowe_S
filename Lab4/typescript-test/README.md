# Zajęcia 4 – TypeScript (Vite + TypeScript)

## Uruchomienie

```bash
npm install
npm run dev      # serwer deweloperski, adres pojawi się w konsoli
npm run build    # sprawdzenie typów (tsc, tryb strict) i wersja produkcyjna
```

Każde zadanie ma swoją zakładkę (1–14) na górze strony. Kod demonstracji: `src/zadania/zadNN.ts`.

## Struktura

| Katalog | Zawartość | Zadania |
| --- | --- | --- |
| `src/model` | `Autor`, `Blog`, `Artykul`, `Komentarz` (każda klasa w osobnym pliku), `walidacja.ts`, `dto.ts` | 1, 2, 9, 10 |
| `src/formatowanie` | strategie formatowania autorów i rejestr `FORMATY` | 3, 7 |
| `src/licznik` | model `Licznik` i widok `LicznikWidok` | 4 |
| `src/autorzy` | `AutorService` (fetch, `Record`, wyszukiwanie), `WidokAutorow` | 5, 6, 7, 10–13 |
| `src/drzewo` | generyczne `Drzewo<T>` | 8 |
| `src/dekoratory` | `measureTime` | 14 |
| `public/dane.json` | lista autorów (rekord nr 7 celowo ma błędny e-mail) | 5 |

## Odpowiedzi na pytania z zadań

**Zadanie 2 – jakie są możliwości?**

1. **Walidacja w setterach wywoływanych z konstruktora** (wybrana). Obiektu nie da się utworzyć
   z błędnymi danymi ani później zepsuć przez przypisanie. Działa w czasie wykonania, więc chroni
   też przed danymi z serwera.
2. **Dekoratory `accessor`** (`@required`, `@validate([...])` z rozdz. 4.1.7) – mniej kodu
   powtarzalnego, ale reguły są rozrzucone po adnotacjach.
3. **Smart konstruktor / metoda fabrykująca** – prywatny konstruktor i `static utworz(...)`
   zwracające wynik albo błąd zamiast wyjątku (jak `Result<T>` w projekcie .NET).
4. **Same typy TypeScript** nie wystarczą: znikają po kompilacji, więc nie sprawdzą np. formatu
   e-maila ani danych przychodzących z `fetch`.

Data utworzenia artykułu jest ustawiana w konstruktorze i ma tylko getter, więc nie da się jej podać
ani zmienić.

**Zadanie 3 – jakie wzorce?** Przede wszystkim **Strategia**: interfejs `FormatAutorow` i klasy
`TabelaHtml`, `ListaHtml`, `FormatJson`, `FormatCsv`, a `EksportAutorow` przechowuje wybraną
strategię. Nowy format to nowa klasa, bez zmian w istniejącym kodzie (zasada otwarte-zamknięte).
Rejestr `FORMATY` pełni rolę prostej **fabryki**. Alternatywy: **Odwiedzający** (Visitor), **Most**
(Bridge), **Metoda szablonowa** (Template Method) dla wspólnego szkieletu formatów HTML.

**Zadanie 8 – jaki wzorzec?** **Odwiedzający (Visitor)** – `OdwiedzajacyDrzewo<T>` i metoda
`przyjmij`; nowe operacje na drzewie (`RysowanieDrzewa`, `LiczenieLisci`) to nowe klasy, bez zmian
w drzewie. Do tego **Iterator** (`for...of` przez `Symbol.iterator`) i **Kompozyt** (węzeł zawiera
węzły). Są dwa przejścia: w głąb (DFS) i wszerz (BFS).

**Zadanie 9 – czym to się różni od ręcznych typów?** Typy z `Omit`/`Pick` są wyprowadzone z klasy,
więc jest jedno źródło prawdy. Gdy do `Autor` dojdzie nowe pole (getter/setter), `NowyAutor`
automatycznie je zawiera, a kompilator wskaże każde miejsce, które tworzy autora bez tego pola.
`AutorNaLiscie` (Pick) się nie zmieni, bo wybiera pola jawnie. Ręcznie zdefiniowane typy trzeba
by pamiętać zaktualizować – łatwo o rozjazd. Pomocniczy `TylkoDane<T>` usuwa metody klasy, żeby
DTO zawierało tylko dane.

**Zadanie 11 – Record a tablica.** `record[id]` to jeden odczyt po kluczu (stały czas, O(1)),
a `tablica.find()` przegląda elementy po kolei (czas liniowy, O(n)). Zakładka 11 mierzy to na
200 000 elementów. Wadą `Record` jest brak kolejności „z definicji” i to, że typ zakłada istnienie
każdego klucza – dlatego `znajdzPoId` ma jawnie zwracany typ `Autor | undefined`.

**Zadanie 12 – błędy kompilatora.** Po usunięciu `@ts-expect-error` w `src/zadania/zad12.ts`:

- `TS2540: Cannot assign to 'email' because it is a read-only property.`
- `TS2339: Property 'push' does not exist on type 'readonly Readonly<Autor>[]'.`

`Readonly` działa tylko w czasie kompilacji i nie blokuje metod zmieniających obiekt
(np. `aktualizuj`). Pełną ochronę w czasie działania daje `Object.freeze`.

## Uwaga o dekoratorach (Vite 8)

TypeScript 7 sprawdza standardowe dekoratory poprawnie, ale transformator Vite 8 (Oxc) zostawia
składnię `@dekorator` bez zmian, a przeglądarki jej jeszcze nie obsługują. Dlatego `vite.config.ts`
zawiera małą wtyczkę, która tłumaczy pliki z dekoratorami przez **esbuild** (dodany do
`devDependencies`).
