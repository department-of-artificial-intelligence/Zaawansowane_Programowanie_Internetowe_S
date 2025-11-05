class Artykul
{
    _id: number;
    _tytul: string;
    _tresc: string;
    _dataUtworzenia: Date;
    _komentarze: Komentarz[];

    
get tytul() { return this._tytul; }
get tresc() { return this._tresc; }

constructor(tytul: string,tresc: string)
{
    
    this._tytul=tytul;
    this._tresc=tresc;
    this._dataUtworzenia =new Date;
}
dodajKomentarz(komentarz: Komentarz)
{
    this._komentarze.push(komentarz);
}
pobierzKomentarze():Komentarz[]
{
    return this._komentarze
}
}