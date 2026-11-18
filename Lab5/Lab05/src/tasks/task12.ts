import { fromEvent } from "rxjs";
import { map } from "rxjs/operators";
import { addInput, addParagraph } from "../dom";

export function task12(parent: HTMLElement): void {
    const input = addInput(parent, "text");
    const paragraph = addParagraph(parent);
    fromEvent(input, "input")
        .pipe(map(() => input.value))
        .subscribe(text => paragraph.textContent = text);
}
