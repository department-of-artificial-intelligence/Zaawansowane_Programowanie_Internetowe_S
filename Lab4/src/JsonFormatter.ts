import { Autor } from "./Autor";
import { IAutorFormatter } from "./IAutorFormatter";

export class JsonFormatter implements IAutorFormatter {
    format(autorzy: Autor[]): string {
        return JSON.stringify(
            autorzy.map(a => ({
                imie: a.imie,
                nazwisko: a.nazwisko,
                email: a.email
            })),
            null,
            2
        );
    }
}