import { ajax } from "rxjs/ajax";
import { retry } from "rxjs/operators";
import { addParagraph } from "../dom";

export function task25(parent: HTMLElement): void {
    const paragraph = addParagraph(parent);
    ajax("/error1")
        .pipe(retry(3))
        .subscribe({
            next: response => paragraph.textContent = `Response: ${response.status}`,
            error: error => paragraph.textContent = `Request failed with status ${error.status}`
        });
}
