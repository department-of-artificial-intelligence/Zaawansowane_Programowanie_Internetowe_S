import { Autor } from "../domain/Autor";

//Interfejs DTO opisujący strukturę danych z JSON
interface IAutorDTO {
    imie: string;
    nazwisko: string;
    email: string;
}

export class AutorService {
    private _url: string;

    constructor(url: string = "/dane.json") {
        this._url = url;
    }

    public async pobierzAutorow(): Promise<Autor[]> {
        const response = await fetch(this._url);
        // Używamy zadeklarowanego typu IAutorDTO[]
        const rawData: IAutorDTO[] = await response.json();
        return rawData.map((data: IAutorDTO) => new Autor(data.imie, data.nazwisko, data.email));
    }

    public znajdzAutoraPoId(autorzy: Autor[], id: number): Autor | undefined {
        return autorzy.find((autor) => autor.id === id);
    }
}