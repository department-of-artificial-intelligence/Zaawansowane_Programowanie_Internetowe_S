import { combineLatest, fromEvent } from "rxjs";
import { filter, map, startWith } from "rxjs/operators";
import { addInput, addParagraph } from "../dom";

export function task14(parent: HTMLElement): void {
    const input = addInput(parent, "text");
    const limitInput = addInput(parent, "number");
    const paragraph = addParagraph(parent);
    const text$ = fromEvent(input, "input")
        .pipe(map(() => input.value));
    const limit$ = fromEvent(limitInput, "input")
        .pipe(
            map(() => limitInput.valueAsNumber),
            startWith(limitInput.valueAsNumber));
    combineLatest([text$, limit$])
        .pipe(filter(([text, limit]) => isNaN(limit) || text.length > limit))
        .subscribe(([text]) => paragraph.textContent = text);
}
