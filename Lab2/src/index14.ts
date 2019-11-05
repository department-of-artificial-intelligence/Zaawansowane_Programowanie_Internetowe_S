import { fromEvent, combineLatest } from "rxjs";
import { map, filter } from "rxjs/operators";

let input1 = document.querySelector("input:nth-of-type(1)");
let input2 = document.querySelector("input:nth-of-type(2)");
let p = document.querySelector("p");

let obs1$ = fromEvent(input1,"input")
                .pipe(map(v=>(<HTMLInputElement>v.target).value),
                    );

let obs2$ = fromEvent(input2,"input")
                .pipe(map(v=>(<HTMLInputElement>v.target).value),
                      map(v=>parseInt(v))
                    );

combineLatest(obs1$, obs2$)
    .pipe(filter(([text, liczba])=>text.length > liczba),
          map(([text, _])=>text)
         )
    .subscribe(v=>p.innerHTML = v);