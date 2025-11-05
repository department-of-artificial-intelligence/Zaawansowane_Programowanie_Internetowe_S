import {Autor} from "./Autor"
import {Artykul} from "./Artykul"

export class Blog{
    private _id: number;
    private _nazwa: string;
    private _autor: Autor;
    private _artykuly: Artykul[];

    constructor(nazwa:string,autor:Autor){
        if (!nazwa.trim()) throw new Error("Pole nazwa nie może być puste");
        this._nazwa = nazwa;
        this._autor = autor;
    }

    get nazwa():string {
        return this._nazwa;
    }
    set nazwa(value: string) {
        this._nazwa = value;
    }
    get autor():Autor {
        return this._autor;
    }
    set autor(value: Autor) {
        this._autor = value;
    } 

    dodajArtykul(artykul: Artykul): void {
        this._artykuly.push(artykul)
    }
    pobierzTytulyArtykulow(): string[] {
        let tytuly
        this._artykuly.forEach(a => tytuly.push(a.tytul))
        return tytuly
    }
    pobierzArtykul(tytul: string): Artykul | undefined {
        const ar = this._artykuly.find(a => a.tytul == tytul)
        if (ar !== undefined && ar !== null) {
            return ar
        }
        console.log("Artykul nie istnieje")
        return undefined
    }
}