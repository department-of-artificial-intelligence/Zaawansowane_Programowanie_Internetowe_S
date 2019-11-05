import { fromEvent, combineLatest, throwError } from "rxjs";
import { map, filter, catchError, retry } from "rxjs/operators";

let input1 = document.querySelector("input:nth-of-type(1)");
let input2 = document.querySelector("input:nth-of-type(2)");
let p = document.querySelector("p");

let obs1$ = fromEvent(input1,"input")
                .pipe(map(v=>(<HTMLInputElement>v.target).value),
                      map(v=>parseInt(v)),
                      filter(x=>!isNaN(x))
                    );

let obs2$ = fromEvent(input2,"input")
                .pipe(map(v=>(<HTMLInputElement>v.target).value),
                      map(v=>parseInt(v)),
                      filter(x=>!isNaN(x))
                    );

combineLatest(obs1$, obs2$)
    .pipe(
          map(([l1, l2])=>l1*l2)
         )
    .subscribe(v=>p.innerHTML = v.toString(), 
               e=>p.innerHTML = "Wprowadziłeś błędną wartość 4 razy koniec zabawy");