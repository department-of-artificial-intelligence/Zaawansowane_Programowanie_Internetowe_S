import type { CreateAuthorDto, ReadonlyAuthors } from "../blog/types";
import type { IAuthorFormatter } from "./IAuthorFormatter";

export class JsonFormatter implements IAuthorFormatter {
    format(authors: ReadonlyAuthors): string {
        const items: CreateAuthorDto[] = authors.map(author => ({
            firstName: author.firstName,
            lastName: author.lastName,
            email: author.email
        }));
        return JSON.stringify(items, null, 2);
    }
}
