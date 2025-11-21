import { Autor } from "./Autor";
import { IAuthorView } from "./IAuthorView";

export class TableView implements IAuthorView {
    public render(container: HTMLElement, autory: Autor[]): void {
        // Generujemy wiersze (tr) dla każdego autora
        const rows = autory.map(autor => {
            return `    <tr><td>${autor.imie}</td><td>${autor.nazwisko}</td><td>${autor.email}</td></tr>`;
        });

        // Składamy kompletną tabelę
        container.innerHTML = `
            <table border="1" style="border-collapse: collapse; width: 100%;">
              <thead>
                <tr><th>Imię</th><th>Nazwisko</th><th>Email</th></tr>
              </thead>
              <tbody>
                ${rows.join("\n")}
              </tbody>
            </table>
        `;
    }
}