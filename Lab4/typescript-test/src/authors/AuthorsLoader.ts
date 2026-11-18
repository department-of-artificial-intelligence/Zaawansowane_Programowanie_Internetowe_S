import { Author } from "../blog/Author";
import type { CreateAuthorDto } from "../blog/types";

export class AuthorsLoader {
    private url: string;

    constructor(url: string) {
        this.url = url;
    }

    load(): Promise<Author[]> {
        return fetch(this.url)
            .then(res => res.json() as Promise<CreateAuthorDto[]>)
            .then(data => data.map(d => new Author(d.firstName, d.lastName, d.email)));
    }
}
