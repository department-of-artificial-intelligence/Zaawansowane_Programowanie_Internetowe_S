import { Autor } from './author.ts';

export interface AutorFormatter {
  format(autorzy: readonly Readonly<Autor>[]): string;
}

export class HtmlTableFormatter implements AutorFormatter {
    public format(autorzy: readonly Autor[]): string {
    const rows = autorzy.map((a) => 
        `<tr>
            <td>${a.id}</td>
            <td>${this.escape(a.imie)}</td>
            <td>${this.escape(a.nazwisko)}</td>
            <td>${this.escape(a.email)}</td>
        </tr>`).join('\n');

    return `<table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Imię</th>
                        <th>Nazwisko</th>
                        <th>Email</th>
                    </tr>
                </thead>
                <tbody>
                    ${rows}
                </tbody>
            </table>`;
    }

    private escape(text: string): string {
    return text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }
}

export class HtmlListFormatter implements AutorFormatter {
    private readonly _ordered: boolean;
    constructor(ordered: boolean = false) {
        this._ordered = ordered;
    }

    public format(autorzy: readonly Autor[]): string {
        const tag = this._ordered ? 'ol' : 'ul';
        const items = autorzy.map((a) => ` <li>${a.imie} ${a.nazwisko} (${a.email})</li>`).join('\n');
        return `<${tag}>\n${items}\n</${tag}>`;
    }
}

export class JsonFormatter implements AutorFormatter {
    public format(autorzy: readonly Autor[]): string {
        const plain = autorzy.map((a) => ({
            id: a.id,
            imie: a.imie,
            nazwisko: a.nazwisko,
            email: a.email,
        }));
        return JSON.stringify(plain, null, 2);
    }
}

export class AutorExporter {
    private _formatter: AutorFormatter
    constructor(formatter: AutorFormatter) {
        this._formatter = formatter;
    }

    public setFormatter(formatter: AutorFormatter): void {
        this._formatter = formatter;
    }

    public export(autorzy: readonly Autor[]): string {
        return this._formatter.format(autorzy);
    }
}