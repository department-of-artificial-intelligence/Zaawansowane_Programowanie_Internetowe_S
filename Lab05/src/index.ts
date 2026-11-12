import { fromEvent } from "rxjs";

let element = document.querySelector<HTMLButtonElement>("#button");

fromEvent(element, "click").subscribe(
   e=>{document.querySelector('#info1').innerHTML += 'click<br/>'}
);
