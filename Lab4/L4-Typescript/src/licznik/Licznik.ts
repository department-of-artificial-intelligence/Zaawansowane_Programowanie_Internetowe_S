export class Licznik {
    private pole: HTMLInputElement;
    private przyciskPlus: HTMLButtonElement;
    private przyciskMinus: HTMLButtonElement;

    constructor(rodzic: HTMLElement) {
        this.pole = document.createElement("input");
        this.pole.type = "number";
        this.pole.value = "0";

        this.przyciskPlus = document.createElement("button");
        this.przyciskPlus.textContent = "+";
        this.przyciskPlus.addEventListener("click", () => this.zmien(1));

        this.przyciskMinus = document.createElement("button");
        this.przyciskMinus.textContent = "-";
        this.przyciskMinus.addEventListener("click", () => this.zmien(-1));

        rodzic.appendChild(this.pole);
        rodzic.appendChild(this.przyciskPlus);
        rodzic.appendChild(this.przyciskMinus);
    }

    private zmien(roznica: number): void {
        this.pole.value = String(Number(this.pole.value) + roznica);
    }
}
