import { Author } from "./models/Author";
import { Blog } from "./models/Blog";
import { Article } from "./models/Article";
import { Comment } from "./models/Comment";
import { AuthorExporter } from "./formatters/AuthorExporter";
import { FormatterFactory, type FormatType } from "./formatters/FormatterFactory";
import { Counter } from "./counter/Counter";
import { CounterView } from "./counter/CounterView";
import { AuthorsApp } from "./authors/AuthorsApp";
import { Tree } from "./tree/Tree";
import { PrintVisitor } from "./tree/PrintVisitor";
import { LeafCollectorVisitor } from "./tree/LeafCollectorVisitor";
import "./style.css";

// page layout for tasks 4-7 (index.html only has <div id="app">)
document.querySelector<HTMLDivElement>("#app")!.innerHTML = `
    <h2>Zadanie 4 - licznik</h2>
    <div id="counter"></div>
    <h2>Zadania 5-7 - autorzy</h2>
    <div id="authors"></div>
`;

// ---------- Zadanie 1 ----------
console.log("=== Zadanie 1 ===");
const author = new Author("Jan", "Kowalski", "jan.kowalski@example.com");
const blog = new Blog("Blog o TypeScript", author);

const article = new Article("Wprowadzenie", "TypeScript jest nadzbiorem JavaScript.");
const comment = new Comment("Świetny artykuł!", "ala");
comment.addReply(new Comment("Dzięki!", "jan"));
article.addComment(comment);

blog.addArticle(article);
blog.addArticle(new Article("Dekoratory", "Dekoratory to specjalne funkcje..."));

console.log("Blog:", blog.name, "autor:", blog.author.toString());
console.log("Tytuły:", blog.getArticleTitles());
console.log("Odpowiedzi:", blog.getArticle("Wprowadzenie")?.getComments()[0].getReplies());

// ---------- Zadanie 2 ----------
console.log("=== Zadanie 2 ===");
function tryCreate(description: string, create: () => unknown): void {
    try {
        create();
        console.log(`OK: ${description}`);
    } catch (e) {
        console.log(`Błąd (${description}): ${(e as Error).message}`);
    }
}
tryCreate("puste imię", () => new Author("", "Nowak", "a@b.pl"));
tryCreate("zły e-mail", () => new Author("Anna", "Nowak", "anna@"));
tryCreate("pusta nazwa bloga", () => new Blog("", author));
tryCreate("blog bez autora", () => new Blog("Blog", null as unknown as Author));
tryCreate("pusty tytuł", () => new Article("", "treść"));
tryCreate("zmiana e-maila na zły", () => { author.email = "zly-adres"; });
console.log("Data utworzenia artykułu:", article.createdAt);

// ---------- Zadanie 3 ----------
console.log("=== Zadanie 3 ===");
const authors = [
    author,
    new Author("Anna", "Nowak", "anna.nowak@example.com")
];
const exporter = new AuthorExporter(FormatterFactory.create("table"));
const formats: FormatType[] = ["table", "ul", "ol", "json", "csv"];
for (const format of formats) {
    exporter.setFormatter(FormatterFactory.create(format));
    console.log(`--- ${format} ---\n` + exporter.export(authors));
}

// ---------- Zadanie 4 ----------
new CounterView(new Counter(), document.getElementById("counter")!);

// ---------- Zadania 5, 6, 7 ----------
new AuthorsApp(document.getElementById("authors")!, "dane.json").start();

// ---------- Zadanie 8 ----------
console.log("=== Zadanie 8 ===");
const tree = new Tree<string>("src");
const models = tree.root.addChild("models");
models.addChild("Author.ts");
models.addChild("Blog.ts");
const treeFolder = tree.root.addChild("tree");
treeFolder.addChild("Tree.ts");
treeFolder.addChild("TreeNode.ts");
tree.root.addChild("index.ts");

const printer = new PrintVisitor<string>();
tree.accept(printer);
console.log(printer.getResult());

const leafCollector = new LeafCollectorVisitor<string>();
tree.accept(leafCollector);
console.log("Liście:", leafCollector.leaves);

const depthFirst: string[] = [];
tree.traverseDepthFirst(node => depthFirst.push(node.value));
console.log("W głąb:", depthFirst.join(", "));

const breadthFirst: string[] = [];
tree.traverseBreadthFirst(node => breadthFirst.push(node.value));
console.log("Wszerz:", breadthFirst.join(", "));

console.log("Iterator:", [...tree].join(", "), "| liczba węzłów:", tree.size);
console.log("Znaleziono:", tree.find(v => v.startsWith("Tree"))?.value);

// a tree storing objects of a programmer-defined type
const authorTree = new Tree<Author>(author);
authorTree.root.addChild(authors[1]);
const authorPrinter = new PrintVisitor<Author>(a => `${a.firstName} ${a.lastName}`);
authorTree.accept(authorPrinter);
console.log(authorPrinter.getResult());
