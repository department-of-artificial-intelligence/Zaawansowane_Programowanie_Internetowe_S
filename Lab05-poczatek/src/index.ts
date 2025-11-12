import { fromEvent } from "rxjs";

let element = document.querySelector<HTMLButtonElement>("#button");
let sum = document.querySelector<HTMLButtonElement>("#sum");

fromEvent(element, "click").subscribe(
   e=>{document.querySelector('#info1').innerHTML += 'click<br/>'}
);


let tab = [1,2,3,4,5,6,7,8,9]
let a = 9
let suma = 0;
for (let i = 0; i < tab.length; i++) {
   if (a < tab[i])
      suma += tab[i];
}
fromEvent(sum, "click").subscribe(
   e=>{
      document.querySelector('#wynik')!.innerHTML += `Suma: ${suma}<br/>`;
   }
);


function greaterThen(value:number , digit : number): boolean{
   return value > digit
}
let filteredValues = tab.filter(value => greaterThen(value, a));
//console.log(filteredValues)

function curryzad5<T1,T2,T3>(fn:(a:T1,tab:T2) => T3){
   return (a1: T1) => (a2: T2) => fn(a1, a2);
}

let curriedGreaterThan = curryzad5(greaterThen);
let greaterThan5 = curriedGreaterThan(5);
let filteredValues2 = tab.filter(value => greaterThan5(value));
//console.log(filteredValues2);


function number(value: string): boolean {
    return !isNaN(Number(value));  }

let tab2 = ["Ala", "1", "Ewa", "12.4"]; 
let suma2 = 0;

for (let i = 0; i < tab2.length; i++) {
    if (number(tab2[i])) {
        suma2 += Number(tab2[i]); 
    }
}

console.log(suma2);  