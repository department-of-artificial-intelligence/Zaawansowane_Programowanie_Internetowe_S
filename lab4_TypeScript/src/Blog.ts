import { Autor } from "./Autor";
import { Artykul } from "./Artykul";

export class Blog {
    private _id: number;
    private _nazwa: string;
    private _autor: Autor;
    private _artykuly: Artykul[] = [];

    constructor(nazwa: string, autor: Autor) {
        this._id = Date.now() + Math.random();

        // Używamy setterów do walidacji
        this.nazwa = nazwa;
        this.autor = autor;
    }

    // --- Właściwości (Gettery / Settery) z walidacją ---

    public get nazwa(): string {
        return this._nazwa;
    }
    public set nazwa(value: string) {
        if (!value) {
            throw new Error("Nazwa bloga jest wymagana.");
        }
        this._nazwa = value;
    }

    public get autor(): Autor {
        return this._autor;
    }
    public set autor(value: Autor) {
        if (!value) { // Sprawdzenie, czy autor nie jest null lub undefined
            throw new Error("Autor bloga musi być podany.");
        }
        this._autor = value;
    }

    // Metody publiczne (bez zmian)
    public dodajArtykul(artykul: Artykul): void {
        this._artykuly.push(artykul);
    }

    public pobierzTytulyArtykulow(): string[] {
        return this._artykuly.map(art => art.tytul);
    }

    public pobierzArtykul(tytul: string): Artykul | undefined {
        return this._artykuly.find(art => art.tytul === tytul);
    }
}