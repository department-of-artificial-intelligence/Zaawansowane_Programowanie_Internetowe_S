import { FORMATY, type NazwaFormatu } from "../formatowanie/EksportAutorow";
import type { Autor } from "../model/Autor";

// Zadania 6 i 7: wyświetlanie autorów, z opcjonalną listą wyboru formy.
// Zadanie 12: widok przyjmuje dane tylko do odczytu, więc nie może ich zmienić.
export class WidokAutorow {
  private readonly wynik: HTMLDivElement;
  private autorzy: readonly Readonly<Autor>[] = [];
  private format: NazwaFormatu;

  constructor(kontener: HTMLElement, formaty: readonly NazwaFormatu[], domyslny: NazwaFormatu) {
    this.format = domyslny;

    if (formaty.length > 1) {
      const etykieta = document.createElement("label");
      etykieta.textContent = "Forma wyświetlania: ";
      const lista = document.createElement("select");
      for (const nazwa of formaty) {
        const opcja = document.createElement("option");
        opcja.value = nazwa;
        opcja.textContent = FORMATY[nazwa].nazwa;
        opcja.selected = nazwa === domyslny;
        lista.appendChild(opcja);
      }
      lista.addEventListener("change", () => {
        this.format = lista.value as NazwaFormatu;
        this.odswiez();
      });
      etykieta.appendChild(lista);
      kontener.appendChild(etykieta);
    }

    this.wynik = document.createElement("div");
    this.wynik.className = "wynik";
    kontener.appendChild(this.wynik);
  }

  pokaz(autorzy: readonly Readonly<Autor>[]): void {
    this.autorzy = autorzy;
    this.odswiez();
  }

  private odswiez(): void {
    // formaty HTML zwracają już bezpieczny (escapowany) HTML
    this.wynik.innerHTML = FORMATY[this.format].formatuj(this.autorzy);
  }
}
