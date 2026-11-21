import { ajax } from "rxjs/ajax";
import { Component } from "./component";
import { Author } from "../types";

export class AuthorsTable extends Component{
    private _authorsTable: HTMLTableElement;

    constructor(dataAddress: string){
        super();
        this._authorsTable = document.createElement("table");
        this._authorsTable.style.border = "1px solid black";
        this._authorsTable.style.borderCollapse = "collapse";
        this.appendChild(this._authorsTable);

        ajax(dataAddress)
            .subscribe({
                next: response => this._fillTableWithData(response.response as Author[]),
                error: err => console.error(err)
            });
    }

    private _fillTableWithData(data: Author[]): void{
        const tbody = document.createElement("tbody");
        
        for(const author of data){
            const tr = document.createElement("tr");
            
            const tdName = document.createElement("td");
            tdName.style.border = "1px solid black";
            tdName.innerText = author.name;

            const tdSurname = document.createElement("td");
            tdSurname.style.border = "1px solid black";
            tdSurname.innerText = author.surname;

            const tdEmail = document.createElement("td");
            tdEmail.style.border = "1px solid black";
            tdEmail.innerText = author.email;

            tr.append(tdName, tdSurname, tdEmail);
            tbody.appendChild(tr);
        }

        this._authorsTable.replaceChildren(tbody);
    }
}