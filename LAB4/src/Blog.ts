import { Author } from "./Author"
import { Article } from "./Article"

export class Blog {
    private _id: number
    private _nazwa: string
    private _autor: Author
    private _artykuly: Article[]
    constructor(nazwa: string, autor: Author) {
        this._nazwa = nazwa
    }
    get nazwa() { return this.nazwa }
    set nazwa(nowaNazwa: string) { this._nazwa = nowaNazwa }
    get autor() { return this._autor }
    set autor(nowyAutor: Author) { this._autor = nowyAutor }

    dodajArtykul(artykul: Article): void {
        this._artykuly.push(artykul)
    }
    pobierzTytulyArtykulow(): string[] {
        let tytuly: string[]
        this._artykuly.forEach(a => tytuly.push(a.tytul))
        return tytuly
    }
    pobierzArtykul(tytul: string): Article | undefined {
        const ar = this._artykuly.find(a => a.tytul == tytul)
        if (ar !== undefined && ar !== null) {
            return ar
        }
        console.log("Artykul nie istnieje")
        return undefined
    }

}