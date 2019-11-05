import { fromEvent } from "rxjs";
import { map, delay } from "rxjs/operators";


let element = <HTMLElement>document.getElementById("element"); 

let zrodlo$ = fromEvent(document, "mousemove") 
               .pipe(map((e: MouseEvent) => { return { x: e.clientX, y: e.clientY } }),
                     delay(100)
                    );

zrodlo$.subscribe(e => { element.style.left = e.x+"px";
                         element.style.top = e.y+"px"; 
                       });