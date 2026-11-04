export class Komentarz {
  private _tresc: string;
  private _data: Date;
  private _odpowiedzi: Komentarz[] = [];
  private _nick: string;

  constructor(tresc: string, nick: string) {
    this._tresc = tresc;
    this._nick = nick;
    this._data = new Date();
  }

  get tresc(): string { return this._tresc; }
  set tresc(value: string) { this._tresc = value; }

  // kopia, żeby nikt nie zmienił daty przez referencję do obiektu Date
  get data(): Date { return new Date(this._data); }

  get nick(): string { return this._nick; }
  set nick(value: string) { this._nick = value; }

  dodajOdpowiedz(odpowiedz: Komentarz): void {
    this._odpowiedzi.push(odpowiedz);
  }

  pobierzOdpowiedzi(): Komentarz[] {
    return [...this._odpowiedzi];
  }
}
