import { combineLatest, fromEvent } from "rxjs";
import { catchError, filter, map } from "rxjs/operators";
import { addInput, addParagraph } from "../dom";
import { toNumber } from "./toNumber";

export function task18(parent: HTMLElement): void {
    const firstInput = addInput(parent, "text");
    const secondInput = addInput(parent, "text");
    const paragraph = addParagraph(parent);
    const first$ = fromEvent(firstInput, "input")
        .pipe(map(() => firstInput.value));
    const second$ = fromEvent(secondInput, "input")
        .pipe(map(() => secondInput.value));
    combineLatest([first$, second$])
        .pipe(
            filter(([first, second]) => first !== "" && second !== ""),
            map(([first, second]) => toNumber(first) * toNumber(second)),
            catchError((error, caught) => {
                paragraph.textContent = error.message;
                return caught;
            }))
        .subscribe(area => paragraph.textContent = `Area: ${area}`);
}
