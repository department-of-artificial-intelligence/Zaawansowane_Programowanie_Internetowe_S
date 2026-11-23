const liczby: number[] = [3, 8, 1, 12, 5, 6];

function curry2<T1, T2, T3>(fn: (arg1: T1, arg2:T2) => T3) {
    return (a1: T1) => (a2: T2) => fn(a1, a2);
}

function greaterThan(limit: number, value: number): boolean {
    return value > limit;
}

const greaterThanC = curry2(greaterThan);

function sumaWiekszychNiz(tablica: number[], prog: number): number {
  return tablica
    .filter(greaterThanC(prog))
    .reduce((acc, x) => acc + x, 0);
}

console.log(`Suma > 4: ${sumaWiekszychNiz(liczby, 4)}`);
console.log(`Suma > 10: ${sumaWiekszychNiz(liczby, 10)}`);

const wiekszeNiz4 = greaterThanC(4);
console.log(wiekszeNiz4(8), wiekszeNiz4(2));