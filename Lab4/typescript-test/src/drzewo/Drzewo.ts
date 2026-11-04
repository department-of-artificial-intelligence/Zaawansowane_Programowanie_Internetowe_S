// Zadanie 8: generyczne drzewo.
// Wzorce: Kompozyt (węzeł zawiera węzły), Odwiedzający (Visitor) do wykonywania
// akcji na wszystkich węzłach oraz Iterator (pętla for...of po wartościach).

export interface OdwiedzajacyDrzewo<T> {
  odwiedz(wezel: WezelDrzewa<T>, glebokosc: number): void;
}

export class WezelDrzewa<T> {
  readonly wartosc: T;
  private readonly _dzieci: WezelDrzewa<T>[] = [];
  private _rodzic: WezelDrzewa<T> | undefined;

  constructor(wartosc: T) {
    this.wartosc = wartosc;
  }

  get dzieci(): readonly WezelDrzewa<T>[] { return this._dzieci; }
  get rodzic(): WezelDrzewa<T> | undefined { return this._rodzic; }
  get jestLisciem(): boolean { return this._dzieci.length === 0; }

  dodajDziecko(wartosc: T): WezelDrzewa<T> {
    const dziecko = new WezelDrzewa(wartosc);
    dziecko._rodzic = this;
    this._dzieci.push(dziecko);
    return dziecko;
  }

  usunDziecko(dziecko: WezelDrzewa<T>): boolean {
    const indeks = this._dzieci.indexOf(dziecko);
    if (indeks < 0) return false;
    this._dzieci.splice(indeks, 1);
    dziecko._rodzic = undefined;
    return true;
  }

  // Visitor: węzeł "przyjmuje" odwiedzającego i przekazuje go dzieciom (preorder)
  przyjmij(odwiedzajacy: OdwiedzajacyDrzewo<T>, glebokosc = 0): void {
    odwiedzajacy.odwiedz(this, glebokosc);
    for (const dziecko of this._dzieci) {
      dziecko.przyjmij(odwiedzajacy, glebokosc + 1);
    }
  }
}

export type AkcjaNaWezle<T> = (wartosc: T, glebokosc: number, wezel: WezelDrzewa<T>) => void;

export class Drzewo<T> {
  readonly korzen: WezelDrzewa<T>;

  constructor(wartoscKorzenia: T) {
    this.korzen = new WezelDrzewa(wartoscKorzenia);
  }

  // przejście w głąb (DFS, preorder)
  przejdzWGlab(akcja: AkcjaNaWezle<T>): void {
    this.korzen.przyjmij({ odwiedz: (wezel, glebokosc) => akcja(wezel.wartosc, glebokosc, wezel) });
  }

  // przejście wszerz (BFS) - poziom po poziomie, z kolejką
  przejdzWszerz(akcja: AkcjaNaWezle<T>): void {
    const kolejka: [WezelDrzewa<T>, number][] = [[this.korzen, 0]];
    while (kolejka.length > 0) {
      const [wezel, glebokosc] = kolejka.shift()!;
      akcja(wezel.wartosc, glebokosc, wezel);
      for (const dziecko of wezel.dzieci) {
        kolejka.push([dziecko, glebokosc + 1]);
      }
    }
  }

  odwiedz(odwiedzajacy: OdwiedzajacyDrzewo<T>): void {
    this.korzen.przyjmij(odwiedzajacy);
  }

  znajdz(warunek: (wartosc: T) => boolean): WezelDrzewa<T> | undefined {
    let znaleziony: WezelDrzewa<T> | undefined;
    this.przejdzWGlab((wartosc, _glebokosc, wezel) => {
      if (znaleziony === undefined && warunek(wartosc)) znaleziony = wezel;
    });
    return znaleziony;
  }

  get rozmiar(): number {
    let licznik = 0;
    this.przejdzWGlab(() => licznik++);
    return licznik;
  }

  // Iterator: for (const wartosc of drzewo) { ... } - w głąb, leniwie (generator)
  *[Symbol.iterator](): Generator<T> {
    const stos: WezelDrzewa<T>[] = [this.korzen];
    while (stos.length > 0) {
      const wezel = stos.pop()!;
      yield wezel.wartosc;
      // dzieci od końca, żeby wyjść w tej samej kolejności co przejdzWGlab
      for (let i = wezel.dzieci.length - 1; i >= 0; i--) stos.push(wezel.dzieci[i]);
    }
  }
}

// Przykładowi odwiedzający
export class RysowanieDrzewa<T> implements OdwiedzajacyDrzewo<T> {
  private readonly linie: string[] = [];
  private readonly opis: (wartosc: T) => string;

  constructor(opis: (wartosc: T) => string) {
    this.opis = opis;
  }

  odwiedz(wezel: WezelDrzewa<T>, glebokosc: number): void {
    const prefiks = glebokosc === 0 ? "" : "│  ".repeat(glebokosc - 1) + "├─ ";
    this.linie.push(prefiks + this.opis(wezel.wartosc));
  }

  get wynik(): string { return this.linie.join("\n"); }
}

export class LiczenieLisci<T> implements OdwiedzajacyDrzewo<T> {
  liczba = 0;

  odwiedz(wezel: WezelDrzewa<T>): void {
    if (wezel.jestLisciem) this.liczba++;
  }
}
