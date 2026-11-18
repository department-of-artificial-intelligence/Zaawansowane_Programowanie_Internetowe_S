import { Autor } from "../blog/Autor";
import { EksporterAutorow } from "./EksporterAutorow";
import { FormatterListyHtml } from "./FormatterListyHtml";
import { FormatterTabeliHtml } from "./FormatterTabeliHtml";
import { IFormatterAutorow } from "./IFormatterAutorow";
import { PobieraczAutorow } from "./PobieraczAutorow";

export class AplikacjaAutorow {
    private autorzy: Autor[] = [];
    private formatery: { nazwa: string, formatter: IFormatterAutorow }[] = [
        { nazwa: "Lista punktowana", formatter: new FormatterListyHtml("ul") },
        { nazwa: "Lista numerowana", formatter: new FormatterListyHtml("ol") },
        { nazwa: "Tabela", formatter: new FormatterTabeliHtml() }
    ];
    private eksporter: EksporterAutorow;
    private wybor: HTMLSelectElement;
    private wynik: HTMLDivElement;

    constructor(rodzic: HTMLElement, pobieracz: PobieraczAutorow) {
        this.eksporter = new EksporterAutorow(this.formatery[0].formatter);

        this.wybor = document.createElement("select");
        this.formatery.forEach((element, indeks) => {
            const opcja = document.createElement("option");
            opcja.value = String(indeks);
            opcja.textContent = element.nazwa;
            this.wybor.appendChild(opcja);
        });
        this.wybor.addEventListener("change", () => {
            this.eksporter.ustawFormatter(this.formatery[Number(this.wybor.value)].formatter);
            this.wyswietl();
        });

        this.wynik = document.createElement("div");

        rodzic.appendChild(this.wybor);
        rodzic.appendChild(this.wynik);

        pobieracz.pobierz().then(autorzy => {
            this.autorzy = autorzy;
            this.wyswietl();
        });
    }

    private wyswietl(): void {
        this.wynik.innerHTML = this.eksporter.eksportuj(this.autorzy);
    }
}
