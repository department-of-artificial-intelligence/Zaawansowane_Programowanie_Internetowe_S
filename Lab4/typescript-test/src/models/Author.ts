export type UpdateAuthorDTO = Partial<Omit<Author, "id">>;

export class Author {
    private _id?: number;
    private _firstName!: string;
    private _lastName!: string;
    private _email!: string;

    private static readonly EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    constructor(firstName: string, lastName: string, email: string) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
    }

    // --- GETTERY I SETTERY Z ZADANIA 2 ---
    get id(): number | undefined {
        return this._id;
    }

    set id(value: number | undefined) {
        this._id = value;
    }

    get firstName(): string {
        return this._firstName;
    }

    set firstName(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("First name is required and cannot be empty.");
        }
        this._firstName = value.trim();
    }

    get lastName(): string {
        return this._lastName;
    }

    set lastName(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Last name is required and cannot be empty.");
        }
        this._lastName = value.trim();
    }

    get email(): string {
        return this._email;
    }

    set email(value: string) {
        if (!value || value.trim().length === 0) {
            throw new Error("Email is required and cannot be empty.");
        }
        if (!Author.EMAIL_REGEX.test(value.trim())) {
            throw new Error(`Invalid email format: "${value}".`);
        }
        this._email = value.trim();
    }

    // ==========================================
    // ZADANIE 10: CZÊŒCIOWA AKTUALIZACJA DANYCH
    // ==========================================
    public update(changes: Partial<Author>): void {
        // Przypisanie przez settery zapewnia zachowanie walidacji
        if (changes.firstName !== undefined) {
            this.firstName = changes.firstName;
        }
        if (changes.lastName !== undefined) {
            this.lastName = changes.lastName;
        }
        if (changes.email !== undefined) {
            this.email = changes.email;
        }
    }

    // ==========================================
    // ZADANIE 13: POBIERANIE PE£NEGO IMIENIA I NAZWISKA
    // ==========================================
    public getFullName(): string {
        return `${this._firstName} ${this._lastName}`;
    }
}