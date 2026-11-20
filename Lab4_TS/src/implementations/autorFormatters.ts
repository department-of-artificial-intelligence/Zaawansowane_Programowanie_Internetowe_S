import { Autor } from "../classes/autor";
import type { IAutorFormatter } from "../interfaces/IAutorData";

export class AutorJsonFormatter implements IAutorFormatter{
    format(autor: Autor): string {
        return JSON.stringify(
            {
                imie: autor.imie,
                nazwisko: autor.nazwisko,
                email: autor.email
            }, null, 2
        );
    }
}

export class AutorHTMLFormatter implements IAutorFormatter{
    format(autor: Autor): string{
        return `<div><b>${autor.imie} ${autor.nazwisko}</b> <i>${autor.email}</i></div>`;
    }
}