import { Autor } from "./Autor";
import { Blog } from "./Blog";
import { Artykul } from "./Artykul";
import { Komentarz } from "./Komentarz";

const autor = new Autor("Jan", "Kowal");
const blog = new Blog("Blog EZ", autor);

const artykul = new Artykul("Nowy artykul", "Opis asdasd");
blog.dodajArtykul(artykul);

const komentarz = new Komentarz("artykul", "user123");
artykul.dodajKomentarz(komentarz);

console.log(artykul);

console.log(blog.pobierzTytulyArtykulow());
console.log(artykul.pobierzKomentarze());


