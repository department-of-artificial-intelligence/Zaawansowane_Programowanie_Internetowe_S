import { Licznik } from "./Licznik";

// Widok: tworzy pole input i przyciski, łączy je z modelem
export class LicznikWidok {
  private readonly licznik: Licznik;
  private readonly pole: HTMLInputElement;

  constructor(kontener: HTMLElement, licznik: Licznik) {
    this.licznik = licznik;

    const minus = this.utworzPrzycisk("-", () => this.licznik.zmniejsz());
    this.pole = document.createElement("input");
    this.pole.type = "number";
    this.pole.className = "licznik-pole";
    const plus = this.utworzPrzycisk("+", () => this.licznik.zwieksz());

    // użytkownik może też wpisać liczbę ręcznie
    this.pole.addEventListener("change", () => {
      const liczba = Number(this.pole.value);
      this.licznik.ustaw(Number.isFinite(liczba) ? Math.trunc(liczba) : 0);
    });

    this.licznik.subskrybuj(wartosc => this.wyswietl(wartosc));
    this.wyswietl(this.licznik.wartosc);

    const wiersz = document.createElement("div");
    wiersz.className = "licznik";
    wiersz.append(minus, this.pole, plus);
    kontener.appendChild(wiersz);
  }

  private wyswietl(wartosc: number): void {
    this.pole.value = String(wartosc);
  }

  private utworzPrzycisk(etykieta: string, akcja: () => void): HTMLButtonElement {
    const przycisk = document.createElement("button");
    przycisk.type = "button";
    przycisk.textContent = etykieta;
    przycisk.addEventListener("click", akcja);
    return przycisk;
  }
}
