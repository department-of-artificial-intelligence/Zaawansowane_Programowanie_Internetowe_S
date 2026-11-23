const liczby: number[] = [3, 8, 1, 12, 5, 6];

function greaterThan(limit: number, value: number): boolean {
    return value > limit;
}
function sumaWiekszychNiz(tablica: number[], prog: number): number {
  return tablica
    .filter(x => greaterThan(prog, x))
    .reduce((acc, x) => acc + x, 0);
}

console.log(`Suma > 4: ${sumaWiekszychNiz(liczby, 4)}`);
console.log(`Suma > 10: ${sumaWiekszychNiz(liczby, 10)}`);