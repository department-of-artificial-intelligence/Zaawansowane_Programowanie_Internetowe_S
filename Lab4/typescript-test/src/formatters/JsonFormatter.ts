import { Autor } from "../domain/Autor";
import type { IAutorFormatter } from "./IAutorFormatter";

export class JsonFormatter implements IAutorFormatter {
    format(autorzy: Autor[]): string {
        const plainData = autorzy.map((a) => ({
            id: a.id,
            imie: a.imie,
            nazwisko: a.nazwisko,
            email: a.email,
        }));

        return JSON.stringify(plainData, null, 2);
    }
}