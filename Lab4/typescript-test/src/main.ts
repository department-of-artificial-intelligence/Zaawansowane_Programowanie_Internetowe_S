import { Author } from "./models/Author";
import { Blog } from "./models/Blog";
import { Article } from "./models/Article";
import {
    HtmlTableAuthorFormatter,
    HtmlListAuthorFormatter
} from "./services/AuthorFormatters";
import { AuthorExporter, AuthorFormatterFactory } from "./services/AuthorExporter";
import { Counter } from "./components/Counter";
import { AuthorService } from "./services/AuthorService";
import { AuthorApp } from "./AuthorApp";
import { Tree, ConsolePrintVisitor, StatisticsVisitor } from "./structures/Tree";
import {
    type CreateAuthorDTO,
    type AuthorListItemDTO,
    type CreateBlogDTO,
    type CreateArticleDTO
} from "./models/dtos";
import { measureTime } from "./decorators/measureTime";

// ==========================================
// TESTY ZADANIA 1 & 2 (Domeny i Walidacja)
// ==========================================
console.log("%c--- TEST POPRAWNYCH DANYCH ---", "color: green; font-weight: bold;");
try {
    const author = new Author("Jan", "Kowalski", "jan.kowalski@pcz.pl");
    const blog = new Blog("Programowanie Internetowe", author);
    const article = new Article("TypeScript - Zadanie 2", "Treœæ artyku³u...");

    blog.addArticle(article);
    console.log("Poprawnie utworzono blog:", blog.name);
    console.log("Autor:", blog.author.firstName, blog.author.lastName);
    console.log("Data utworzenia artyku³u:", article.creationDate.toISOString());
} catch (error) {
    console.error("Niespodziewany b³¹d:", (error as Error).message);
}

console.log("%c--- TESTY B£ÊDNYCH DANYCH ---", "color: red; font-weight: bold;");

// 1. Niepoprawny email
try {
    new Author("Adam", "Nowak", "niepoprawny-email");
} catch (error) {
    console.warn("Przechwycono oczekiwany b³¹d (Email):", (error as Error).message);
}

// 2. Pusta nazwa bloga
try {
    const author = new Author("Anna", "Zieliñska", "anna@example.com");
    new Blog("   ", author);
} catch (error) {
    console.warn("Przechwycono oczekiwany b³¹d (Blog):", (error as Error).message);
}

// 3. Pusty tytu³ artyku³u
try {
    new Article("", "Treœæ bez tytu³u");
} catch (error) {
    console.warn("Przechwycono oczekiwany b³¹d (Article):", (error as Error).message);
}

// ==========================================
// TESTY ZADANIA 3 (Wzorzec Strategia)
// ==========================================
const authors: Author[] = [
    new Author("John", "Doe", "john.doe@example.com"),
    new Author("Alice", "Smith", "alice.smith@domain.com"),
    new Author("Bob", "Martin", "bob.martin@clean-code.com"),
];

const exporter = new AuthorExporter(new HtmlListAuthorFormatter());
const listOutput = exporter.export(authors);
exporter.setFormatter(new HtmlTableAuthorFormatter());
const tableOutput = exporter.export(authors);
exporter.setFormatter(AuthorFormatterFactory.create("json"));
const jsonOutput = exporter.export(authors);

// ==========================================
// SZKIELET WIDOKU DLA ZADAÑ 3, 4, 5, 6 i 7
// ==========================================
const app = document.getElementById("app");
if (app) {
    app.innerHTML = `
    <h1>Zadanie 3 - Author Exporter (Strategy Pattern)</h1>
    <h3>Tabela HTML:</h3>
    ${tableOutput}
    <h3>Lista HTML:</h3>
    ${listOutput}
    <h3>JSON:</h3>
    <pre style="background: #222; color: #fff; padding: 10px; border-radius: 4px;">${jsonOutput}</pre>

    <hr style="margin: 30px 0;" />

    <h1>Zadanie 4 - Obiektowy Licznik (Counter)</h1>
    <div id="counter-root"></div>

    <hr style="margin: 30px 0;" />

    <h1>Zadania 5, 6 i 7 - Asynchroniczne pobieranie i wybór formy wyœwietlania</h1>
    <div id="authors-app-root"></div>
  `;

    // Inicjalizacja Zadania 4
    const counterRoot = document.getElementById("counter-root");
    if (counterRoot) {
        new Counter(counterRoot, 10);
    }

    // Inicjalizacja Zadañ 5, 6 & 7
    const authorsAppRoot = document.getElementById("authors-app-root");
    if (authorsAppRoot) {
        const authorService = new AuthorService("/dane.json");
        const authorApp = new AuthorApp(authorService, authorsAppRoot);
        authorApp.init();
    }
}

