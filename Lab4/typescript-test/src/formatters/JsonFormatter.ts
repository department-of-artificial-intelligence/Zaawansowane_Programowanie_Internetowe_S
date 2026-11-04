import { Author } from "../models/Author";
import type { AuthorFormatter } from "./AuthorFormatter";

export class JsonFormatter implements AuthorFormatter {
    format(authors: Author[]): string {
        const data = authors.map(a => ({
            id: a.id,
            firstName: a.firstName,
            lastName: a.lastName,
            email: a.email
        }));
        return JSON.stringify(data, null, 2);
    }
}
