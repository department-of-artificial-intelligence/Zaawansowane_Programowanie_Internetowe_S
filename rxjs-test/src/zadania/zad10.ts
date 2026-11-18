import { fromEvent, map, scan, Subscription } from "rxjs";
import { akapit, przycisk, wiersz } from "../ui";
import type { Zadanie } from "./Zadanie";

// Zadanie 10: przycisk 3 liczy kliknięcia tylko wtedy, gdy istnieje subskrypcja.
// "Włącz" tworzy subskrypcję, "Wyłącz" ją odpina (unsubscribe).
class SterowanyLicznik {
  private readonly wlacz: HTMLButtonElement;
  private readonly wylacz: HTMLButtonElement;
  private readonly liczony: HTMLButtonElement;
  private readonly akapit: HTMLParagraphElement;
  private subskrypcja: Subscription | undefined;
  // ostatnia wartość, żeby po ponownym włączeniu liczyć dalej, a nie od zera
  private ostatniaWartosc = 0;

  constructor(wlacz: HTMLButtonElement, wylacz: HTMLButtonElement,
              liczony: HTMLButtonElement, akapit: HTMLParagraphElement) {
    this.wlacz = wlacz;
    this.wylacz = wylacz;
    this.liczony = liczony;
    this.akapit = akapit;
  }

  start(): Subscription {
    const wszystko = new Subscription();
    wszystko.add(fromEvent(this.wlacz, "click").subscribe(() => this.wlaczLiczenie()));
    wszystko.add(fromEvent(this.wylacz, "click").subscribe(() => this.wylaczLiczenie()));
    wszystko.add(() => this.wylaczLiczenie());   // sprzątanie przy zmianie zakładki
    this.pokaz(this.ostatniaWartosc);
    return wszystko;
  }

  private wlaczLiczenie(): void {
    if (this.subskrypcja) return;   // już włączony - nie subskrybujemy drugi raz
    this.subskrypcja = fromEvent(this.liczony, "click")
      .pipe(
        scan(liczba => liczba + 1, this.ostatniaWartosc),
        map(liczba => (this.ostatniaWartosc = liczba)),
      )
      .subscribe(liczba => this.pokaz(liczba));
    this.pokaz(this.ostatniaWartosc);
  }

  private wylaczLiczenie(): void {
    this.subskrypcja?.unsubscribe();
    this.subskrypcja = undefined;
    this.pokaz(this.ostatniaWartosc);
  }

  private pokaz(liczba: number): void {
    const stan = this.subskrypcja ? "włączony" : "wyłączony";
    this.akapit.textContent = `Kliknięcia przycisku 3: ${liczba} (licznik ${stan})`;
  }
}

export const zadanie10: Zadanie = {
  id: "10",
  tytul: "Włączanie i wyłączanie licznika (subscribe / unsubscribe)",
  opis: "Przycisk 1 subskrybuje kliknięcia przycisku 3, przycisk 2 odpina subskrypcję. " +
    "Kliknięcia przycisku 3 przy wyłączonym liczniku nie są liczone.",
  uruchom(kontener) {
    const b1 = przycisk("1. Włącz licznik");
    const b2 = przycisk("2. Wyłącz licznik");
    const b3 = przycisk("3. Kliknij mnie");
    const p = akapit();
    kontener.append(wiersz(b1, b2, b3), p);
    return new SterowanyLicznik(b1, b2, b3, p).start();
  },
};
