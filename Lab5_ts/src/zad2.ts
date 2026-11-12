// Zadanie 2 - suma tylko wartości parzystych
import { numbers } from "./zad1";

export function zad2(): void {
  const sum = numbers
    .filter(v => v % 2 === 0)
    .reduce((acc, v) => acc + v, 0);
  console.log("Zad 2:", sum);
}