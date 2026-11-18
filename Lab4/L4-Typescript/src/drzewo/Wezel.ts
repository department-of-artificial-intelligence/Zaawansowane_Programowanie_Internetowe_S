import { IOdwiedzajacy } from "./IOdwiedzajacy";

export class Wezel<T> {
    private _dzieci: Wezel<T>[] = [];

    constructor(private _wartosc: T) { }

    get wartosc(): T { return this._wartosc; }

    get dzieci(): Wezel<T>[] { return this._dzieci; }

    dodajDziecko(wartosc: T): Wezel<T> {
        const dziecko = new Wezel<T>(wartosc);
        this._dzieci.push(dziecko);
        return dziecko;
    }

    przyjmij(odwiedzajacy: IOdwiedzajacy<T>, poziom: number = 0): void {
        odwiedzajacy.odwiedz(this._wartosc, poziom);
        this._dzieci.forEach(dziecko => dziecko.przyjmij(odwiedzajacy, poziom + 1));
    }
}
