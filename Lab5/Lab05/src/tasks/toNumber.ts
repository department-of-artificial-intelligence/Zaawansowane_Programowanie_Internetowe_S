export function toNumber(value: string): number {
    const number = Number(value);
    if (value.trim() === "" || isNaN(number))
        throw new Error("Both fields must contain numbers");
    return number;
}
