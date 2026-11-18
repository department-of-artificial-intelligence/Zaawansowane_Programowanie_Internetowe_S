import { Autor } from "../blog/Autor";

interface DaneAutora {
    imie: string;
    nazwisko: string;
    email: string;
}

export class PobieraczAutorow {
    constructor(private adres: string) { }

    pobierz(): Promise<Autor[]> {
        return fetch(this.adres)
            .then(odpowiedz => odpowiedz.json())
            .then((dane: DaneAutora[]) =>
                dane.map(d => new Autor(d.imie, d.nazwisko, d.email)));
    }
}
