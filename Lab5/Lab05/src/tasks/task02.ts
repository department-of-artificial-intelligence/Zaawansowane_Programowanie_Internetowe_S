import { addParagraph } from "../dom";

export function task02(parent: HTMLElement): void {
    const numbers = [3, 8, 1, 12, 5, 6, 9, 2];
    const sum = numbers
        .filter(value => value % 2 === 0)
        .reduce((total, value) => total + value, 0);
    addParagraph(parent).textContent = `Sum of even values: ${sum}`;
}
