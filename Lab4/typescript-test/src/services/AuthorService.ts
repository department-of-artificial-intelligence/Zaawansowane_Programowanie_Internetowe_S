import { Author } from "../models/Author";
import { measureTime } from "../decorators/measureTime"; // <-- Nowy import

// Interfejs DTO reprezentuj¹cy strukturê danych w pliku JSON
export interface IAuthorDto {
    id: number;
    firstName?: string;
    lastName?: string;
    email: string;
    // Opcjonalne wsparcie dla polskich kluczy, gdyby dane mia³y inne nazwy
    imie?: string;
    nazwisko?: string;
}

// Zadanie 11: Typ reprezentuj¹cy mapê autorów indeksowan¹ ich identyfikatorem ID
export type AuthorRegistry = Record<number, Author>;

export class AuthorService {
    private _url: string;

    constructor(url: string = "/dane.json") {
        this._url = url;
    }

    // Udekorowanie metody asynchronicznej
    @measureTime
    public async fetchAuthors(): Promise<Author[]> {
        const response = await fetch(this._url);

        if (!response.ok) {
            throw new Error(`Nie uda³o siê pobraæ pliku ${this._url} (Status: ${response.status})`);
        }

        // Odczyt danych z rzutowaniem na tablicê DTO
        const data = (await response.json()) as IAuthorDto[];

        // Konwersja obiektów DTO na obiekty klasy domenowej Author
        return data.map((dto) => {
            const firstName = dto.firstName ?? dto.imie ?? "";
            const lastName = dto.lastName ?? dto.nazwisko ?? "";

            const author = new Author(firstName, lastName, dto.email);
            author.id = dto.id;
            return author;
        });
    }

    // ==========================================
    // ZADANIE 10: CZÊŒCIOWA AKTUALIZACJA DANYCH
    // ==========================================
    public updateAuthor(
        authors: Author[],
        id: number,
        changes: Partial<Author>
    ): Author | undefined {
        const author = authors.find((a) => a.id === id);
        if (!author) {
            return undefined;
        }

        author.update(changes);
        return author;
    }

    // ==========================================
    // ZADANIE 11: POBIERANIE W FORMIE RECORD
    // ==========================================
    @measureTime
    public async fetchAuthorsRecord(): Promise<AuthorRegistry> {
        const authorsList = await this.fetchAuthors();

        // Mapowanie tablicy na strukturê Record<number, Author>
        return authorsList.reduce<AuthorRegistry>((registry, author) => {
            if (author.id !== undefined) {
                registry[author.id] = author;
            }
            return registry;
        }, {});
    }
}