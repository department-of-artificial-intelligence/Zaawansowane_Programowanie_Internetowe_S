import { combineLatest, fromEvent } from "rxjs";
import { map } from "rxjs/operators";
import { addInput, addParagraph } from "../dom";
import { toNumber } from "./toNumber";

export function task15(parent: HTMLElement): void {
    const firstInput = addInput(parent, "text");
    const secondInput = addInput(parent, "text");
    const paragraph = addParagraph(parent);
    const first$ = fromEvent(firstInput, "input")
        .pipe(map(() => firstInput.value));
    const second$ = fromEvent(secondInput, "input")
        .pipe(map(() => secondInput.value));
    combineLatest([first$, second$])
        .pipe(map(([first, second]) => toNumber(first) * toNumber(second)))
        .subscribe({
            next: area => paragraph.textContent = `Area: ${area}`,
            error: error => paragraph.textContent = error.message
        });
}
