import { Drzewo, LiczenieLisci, RysowanieDrzewa } from "../drzewo/Drzewo";
import { Konsola } from "../ui/Konsola";
import { naglowek, type Zadanie } from "./Zadanie";

interface Plik {
  nazwa: string;
  rozmiarKB: number;
}

export const zadanie08: Zadanie = {
  numer: 8,
  tytul: "Generyczne drzewo (Visitor, Iterator)",
  uruchom(kontener) {
    naglowek(kontener,
      "Drzewo<T> przechowuje wartości dowolnego typu. Odwiedzanie wszystkich węzłów: " +
      "wzorzec Odwiedzający (Visitor) oraz Iterator (for...of). Struktura węzłów to Kompozyt.");
    const k = new Konsola(kontener);

    const drzewo = new Drzewo<Plik>({ nazwa: "projekt", rozmiarKB: 0 });
    const src = drzewo.korzen.dodajDziecko({ nazwa: "src", rozmiarKB: 0 });
    src.dodajDziecko({ nazwa: "main.ts", rozmiarKB: 4 });
    const model = src.dodajDziecko({ nazwa: "model", rozmiarKB: 0 });
    model.dodajDziecko({ nazwa: "Autor.ts", rozmiarKB: 2 });
    model.dodajDziecko({ nazwa: "Blog.ts", rozmiarKB: 2 });
    drzewo.korzen.dodajDziecko({ nazwa: "package.json", rozmiarKB: 1 });
    drzewo.korzen.dodajDziecko({ nazwa: "index.html", rozmiarKB: 1 });

    const rysunek = new RysowanieDrzewa<Plik>(p => p.rozmiarKB ? `${p.nazwa} (${p.rozmiarKB} KB)` : p.nazwa);
    drzewo.odwiedz(rysunek);
    k.log("Visitor - rysowanie drzewa:\n" + rysunek.wynik);

    const liscie = new LiczenieLisci<Plik>();
    drzewo.odwiedz(liscie);
    k.log(`Visitor - liczba liści (plików): ${liscie.liczba}, wszystkich węzłów: ${drzewo.rozmiar}`);

    const wGlab: string[] = [];
    drzewo.przejdzWGlab(p => wGlab.push(p.nazwa));
    k.log("W głąb (DFS):", wGlab.join(" → "));

    const wszerz: string[] = [];
    drzewo.przejdzWszerz((p, glebokosc) => wszerz.push(`${p.nazwa}[${glebokosc}]`));
    k.log("Wszerz (BFS):", wszerz.join(" → "));

    let suma = 0;
    for (const plik of drzewo) suma += plik.rozmiarKB;   // Iterator
    k.log(`Iterator for...of - łączny rozmiar: ${suma} KB`);

    const blog = drzewo.znajdz(p => p.nazwa === "Blog.ts");
    k.log("Ścieżka do Blog.ts:", sciezka(blog).join("/"));

    // Generyki: to samo drzewo dla liczb
    const liczby = new Drzewo<number>(1);
    liczby.korzen.dodajDziecko(2).dodajDziecko(4);
    liczby.korzen.dodajDziecko(3);
    k.log("Drzewo<number>, wartości:", [...liczby]);

    function sciezka(wezel: typeof blog): string[] {
      const czesci: string[] = [];
      for (let w = wezel; w !== undefined; w = w.rodzic) czesci.unshift(w.wartosc.nazwa);
      return czesci;
    }
  },
};
