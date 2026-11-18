import { addParagraph } from "../dom";

function greaterThan(limit: number, value: number): boolean {
    return value > limit;
}

export function task04(parent: HTMLElement): void {
    const numbers = [3, 8, 1, 12, 5, 6, 9, 2];
    const limit = 5;
    const sum = numbers
        .filter(value => greaterThan(limit, value))
        .reduce((total, value) => total + value, 0);
    addParagraph(parent).textContent = `Sum of values greater than ${limit}: ${sum}`;
}
