import { Category } from "./category";

export class Email {
    public get value(): string {
        return this._value;
    }

    public get categoryId(): number {
        return this._category.id;
    }

    public get category(): Category {
        return this._category;
    }

    private _value:string;
    private _category:Category;

    constructor(value:string, category:Category) {
        this._category = category;
        if (this.checkEmail(value)) {
            this._value = value;
        } else {
            throw new Error("Adres email nie ma wlasciwego formatu");
        }
    }

    private checkEmail(text:string):boolean {
        let pattern =  /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))\$/;
        return pattern.test(text);
    }
}
