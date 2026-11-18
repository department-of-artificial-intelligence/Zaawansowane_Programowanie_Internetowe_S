import { combineLatest, fromEvent, throwError } from "rxjs";
import { catchError, map, retry } from "rxjs/operators";
import { addInput, addParagraph } from "../dom";
import { toNumber } from "./toNumber";

export function task16(parent: HTMLElement): void {
    const firstInput = addInput(parent, "text");
    const secondInput = addInput(parent, "text");
    const paragraph = addParagraph(parent);
    const first$ = fromEvent(firstInput, "input")
        .pipe(map(() => firstInput.value));
    const second$ = fromEvent(secondInput, "input")
        .pipe(map(() => secondInput.value));
    combineLatest([first$, second$])
        .pipe(
            map(([first, second]) => toNumber(first) * toNumber(second)),
            catchError(error => {
                paragraph.textContent = error.message;
                return throwError(() => error);
            }),
            retry(3))
        .subscribe({
            next: area => paragraph.textContent = `Area: ${area}`,
            error: () => paragraph.textContent = "Wprowadziłeś błędną wartość 4 razy. Koniec zabawy"
        });
}
