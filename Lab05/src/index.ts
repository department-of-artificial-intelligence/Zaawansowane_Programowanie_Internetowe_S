import { fromEvent } from "rxjs";

let element = document.querySelector<HTMLButtonElement>("#button");

fromEvent(element, "click").subscribe(
   e=>{document.querySelector('#info1').innerHTML += 'click<br/>'}
);



const numbers: number[] = [3, 7, 2, 4, 1, 13, 12];
console.log(numbers);

const sum = numbers
  .filter(num => num % 2 === 0)
  .reduce((acc, value) => acc + value, 0);

console.log('Suma:', sum);


const moreThan = 5;
const sumGreaterThan = numbers
  .filter(num => num > moreThan)
  .reduce((acc, value) => acc + value, 0);

console.log(`Suma wieksza od 5:`, sumGreaterThan);



function greaterThan(threshold: number) {
  return (value: number) => value > threshold;
}

const sumFunc = numbers
  .filter(greaterThan(10))
  .reduce((acc, value) => acc + value, 0);

console.log(`Suma liczb =>:`, sumFunc);



function curry2<T1, T2, T3>(fn: (a: T1, b: T2) => T3) {
  return (a: T1) => (b: T2) => fn(a, b);
}

function greaterThanBool(threshold: number, value: number): boolean {
  return value > threshold;
}

const curriedGreaterThan = curry2(greaterThanBool);
const isGreaterThan6 = curriedGreaterThan(6);

const sumGreaterThanCurry = numbers
  .filter(isGreaterThan6)
  .reduce((acc, value) => acc + value, 0);

console.log("Suma z curringiem wieksze od 6: ", sumGreaterThanCurry);



//suma stringow
const values: string[] = ["Ala", "1", "Ewa", "12.4"];

function isNumeric(value: string): boolean {
  return !isNaN(Number(value));
}

const suma = values
  .filter(isNumeric)
  .map(Number)
  .reduce((acc, val) => acc + val, 0);

console.log("Suma wartości liczbowych:", suma);