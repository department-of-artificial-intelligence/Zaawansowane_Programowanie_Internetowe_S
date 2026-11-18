import { Autor } from "../blog/Autor";
import { IFormatterAutorow } from "./IFormatterAutorow";

export class EksporterAutorow {
    constructor(private formatter: IFormatterAutorow) { }

    ustawFormatter(formatter: IFormatterAutorow): void {
        this.formatter = formatter;
    }

    eksportuj(autorzy: Autor[]): string {
        return this.formatter.formatuj(autorzy);
    }
}
