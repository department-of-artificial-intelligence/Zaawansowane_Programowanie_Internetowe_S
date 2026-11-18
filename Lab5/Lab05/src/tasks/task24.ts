import { ajax } from "rxjs/ajax";
import { addParagraph } from "../dom";

export function task24(parent: HTMLElement): void {
    const paragraph = addParagraph(parent);
    ajax("/error")
        .subscribe({
            next: response => paragraph.textContent = `Response: ${response.status}`,
            error: error => paragraph.textContent = `Request failed with status ${error.status}`
        });
}
