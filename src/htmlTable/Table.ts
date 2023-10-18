import { Author } from "../classes/Author";
import { IShow } from "../infterfaces/IShow";


export class Table implements IShow {
    private _authors: Author[];

    constructor(authors: Author[]){
        this._authors = authors;
    }


    show(): string {
        let tableHtml = '<table>';
        tableHtml += '<tr><th>Imię i Nazwisko</th><th>Email</th></tr>';

        for (const author of this._authors) {
            tableHtml += `<tr><td>${author.firstName} ${author.lastName}</td><td>${author.email}</td></tr>`;
        }

        tableHtml += '</table>';
        return tableHtml;
    }
}