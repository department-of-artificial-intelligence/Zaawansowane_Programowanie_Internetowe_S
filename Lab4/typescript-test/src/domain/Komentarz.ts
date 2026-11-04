export class Komentarz {
    private _tresc!: string;
    private _data: Date;
    private _odpowiedzi: Komentarz[] = [];
    private _nick!: string;

    constructor(tresc: string, nick: string) {
        this.tresc = tresc;
        this.nick = nick;
        this._data = new Date();
    }

    get tresc(): string {
        return this._tresc;
    }

    set tresc(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Treść komentarza nie może być pusta.");
        }
        this._tresc = value.trim();
    }

    get nick(): string {
        return this._nick;
    }

    set nick(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Nick nie może być pusty.");
        }
        this._nick = value.trim();
    }

    get data(): Date {
        return this._data;
    }

    dodajOdpowiedz(odpowiedz: Komentarz): void {
        if (!odpowiedz) {
            throw new Error("Odpowiedź musi być poprawnym obiektem Komentarz.");
        }
        this._odpowiedzi.push(odpowiedz);
    }

    pobierzOdpowiedzi(): Komentarz[] {
        return this._odpowiedzi;
    }
}