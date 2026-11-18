import { Article } from "./blog/Article";
import { Author } from "./blog/Author";
import { Blog } from "./blog/Blog";
import { Comment } from "./blog/Comment";
import { AuthorExporter } from "./authors/AuthorExporter";
import { AuthorsApp } from "./authors/AuthorsApp";
import { AuthorsLoader } from "./authors/AuthorsLoader";
import { HtmlListFormatter } from "./authors/HtmlListFormatter";
import { HtmlTableFormatter } from "./authors/HtmlTableFormatter";
import { JsonFormatter } from "./authors/JsonFormatter";
import { Counter } from "./counter/Counter";
import { Tree } from "./tree/Tree";

function attempt(description: string, create: () => unknown): void {
    try {
        create();
        console.log(description + ": created");
    } catch (error) {
        console.log(description + ": " + (error as Error).message);
    }
}

function blogDemo(): void {
    const author = new Author("Ala", "Kot", "ala.kot@example.com");
    const blog = new Blog("TypeScript blog", author);
    const article = new Article("First post", "Content of the first post");
    const comment = new Comment("Great post", "reader");
    comment.addReply(new Comment("Thank you", "ala"));
    article.addComment(comment);
    blog.addArticle(article);

    console.log(blog.getArticleTitles());
    console.log(blog.getArticle("First post")?.getComments().length);

    attempt("Author without first name", () => new Author("", "Kot", "ala.kot@example.com"));
    attempt("Author without last name", () => new Author("Ala", "", "ala.kot@example.com"));
    attempt("Author without email", () => new Author("Ala", "Kot", ""));
    attempt("Author with invalid email", () => new Author("Ala", "Kot", "ala.kot"));
    attempt("Blog without name", () => new Blog("", author));
    attempt("Blog without author", () => new Blog("Blog", undefined as unknown as Author));
    attempt("Article without title", () => new Article("", "Content"));
}

function exporterDemo(): void {
    const authors = [
        new Author("Anna", "Nowak", "anna.nowak@example.com"),
        new Author("Jan", "Kowalski", "jan.kowalski@example.com")
    ];
    const exporter = new AuthorExporter(new HtmlTableFormatter());
    console.log(exporter.exportAuthors(authors));
    exporter.setFormatter(new HtmlListFormatter("ul"));
    console.log(exporter.exportAuthors(authors));
    exporter.setFormatter(new JsonFormatter());
    console.log(exporter.exportAuthors(authors));
}

function treeDemo(): void {
    const folders = new Tree<string>("Documents");
    const work = folders.root.addChild("Work");
    work.addChild("Lab1");
    work.addChild("Lab4");
    folders.root.addChild("Photos").addChild("Holidays");
    console.log(folders.root.value);
    console.log(folders.root.children.map(child => child.value));
    console.log(work.children.map(child => child.value));

    const numbers = new Tree<number>(1);
    numbers.root.addChild(2).addChild(4);
    numbers.root.addChild(3);
    console.log(numbers.root.children.map(child => child.value));
}

function section(title: string): HTMLElement {
    const element = document.createElement("div");
    const heading = document.createElement("h2");
    heading.textContent = title;
    element.appendChild(heading);
    document.querySelector<HTMLDivElement>("#app")!.appendChild(element);
    return element;
}

blogDemo();
exporterDemo();
treeDemo();

new Counter(section("Counter"));

const authorsApp = new AuthorsApp(section("Authors"), new AuthorsLoader("dane.json"));
authorsApp.start().then(authors => {
    authorsApp.showAuthor(authors[0].id);
    authorsApp.showAuthor(0);
    authorsApp.updateAuthor(authors[0].id, { email: "anna.new@example.com" });
});
