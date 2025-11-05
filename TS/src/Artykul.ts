class Artykul {
  _id: number;
  _tytul: string;
  _tresc: string;
  _dataUtworzenia: Date = new Date;
  _komentarze: Komentarz[];

  get tytul(): string {
    return this._tytul;
  }
  get tresc(): string {
    return this._tresc;
  }

  constructor(tytul: string, tresc: string) {
    if (tytul.trim() === "") {
      throw "Tytul nie może być pusta.";
    }
    this._tytul = tytul;
    this._tresc = tresc;
  }

  dodajKomentarz(komentarz: Komentarz): void {
    this._komentarze.push(komentarz);
  }

  pobierzKomentarze(): Komentarz[] {
    return this._komentarze;
  }
}
