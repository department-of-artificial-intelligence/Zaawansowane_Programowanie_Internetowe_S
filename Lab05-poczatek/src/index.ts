// ==========================================
// ZADANIE 1: Funkcyjne sumowanie tablicy
// ==========================================
console.log("%c--- ZADANIE 1 ---", "color: green; font-weight: bold;");

const numbers: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// Wykorzystanie funkcji wyższego rzędu reduce zamiast pętli mutującej zmienną zewnętrzną
const sum: number = numbers.reduce((acc: number, curr: number): number => acc + curr, 0);

console.log("Tablica wejściowa:", numbers);
console.log("Suma wszystkich elementów:", sum);

// ==========================================
// ZADANIE 2: Funkcyjne sumowanie wartości parzystych
// ==========================================
console.log("%c--- ZADANIE 2 ---", "color: #06b6d4; font-weight: bold;");

const evenSum: number = numbers
  .filter((n: number): boolean => n % 2 === 0)
  .reduce((acc: number, curr: number): number => acc + curr, 0);

console.log("Suma wartości parzystych:", evenSum);

// ==========================================
// ZADANIE 3: Funkcyjne sumowanie wartości większych od zadanej liczby
// ==========================================
console.log("%c--- ZADANIE 3 ---", "color: #8b5cf6; font-weight: bold;");

const threshold: number = 5;

const greaterThanSum: number = numbers
  .filter((n: number): boolean => n > threshold)
  .reduce((acc: number, curr: number): number => acc + curr, 0);

console.log(`Suma wartości większych od ${threshold}:`, greaterThanSum);

// ==========================================
// ZADANIE 4: Wydzielony predykat greaterThan
// ==========================================
console.log("%c--- ZADANIE 4 ---", "color: #ec4899; font-weight: bold;");

// Predykat przyjmujący próg porównania (limit) oraz badaną liczbę (value)
function greaterThan(limit: number, value: number): boolean {
  return value > limit;
}

const limitValue: number = 5;

const sumWithPredicate: number = numbers
  .filter((n: number): boolean => greaterThan(limitValue, n))
  .reduce((acc: number, curr: number): number => acc + curr, 0);

console.log(`Suma wartości większych od ${limitValue} (predykat greaterThan):`, sumWithPredicate);

// ==========================================
// ZADANIE 5: Zastosowanie funkcji curry2
// ==========================================
console.log("%c--- ZADANIE 5 ---", "color: #f59e0b; font-weight: bold;");

// Funkcja curry2 zamieniająca funkcję dwuargumentową w ciąg funkcji jednoargumentowych
function curry2<T1, T2, T3>(fn: (arg1: T1, arg2: T2) => T3) {
  return (a1: T1) => (a2: T2) => fn(a1, a2);
}

// Przekształcenie predykatu greaterThan na postać curry
const curriedGreaterThan = curry2(greaterThan);

// Częściowa aplikacja pierwszego argumentu zwraca gotowy predykat jednoargumentowy
const isGreaterThan5 = curriedGreaterThan(5);

const sumWithCurrying: number = numbers
  .filter(isGreaterThan5)
  .reduce((acc: number, curr: number): number => acc + curr, 0);

console.log("Suma wartości większych od 5 (z użyciem curry2):", sumWithCurrying);

// ==========================================
// ZADANIE 6: Funkcyjne sumowanie liczb z tablicy stringów
// ==========================================
console.log("%c--- ZADANIE 6 ---", "color: #10b981; font-weight: bold;");

const mixedStrings: string[] = ["Ala", "1", "Ewa", "12.4"];

const numericStringsSum: number = mixedStrings
  .map((item: string): number => (item.trim() === "" ? NaN : Number(item)))
  .filter((num: number): boolean => Number.isFinite(num))
  .reduce((acc: number, curr: number): number => acc + curr, 0);

console.log("Tablica wejściowa:", mixedStrings);
console.log("Suma elementów reprezentujących liczby:", numericStringsSum);