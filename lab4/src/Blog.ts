class Blog
{
_id: number
_nazwa: string
_autor: Autor
_artykuly: Artykul[]

age: number;


get nazwa() { return this._nazwa; }
get autor() { return this._autor; }


constructor(nazwa: string, autor: Autor)
{
    this._nazwa=nazwa;
    this._autor=autor;
}

dodajArtykul( artykul: Artykul)
{
    this._artykuly.push(artykul);
}
pobierzTytulyArtykulow():string[]
{
return this._artykuly.map((a)=>a.tytul) }

pobierzArtykul(tytul:string)
{
let znalezionyArtykul = this._artykuly.find(artykul => artykul.tytul === tytul);
  
    console.log(znalezionyArtykul)
    

}

}