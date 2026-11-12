// Zadanie 4 - predykat jako osobna funkcja greaterThan
import { numbers } from "./zad1";

export const greaterThan = (limit: number) => (value: number): boolean => value > limit;

export function zad4(limit: number): void {
  const sum = numbers
    .filter(greaterThan(limit))
    .reduce((acc, v) => acc + v, 0);
  console.log("Zad 4:", sum);
}