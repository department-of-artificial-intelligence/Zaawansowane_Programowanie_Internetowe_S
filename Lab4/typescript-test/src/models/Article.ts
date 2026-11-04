import { Comment } from "./Comment";
import { validate } from "../validation/validate";
import { required } from "../validation/rules";

export class Article {
    private static nextId = 1;

    private _id: number;
    private _title!: string;
    private _content: string;
    private readonly _createdAt: Date;
    private _comments: Comment[] = [];

    constructor(title: string, content: string) {
        this.title = title;
        this._content = content;
        this._createdAt = new Date(); // always the current date, no setter
        this._id = Article.nextId++; // only after successful validation
    }

    get id(): number { return this._id; }

    @validate(required("Tytuł artykułu nie może być pusty"))
    set title(value: string) { this._title = value; }
    get title(): string { return this._title; }

    get content(): string { return this._content; }
    set content(value: string) { this._content = value; }

    get createdAt(): Date { return new Date(this._createdAt); }

    addComment(comment: Comment): void {
        this._comments.push(comment);
    }

    getComments(): Comment[] {
        return [...this._comments];
    }
}