// ==========================================
// TESTY ZADANIA 8 (Generyczna struktura Drzewa)
// ==========================================
console.log("%c--- ZADANIE 8: GENERYCZNA STRUKTURA DRZEWA ---", "color: #8b5cf6; font-weight: bold;");

// 1. Test drzewa z obiektami klasy Author (hierarchia redakcyjna)
const editorInChief = new Author("Adam", "Mickiewicz", "adam@litwa.pl");
const editorialTree = new Tree<Author>(editorInChief);

// Dodanie sekcji i podw³adnych autorów
if (editorialTree.root) {
    const fictionEditor = editorialTree.root.addChild(new Author("Juliusz", "S³owacki", "juliusz@slowacki.pl"));
    const scienceEditor = editorialTree.root.addChild(new Author("Stanis³aw", "Lem", "lem@solaris.pl"));

    fictionEditor.addChild(new Author("Henryk", "Sienkiewicz", "henryk@trylogia.pl"));
    scienceEditor.addChild(new Author("Jacek", "Dukaj", "jacek@dukaj.pl"));
}

// Wykorzystanie akcji wy¿szego rzêdu (DFS traversal)
console.log("%cPrzejœcie DFS (W g³¹b):", "font-weight: bold;");
editorialTree.traverseDFS((author, depth) => {
    console.log(`${"-".repeat(depth * 2)}> ${author.firstName} ${author.lastName} (${author.email})`);
});

// Wykorzystanie wzorca Wizytator (Visitor)
console.log("%cWzorzec Wizytator (ConsolePrintVisitor):", "font-weight: bold;");
const printVisitor = new ConsolePrintVisitor<Author>((a) => `${a.firstName} ${a.lastName}`);
editorialTree.accept(printVisitor);

// Zliczenie statystyk struktury
const statsVisitor = new StatisticsVisitor<Author>();
editorialTree.accept(statsVisitor);
console.log(`Liczba wszystkich osób w hierarchii: ${statsVisitor.totalNodes}, Liœcie (bez podw³adnych): ${statsVisitor.leafCount}`);

// Test BFS
console.log("%cPrzejœcie BFS (Wszerz):", "font-weight: bold;");
editorialTree.traverseBFS((author) => {
    console.log(`Wêze³: ${author.firstName} ${author.lastName}`);
});

// ==========================================
// TESTY ZADANIA 9 (Typy narzêdziowe Omit i Pick)
// ==========================================
console.log("%c--- ZADANIE 9: TYPY NARZÊDZIOWE OMIT I PICK ---", "color: #06b6d4; font-weight: bold;");

// 1. Obiekt zgodny z CreateAuthorDTO (bez pola id, getFullName i metody update)
const newAuthorPayload: CreateAuthorDTO = {
    firstName: "Cyprian",
    lastName: "Norwid",
    email: "cyprian@norwid.pl",
};

// Utworzenie w³aœciwej instancji Author na podstawie DTO
const norwid = new Author(
    newAuthorPayload.firstName,
    newAuthorPayload.lastName,
    newAuthorPayload.email
);
console.log("Utworzono autora z DTO:", norwid.firstName, norwid.lastName);

// 2. Obiekt zgodny z AuthorListItemDTO (tylko imiê i nazwisko)
const authorSummary: AuthorListItemDTO = {
    firstName: norwid.firstName,
    lastName: norwid.lastName,
};
console.log("Podsumowanie do listy (Pick):", authorSummary);

// 3. Obiekt zgodny z CreateBlogDTO
const newBlogPayload: CreateBlogDTO = {
    name: "Romantyzm Polski",
    author: norwid,
};
const norwidBlog = new Blog(newBlogPayload.name, newBlogPayload.author);
console.log("Utworzono blog z DTO:", norwidBlog.name);

// 4. Obiekt zgodny z CreateArticleDTO
const newArticlePayload: CreateArticleDTO = {
    title: "Bema pamiêci ¿a³obny-rapsod",
    content: "Czemu, Cieniu, odchodzisz, rêce z³amawszy na pancerz...",
};
const articleNorwid = new Article(newArticlePayload.title, newArticlePayload.content);
norwidBlog.addArticle(articleNorwid);
console.log("Dodano artyku³ z DTO:", articleNorwid.title);

// ==========================================
// TESTY ZADANIA 10 (Typ narzêdziowy Partial)
// ==========================================
console.log("%c--- ZADANIE 10: CZÊŒCIOWA AKTUALIZACJA (PARTIAL) ---", "color: #ec4899; font-weight: bold;");

