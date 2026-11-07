import { Comment } from "./Comment";

export class Article {
    private _id?: number;
    private _title!: string;
    private _content: string;
    private _creationDate: Date;
    private _comments: Comment[] = [];

    constructor(title: string, content: string) {
        this.title = title;
        this._content = content;
        this._creationDate = new Date(); // Ustalenie na aktualn¹ datê przy tworzeniu
    }

    get id(): number | undefined {
        return this._id;
    }

    set id(value: number | undefined) {
        this._id = value;
    }

    get title(): string {
        return this._title;
    }

    set title(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Article title is required and cannot be empty.");
        }
        this._title = value.trim();
    }

    get content(): string {
        return this._content;
    }

    set content(value: string) {
        this._content = value;
    }

    get creationDate(): Date {
        return this._creationDate;
    }

    addComment(comment: Comment): void {
        this._comments.push(comment);
    }

    getComments(): Comment[] {
        return this._comments;
    }
}