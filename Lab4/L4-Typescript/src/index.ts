import { Artykul } from "./blog/Artykul";
import { Autor } from "./blog/Autor";
import { Blog } from "./blog/Blog";
import { Komentarz } from "./blog/Komentarz";
import { AplikacjaAutorow } from "./autorzy/AplikacjaAutorow";
import { EksporterAutorow } from "./autorzy/EksporterAutorow";
import { FormatterJson } from "./autorzy/FormatterJson";
import { FormatterListyHtml } from "./autorzy/FormatterListyHtml";
import { FormatterTabeliHtml } from "./autorzy/FormatterTabeliHtml";
import { PobieraczAutorow } from "./autorzy/PobieraczAutorow";
import { Drzewo } from "./drzewo/Drzewo";
import { SumujacyOdwiedzajacy } from "./drzewo/SumujacyOdwiedzajacy";
import { Licznik } from "./licznik/Licznik";

function sprobuj(opis: string, tworzenie: () => unknown): void {
    try {
        tworzenie();
        console.log(opis + ": utworzono");
    } catch (blad) {
        console.log(opis + ": " + (blad as Error).message);
    }
}

function zadania1i2(): void {
    const autor = new Autor("Ala", "Kot", "ala.kot@przyklad.pl");
    const blog = new Blog("Blog o TypeScript", autor);
    const artykul = new Artykul("Pierwszy wpis", "Treść pierwszego wpisu");
    const komentarz = new Komentarz("Świetny wpis", "czytelnik");
    komentarz.dodajOdpowiedz(new Komentarz("Dziękuję", "ala"));
    artykul.dodajKomentarz(komentarz);
    blog.dodajArtykul(artykul);

    console.log(blog.pobierzTytulyArtykulow());
    console.log(blog.pobierzArtykul("Pierwszy wpis")?.pobierzKomentarze().length);

    sprobuj("Autor bez imienia", () => new Autor("", "Kot", "ala.kot@przyklad.pl"));
    sprobuj("Autor bez nazwiska", () => new Autor("Ala", "", "ala.kot@przyklad.pl"));
    sprobuj("Autor bez emaila", () => new Autor("Ala", "Kot", ""));
    sprobuj("Autor z błędnym emailem", () => new Autor("Ala", "Kot", "ala.kot"));
    sprobuj("Blog bez nazwy", () => new Blog("", autor));
    sprobuj("Blog bez autora", () => new Blog("Blog", undefined as unknown as Autor));
    sprobuj("Artykuł bez tytułu", () => new Artykul("", "Treść"));
}

function zadanie3(): void {
    const autorzy = [
        new Autor("Anna", "Nowak", "anna.nowak@przyklad.pl"),
        new Autor("Jan", "Kowalski", "jan.kowalski@przyklad.pl")
    ];
    const eksporter = new EksporterAutorow(new FormatterTabeliHtml());
    console.log(eksporter.eksportuj(autorzy));
    eksporter.ustawFormatter(new FormatterListyHtml("ul"));
    console.log(eksporter.eksportuj(autorzy));
    eksporter.ustawFormatter(new FormatterJson());
    console.log(eksporter.eksportuj(autorzy));
}

function zadanie8(): void {
    const katalogi = new Drzewo<string>("Dokumenty");
    const prace = katalogi.korzen.dodajDziecko("Prace");
    prace.dodajDziecko("Lab1");
    prace.dodajDziecko("Lab4");
    katalogi.korzen.dodajDziecko("Zdjęcia").dodajDziecko("Wakacje");
    katalogi.doKazdego((wartosc, poziom) => console.log("  ".repeat(poziom) + wartosc));

    const liczby = new Drzewo<number>(1);
    liczby.korzen.dodajDziecko(2).dodajDziecko(4);
    liczby.korzen.dodajDziecko(3);
    const sumujacy = new SumujacyOdwiedzajacy();
    liczby.przyjmij(sumujacy);
    console.log("Suma wartości w drzewie: " + sumujacy.suma);
}

function sekcja(tytul: string): HTMLElement {
    const element = document.createElement("div");
    const naglowek = document.createElement("h2");
    naglowek.textContent = tytul;
    element.appendChild(naglowek);
    document.body.appendChild(element);
    return element;
}

zadania1i2();
zadanie3();
zadanie8();

new Licznik(sekcja("Licznik"));
new AplikacjaAutorow(sekcja("Autorzy"), new PobieraczAutorow("dane.json"));
