// Funkcja pomocnicza do zadań 15-18: zamienia tekst z pola input na liczbę,
// a jeśli się nie da - rzuca wyjątek z opisem błędu
export function parseNumber(value: string, fieldName: string): number {
  if (value.trim() === "") {
    throw new Error(`Pole "${fieldName}" jest puste`);
  }
  const result = Number(value.replace(",", "."));
  if (isNaN(result)) {
    throw new Error(`Wartość "${value}" w polu "${fieldName}" nie jest liczbą`);
  }
  return result;
}