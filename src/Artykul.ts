import { Komentarz } from "./Komentarz";

export class Artykul {
  private _id: number;
  private _tytul: string;
  private _tresc: string;
  private _dataUtworzenia: Date;
  private _komentarze: Komentarz[] = [];

  constructor(tytul: string, tresc: string) {
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

  dodajKomentarz(komentarz: Komentarz): void {
    this._komentarze.push(komentarz);
  }

  pobierzKomentarze(): Komentarz[] {
    return this._komentarze;
  }
}
