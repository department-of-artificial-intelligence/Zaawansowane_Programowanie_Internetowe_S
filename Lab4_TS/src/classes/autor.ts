import type { AutorDTO } from "../types";

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export class Autor{
    private _id: number;
    private _imie: string;
    private _nazwisko: string;
    private _email: string;

    get imie(): string{
        return this._imie;
    }
    set imie(value: string){
        const clean = value.trim();
        if(!clean)
            throw new Error("Imie wymagane !");
        this._imie = clean;
    }

    get nazwisko(): string{
        return this._nazwisko;
    }
    set nazwisko(value: string){
        const clean = value.trim();
        if(!clean)
            throw new Error("Nazwisko wymagane !");
        this._nazwisko = clean;
    }

    get email(): string{
        return this._email;
    }
    set email(value: string){
        const clean = value.trim();
        if(!EMAIL_REGEX.test(clean))
            throw new Error(`Nieprawidlowy email adres: ${value}`);
        this._email = clean;
    }

    constructor(imie: string, nazwisko: string, email: string, id: number = 0){
        this._id = id;
        this._imie = "";
        this.imie = imie;
        this._nazwisko = "";
        this.nazwisko = nazwisko;
        this._email = "";
        this.email = email;
    }

    update(dto: Partial<AutorDTO>): void{
        dto.imie !== undefined ? this.imie = dto.imie : undefined;
        dto.nazwisko !== undefined ? this.nazwisko = dto.nazwisko : undefined;
        dto.email !== undefined ? this.email = dto.email : undefined;
    }
}