import { addParagraph } from "../dom";

export function task06(parent: HTMLElement): void {
    const texts = ["Ala", "1", "Ewa", "12.4"];
    const sum = texts
        .map(Number)
        .filter(value => !isNaN(value))
        .reduce((total, value) => total + value, 0);
    addParagraph(parent).textContent = `Sum of the numeric strings: ${sum}`;
}
