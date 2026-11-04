import { AutorService } from "../autorzy/AutorService";
import { measureTime } from "../dekoratory/measureTime";
import { Konsola } from "../ui/Konsola";
import { ADRES_DANYCH, naglowek, type Zadanie } from "./Zadanie";

class Obliczenia {
  @measureTime
  fibonacci(n: number): number {
    let [a, b] = [0, 1];
    for (let i = 0; i < n; i++) [a, b] = [b, a + b];
    return a;
  }

  @measureTime
  sumaKwadratow(ile: number): number {
    let suma = 0;
    for (let i = 1; i <= ile; i++) suma += i * i;
    return suma;
  }

  @measureTime
  async poczekaj(ms: number): Promise<string> {
    await new Promise(resolve => setTimeout(resolve, ms));
    return `odczekano ${ms} ms`;
  }

  @measureTime
  dzielenie(a: number, b: number): number {
    if (b === 0) throw new Error("Dzielenie przez zero");
    return a / b;
  }
}

export const zadanie14: Zadanie = {
  numer: 14,
  tytul: "Dekorator measureTime",
  async uruchom(kontener) {
    naglowek(kontener,
      "@measureTime wypisuje czas wykonania w konsoli przeglądarki (F12). Tutaj komunikaty " +
      "są dodatkowo kopiowane na stronę. Wynik metody pozostaje bez zmian.");
    const k = new Konsola(kontener);

    // przechwycenie console.log tylko na czas demonstracji
    const oryginalnyLog = console.log;
    console.log = (...args: unknown[]) => {
      oryginalnyLog(...args);
      if (typeof args[0] === "string" && args[0].startsWith("[measureTime]")) {
        k.pokaz(args.map(String).join(" "));
      }
    };

    try {
      const obliczenia = new Obliczenia();
      k.log("fibonacci(40) =", obliczenia.fibonacci(40));
      k.log("sumaKwadratow(1 000 000) =", obliczenia.sumaKwadratow(1_000_000));
      k.log("poczekaj(300) ->", await obliczenia.poczekaj(300));
      try {
        obliczenia.dzielenie(1, 0);
      } catch (blad) {
        k.blad(blad);
      }
      await new AutorService(ADRES_DANYCH).wczytaj();
    } finally {
      console.log = oryginalnyLog;
    }
  },
};
