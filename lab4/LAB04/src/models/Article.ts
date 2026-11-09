import { Comment } from "./Comment";

export class Article {
  private _id: number;
  private _title: string;
  private _content: string;
  private _createdAt: Date;
  private _comments: Comment[];

  constructor(id: number, title: string, content: string) {
    if (!title.trim()) {
      throw new Error("Article title cannot be empty");
    }

    this._id = id;
    this._title = title;
    this._content = content;
    this._createdAt = new Date();
    this._comments = [];
  }

  get title(): string {
    return this._title;
  }

  set title(value: string) {
    this._title = value;
  }

  get content(): string {
    return this._content;
  }

  set content(value: string) {
    this._content = value;
  }

  addComment(comment: Comment): void {
    this._comments.push(comment);
  }

  getComments(): Comment[] {
    return this._comments;
  }
}