const testAuthor = new Author("Boles³aw", "Prus", "boleslaw.prus@lalka.pl");
testAuthor.id = 10;
console.log("Stan pocz¹tkowy:", `${testAuthor.firstName} ${testAuthor.lastName} <${testAuthor.email}>`);

// 1. Aktualizacja samego adresu e-mail (bez podawania imienia i nazwiska)
testAuthor.update({ email: "prus.nowy@poczta.pl" });
console.log("Po zmianie samego emaila:", testAuthor.email);

// 2. Aktualizacja tylko imienia i nazwiska (bez ruszania emaila)
testAuthor.update({ firstName: "Aleksander", lastName: "G³owacki" });
console.log("Po zmianie personaliów:", `${testAuthor.firstName} ${testAuthor.lastName} <${testAuthor.email}>`);

// 3. Weryfikacja, czy walidacja z Zadania 2 nadal chroni obiekt przed b³êdnymi danymi
try {
    testAuthor.update({ email: "zly-email-bez-malpy" });
} catch (error) {
    console.warn("Przechwycono b³¹d walidacji podczas update:", (error as Error).message);
}

// ==========================================
// KLASA DLA ZADANIA 14 (Dekorator na poziomie pliku)
// ==========================================
class MathHeavyTask {
    @measureTime
    public computeSum(count: number): number {
        let total = 0;
        for (let i = 0; i < count; i++) {
            total += Math.sqrt(i);
        }
        return total;
    }
}

// ==========================================
// TESTY ASYNCHRONICZNE: ZADANIA 11, 12, 13 i 14
// ==========================================
void (async () => {
    // --- ZADANIE 11: STRUKTURA RECORD ---
    console.log("%c--- ZADANIE 11: STRUKTURA RECORD I WYSZUKIWANIE ---", "color: #eab308; font-weight: bold;");

    const authorServiceTest = new AuthorService("/dane.json");
    const authorsRecord = await authorServiceTest.fetchAuthorsRecord();

    console.log("Struktura Record<number, Author>:", authorsRecord);

    // Wyszukiwanie O(1) po kluczu
    const targetId = 3;
    const foundAuthor = authorsRecord[targetId];

    if (foundAuthor) {
        console.log(
            `%cZnaleziono autora o ID = ${targetId} w czasie O(1):`,
            "color: green; font-weight: bold;",
            `${foundAuthor.firstName} ${foundAuthor.lastName} (${foundAuthor.email})`
        );
    } else {
        console.warn(`Nie znaleziono autora o ID = ${targetId}`);
    }

    console.log("Dostêpne identyfikatory w rekordzie:", Object.keys(authorsRecord));

    // --- ZADANIE 12: READONLY ---
    console.log("%c--- ZADANIE 12: TYP NARZÊDZIOWY READONLY ---", "color: #ef4444; font-weight: bold;");

    const originalAuthor = new Author("Stefan", "¯eromski", "stefan@zeromski.pl");
    const readonlyAuthor: Readonly<Author> = originalAuthor;
    const readonlyAuthorsList: readonly Readonly<Author>[] = [readonlyAuthor];

    console.log("Pomyœlnie odczytano dane Readonly:", readonlyAuthor.firstName, readonlyAuthor.email);
    console.log("D³ugoœæ zabezpieczonej listy autorów (Readonly):", readonlyAuthorsList.length);

    // --- ZADANIE 13: OPERATORY ?. ORAZ ?? ---
    console.log("%c--- ZADANIE 13: OPERATORY ?. ORAZ ?? ---", "color: #0ea5e9; font-weight: bold;");

    const findAuthorById = (record: Record<number, Author>, id: number): Author | undefined => {
        return record[id];
    };

    const authorExists = findAuthorById(authorsRecord, 2);
    const displayExisting = authorExists?.getFullName() ?? "Nie znaleziono autora";
    console.log("Wynik wyszukiwania dla ID = 2:", displayExisting);

    const authorMissing = findAuthorById(authorsRecord, 99);
    const displayMissing = authorMissing?.getFullName() ?? "Nie znaleziono autora";
    console.log("Wynik wyszukiwania dla ID = 99:", displayMissing);

    // --- ZADANIE 14: DEKORATOR MEASURETIME ---
    console.log("%c--- ZADANIE 14: DEKORATOR METODY MEASURETIME ---", "color: #14b8a6; font-weight: bold;");

    const task = new MathHeavyTask();
    const sumResult = task.computeSum(1_000_000);
    console.log("Wynik obliczeñ (nienaruszony):", Math.round(sumResult));
})();