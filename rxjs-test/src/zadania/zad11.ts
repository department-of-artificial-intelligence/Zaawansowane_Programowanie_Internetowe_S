import { fromEvent, scan, shareReplay, startWith, Subscription, type Observable } from "rxjs";
import { akapit, etykieta, pole, przycisk, wiersz } from "../ui";
import type { Zadanie } from "./Zadanie";

// Przełącza wyświetlanie licznika w jednym polu input: subskrybuje albo odpina
class WyswietlanieWPolu {
  private readonly zrodlo$: Observable<number>;
  private readonly pole: HTMLInputElement;
  private readonly przycisk: HTMLButtonElement;
  private readonly nazwa: string;
  private subskrypcja: Subscription | undefined;

  constructor(zrodlo$: Observable<number>, pole: HTMLInputElement,
              przycisk: HTMLButtonElement, nazwa: string) {
    this.zrodlo$ = zrodlo$;
    this.pole = pole;
    this.przycisk = przycisk;
    this.nazwa = nazwa;
    this.odswiezPrzycisk();
  }

  przelacz(): void {
    if (this.subskrypcja) {
      this.zablokuj();
    } else {
      this.subskrypcja = this.zrodlo$.subscribe(n => (this.pole.value = String(n)));
      this.pole.classList.remove("zablokowane");
    }
    this.odswiezPrzycisk();
  }

  zablokuj(): void {
    this.subskrypcja?.unsubscribe();
    this.subskrypcja = undefined;
    this.pole.classList.add("zablokowane");
  }

  private odswiezPrzycisk(): void {
    this.przycisk.textContent = this.subskrypcja
      ? `Zablokuj wyświetlanie w ${this.nazwa}`
      : `Wyświetlaj w ${this.nazwa}`;
  }
}

// Zadanie 11: jeden licznik kliknięć, dwa pola, które można niezależnie podpinać
class LicznikWDwochPolach {
  private readonly uzyjShareReplay: boolean;

  constructor(uzyjShareReplay: boolean) {
    this.uzyjShareReplay = uzyjShareReplay;
  }

  start(kontener: HTMLElement): Subscription {
    const b1 = przycisk("Kliknij mnie (liczony)");
    const b2 = przycisk("");
    const b3 = przycisk("");
    const p1 = pole("text");
    const p2 = pole("text");
    p1.readOnly = p2.readOnly = true;
    const info = akapit();
    kontener.append(wiersz(b1, b2, b3), wiersz(etykieta("Pole 1: ", p1), etykieta("Pole 2: ", p2)), info);

    const licznik$ = fromEvent(b1, "click").pipe(
      scan(n => n + 1, 0),
      startWith(0),
    );
    // shareReplay(1): jedno wspólne wykonanie strumienia + ostatnia wartość dla nowych subskrybentów
    const zrodlo$ = this.uzyjShareReplay ? licznik$.pipe(shareReplay(1)) : licznik$;

    const wszystko = new Subscription();
    // stała subskrypcja pokazująca "prawdziwą" liczbę kliknięć od otwarcia zakładki
    wszystko.add(zrodlo$.subscribe(n => (info.textContent = `Akapit (subskrypcja od początku): ${n}`)));

    const w1 = new WyswietlanieWPolu(zrodlo$, p1, b2, "polu 1");
    const w2 = new WyswietlanieWPolu(zrodlo$, p2, b3, "polu 2");
    wszystko.add(fromEvent(b2, "click").subscribe(() => w1.przelacz()));
    wszystko.add(fromEvent(b3, "click").subscribe(() => w2.przelacz()));
    wszystko.add(() => { w1.zablokuj(); w2.zablokuj(); });
    return wszystko;
  }
}

function sekcja(kontener: HTMLElement, tytul: string): HTMLElement {
  const s = document.createElement("section");
  const h = document.createElement("h3");
  h.textContent = tytul;
  s.appendChild(h);
  kontener.appendChild(s);
  return s;
}

export const zadanie11: Zadanie = {
  id: "11",
  tytul: "Dwa pola, jeden licznik (shareReplay)",
  opis: "Kliknij kilka razy, potem włącz pole 1, kliknij dalej i włącz pole 2. " +
    "Bez shareReplay każde pole pokazuje inną liczbę – wyjaśnienie w README.md.",
  uruchom(kontener) {
    const wszystko = new Subscription();
    wszystko.add(new LicznikWDwochPolach(false)
      .start(sekcja(kontener, "Bez shareReplay (obserwowany efekt)")));
    wszystko.add(new LicznikWDwochPolach(true)
      .start(sekcja(kontener, "Z shareReplay(1) (poprawione)")));
    return wszystko;
  },
};
