import { Author } from "./Author";
import { Article } from "./Article";

export class Blog {
    private _id?: number;
    private _name!: string;
    private _author!: Author;
    private _articles: Article[] = [];

    constructor(name: string, author: Author) {
        this.name = name;
        this.author = author;
    }

    get id(): number | undefined {
        return this._id;
    }

    set id(value: number | undefined) {
        this._id = value;
    }

    get name(): string {
        return this._name;
    }

    set name(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Blog name is required and cannot be empty.");
        }
        this._name = value.trim();
    }

    get author(): Author {
        return this._author;
    }

    set author(value: Author) {
        if (!value || !(value instanceof Author)) {
            throw new Error("A valid Author must be provided for the blog.");
        }
        this._author = value;
    }

    addArticle(article: Article): void {
        if (!article) {
            throw new Error("Cannot add an empty article.");
        }
        this._articles.push(article);
    }

    getArticleTitles(): string[] {
        return this._articles.map((article) => article.title);
    }

    getArticle(title: string): Article | undefined {
        return this._articles.find((article) => article.title === title);
    }
}