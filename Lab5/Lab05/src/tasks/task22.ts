import { ajax } from "rxjs/ajax";

type AuthorDTO = {
    id: number;
    firstName: string;
    lastName: string;
};

export function task22(parent: HTMLElement): void {
    const table = document.createElement("table");
    table.setAttribute("border", "1");
    parent.appendChild(table);
    ajax<AuthorDTO[]>("dane.json")
        .subscribe(response => {
            const header = table.insertRow();
            ["Id", "First name", "Last name"].forEach(title => {
                const cell = document.createElement("th");
                cell.textContent = title;
                header.appendChild(cell);
            });
            response.response.forEach(author => {
                const row = table.insertRow();
                row.insertCell().textContent = String(author.id);
                row.insertCell().textContent = author.firstName;
                row.insertCell().textContent = author.lastName;
            });
        });
}
