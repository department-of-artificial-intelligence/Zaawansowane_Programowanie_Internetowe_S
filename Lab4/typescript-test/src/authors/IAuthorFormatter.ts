import type { ReadonlyAuthors } from "../blog/types";

export interface IAuthorFormatter {
    format(authors: ReadonlyAuthors): string;
}
