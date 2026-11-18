import type { Comment } from "./Comment";

export class Article {
    private static nextId: number = 1;

    private _id: number = Article.nextId++;
    private _title!: string;
    private _content: string;
    private _creationDate: Date = new Date();
    private _comments: Comment[] = [];

    constructor(title: string, content: string) {
        this.title = title;
        this._content = content;
    }

    get id(): number { return this._id; }

    get creationDate(): Date { return this._creationDate; }

    get title(): string { return this._title; }
    set title(value: string) {
        if (value === null || value === undefined || value.trim().length === 0)
            throw new Error("Article title cannot be empty");
        this._title = value;
    }

    get content(): string { return this._content; }
    set content(value: string) { this._content = value; }

    addComment(comment: Comment): void {
        this._comments.push(comment);
    }

    getComments(): Comment[] {
        return this._comments;
    }
}
