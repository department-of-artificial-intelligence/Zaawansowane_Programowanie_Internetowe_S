// Zadanie 3 - suma tylko wartości większych od zadanej liczby
import { numbers } from "./zad1";

export function zad3(limit: number): void {
  const sum = numbers
    .filter(v => v > limit)
    .reduce((acc, v) => acc + v, 0);
  console.log("Zad 3:", sum);
}