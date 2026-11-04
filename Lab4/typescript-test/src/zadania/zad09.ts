import { Autor } from "../model/Autor";
import {
  utworzArtykul, utworzAutora, utworzBlog,
  type AutorNaLiscie, type NowyAutor,
} from "../model/dto";
import { Konsola } from "../ui/Konsola";
import { naglowek, type Zadanie } from "./Zadanie";

export const zadanie09: Zadanie = {
  numer: 9,
  tytul: "DTO przez Omit i Pick",
  uruchom(kontener) {
    naglowek(kontener,
      "Typy NowyAutor, NowyBlog, NowyArtykul (Omit) i AutorNaLiscie (Pick) w src/model/dto.ts. " +
      "Są wyprowadzone z klas, więc zmiana klasy automatycznie zmienia DTO (patrz README.md).");
    const k = new Konsola(kontener);

    const dto: NowyAutor = { imie: "Ala", nazwisko: "Kowalska", email: "ala@przyklad.pl" };
    const autor = utworzAutora(dto);
    const blog = utworzBlog({ nazwa: "Blog Ali", autor });
    const artykul = utworzArtykul({ tytul: "Pierwszy wpis", tresc: "Witajcie!" });
    blog.dodajArtykul(artykul);
    k.log(`Utworzono: autor #${autor.id}, blog "${blog.nazwa}", artykuł "${artykul.tytul}"`);

    // @ts-expect-error - DTO do tworzenia nie ma identyfikatora (nadaje go aplikacja)
    utworzAutora({ id: 5, imie: "X", nazwisko: "Y", email: "x@y.pl" });

    // obiekt klasy Autor pasuje do AutorNaLiscie (typowanie strukturalne)
    const lista: AutorNaLiscie[] = [autor, new Autor("Jan", "Nowak", "jan@przyklad.pl")];
    k.log("Autorzy na liście:", lista.map(a => `${a.imie} ${a.nazwisko}`));
  },
};
