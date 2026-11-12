// Zadanie 1 - suma wszystkich elementów tablicy
export const numbers: number[] = [3, 8, 12, 5, 7, 20, 1, 14];

export function zad1(): void {
  const sum = numbers.reduce((acc, v) => acc + v, 0);
  console.log("Zad 1:", sum);
}