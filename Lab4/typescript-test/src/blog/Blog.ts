import type { Article } from "./Article";
import type { Author } from "./Author";

export class Blog {
    private static nextId: number = 1;

    private _id: number = Blog.nextId++;
    private _name!: string;
    private _author!: Author;
    private _articles: Article[] = [];

    constructor(name: string, author: Author) {
        this.name = name;
        this.author = author;
    }

    get id(): number { return this._id; }

    get name(): string { return this._name; }
    set name(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Blog name cannot be empty");
        this._name = value;
    }

    get author(): Author { return this._author; }
    set author(value: Author) {
        if (value === null || value === undefined)
            throw new Error("Blog author is required");
        this._author = value;
    }

    addArticle(article: Article): void {
        this._articles.push(article);
    }

    getArticleTitles(): string[] {
        return this._articles.map(article => article.title);
    }

    getArticle(title: string): Article | undefined {
        return this._articles.find(article => article.title === title);
    }
}
