import { Author } from "../models/Author";

// Strategy - common interface for every output format
export interface AuthorFormatter {
    format(authors: Author[]): string;
}
