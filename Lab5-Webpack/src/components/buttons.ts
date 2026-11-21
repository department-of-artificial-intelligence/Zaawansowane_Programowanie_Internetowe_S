import { concatMap, delay, fromEvent, map, mergeMap, switchMap, tap } from "rxjs";
import { ajax } from "rxjs/ajax";
import { Component } from "./component";

const REQUEST_URL = "http://localhost:9000/dane.json";
const SIMULATED_DELAY_MS = 1500;

function createRequestStream$(button: HTMLButtonElement, label: string) {
    let clickCount = 0;

    return fromEvent(button, "click").pipe(
        map(() => {
            clickCount += 1;
            return { clickNumber: clickCount, label };
        }),
        tap(({ clickNumber, label: variant }) => {
            console.log(`[${variant}] Click #${clickNumber} -> starting request (delay ${SIMULATED_DELAY_MS}ms)`);
        })
    );
}

export class SwitchMapButton extends Component {
    private _button: HTMLButtonElement;

    constructor(url: string = REQUEST_URL, delayMs: number = SIMULATED_DELAY_MS) {
        super();

        this._button = document.createElement("button");
        this._button.type = "button";
        this._button.innerText = "Send (switchMap)";
        this.appendChild(this._button);

        createRequestStream$(this._button, "switchMap")
            .pipe(
                switchMap(({ clickNumber }) =>
                    ajax.getJSON(url).pipe(
                        delay(delayMs),
                        map((response) => ({ clickNumber, response }))
                    )
                )
            )
            .subscribe({
                next: ({ clickNumber, response }) => {
                    console.log(`[switchMap] Response for click #${clickNumber}:`, response);
                },
                error: (err) => console.error("[switchMap] Error:", err),
            });
    }
}

export class MergeMapButton extends Component {
    private _button: HTMLButtonElement;

    constructor(url: string = REQUEST_URL, delayMs: number = SIMULATED_DELAY_MS) {
        super();

        this._button = document.createElement("button");
        this._button.type = "button";
        this._button.innerText = "Send (mergeMap)";
        this.appendChild(this._button);

        createRequestStream$(this._button, "mergeMap")
            .pipe(
                mergeMap(({ clickNumber }) =>
                    ajax.getJSON(url).pipe(
                        delay(delayMs),
                        map((response) => ({ clickNumber, response }))
                    )
                )
            )
            .subscribe({
                next: ({ clickNumber, response }) => {
                    console.log(`[mergeMap] Response for click #${clickNumber}:`, response);
                },
                error: (err) => console.error("[mergeMap] Error:", err),
            });
    }
}

export class ConcatMapButton extends Component {
    private _button: HTMLButtonElement;

    constructor(url: string = REQUEST_URL, delayMs: number = SIMULATED_DELAY_MS) {
        super();

        this._button = document.createElement("button");
        this._button.type = "button";
        this._button.innerText = "Send (concatMap)";
        this.appendChild(this._button);

        createRequestStream$(this._button, "concatMap")
            .pipe(
                concatMap(({ clickNumber }) =>
                    ajax.getJSON(url).pipe(
                        delay(delayMs),
                        map((response) => ({ clickNumber, response }))
                    )
                )
            )
            .subscribe({
                next: ({ clickNumber, response }) => {
                    console.log(`[concatMap] Response for click #${clickNumber}:`, response);
                },
                error: (err) => console.error("[concatMap] Error:", err),
            });
    }
}