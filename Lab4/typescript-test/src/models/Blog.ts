import { Author } from "./Author";
import { Article } from "./Article";
import { validate } from "../validation/validate";
import { required } from "../validation/rules";

export class Blog {
    private static nextId = 1;

    private _id: number;
    private _name!: string;
    private _author!: Author;
    private _articles: Article[] = [];

    constructor(name: string, author: Author) {
        this.name = name;
        this.author = author;
        this._id = Blog.nextId++; // only after successful validation
    }

    get id(): number { return this._id; }

    @validate(required("Nazwa bloga nie może być pusta"))
    set name(value: string) { this._name = value; }
    get name(): string { return this._name; }

    @validate(required("Autor bloga musi być podany"))
    set author(value: Author) { this._author = value; }
    get author(): Author { return this._author; }

    addArticle(article: Article): void {
        this._articles.push(article);
    }

    getArticleTitles(): string[] {
        return this._articles.map(a => a.title);
    }

    getArticle(title: string): Article | undefined {
        return this._articles.find(a => a.title === title);
    }
}
