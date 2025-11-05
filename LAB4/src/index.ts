import { Author } from "./Author";
import { Article } from "./Article";
import { Blog } from "./Blog";
import { Coment } from "./Coment";

function hello() {
    console.log("Witaj w swiecie Typescript");
}
hello();


// Lab 5
const a: number[] = [1, 2, 3, 4, 5, 6, 7, 8]
function sum_n(numery: number[]): number {
    let sum: number = numery.reduce(function (a, b) { return a + b; })
    return sum
}
function sum(numery: number[]): number {
    let sum: number = numery.filter(x => x % 2 == 0).reduce(function (a, b) { return a + b; })
    return sum
}
function sum2(numery: number[], num: number): number {
    let sum: number = numery.filter(x => x > num).reduce(function (a, b) { return a + b; })
    return sum
}

function greaterThen(numery: number[], pred: number): number[] {
    let t = numery.filter(x => x > pred);
    return t;
}

function sum3(numery: number[], pred: number): number {
    let t = sum_n(greaterThen(numery, pred))
    return t;
}
//console.log(sum(a))
//console.log(sum2(a, 4))
//console.log(sum3(a, 4))

function curry2<T1, T2, T3>(fn: (arg1: T1, arg2: T2) => T3) {
    return (a1: T1) => (a2: T2) => fn(a1, a2);
}

let suma = curry2(sum3);
let wiecejNizPred = suma(a)
let result = wiecejNizPred(4)

let result2 = wiecejNizPred(6)

//console.log(result)
//console.log(result2)

const s : string[] = ["Ala", "1","Ewa","12.4"]