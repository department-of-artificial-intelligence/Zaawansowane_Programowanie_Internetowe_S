import { fromEvent } from "rxjs";

let input = document.querySelector("input");
let p = document.querySelector("p");

fromEvent(input,"input")
    .subscribe(v =>p.innerHTML = (<HTMLInputElement>v.target).value);