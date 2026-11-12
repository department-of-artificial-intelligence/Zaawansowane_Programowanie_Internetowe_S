// Zadanie 5 - greaterThan zbudowane za pomocą curry2
import { numbers } from "./zad1";

export function curry2<T1, T2, T3>(fn: (arg1: T1, arg2: T2) => T3) {
  return (a1: T1) => (a2: T2) => fn(a1, a2);
}

const greaterThan = curry2((limit: number, value: number) => value > limit);

export function zad5(limit: number): void {
  const sum = numbers
    .filter(greaterThan(limit))
    .reduce((acc, v) => acc + v, 0);
  console.log("Zad 5:", sum);
}