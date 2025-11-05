export class Komentarz {
    private _tresc: string;
    private _data: Date;
    private _nick: string;

    constructor(tresc: string, nick: string) {
        if (!tresc?.trim()) throw new Error("Treść komentarza nie może być pusta.");
        if (!nick?.trim()) throw new Error("Nick nie może być pusty.");
        this._tresc = tresc.trim();
        this._nick = nick.trim();
        this._data = new Date(); // domyślna data — choć nie wymagana w zadaniu, ale rozsądna
    }

    get tresc(): string {
        return this._tresc;
    }

    set tresc(value: string) {
        if (!value?.trim()) throw new Error("Treść komentarza nie może być pusta.");
        this._tresc = value.trim();
    }

    get data(): Date {
        return this._data;
    }

    set data(value: Date) {
        if (!(value instanceof Date)) throw new Error("Data musi być obiektem Date.");
        this._data = value;
    }

    get nick(): string {
        return this._nick;
    }

    set nick(value: string) {
        if (!value?.trim()) throw new Error("Nick nie może być pusty.");
        this._nick = value.trim();
    }

    dodajOdpowiedz(odpowiedz: Komentarz): void {
        // Wymaga rozszerzenia klasy — ale nie jest to wymagane w zadaniu,
        // więc zostawiam pustą implementację.
    }

    pobierzOdpowiedzi(): Komentarz[] {
        return []; // zgodnie z wcześniejszym diagramem
    }
}
