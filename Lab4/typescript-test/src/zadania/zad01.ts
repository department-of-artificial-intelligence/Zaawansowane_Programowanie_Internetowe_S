import { Artykul } from "../model/Artykul";
import { Autor } from "../model/Autor";
import { Blog } from "../model/Blog";
import { Komentarz } from "../model/Komentarz";
import { Konsola } from "../ui/Konsola";
import { naglowek, type Zadanie } from "./Zadanie";

export const zadanie01: Zadanie = {
  numer: 1,
  tytul: "Klasy z diagramu",
  uruchom(kontener) {
    naglowek(kontener, "Klasy Blog, Artykul, Komentarz i Autor (każda w osobnym pliku w src/model).");
    const k = new Konsola(kontener);

    const autor = new Autor("Ala", "Kowalska", "ala@przyklad.pl");
    const blog = new Blog("Blog o TypeScript", autor);

    const artykul1 = new Artykul("Typy w TypeScript", "string, number, boolean...");
    const artykul2 = new Artykul("Klasy i interfejsy", "class, interface, implements...");
    blog.dodajArtykul(artykul1);
    blog.dodajArtykul(artykul2);

    const komentarz = new Komentarz("Świetny artykuł!", "czytelnik1");
    komentarz.dodajOdpowiedz(new Komentarz("Dziękuję :)", "ala"));
    artykul1.dodajKomentarz(komentarz);

    k.log(`Blog #${blog.id}: "${blog.nazwa}", autor: ${blog.autor}`);
    k.log("Tytuły artykułów:", blog.pobierzTytulyArtykulow());

    const znaleziony = blog.pobierzArtykul("Typy w TypeScript");
    k.log(`Artykuł #${znaleziony?.id}: "${znaleziony?.tytul}", utworzony:`, znaleziony?.dataUtworzenia);
    for (const kom of znaleziony?.pobierzKomentarze() ?? []) {
      k.log(`  ${kom.nick}: ${kom.tresc}`);
      for (const odp of kom.pobierzOdpowiedzi()) {
        k.log(`    ↳ ${odp.nick}: ${odp.tresc}`);
      }
    }
    k.log("Artykuł, którego nie ma:", blog.pobierzArtykul("Nie istnieje") ?? "undefined");
  },
};
