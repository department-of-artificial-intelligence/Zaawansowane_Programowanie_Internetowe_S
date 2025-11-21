import { Autor } from "./Autor";
import { IAuthorView } from "./IAuthorView";

export class NumberedListView implements IAuthorView {
    public render(container: HTMLElement, autory: Autor[]): void {
        const listItems = autory.map(autor => {
            return `<li>${autor.imie} ${autor.nazwisko} (${autor.email})</li>`;
        });
        container.innerHTML = `<ol>${listItems.join("")}</ol>`;
    }
}