class Komentarz {
  _tresc: string;
  _data: Date;
  _odpowiedzi: Komentarz[];
  _nick: string;

  get tresc(): string {
    return this._tresc;
  }
  get data(): Date {
    return this._data;
  }
  get nick(): string {
    return this._nick;
  }

  constructor(tresc: string, nick: string) {
    this._tresc = tresc;
    this._nick = nick;
  }

  dodajOdpowiedz(odpowiedz: Komentarz): void {
    this._odpowiedzi.push(odpowiedz);
  }

  pobierzOdpowiedzi(): Komentarz[] {
    return this._odpowiedzi;
  }
}
