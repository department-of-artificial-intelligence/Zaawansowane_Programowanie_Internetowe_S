import { Author } from "../models/Author";
import type { AuthorDto } from "./AuthorDto";

export class AuthorService {
    private url: string;

    constructor(url: string) {
        this.url = url;
    }

    async getAuthors(): Promise<Author[]> {
        const response = await fetch(this.url);
        if (!response.ok) {
            throw new Error(`Nie udało się pobrać danych (${response.status})`);
        }
        const data = await response.json() as AuthorDto[];
        return data
            .map(dto => this.toAuthor(dto))
            .filter((a): a is Author => a !== null);
    }

    private toAuthor(dto: AuthorDto): Author | null {
        try {
            return new Author(dto.firstName, dto.lastName, dto.email);
        } catch (e) {
            // the record breaks the validation rules from task 2 - skip it
            console.warn("Pominięto niepoprawny rekord:", dto, (e as Error).message);
            return null;
        }
    }
}
