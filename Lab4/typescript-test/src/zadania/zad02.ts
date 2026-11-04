import { Artykul } from "../model/Artykul";
import { Autor } from "../model/Autor";
import { Blog } from "../model/Blog";
import { Konsola } from "../ui/Konsola";
import { naglowek, type Zadanie } from "./Zadanie";

export const zadanie02: Zadanie = {
  numer: 2,
  tytul: "Walidacja obiektów",
  uruchom(kontener) {
    naglowek(kontener,
      "Walidacja w setterach, wywoływanych także z konstruktora. Niepoprawnego obiektu " +
      "nie da się utworzyć ani później zepsuć. Inne możliwości opisuje README.md.");
    const k = new Konsola(kontener);

    const proba = (opis: string, akcja: () => unknown) => {
      try {
        akcja();
        k.log(`✓ ${opis}`);
      } catch (blad) {
        k.blad(`${opis} -> ${(blad as Error).message}`);
      }
    };

    proba("Poprawny autor", () => new Autor("Ala", "Kot", "ala@kot.pl"));
    proba("Autor bez imienia", () => new Autor("", "Kot", "ala@kot.pl"));
    proba("Autor z imieniem ze spacji", () => new Autor("   ", "Kot", "ala@kot.pl"));
    proba("Autor bez e-maila", () => new Autor("Ala", "Kot", ""));
    proba("Autor z błędnym e-mailem", () => new Autor("Ala", "Kot", "ala.kot.pl"));

    const autor = new Autor("Ala", "Kot", "ala@kot.pl");
    proba("Zmiana e-maila na błędny po utworzeniu", () => { autor.email = "zly@"; });
    k.log("  e-mail po nieudanej zmianie:", autor.email);

    proba("Blog z pustą nazwą", () => new Blog("", autor));
    // "as unknown as Autor" oszukuje kompilator - tak jak dane z JavaScriptu albo z serwera
    proba("Blog bez autora", () => new Blog("Mój blog", null as unknown as Autor));
    proba("Poprawny blog", () => new Blog("Mój blog", autor));

    proba("Artykuł z pustym tytułem", () => new Artykul("", "treść"));
    const artykul = new Artykul("Tytuł", "treść");
    k.log("Data utworzenia artykułu (zawsze bieżąca):", artykul.dataUtworzenia);
    // @ts-expect-error - dataUtworzenia ma tylko getter, kompilator nie pozwoli jej zmienić
    proba("Zmiana daty utworzenia", () => { artykul.dataUtworzenia = new Date(2000, 0, 1); });
  },
};
