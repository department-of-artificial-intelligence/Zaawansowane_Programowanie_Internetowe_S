import { Autor } from "./autor";
import { Artykul } from "./artykul";

export class Blog {
    private _id: number;
    private _nazwa: string;
    private _artykuly: Artykul[];
    private _autor: Autor;

    get autor(): Autor{
        return this._autor;
    }
    set autor(value: Autor){
        this._autor = value;
    }

    get nazwa(): string{
        return this._nazwa;
    }
    set nazwa(value: string){
        const clean = value.trim();
        if(!clean)
            throw new Error("Nazwa sa wymagana !");
        this._nazwa = clean;
    }

    constructor(nazwa: string, autor: Autor, id: number = 0){
        this._id = id;
        this._nazwa = "";
        this.nazwa = nazwa;
        this.autor = autor;
    }

    dodajArtykul(artykul: Artykul): void{
        this._artykuly.push(artykul);
    }

    pobierzTytulyArtykulow(): string[]{
        return this._artykuly.map(a => a.tytul);
    }

    pobierzArtykul(tytul: string): Artykul | undefined{
        return this._artykuly.find(a => a.tytul === tytul);
    }
}