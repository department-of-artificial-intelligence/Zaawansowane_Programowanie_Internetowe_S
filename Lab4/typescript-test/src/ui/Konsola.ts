// Wypisuje komunikaty jednocześnie w konsoli przeglądarki i na stronie
export class Konsola {
  private readonly element: HTMLPreElement;

  constructor(kontener: HTMLElement) {
    this.element = document.createElement("pre");
    this.element.className = "konsola";
    kontener.appendChild(this.element);
  }

  log(...wartosci: unknown[]): void {
    console.log(...wartosci);
    this.dopisz(wartosci.map(w => this.naTekst(w)).join(" "));
  }

  // tylko na stronę, bez console.log (np. gdy sami przechwytujemy console.log)
  pokaz(tekst: string): void {
    this.dopisz(tekst);
  }

  blad(wartosc: unknown): void {
    console.error(wartosc);
    const tekst = wartosc instanceof Error ? `${wartosc.name}: ${wartosc.message}` : String(wartosc);
    this.dopisz(`✗ ${tekst}`, "blad");
  }

  private dopisz(tekst: string, klasa?: string): void {
    const linia = document.createElement("div");
    if (klasa) linia.className = klasa;
    linia.textContent = tekst;
    this.element.appendChild(linia);
  }

  private naTekst(wartosc: unknown): string {
    if (typeof wartosc === "string") return wartosc;
    if (wartosc instanceof Date) return wartosc.toLocaleString("pl-PL");
    if (typeof wartosc === "object" && wartosc !== null && !Array.isArray(wartosc)
        && wartosc.toString !== Object.prototype.toString) {
      return String(wartosc);
    }
    return JSON.stringify(wartosc);
  }
}
