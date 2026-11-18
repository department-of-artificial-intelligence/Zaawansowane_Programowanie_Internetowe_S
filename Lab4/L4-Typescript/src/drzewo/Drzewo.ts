import { IOdwiedzajacy } from "./IOdwiedzajacy";
import { Wezel } from "./Wezel";

export class Drzewo<T> {
    private _korzen: Wezel<T>;

    constructor(wartoscKorzenia: T) {
        this._korzen = new Wezel<T>(wartoscKorzenia);
    }

    get korzen(): Wezel<T> { return this._korzen; }

    przyjmij(odwiedzajacy: IOdwiedzajacy<T>): void {
        this._korzen.przyjmij(odwiedzajacy);
    }

    doKazdego(akcja: (wartosc: T, poziom: number) => void): void {
        this.przyjmij({ odwiedz: akcja });
    }
}
