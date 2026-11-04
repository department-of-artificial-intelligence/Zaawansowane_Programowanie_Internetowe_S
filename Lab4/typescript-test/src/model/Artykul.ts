import { Komentarz } from "./Komentarz";
import { wymagany } from "./walidacja";

export class Artykul {
  private static nastepneId = 1;

  private _id: number;
  private _tytul!: string;
  private _tresc: string;
  private readonly _dataUtworzenia: Date;
  private _komentarze: Komentarz[] = [];

  constructor(tytul: string, tresc: string) {
    this.tytul = tytul;                 // Zadanie 2: tytuł nie może być pusty
    this._tresc = tresc;
    this._dataUtworzenia = new Date();  // Zadanie 2: data zawsze bieżąca, nie da się jej podać
    this._id = Artykul.nastepneId++;
  }

  get id(): number { return this._id; }

  get tytul(): string { return this._tytul; }
  set tytul(value: string) { this._tytul = wymagany(value, "tytul"); }

  get tresc(): string { return this._tresc; }
  set tresc(value: string) { this._tresc = value; }

  // tylko getter - daty utworzenia nie można zmienić
  get dataUtworzenia(): Date { return new Date(this._dataUtworzenia); }

  dodajKomentarz(komentarz: Komentarz): void {
    this._komentarze.push(komentarz);
  }

  pobierzKomentarze(): Komentarz[] {
    return [...this._komentarze];
  }
}
