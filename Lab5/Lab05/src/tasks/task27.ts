import { fromEvent, Observable, of, OperatorFunction } from "rxjs";
import { concatMap, delay, mergeMap, scan, switchMap, tap } from "rxjs/operators";
import { addButton, addParagraph } from "../dom";

type Flatten = (project: (click: number) => Observable<number>) => OperatorFunction<number, number>;

function createApp(parent: HTMLElement, label: string, flatten: Flatten): void {
    const button = addButton(parent, label);
    const paragraph = addParagraph(parent);
    fromEvent(button, "click")
        .pipe(
            scan(count => count + 1, 0),
            flatten(click => of(click)
                .pipe(
                    tap(() => paragraph.innerHTML += `request ${click} sent<br/>`),
                    delay(1000))))
        .subscribe(click => paragraph.innerHTML += `response ${click} received<br/>`);
}

export function task27(parent: HTMLElement): void {
    createApp(parent, "switchMap", project => switchMap(project));
    createApp(parent, "mergeMap", project => mergeMap(project));
    createApp(parent, "concatMap", project => concatMap(project));
}
