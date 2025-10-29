import { Coment } from "./Coment"
export class Article {
    private _id: number
    private _tytul: string
    private _tresc: string
    private _dataUtworzenia: Date
    private _komentarze: Coment[]

    constructor(tytul: string, tresc: string) {
        this._tytul = tytul
        this._tresc = tresc
    }

    get tytul() { return this._tytul }
    set tytul(nowyTytul: string) { this._tytul = nowyTytul }
    get tresc() { return this._tresc }
    set tresc(nowaTresc: string) { this._tresc = nowaTresc }

    dodajKomentarz(koemntarz: Coment): void {
        this._komentarze.push(koemntarz)
    }

    pobierzKomentarze(): Coment[] {
        let com: Coment[]
        this._komentarze.forEach(c => com.push(c))
        return com
    }
}