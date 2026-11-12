import { CounterComponent } from "./components/CounterComponent";
import { AutorListComponent } from "./components/AutorListComponent";
import { AutorService } from "./services/AutorService";
import { Tree } from "./structures/Tree";
import { Autor } from "./domain/Autor";
import { Blog } from "./domain/Blog";
import { Artykul } from "./domain/Artykul";
import type { CreateAutorDTO, AutorListItemDTO } from "./domain/DTO/types";

//Zadanie 4: Komponent licznika
const appContainer = document.getElementById("app");
if (appContainer) {
    const counter = new CounterComponent(0);
    counter.mount(appContainer);
}

//Zadanie 5, 6, 7, 12, 13: Asynchroniczne pobieranie, szukanie i prezentacja
async function main() {
    const autorService = new AutorService("/dane.json");
    const autorListComponent = new AutorListComponent();

    if (appContainer) {
        autorListComponent.mount(appContainer);
    }

    try {
        const autorzy = await autorService.pobierzAutorow();

        //Wyświetlenie w interfejsie graficznym (Zadanie 7)
        autorListComponent.render(autorzy);

        //Test wyszukiwania z ?. oraz ?? (Zadanie 13)
        const autorExist = autorzy.length > 0 ? autorService.znajdzAutoraPoId(autorzy, autorzy[0].id) : undefined;
        const wynik1 = autorExist?.pelneImie ?? "Nie znaleziono autora";
        console.log("Wyszukiwanie ID 1:", wynik1);

        const autorNotExist = autorService.znajdzAutoraPoId(autorzy, 999);
        const wynik2 = autorNotExist?.pelneImie ?? "Nie znaleziono autora";
        console.log("Wyszukiwanie ID 999:", wynik2);

    } catch (error) {
        console.error("Błąd podczas pobierania danych:", error);
    }
}

main();

//Zadanie 8: Struktura drzewiasta (DFS / BFS)
const treeKategorie = new Tree<string>("Kategorie");
if (treeKategorie.root) {
    const prog = treeKategorie.root.addChild("Programowanie");
    prog.addChild("TypeScript");
    prog.addChild("JavaScript");

    const design = treeKategorie.root.addChild("Design");
    design.addChild("UI/UX");
}

console.log("DFS (W głąb):");
treeKategorie.traverseDFS((value) => console.log(`Kategoria: ${value}`));

const autorTree = new Tree<Autor>(new Autor("Jan", "Kowalski", "jan@example.com"));
if (autorTree.root) {
    const podAutor1 = autorTree.root.addChild(new Autor("Anna", "Nowak", "anna@example.com"));
    podAutor1.addChild(new Autor("Piotr", "Zieliński", "piotr@example.com"));
}

console.log("BFS (Wszerz):");
autorTree.traverseBFS((autor) => {
    console.log(`Autor w drzewie: ${autor.imie} ${autor.nazwisko}`);
});

//Zadanie 9: Typy DTO
const daneFormularza: CreateAutorDTO = {
    imie: "Jan",
    nazwisko: "Kowalski",
    email: "jan.kowalski@example.com"
};

const nowyAutor = new Autor(
    daneFormularza.imie,
    daneFormularza.nazwisko,
    daneFormularza.email
);

function wyswietlWskaznikAutora(autor: AutorListItemDTO): void {
    console.log(`Autor na liście: ${autor.imie} ${autor.nazwisko}`);
}
wyswietlWskaznikAutora(nowyAutor);

//Zadanie 10: Partial i cząstkowa aktualizacja
const autorDoZmiany = new Autor("Jan", "Kowalski", "jan.kowalski@example.com");
console.log("Przed zmianą:", `${autorDoZmiany.imie} ${autorDoZmiany.nazwisko} (${autorDoZmiany.email})`);

autorDoZmiany.aktualizuj({
    email: "jan.nowy@example.com"
});
console.log("Po zmianie emaila:", `${autorDoZmiany.imie} ${autorDoZmiany.nazwisko} (${autorDoZmiany.email})`);

//Zadanie 14: Dekorator @measureTime (Pomiar czasu wykonania metod)
const blogAutor = new Autor("Jan", "Kowalski", "jan.kowalski@example.com");
const blog = new Blog("Tech Blog", blogAutor);

for (let i = 1; i <= 1000; i++) {
    blog.dodajArtykul(new Artykul(`Artykuł #${i}`, `Treść artykułu numer ${i}...`));
}

console.log("Pobieranie tytułów artykułów (z pomiarem czasu):");
blog.pobierzTytulyArtykulow();

console.log("Wyszukiwanie artykułu (z pomiarem czasu):");
blog.pobierzArtykul("Artykuł #500");