export class Article {
  private static _idCounter: number = 1;
  private _id: number;
  private _title!: string;
  private _content: string;
  private _creationDate: Date;
  private _comments: Comment[] = [];

  constructor(title: string, content: string) {
    this._id = Article._idCounter++;
    this.title = title;
    this._content = content;
    this._creationDate = new Date();
  }
  get id(): number {
    return this._id;
  }
  get title(): string {
    return this._title;
  }
  set title(value: string) {
    if (!value || value.trim().length === 0) {
      throw new Error("Tytuł jest wymagany");
    }
    this._title = value;
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

  addComment(comment: Comment) {
    this._comments.push(comment);
  }
  getComment() {
    return this._comments;
  }
}
