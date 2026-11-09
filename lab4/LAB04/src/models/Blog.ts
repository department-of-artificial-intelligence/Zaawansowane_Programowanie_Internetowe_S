import { Author } from "./Author";
import { Article } from "./Article";

export class Blog {
  private _id: number;
  private _name: string;
  private _author: Author;
  private _articles: Article[];

  constructor(id: number, name: string, author: Author) {
    if (!name.trim()) {
      throw new Error("Blog name cannot be empty");
    }

    if (!author) {
      throw new Error("Author is required");
    }
    this._id = id;
    this._name = name;
    this._author = author;
    this._articles = [];
  }

  get name(): string {
    return this._name;
  }

  set name(value: string) {
    this._name = value;
  }

  get author(): Author {
    return this._author;
  }

  set author(value: Author) {
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