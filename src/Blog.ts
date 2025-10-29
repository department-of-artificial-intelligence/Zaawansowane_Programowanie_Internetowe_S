import { Artykul } from "./Artykul";
import { Autor } from "./Autor";

export class Blog {
    private _id: number;
    private _nazwa: string;
    private _autor: Autor;
    private _artykul: Artykul[] = [];

    constructor(nazwa: string, autor: Autor){
        this._nazwa = nazwa;
        this._autor = autor;
    }


    get nazwa(): string{
        return this._nazwa;
    }

    get autor(): Autor{
        return this._autor;
    }

    public dodajArtykul(artykul: Artykul): void{
        this._artykul.push(artykul);
    }
    
    public pobierzTytulyArtykulow(): string[] {
        return this._artykul.map(a => a.tytul);
    }

    public pobierzArtykul(tytul: string): Artykul | undefined {
        return this._artykul.find(a => a.tytul == tytul);
    } 
}