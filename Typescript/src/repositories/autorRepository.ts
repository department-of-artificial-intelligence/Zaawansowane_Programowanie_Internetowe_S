//import * as fs from "fs";
/*import { Autor } from "../classes/autor";

export class AutorRepository {
    private url = "./data.json";

    zapisz(autorzy: Autor[]): void {
        const dane = autorzy.map(a => ({
            id: a["_id"],
            imie: a.imie,
            nazwisko: a.nazwisko,
            email: a.email,
        }));
        fs.writeFileSync(this.url, JSON.stringify(dane, null, 2), "utf-8");
    }

    wczytaj(): Autor[] {
        if (!fs.existsSync(this.url)) {
            return [];
        }

        const dane = JSON.parse(fs.readFileSync(this.url, "utf-8"));
        return dane.map(
            (a: any) => {
                const autor = new Autor(a.imie, a.nazwisko);
                autor.email = a.email;
                return autor;
            }
        );
    }
} */