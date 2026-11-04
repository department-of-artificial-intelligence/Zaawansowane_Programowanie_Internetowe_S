import { Author } from "../models/Author";
import type { AuthorFormatter } from "./AuthorFormatter";

export class CsvFormatter implements AuthorFormatter {
    format(authors: Author[]): string {
        const escape = (v: string) => `"${v.replace(/"/g, '""')}"`;
        const lines = authors.map(a =>
            [String(a.id), a.firstName, a.lastName, a.email].map(escape).join(";"));
        return ["id;firstName;lastName;email", ...lines].join("\n");
    }
}
