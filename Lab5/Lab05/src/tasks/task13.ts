import { fromEvent } from "rxjs";
import { filter, map } from "rxjs/operators";
import { addInput, addParagraph } from "../dom";

export function task13(parent: HTMLElement): void {
    const input = addInput(parent, "text");
    const paragraph = addParagraph(parent);
    fromEvent(input, "input")
        .pipe(
            map(() => input.value),
            filter(text => text.length > 5))
        .subscribe(text => paragraph.textContent = text);
}
