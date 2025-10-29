import { Komentarz } from "./Komentarz"

export class Artykul{
    private _id : number
    private _tytul : string
    private _tresc : string
    private _dataUtorzenia : Date
    private _komentarze : Komentarz[]
}