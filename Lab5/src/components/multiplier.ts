
import { combineLatest, fromEvent, map, startWith, catchError, of,filter, switchMap, EMPTY} from "rxjs";

export class InputMultiplier {
    private _paragraph: HTMLParagraphElement;
    private _firstInput: HTMLInputElement;
    private _secondInput: HTMLInputElement;
    private _wrapper: HTMLDivElement;

    constructor() {
        this._wrapper = document.createElement("div");
        this._paragraph = document.createElement("p");
        this._firstInput = document.createElement("input");
        this._firstInput.type = "text";
        this._secondInput = document.createElement("input");
        this._secondInput.type = "text";

        const firstInputSource$ = fromEvent(this._firstInput, "input").pipe(map(() => this._firstInput.value), startWith(this._firstInput.value));
        const secondInputSource$ = fromEvent(this._secondInput, "input").pipe(map(() => this._secondInput.value), startWith(this._secondInput.value));

        combineLatest([firstInputSource$,secondInputSource$])
        .pipe(
            filter(([first, second]) => first.trim() !== "" && second.trim() !== ""),
            switchMap(([first, second]) =>
                of([Number(first), Number(second)]).pipe(
                    map(([firstNumber, secondNumber]) => {
                        if(Number.isNaN(firstNumber) && Number.isNaN(secondNumber))
                            throw new Error("Both inputs contain incorrect values");
                        
                        if(Number.isNaN(firstNumber))
                            throw new Error("First input contains an incorrect value");

                        if(Number.isNaN(secondNumber))
                            throw new Error("Second input contains an incorrect value");


                        return firstNumber * secondNumber;
                    }),
                    catchError(error => {
                        this._paragraph.innerText = error.message;
                        return EMPTY;
                    })
                )
            )
        )
        .subscribe(result => this._paragraph.innerText = String(result));

        this._wrapper.append(this._firstInput, this._secondInput, this._paragraph);
    }

    public mount(container: HTMLElement): void {
        container.appendChild(this._wrapper);
    }
}
