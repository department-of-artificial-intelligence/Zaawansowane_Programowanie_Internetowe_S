// Zadanie 14: standardowy dekorator metody mierzący czas jej wykonania.
// Zwraca nową funkcję, która wywołuje oryginał i zwraca dokładnie jego wynik.
export function measureTime<This, Args extends unknown[], Result>(
  target: (this: This, ...args: Args) => Result,
  context: ClassMethodDecoratorContext<This, (this: This, ...args: Args) => Result>
): (this: This, ...args: Args) => Result {
  const nazwa = String(context.name);

  return function (this: This, ...args: Args): Result {
    const start = performance.now();
    const wypisz = () =>
      console.log(`[measureTime] ${nazwa}: ${(performance.now() - start).toFixed(3)} ms`);

    let wynik: Result;
    try {
      wynik = target.apply(this, args);
    } catch (blad) {
      wypisz();       // czas mierzymy też wtedy, gdy metoda rzuci wyjątek
      throw blad;     // i przekazujemy wyjątek dalej bez zmian
    }

    // metoda asynchroniczna: mierzymy do zakończenia obietnicy, a nie do jej zwrócenia
    if (wynik instanceof Promise) {
      return wynik.finally(wypisz) as Result;
    }

    wypisz();
    return wynik;
  };
}
