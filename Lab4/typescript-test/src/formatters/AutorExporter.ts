import { Autor } from "../domain/Autor";
import type { IAutorFormatter } from "./IAutorFormatter";

export class AutorExporter {
    private formatter: IAutorFormatter;

    constructor(formatter: IAutorFormatter) {
        this.formatter = formatter;
    }

    setFormatter(formatter: IAutorFormatter): void {
        this.formatter = formatter;
    }

    exportuj(autorzy: Autor[]): string {
        return this.formatter.format(autorzy);
    }
}