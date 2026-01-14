export class Course {
    _id: number | undefined;
    _name: string;

    constructor(name: string) {
        this._name = name;
    }

    public get name() {
        return this._name;
    }

    public get id() {
        return this._id;
    }

    public set id(id: number | undefined) {
        this._id = id;
    }
}