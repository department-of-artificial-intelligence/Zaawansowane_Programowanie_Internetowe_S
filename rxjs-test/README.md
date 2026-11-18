# Zajęcia 5 – programowanie funkcyjne i RxJS

```bash
npm install
npm run dev
```

Zakładki na górze strony: `1-6` (tablice w stylu funkcyjnym, `src/funkcyjne.ts`) oraz 7–20 (RxJS,
każde zadanie w osobnym pliku `src/zadania/zadNN.ts`, zawsze jako klasa). Przy zmianie zakładki
subskrypcje poprzedniego zadania są odpinane (`unsubscribe`).

## Odpowiedzi na pytania z zadań

**Zadanie 11 – co widać i dlaczego?** Bez `shareReplay` każde pole pokazuje inną liczbę, mniejszą
niż faktyczna liczba kliknięć, a po zablokowaniu i odblokowaniu pole zaczyna znowu od 0.
Obiekt obserwowalny w RxJS jest „zimny”: każde `subscribe` uruchamia osobne wykonanie całego
łańcucha `fromEvent → scan`, z własnym stanem `scan` zaczynającym od 0. Pole liczy więc tylko
kliknięcia od momentu, w którym je podpięto. `shareReplay(1)` sprawia, że wszyscy subskrybenci
korzystają z jednego, wspólnego wykonania, a nowy subskrybent od razu dostaje ostatnią wartość –
wszystkie pola pokazują tę samą, prawdziwą liczbę. Zakładka 11 pokazuje obie wersje obok siebie.

**Zadanie 15 – co widać?** Pierwszy błąd (puste pole albo tekst zamiast liczby) trafia do funkcji
`error` w `subscribe` i **kończy strumień**. Po zdarzeniu `error` obiekt obserwowalny nie emituje już
nic, więc poprawienie wartości niczego nie zmienia – aplikacja przestaje reagować. Zadania 16–18 to
naprawiają: `retry` / `catchError` ponownie subskrybują strumień, a `filter` pomija puste pola.

## Uwagi do rozwiązań

- **Zadanie 10:** licznik po ponownym włączeniu liczy dalej, a nie od zera (ostatnia wartość
  jest zapamiętywana i podawana jako wartość początkowa `scan`).
- **Zadania 13–14:** `filter` zatrzymuje teksty niespełniające warunku, więc w paragrafie zostaje
  ostatni tekst, który warunek spełnił.
- **Zadanie 14:** `startWith` przy obu strumieniach jest potrzebny, bo `combineLatest` emituje
  dopiero wtedy, gdy każde źródło wyemitowało co najmniej jedną wartość.
- **Zadania 15–18:** treść mówi o polu kwadratu z dwóch pól – liczone jest pole prostokąta `a · b`
  (dla `a = b` to pole kwadratu). Strumień reaguje na zatwierdzenie wartości (zdarzenie `change`:
  Enter albo przejście do drugiego pola), a nie na każdy klawisz – inaczej niedokończona wartość
  (np. samo `-`) byłaby błędem. `skipWhile` sprawia, że nic nie jest liczone, dopóki oba pola nie
  zostaną wypełnione po raz pierwszy. Zatwierdzenie któregokolwiek pola emituje parę obu wartości
  (`merge`), dzięki czemu po `retry` wystarczy poprawić jedno pole.
- **Zadanie 16:** `catchError` pokazuje komunikat i przekazuje błąd dalej przez `throwError`,
  `retry(3)` daje 3 poprawki, a 4. błąd trafia do `error` w `subscribe` z komunikatem
  „Wprowadziłeś błędną wartość 4 razy. Koniec zabawy”.
- **Zadanie 17:** `catchError((e, zrodlo$) => zrodlo$)` – zwrócenie strumienia źródłowego oznacza
  ponowną subskrypcję po każdym błędzie, bez limitu.
- **Zadanie 20:** obok opóźnionego diva jest mała szara kropka bez opóźnienia, żeby było widać różnicę.
