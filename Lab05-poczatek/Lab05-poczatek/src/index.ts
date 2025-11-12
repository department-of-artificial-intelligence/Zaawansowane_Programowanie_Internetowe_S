import { fromEvent } from "rxjs";

let element = document.querySelector<HTMLButtonElement>("#button");

fromEvent(element, "click").subscribe((e) => {
  const sum = func(tablica, greaterThan(29));
  document.querySelector("#info1").innerHTML = `Wynik: ${sum}<br/>`;
});

let tablica = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30];

const greaterThan = (liczba: number) => (value: number): boolean => value > liczba;

const func = (tablica: number[], predicate: (value: number) => boolean): number => {
  let suma = 0;

  for (let i = 0; i < tablica.length; i++) {
   if (predicate(tablica[i])) {
      suma += tablica[i];
    }
  }

  return suma;
};

function curry2<T1, T2, T3>(func:(arg1:T1, arg2:T2)=>T3) {
   return (a1:T1)=>(a2:T2)=>func(a1,a2);
}
