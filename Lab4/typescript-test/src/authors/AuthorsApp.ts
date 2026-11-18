import type { Author } from "../blog/Author";
import type { CreateAuthorDto } from "../blog/types";
import { measureTime } from "../decorators/measureTime";
import { AuthorExporter } from "./AuthorExporter";
import type { AuthorsLoader } from "./AuthorsLoader";
import { HtmlListFormatter } from "./HtmlListFormatter";
import { HtmlTableFormatter } from "./HtmlTableFormatter";
import type { IAuthorFormatter } from "./IAuthorFormatter";

export class AuthorsApp {
    private authors: Record<number, Author> = {};
    private formatters: { name: string, formatter: IAuthorFormatter }[] = [
        { name: "Bulleted list", formatter: new HtmlListFormatter("ul") },
        { name: "Numbered list", formatter: new HtmlListFormatter("ol") },
        { name: "Table", formatter: new HtmlTableFormatter() }
    ];
    private loader: AuthorsLoader;
    private exporter: AuthorExporter;
    private select: HTMLSelectElement;
    private result: HTMLDivElement;

    constructor(parent: HTMLElement, loader: AuthorsLoader) {
        this.loader = loader;
        this.exporter = new AuthorExporter(this.formatters[0].formatter);

        this.select = document.createElement("select");
        this.formatters.forEach((item, index) => {
            const option = document.createElement("option");
            option.value = String(index);
            option.textContent = item.name;
            this.select.appendChild(option);
        });
        this.select.addEventListener("change", () => {
            this.exporter.setFormatter(this.formatters[Number(this.select.value)].formatter);
            this.render();
        });

        this.result = document.createElement("div");

        parent.appendChild(this.select);
        parent.appendChild(this.result);
    }

    start(): Promise<Author[]> {
        return this.loader.load().then(authors => {
            authors.forEach(author => {
                this.authors[author.id] = author;
            });
            this.render();
            return authors;
        });
    }

    findAuthor(id: number): Author | undefined {
        return this.authors[id];
    }

    showAuthor(id: number): void {
        const author = this.findAuthor(id);
        const firstName = author?.firstName ?? "Author not found";
        const lastName = author?.lastName ?? "";
        console.log(`${firstName} ${lastName}`.trim());
    }

    updateAuthor(id: number, changes: Partial<CreateAuthorDto>): void {
        const author = this.findAuthor(id);
        if (author === undefined)
            throw new Error(`Author ${id} not found`);
        Object.assign(author, changes);
        this.render();
    }

    @measureTime
    private render(): void {
        this.result.innerHTML = this.exporter.exportAuthors(Object.values(this.authors));
    }
}
