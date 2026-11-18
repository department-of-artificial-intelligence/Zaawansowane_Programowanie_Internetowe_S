import { addParagraph } from "../dom";

function curry2<T1, T2, T3>(fn: (arg1: T1, arg2: T2) => T3) {
    return (a1: T1) => (a2: T2) => fn(a1, a2);
}

function greaterThan(limit: number, value: number): boolean {
    return value > limit;
}

export function task05(parent: HTMLElement): void {
    const numbers = [3, 8, 1, 12, 5, 6, 9, 2];
    const limit = 5;
    const sum = numbers
        .filter(curry2(greaterThan)(limit))
        .reduce((total, value) => total + value, 0);
    addParagraph(parent).textContent = `Sum of values greater than ${limit}: ${sum}`;
}
