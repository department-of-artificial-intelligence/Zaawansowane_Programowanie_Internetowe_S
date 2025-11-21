import { Autor } from "./Autor";
import { IAuthorFormatter } from "./IAuthorFormatter";

// Konkretna strategia: formatowanie do JSON
export class JsonFormatter implements IAuthorFormatter {
    public format(autory: Autor[]): string {
        // Używamy JSON.stringify do serializacji.
        // Drugi argument (null) i trzeci (2) służą do "ładnego" formatowania z wcięciami.
        return JSON.stringify(autory, null, 2);
    }
}