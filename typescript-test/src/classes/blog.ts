import { Article } from "./article";
import { Author } from "./author";
export class Blog {
  private static _idCounter: number = 1;
  private _id: number;
  private _name!: string;
  private _author!: Author;
  private _articles: Article[] = [];

  constructor(name: string, author: Author) {
    this._id = Blog._idCounter++;
    this.name = name;
    this.author = author;
  }
  get id(): number {
    return this._id;
  }
  get name(): string {
    return this._name;
  }
  set name(value: string) {
    if (!value || value.trim().length === 0) {
      throw new Error("Nazwa bloga jest wymagana");
    }
    this._name = value.trim();
  }
  get author(): Author {
    return this._author;
  }
  set author(value: Author) {
    if (!value) {
      throw new Error("Author jest wymagany");
    }
    this._author = value;
  }

  addArticle(article: Article) {
    this._articles.push(article);
  }
  getArticlesTitles(): string[] {
    return this._articles.map((article) => article.title);
  }
  getArticle(title: string): Article | undefined {
    return this._articles.find((article) => article.title === title);
  }
}
