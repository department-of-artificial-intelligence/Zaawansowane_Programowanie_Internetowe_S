import { Komentarz } from "./Komentarz.js";

export class Artykul {
  private _id: number;
  private _tytul: string;
  private _tresc: string;
  private _dataUtworzenia: Date;
  private _komentarze: Komentarz[] = [];

  constructor(tytul: string, tresc: string) {
    if (!tytul || tytul.trim().length === 0) {
      throw new Error("Tytuł artykułu nie może być pusty.");
    }

    if (!tresc || tresc.trim().length === 0) {
      throw new Error("Treść artykułu nie może być pusta.");
    }

    this._id = Date.now();
    this._tytul = tytul;
    this._tresc = tresc;
    this._dataUtworzenia = new Date();
  }

  get tytul(): string {
    return this._tytul;
  }

  get tresc(): string {
    return this._tresc;
  }

  get dataUtworzenia(): Date {
    return this._dataUtworzenia;
  }

  dodajKomentarz(komentarz: Komentarz): void {
    this._komentarze.push(komentarz);
  }

  pobierzKomentarze(): Komentarz[] {
    return this._komentarze;
  }
}
