import { fromEvent, combineLatest } from "rxjs";
import { map, filter } from "rxjs/operators";

let input1 = document.querySelector("input:nth-of-type(1)");
let input2 = document.querySelector("input:nth-of-type(2)");
let p = document.querySelector("p");

function sprawdzLiczbe(x)
{
    if(isNaN(x))
        throw Error("Nie wprowadziłeś poprawnej liczby");
    else
        return x;
}
let obs1$ = fromEvent(input1,"input")
                .pipe(map(v=>(<HTMLInputElement>v.target).value),
                      map(v=>parseInt(v)),
                      map(v=>sprawdzLiczbe(v))
                    );

let obs2$ = fromEvent(input2,"input")
                .pipe(map(v=>(<HTMLInputElement>v.target).value),
                      map(v=>parseInt(v)),
                      map(v=>sprawdzLiczbe(v))
                    );

combineLatest(obs1$, obs2$)
    .pipe(
          map(([l1, l2])=>l1*l2)
         )
    .subscribe(v=>p.innerHTML = v.toString(), 
               e=>p.innerHTML = e.message);