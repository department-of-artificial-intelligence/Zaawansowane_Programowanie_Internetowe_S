import { Author } from "../classes/author";
import type { IAuthor } from "../interfaces/IAuthor";
import type { AuthorDto, AuthorsById } from "../types/DomainTypes";

export class AuthorService {
  private _url: string;
  private _authors: AuthorsById = {};

  constructor(url: string) {
    this._url = url;
  }

  get authors(): AuthorsById {
    return this._authors;
  }

  public addAuthor(author: Author): void {
    this._authors[author.id] = author;
  }

  public getAuthorById(id: number): Author | undefined {
    return this._authors[id];
  }
  public getReadonlyAuthors(): readonly Readonly<Author>[] {
    return Object.values(this._authors);
  }
  public fetchAuthors(): Promise<AuthorsById> {
    return fetch(this._url)
      .then((res) => {
        if (!res.ok) {
          throw new Error(`Błąd pobierania danych: ${res.status}`);
        }
        return res.json() as Promise<IAuthor[]>;
      })
      .then((data) => {
        this._authors = {};
        data.forEach((item) => {
          const author = new Author(item.firstName, item.lastName, item.email);
          this._authors[author.id] = author;
        });
        return this._authors;
      });
  }
  public updateAuthor(id: number, changes: Partial<AuthorDto>): Author {
    const author = this._authors[id];

    if (!author) {
      throw new Error(`Nie znaleziono autora o ID: ${id}`);
    }

    if (changes.firstName !== undefined) {
      author.firstName = changes.firstName;
    }
    if (changes.lastName !== undefined) {
      author.lastName = changes.lastName;
    }
    if (changes.email !== undefined) {
      author.email = changes.email;
    }

    return author;
  }
}
