type Obserwator = (wartosc: number) => void;

// Model: sama logika, bez wiedzy o HTML (łatwo go przetestować i użyć ponownie)
export class Licznik {
  private _wartosc: number;
  private readonly obserwatorzy: Obserwator[] = [];

  constructor(wartoscPoczatkowa = 0) {
    this._wartosc = wartoscPoczatkowa;
  }

  get wartosc(): number { return this._wartosc; }

  zwieksz(): void { this.ustaw(this._wartosc + 1); }
  zmniejsz(): void { this.ustaw(this._wartosc - 1); }

  ustaw(wartosc: number): void {
    this._wartosc = wartosc;
    this.obserwatorzy.forEach(obserwator => obserwator(wartosc));
  }

  // wzorzec Obserwator: widok dowiaduje się o zmianach wartości
  subskrybuj(obserwator: Obserwator): void {
    this.obserwatorzy.push(obserwator);
  }
}
