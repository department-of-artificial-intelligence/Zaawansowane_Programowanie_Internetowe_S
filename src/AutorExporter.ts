import { Autor } from "./Autor";
import { IAutorFormatter } from "./IAutorFormatter";

export class AutorExporter {
    constructor(private formatter: IAutorFormatter) {}

    export(autorzy: Autor[]): string {
        return this.formatter.format(autorzy);
    }
}
