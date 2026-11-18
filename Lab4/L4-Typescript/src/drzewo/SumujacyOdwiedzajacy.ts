import { IOdwiedzajacy } from "./IOdwiedzajacy";

export class SumujacyOdwiedzajacy implements IOdwiedzajacy<number> {
    private _suma: number = 0;

    get suma(): number { return this._suma; }

    odwiedz(wartosc: number, poziom: number): void {
        this._suma += wartosc;
    }
}
